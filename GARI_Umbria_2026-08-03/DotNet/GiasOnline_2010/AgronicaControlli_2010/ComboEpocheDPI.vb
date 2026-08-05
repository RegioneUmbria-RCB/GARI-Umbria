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

<DefaultProperty("Text"), ToolboxData("<{0}:ComboEpocheDPI runat=server></{0}:ComboEpocheDPI>")> Public Class ComboEpocheDPI
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboOperazioni
    '    Inherits ScriptControl
    '    Implements IScriptControl

    Public ddl_ComboEpocheDPI As DropDownList
    Private _TipoTestata As Integer
    Private _IdRcdpi As Integer
    Private _Dpi_Cod As Integer
    Private _Modulo As Integer
    Private _IncludiTutti As Boolean
    Private _Bootstrap As Boolean
    Private _Ricerca As Boolean = True
    Public Sub New()
        _Bootstrap = False
    End Sub


    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_ComboEpocheDPI = New DropDownList
        ddl_ComboEpocheDPI.ID = Me.ClientID & "ComboEpocheDPI"
        If _Bootstrap = False Then
            ddl_ComboEpocheDPI.CssClass = "myCombo"
        Else
            ddl_ComboEpocheDPI.CssClass = "selectpicker ComboEpoche"
            If (Ricerca = True) Then
                ddl_ComboEpocheDPI.Attributes.Add("data-live-search", "true")
            End If

            ddl_ComboEpocheDPI.Attributes.Add("data-container", "body")
        End If

        Me.Controls.Add(ddl_ComboEpocheDPI)
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

    Public Property Modulo() As Integer
        Get
            Return _Modulo
        End Get
        Set(ByVal value As Integer)
            _Modulo = value
        End Set
    End Property
    Public Property Valore_Combo() As String
        Get
            Return ddl_ComboEpocheDPI.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_ComboEpocheDPI.Items.FindByValue(value)) Then
                    ddl_ComboEpocheDPI.SelectedIndex = ddl_ComboEpocheDPI.Items.IndexOf(ddl_ComboEpocheDPI.Items.FindByValue(value))
                End If
            End If
        End Set
    End Property


    Public Property Testo_Combo() As String
        Get
            Return ddl_ComboEpocheDPI.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_ComboEpocheDPI.Items.FindByText(value)) Then
                    ddl_ComboEpocheDPI.SelectedIndex = ddl_ComboEpocheDPI.Items.IndexOf(ddl_ComboEpocheDPI.Items.FindByText(value))
                End If
            End If
        End Set
    End Property



    Public Property IncludiTutti() As Boolean
        Get
            Return _IncludiTutti
        End Get
        Set(ByVal value As Boolean)
            _IncludiTutti = value
        End Set
    End Property
    Public Property Dpi_Cod() As Integer
        Get
            Return _Dpi_Cod
        End Get
        Set(ByVal value As Integer)
            _Dpi_Cod = value
        End Set
    End Property
    Public Property IdRcdpi() As Integer
        Get
            Return _IdRcdpi
        End Get
        Set(ByVal value As Integer)
            _IdRcdpi = value
        End Set
    End Property
    Public Property TipoTestata() As Integer
        Get
            Return _TipoTestata
        End Get
        Set(ByVal value As Integer)
            _TipoTestata = value
        End Set
    End Property
#End Region




    Public Sub CaricaComboEpocheDPI()

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari


        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim Dati As String
        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

        Dim objAgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig

        objWs.NewWS(ObjDownloadWs, _
                objAgroWebConfig.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari, _
                objParametri_Utenti)

        ddl_ComboEpocheDPI.Items.Clear()

        If IncludiTutti Then
            ddl_ComboEpocheDPI.Items.Add(New ListItem("Tutte le epoche", "0"))
        End If

        Select Case TipoTestata

            Case 0 'difesa

                '##############################  Chiama WS - "Leggi_EpocheDifesa" ################################

                Try

                    Dati = ObjDownloadWs.Leggi_EpocheDifesa(CInt(IdRcdpi), _
                                                            CInt(Dpi_Cod), _
                                                            CInt(Modulo), _
                                                            CStr(HttpContext.Current.Session("ASG_Utente_Username_Crypt").ToString), _
                                                            CStr(HttpContext.Current.Session("ASG_Utente_Password_Crypt").ToString))


                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                        Dim XmlDocumento As New System.Xml.XmlDocument
                        Dim XmlNodo As System.Xml.XmlNodeList
                        Dim XmlElemento As System.Xml.XmlElement
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")

                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo

                                ddl_ComboEpocheDPI.Items.Add(New ListItem(CStr(XmlElemento.GetAttribute("descrizioneperiododa")), _
                                                CInt(XmlElemento.GetAttribute("modulo"))))


                            Next
                        End If

                    End If


                Catch ex As Exception

                    'Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!"

                End Try

            Case 1 'diserbo

                '##############################  Chiama WS - "Leggi_EpocheDiserbo" ################################

                Try

                    Dati = ObjDownloadWs.Leggi_EpocheDiserbo(CInt(IdRcdpi), _
                                                            CInt(Dpi_Cod), _
                                                            CStr(HttpContext.Current.Session("ASG_Utente_Username_Crypt").ToString), _
                                                            CStr(HttpContext.Current.Session("ASG_Utente_Password_Crypt").ToString))


                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                        Dim XmlDocumento As New System.Xml.XmlDocument
                        Dim XmlNodo As System.Xml.XmlNodeList
                        Dim XmlElemento As System.Xml.XmlElement
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")

                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo

                                ddl_ComboEpocheDPI.Items.Add(New ListItem(CStr(XmlElemento.GetAttribute("descrizione")), _
                                                CInt(XmlElemento.GetAttribute("ep_cod"))))


                            Next
                        End If

                    End If

                Catch ex As Exception

                    'Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!"

                End Try

        End Select

    End Sub


#Region "JS"
    Public Function GetJS()
        Dim StrSelect As New StringBuilder

        'StrSelect.AppendLine("<script type='text/javascript'>")


        If Bootstrap = False Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" & ddl_ComboEpocheDPI.ClientID & "').combobox();")
            StrSelect.AppendLine("});")
        End If



        'StrSelect.AppendLine("</script>")
        Return StrSelect.ToString
    End Function
#End Region

End Class
