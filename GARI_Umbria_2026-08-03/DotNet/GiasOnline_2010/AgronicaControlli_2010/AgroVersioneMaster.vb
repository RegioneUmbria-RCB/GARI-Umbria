Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class AgroVersioneMaster
    Inherits AgroControlliCommons
    Implements iAgronicaControlliCommons


    Private _SitoOspite As Enum_SiteRedirector = Enum_SiteRedirector.GiasLan

    Private _versioneMasterPlaceHolder As PlaceHolder
    Private _versioneMaster As String = ""
    Private ReadOnly _masterVersionDisponibili As String() = {VERSIONE_MASTER_DEFAULT, "2022"}

    Public Property MemorizzaInSessioneDopoLettura As Boolean = False
    Private Property ListaStiliPersonalizzati2022 As String()

    Public Property VersioneMaster As String
        Get
            Return _versioneMaster
        End Get
        Set(value As String)
            SetVersion(value)
        End Set
    End Property

    Public Property VersioneMasterPlaceHolder As PlaceHolder
        Get
            Return _versioneMasterPlaceHolder
        End Get
        Set(value As PlaceHolder)
            _versioneMasterPlaceHolder = value
        End Set
    End Property

    Public Property SitoOspite As Enum_SiteRedirector
        Get
            Return _SitoOspite
        End Get
        Set(value As Enum_SiteRedirector)
            _SitoOspite = value
        End Set
    End Property

    Protected Overrides Sub Inizializza()

        MyBase.inizializza()

        If _versioneMasterPlaceHolder Is Nothing Then
            Throw New Exception("AgroVersioneMaster: Non è stato specificato il placeholder dove impostare i dati dell'header HTML")
        End If

        If _SitoOspite = Enum_SiteRedirector.GiasLan Then
            Throw New Exception("AgroVersioneMaster: Non è stato specificato il sito ospite per una corretta gestione della versione per lo stile della Master Page.")
        End If

        'Leggo gli stili personalizzati aggiuntivi per cliente
        ListaStiliPersonalizzati2022 = LeggiStiliPersonalizzati()

        If _versioneMaster <> "" Then
            Exit Sub
        End If

        Dim objParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        'cerco versione personalizzata su db Server
        _versioneMaster = LeggiDaSessioneOppureDaConfigSiti("VersioneMaster" & _SitoOspite, "", agronicacoreparametri_tipoDB.Server, MemorizzaInSessioneDopoLettura)

        'cerco versione non personalizzata su db Server
        If String.IsNullOrEmpty(_versioneMaster) Then
            _versioneMaster = LeggiDaSessioneOppureDaConfigSiti("VersioneMaster", "", agronicacoreparametri_tipoDB.Server, MemorizzaInSessioneDopoLettura)
        End If

        If String.IsNullOrEmpty(_versioneMaster) Then
            'cerco versione personalizzata su Super Server
            _versioneMaster = LeggiDaSessioneOppureDaConfigSiti("VersioneMaster" & _SitoOspite, "", agronicacoreparametri_tipoDB.SuperServer, MemorizzaInSessioneDopoLettura)
        End If

        If String.IsNullOrEmpty(_versioneMaster) Then

            'cerco versione comune per tutti ed in deficit uso la versione di default
            _versioneMaster = LeggiDaSessioneOppureDaConfigSiti("VersioneMaster", "UltimaVersione", agronicacoreparametri_tipoDB.SuperServer, MemorizzaInSessioneDopoLettura)

            If _versioneMaster.ToLower = "ultimaversione" OrElse _versioneMaster = "" Then
                _versioneMaster = VERSIONE_MASTER_DEFAULT
            End If

        End If

        If MemorizzaInSessioneDopoLettura Then
            HttpContext.Current.Session("ASG_M_VersioneMaster") = _versioneMaster
        End If

    End Sub

    Private Sub SetVersion(versione As String)
        If Not _masterVersionDisponibili.Contains(versione) Then
            Throw New Exception("Versione Master " & versione & " sconosciuta, valori ammessi: " & String.Join(",", _masterVersionDisponibili))
        End If
        _versioneMaster = versione
    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        Inizializza()

    End Sub

    Private Sub AppendToHeader(control As LiteralControl)
        _versioneMasterPlaceHolder.Controls.Add(control)
    End Sub

    Private Sub AppendCss(ByVal css As String)
        Dim PuntoInterrogativo As String = "?"

        If GiasVersioneCorrente.StartsWith("?") Then
            PuntoInterrogativo = ""
        End If
        Dim includeTemplate As String = "<link rel='stylesheet' href='{0}' media='screen'/>" & vbCrLf

        Dim include As New LiteralControl([String].Format(includeTemplate, PATH_GIASBASE & css & PuntoInterrogativo & _GiasVersioneCorrente))
        AppendToHeader(include)
    End Sub

    Private Sub AgroVersioneMaster_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        Inizializza()

        'Dim PuntoInterrogativo As String = "?"

        'If GiasVersioneCorrente.StartsWith("?") Then
        '    PuntoInterrogativo = ""
        'End If
        'Dim includeTemplate As String = "<link rel='stylesheet' href='{0}' media='screen'/>" & vbCrLf

        'Dim include As New LiteralControl([String].Format(includeTemplate, PATH_GIASBASE & "agronica/Styles/bootstrap_AGRONICA.css" & PuntoInterrogativo & _GiasVersioneCorrente))
        'AppendToHeader(include)

        AppendCss("agronica/Styles/bootstrap_AGRONICA.css")

        Select Case _versioneMaster
            Case "2022"
                AppendCss("agronica/Styles/styleGiasUtils.css")
                AppendCss("agronica/Styles/styleGiasIcons.css")
                AppendCss("agronica/Styles/styleGiasComponents.css")
                AppendCss("agronica/Styles/styleGiasPages.css")

                If _SitoOspite = Enum_SiteRedirector.Sito_AgronicaAudit Then
                    AppendCss("agronica/Styles/styleGiasAudit.css")
                End If

                'Per la nuova versione di Kendo >= 2023 è necessario aggiungere un ulteriore css
                Dim verKendoSession As String = CStr(HttpContext.Current.Session("ASG_M_VersioneKendo"))

                If Not String.IsNullOrEmpty(verKendoSession) Then

                    Dim annoVersione As String = verKendoSession.Split({"."c})(0)

                    If IsNumeric(annoVersione) AndAlso CInt(annoVersione) >= 2023 Then
                        AppendCss("agronica/Styles/styleGiasUtils2023.css")
                        AppendCss("agronica/Styles/styleGiasIcons2023.css")
                        AppendCss("agronica/Styles/styleGiasComponents2023.css")
                        AppendCss("agronica/Styles/styleGiasPages2023.css")
                        If _SitoOspite = Enum_SiteRedirector.Sito_AgronicaAudit Then
                            AppendCss("agronica/Styles/styleGiasAudit2023.css")
                        End If
                    End If

                End If

                'Leggo gli stili personalizzati aggiuntivi per cliente
                If ListaStiliPersonalizzati2022 IsNot Nothing Then
                    For Each curStile As String In ListaStiliPersonalizzati2022
                        If curStile <> "" Then
                            AppendCss("agronica/Styles/" & curStile)
                        End If
                    Next
                End If

            Case Else
                Return
        End Select

    End Sub

    Private Function LeggiStiliPersonalizzati() As String()
        Dim ListaStiliPersonalizzati As String = LeggiDaSessioneOppureDaConfigSiti("ListaStiliPersonalizzati_2022", "",
                                                                                   agronicacoreparametri_tipoDB.Server, paramSessioneObjParametriValue:="")

        If ListaStiliPersonalizzati = "" Then
            ListaStiliPersonalizzati = LeggiDaSessioneOppureDaConfigSiti("ListaStiliPersonalizzati_2022", "",
                                                                         agronicacoreparametri_tipoDB.SuperServer)
        End If

        If ListaStiliPersonalizzati <> "" Then
            Return ListaStiliPersonalizzati.Split(";")
        Else
            Return Nothing
        End If

    End Function

End Class
