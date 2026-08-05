Imports System.Text
Imports Agronica.Helpers.GiasBase
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class AgroKendo
    Inherits AgroControlliCommons
    Implements iAgronicaControlliCommons


    Private _SitoOspite As Enum_SiteRedirector = Enum_SiteRedirector.GiasLan

    Private _kendoPlaceHeader As PlaceHolder


    Public Property VersioneKendo As String
        Get
            Return _VersioneKendo
        End Get
        Set(value As String)
            _VersioneKendo = value
        End Set
    End Property

    Public Property KendoPlaceHeader As PlaceHolder
        Get
            Return _kendoPlaceHeader
        End Get
        Set(value As PlaceHolder)
            _kendoPlaceHeader = value
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

    Private _VersioneKendo As String = ""


    Protected Overrides Sub Inizializza()

        MyBase.inizializza()

        If _kendoPlaceHeader Is Nothing Then
            Throw New Exception("AgroKendo: Non è stato specificato il placeholder dove impostare i dati dell'header HTML")
        End If

        If _SitoOspite = Enum_SiteRedirector.GiasLan Then
            Throw New Exception("AgroKendo: Non è stato specificato il sito ospite per una corretta gestione della versione Kendo.")
        End If

        If _VersioneKendo <> "" Then
            Exit Sub
        End If

        Dim NomeRoutine As String = "AgroKendo.Inizializza"

        'cerco versione personalizzata su db Server
        _VersioneKendo = LeggiDaSessioneOppureDaConfigSiti("VersioneKendo" & _SitoOspite, "", agronicacoreparametri_tipoDB.Server)

        If String.IsNullOrEmpty(_VersioneKendo) Then
            'cerco versione personalizzata su db super server
            _VersioneKendo = LeggiDaSessioneOppureDaConfigSiti("VersioneKendo" & _SitoOspite, "", agronicacoreparametri_tipoDB.SuperServer)
        End If

        '_VersioneKendo = "2022.3.1109"
        '_VersioneKendo = "2023.2.606"

        If String.IsNullOrEmpty(_VersioneKendo) Then

            'cerco versione comune per tutti ed in deficit uso la versione 2022.3.1109
            _VersioneKendo = LeggiDaSessioneOppureDaConfigSiti("VersioneKendo", "UltimaVersione", agronicacoreparametri_tipoDB.SuperServer)

            If _VersioneKendo.ToLower = "ultimaversione" OrElse _VersioneKendo = "" Then

                _VersioneKendo = "2022.3.1109"

            End If

        End If

        'IMPORTANTE: Memorizzare sempre in sessione la versione Kendo perché verrà usata successivamente da AgroVersioneMaster per includere ulteriori css
        HttpContext.Current.Session("ASG_M_VersioneKendo") = _VersioneKendo

    End Sub


    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        Inizializza()

        Dim PuntoInterrogativo As String = "?"

        If GiasVersioneCorrente.StartsWith("?") Then
            PuntoInterrogativo = ""
        End If

        Dim str As New StringBuilder

        str.Append("<script src=""" & _LinkGiasBase & "kendoui/" & _VersioneKendo & "/js/jszip.min.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")

        str.Append("<script src=""" & _LinkGiasBase & "kendoui/" & _VersioneKendo & "/js/kendo.all.min.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")
        str.Append("<script src=""" & _LinkGiasBase & "kendoui/" & _VersioneKendo & "/js/messages/kendo.messages." & Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.CultureName & ".min.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")
        str.Append("<script src=""" & _LinkGiasBase & "kendoui/" & _VersioneKendo & "/js/cultures/kendo.culture.it-IT.min.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")

        str.Append("<script src=""" & _LinkGiasBase & "kendoui/ScriptsGiasKendo/" & _VersioneKendo & "/funzioniComuniKendoConfigurazioni.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")
        str.Append("<script src=""" & _LinkGiasBase & "kendoui/ScriptsGiasKendo/" & _VersioneKendo & "/funzioniComuniKendoDropDown.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")
        str.Append("<script src=""" & _LinkGiasBase & "kendoui/ScriptsGiasKendo/" & _VersioneKendo & "/funzioniComuniKendoChart.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")
        str.Append("<script src=""" & _LinkGiasBase & "kendoui/ScriptsGiasKendo/" & _VersioneKendo & "/funzioniComuniKendoGrid.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")
        str.Append("<script src=""" & _LinkGiasBase & "kendoui/ScriptsGiasKendo/" & _VersioneKendo & "/funzioniComuniKendoPivotGrid.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")
        str.Append("<script src=""" & _LinkGiasBase & "kendoui/ScriptsGiasKendo/" & _VersioneKendo & "/funzioniComuniKendoTreeList.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")
        str.Append("<script src=""" & _LinkGiasBase & "kendoui/ScriptsGiasKendo/" & _VersioneKendo & "/funzioniComuniKendoDatePicker.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")
        str.Append("<script src=""" & _LinkGiasBase & "kendoui/ScriptsGiasKendo/" & _VersioneKendo & "/funzioniComuniKendoTreeView.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")
        str.Append("<script src=""" & _LinkGiasBase & "kendoui/ScriptsGiasKendo/" & _VersioneKendo & "/funzioniComuniKendoDialogTreeViewFilter.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")
        str.Append("<script src=""" & _LinkGiasBase & "kendoui/ScriptsGiasKendo/" & _VersioneKendo & "/funzioniComuniKendoInputVari.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")
        str.Append("<script src=""" & _LinkGiasBase & "kendoui/ScriptsGiasKendo/" & _VersioneKendo & "/funzioniComuniKendoEditor.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")
        str.Append("<script src=""" & _LinkGiasBase & "kendoui/ScriptsGiasKendo/" & _VersioneKendo & "/funzioniComuniKendoNotification.js" & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")


        writer.Write(str.ToString)
        MyBase.Render(writer)


    End Sub

    Private Sub AgroKendo_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        Inizializza()

        Dim PuntoInterrogativo As String = "?"

        If GiasVersioneCorrente.StartsWith("?") Then
            PuntoInterrogativo = ""
        End If

        If Me.Page.ToString = "ASP.index_aspx" Then
            GiasBaseHelper.Setta_Link_GiasBase("ASG_objParametri_Super_Server", _LinkGiasBase)
        ElseIf Me.Page.ToString = "ASP.login_login_aspx" Then
            GiasBaseHelper.Setta_Link_GiasBase("ASG_objParametri_Super_Server", _LinkGiasBase)
        Else
            _LinkGiasBase = MyBase.PATH_GIASBASE
        End If

        If Not String.IsNullOrEmpty(_VersioneKendo) Then
            Dim annoVersione As String = _VersioneKendo.Split({"."c})(0)

            If IsNumeric(annoVersione) AndAlso CInt(annoVersione) >= 2023 Then

                'Nuove versioni con temi SASS
                AppendToHeader(PuntoInterrogativo, "bootstrap-3.css")

            Else

                'Vecchie versioni con temi LESS
                AppendToHeader(PuntoInterrogativo, "kendo.common-bootstrap.min.css")
                'AppendToHeader(PuntoInterrogativo, "kendo.common.min.css")
                AppendToHeader(PuntoInterrogativo, "kendo.bootstrap.min.css")
                AppendToHeader(PuntoInterrogativo, "kendo.bootstrap.mobile.min.css")

            End If

        End If

        Dim includeTemplate As String = "<link rel='stylesheet' type='text/css' href='{0}' />" & vbCrLf
        Dim include As New LiteralControl([String].Format(includeTemplate, _LinkGiasBase & "kendoui/StylesGiasKendo/" & _VersioneKendo & "/Gias_Kendo.css" & PuntoInterrogativo & _GiasVersioneCorrente))
        _kendoPlaceHeader.Controls.Add(include)

    End Sub


    Private Sub AppendToHeader(ByVal puntoInterrogativo As String, ByVal css As String)

        Dim includeTemplate As String = "<link rel='stylesheet' type='text/css' href='{0}' />" & vbCrLf
        Dim include As New LiteralControl([String].Format(includeTemplate, _LinkGiasBase & "kendoui/" & _VersioneKendo & "/styles/" & css & puntoInterrogativo & _GiasVersioneCorrente))
        _kendoPlaceHeader.Controls.Add(include)
    End Sub

End Class
