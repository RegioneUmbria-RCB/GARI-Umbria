Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class AgroVersioneHeader
    Inherits AgroControlliCommons
    Implements iAgronicaControlliCommons


    Private _SitoOspite As AgronicaCoreDataProvider.TipiEnumerativi.Enum_SiteRedirector =
        AgronicaCoreDataProvider.TipiEnumerativi.Enum_SiteRedirector.GiasLan

    Private _versioneHeaderPlaceHolder As PlaceHolder
    Private _versioneHeader As String = ""

    Private _headerVersionDisponibili As String() = {VERSIONE_HEADER_DEFAULT, "2022"}

    Public Property MemorizzaInSessioneDopoLettura As Boolean = True

    Public Property VersioneHeader As String
        Get
            Return _versioneHeader
        End Get
        Set(value As String)
            SetVersion(value)
        End Set
    End Property

    Public Property VersioneHeaderPlaceHolder As PlaceHolder
        Get
            Return _versioneHeaderPlaceHolder
        End Get
        Set(value As PlaceHolder)
            _versioneHeaderPlaceHolder = value
        End Set
    End Property

    Public Property SitoOspite As TipiEnumerativi.Enum_SiteRedirector
        Get
            Return _SitoOspite
        End Get
        Set(value As TipiEnumerativi.Enum_SiteRedirector)
            _SitoOspite = value
        End Set
    End Property

    Protected Overrides Sub Inizializza()

        MyBase.inizializza()

        If _versioneHeaderPlaceHolder Is Nothing Then
            Throw New Exception("AgroVersioneHeader: Non è stato specificato il placeholder dove impostare i dati dell'header HTML")
        End If

        If _SitoOspite = TipiEnumerativi.Enum_SiteRedirector.GiasLan Then
            Throw New Exception("AgroVersioneHeader: Non è stato specificato il sito ospite per una corretta gestione della versione per lo stile della Header nella Master Page.")
        End If

        If _versioneHeader <> "" Then
            Exit Sub
        End If

        Dim objParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        'cerco versione personalizzata su db Server
        _versioneHeader = LeggiDaSessioneOppureDaConfigSiti("VersioneHeader" & _SitoOspite, "", TipiEnumerativi.agronicacoreparametri_tipoDB.Server, MemorizzaInSessioneDopoLettura)

        'cerco versione non personalizzata su db Server
        If String.IsNullOrEmpty(_versioneHeader) Then
            _versioneHeader = LeggiDaSessioneOppureDaConfigSiti("VersioneHeader", "", TipiEnumerativi.agronicacoreparametri_tipoDB.Server, MemorizzaInSessioneDopoLettura)
        End If

        If String.IsNullOrEmpty(_versioneHeader) Then
            'cerco versione personalizzata su Super Server
            _versioneHeader = LeggiDaSessioneOppureDaConfigSiti("VersioneHeader" & _SitoOspite, "", TipiEnumerativi.agronicacoreparametri_tipoDB.SuperServer, MemorizzaInSessioneDopoLettura)
        End If

        If String.IsNullOrEmpty(_versioneHeader) Then

            'cerco versione comune per tutti ed in deficit uso la versione di default
            _versioneHeader = LeggiDaSessioneOppureDaConfigSiti("VersioneHeader", "VersioneHeader", TipiEnumerativi.agronicacoreparametri_tipoDB.SuperServer, MemorizzaInSessioneDopoLettura)

            If _versioneHeader.ToLower = "VersioneHeader" Or _versioneHeader = "" Then
                _versioneHeader = VERSIONE_HEADER_DEFAULT
            End If

        End If

        If MemorizzaInSessioneDopoLettura Then
            HttpContext.Current.Session("ASG_M_VersioneHeader") = _versioneHeader
        End If

    End Sub

    Private Sub SetVersion(versione As String)
        If Not _headerVersionDisponibili.Contains(versione) Then
            Throw New Exception("Versione Header " & versione & " sconosciuta, valori ammessi: " & String.Join(",", _headerVersionDisponibili))
        End If
        _versioneHeader = versione
    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        Inizializza()

    End Sub

    Private Sub AppendToHeader(control As LiteralControl)
        _versioneHeaderPlaceHolder.Controls.Add(control)
    End Sub

    Private Sub AgroVersioneHeader_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Inizializza()
    End Sub
End Class
