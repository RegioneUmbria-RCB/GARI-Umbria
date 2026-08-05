Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieDAL

Public Class bootstrap
    Inherits AgroControlliCommons



    Private _VersioneBootstrap As String = ""
    Private _VersioneBootstrapBase As String = "3.4.1"
    Private _BootStrapVersioniDisponibili As String() = {"3.3.2", "3.3.4", "3.4.1", "4.5.2", "5.3.3"}

    Private _SitoOspite As TipiEnumerativi.Enum_SiteRedirector =
        AgronicaCoreDataProvider.TipiEnumerativi.Enum_SiteRedirector.Sito_AgronicaAgenda_2010
    Private _bootstrapPlaceHeader As PlaceHolder

    Public Property SitoOspite As TipiEnumerativi.Enum_SiteRedirector
        Get
            Return _SitoOspite
        End Get
        Set(value As TipiEnumerativi.Enum_SiteRedirector)
            _SitoOspite = value
        End Set
    End Property

    Public Property MemorizzaInSessioneDopoLettura As Boolean = True


    Protected Overrides Sub Inizializza()

        MyBase.inizializza()



        If _SitoOspite = TipiEnumerativi.Enum_SiteRedirector.GiasLan Then
            Throw New Exception("AgroBootstrap: Non è stato specificato il sito ospite per una corretta gestione della versione Bootstrap.")
        End If


        If _VersioneBootstrap <> "" Then
            Exit Sub
        End If


        Dim NomeRoutine As String = "AgroBootstrap.Inizializza"

        Dim sessionServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objLog As New AgronicaCoreDataProvider.LogProvider


        'cerco versione personalizzata su db Server
        If Not sessionServer Is Nothing Then
            _VersioneBootstrap = LeggiDaSessioneOppureDaConfigSiti("VersioneBootstrap" & _SitoOspite, "", TipiEnumerativi.agronicacoreparametri_tipoDB.Server, MemorizzaInSessioneDopoLettura)
        End If


        'cerco versione non personalizzata su db Server
        If String.IsNullOrEmpty(_VersioneBootstrap) Then
            _VersioneBootstrap = LeggiDaSessioneOppureDaConfigSiti("VersioneBootstrap", "", TipiEnumerativi.agronicacoreparametri_tipoDB.Server, MemorizzaInSessioneDopoLettura)
        End If

        If String.IsNullOrEmpty(_VersioneBootstrap) Then
            'cerco versione personalizzata
            _VersioneBootstrap = LeggiDaSessioneOppureDaConfigSiti("VersioneBootstrap" & _SitoOspite, "", TipiEnumerativi.agronicacoreparametri_tipoDB.SuperServer, MemorizzaInSessioneDopoLettura)
        End If



        If String.IsNullOrEmpty(_VersioneBootstrap) Then

            'cerco versione comune per tutti ed in deficit uso la versione 2017.3.913
            _VersioneBootstrap = LeggiDaSessioneOppureDaConfigSiti("VersioneBootstrap", "UltimaVersione", TipiEnumerativi.agronicacoreparametri_tipoDB.SuperServer, MemorizzaInSessioneDopoLettura)

            If _VersioneBootstrap.ToLower = "ultimaversione" Or _VersioneBootstrap = "" Then

                'Dim xBase As String = ""
                Dim xBootstrap As String = ""
                Dim xRisp As String = ""


                _VersioneBootstrap = _VersioneBootstrapBase


            End If

        End If

        If MemorizzaInSessioneDopoLettura Then
            HttpContext.Current.Session("ASG_M_VersioneBootstrap") = _VersioneBootstrap
        End If

    End Sub


    Public Property Versione As String
        Get
            Return _VersioneBootstrap
        End Get
        Set(value As String)
            SetVersion(value)
        End Set
    End Property

    Public Property BootstrapPlaceHeader As PlaceHolder
        Get
            Return _bootstrapPlaceHeader
        End Get
        Set(value As PlaceHolder)
            _bootstrapPlaceHeader = value
        End Set
    End Property

    ''' <summary>
    ''' imposta una versione fra quelle disponibili.
    ''' </summary>
    ''' <param name="versione">"1.12.3", "3.5.1"</param>
    Private Sub SetVersion(versione As String)
        If Not _BootStrapVersioniDisponibili.Contains(versione) Then
            Throw New Exception("Versione Bootstrap " & versione & " sconosciuta, valori ammessi: " & String.Join(",", _BootStrapVersioniDisponibili))
        End If
        _VersioneBootstrap = versione
    End Sub

#Region "Methods & Event Handlers"


    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        Inizializza()

        Dim PuntoInterrogativo As String = "?"

        If GiasVersioneCorrente.StartsWith("?") Then
            PuntoInterrogativo = ""
        End If

        If Not _BootStrapVersioniDisponibili.Contains(_VersioneBootstrap) Then
            Throw New Exception("Versione Bootstrap " & _VersioneBootstrap & " sconosciuta, valori ammessi: " & String.Join(",", _BootStrapVersioniDisponibili))
        End If

        Dim str As New StringBuilder
        str.Append("<script src=""" & Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.bootstrap." & _VersioneBootstrap & ".min.js") & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")

        writer.Write(str.ToString)
        MyBase.Render(writer)


    End Sub


    Private Sub AppendToHeader(ByVal _basePath As String, ByVal puntoInterrogativo As String, ByVal css As String)
        Dim includeTemplate As String = "<link rel='stylesheet' type='text/css' href='{0}' />" & vbCrLf
        Dim include As New LiteralControl([String].Format(includeTemplate, Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.bootstrap." & _VersioneBootstrap & ".min.css") & puntoInterrogativo & _GiasVersioneCorrente))
        'Page.Header.Controls.Add(include)
        _bootstrapPlaceHeader.Controls.Add(include)
    End Sub

    Private Sub bootstrap_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        Inizializza()

        Dim PuntoInterrogativo As String = "?"

        If GiasVersioneCorrente.StartsWith("?") Then
            PuntoInterrogativo = ""
        End If

        If Not _bootstrapPlaceHeader Is Nothing Then
            AppendToHeader(_BasePath, PuntoInterrogativo, "kendo.common-bootstrap.min.css")
        End If


    End Sub

    'Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)

    '    writer.AddStyleAttribute("SitoOspite", "0")

    '    MyBase.AddAttributesToRender(writer)
    'End Sub

#End Region

End Class

