Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieDAL

Public Class jquery
    Inherits AgroControlliCommons

    Private _VersionejQuery As String = ""
    Private _VersionejQueryBase As String = "3.5.1"

    Private _JQueryVersionDisponibili As String() = {"1.12.3", "3.5.1"}



    Private _SitoOspite As TipiEnumerativi.Enum_SiteRedirector =
        AgronicaCoreDataProvider.TipiEnumerativi.Enum_SiteRedirector.Sito_AgronicaAgenda_2010

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
            Throw New Exception("AgrojQuery: Non è stato specificato il sito ospite per una corretta gestione della versione jQuery.")
        End If


        If _VersionejQuery <> "" Then
            Exit Sub
        End If


        Dim NomeRoutine As String = "AgrojQuery.Inizializza"

        Dim sessionServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objLog As New AgronicaCoreDataProvider.LogProvider


        'cerco versione personalizzata su db Server
        If Not sessionServer Is Nothing Then
            _VersionejQuery = LeggiDaSessioneOppureDaConfigSiti("VersionejQuery" & _SitoOspite, "", TipiEnumerativi.agronicacoreparametri_tipoDB.Server, MemorizzaInSessioneDopoLettura)
        End If


        'cerco versione non personalizzata su db Server
        If String.IsNullOrEmpty(_VersionejQuery) Then
            _VersionejQuery = LeggiDaSessioneOppureDaConfigSiti("VersionejQuery", "", TipiEnumerativi.agronicacoreparametri_tipoDB.Server, MemorizzaInSessioneDopoLettura)
        End If

        If String.IsNullOrEmpty(_VersionejQuery) Then
            'cerco versione personalizzata
            _VersionejQuery = LeggiDaSessioneOppureDaConfigSiti("VersionejQuery" & _SitoOspite, "", TipiEnumerativi.agronicacoreparametri_tipoDB.SuperServer, MemorizzaInSessioneDopoLettura)
        End If



        If String.IsNullOrEmpty(_VersionejQuery) Then

            'cerco versione comune per tutti ed in deficit uso la versione 2017.3.913
            _VersionejQuery = LeggiDaSessioneOppureDaConfigSiti("VersionejQuery", "UltimaVersione", TipiEnumerativi.agronicacoreparametri_tipoDB.SuperServer, MemorizzaInSessioneDopoLettura)

            If _VersionejQuery.ToLower = "ultimaversione" Or _VersionejQuery = "" Then

                'Dim xBase As String = ""
                Dim xjQuery As String = ""
                Dim xRisp As String = ""


                _VersionejQuery = _VersionejQueryBase


            End If

        End If

        If MemorizzaInSessioneDopoLettura Then
            HttpContext.Current.Session("ASG_M_VersionejQuery") = _VersionejQuery
        End If

    End Sub


    Public Property Versione As String
        Get
            Return _VersionejQuery
        End Get
        Set(value As String)
            SetVersion(value)
        End Set
    End Property


    ''' <summary>
    ''' imposta una versione fra quelle disponibili.
    ''' </summary>
    ''' <param name="versione">"1.12.3", "3.5.1"</param>
    Private Sub SetVersion(versione As String)
        If Not _JQueryVersionDisponibili.Contains(versione) Then
            Throw New Exception("Versione JQuery " & versione & " sconosciuta, valori ammessi: " & String.Join(",", _JQueryVersionDisponibili))
        End If
        _VersionejQuery = versione
    End Sub

#Region "Methods & Event Handlers"


    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        Inizializza()

        Dim PuntoInterrogativo As String = "?"

        If GiasVersioneCorrente.StartsWith("?") Then
            PuntoInterrogativo = ""
        End If

        If Not _JQueryVersionDisponibili.Contains(_VersionejQuery) Then
            Throw New Exception("Versione JQuery " & _VersionejQuery & " sconosciuta, valori ammessi: " & String.Join(",", _JQueryVersionDisponibili))
        End If

        Dim str As New StringBuilder
        str.Append("<script src=""" & Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.jquery.min." & _VersionejQuery & ".js") & PuntoInterrogativo & _GiasVersioneCorrente & """ type=""text/javascript""></script>")

        writer.Write(str.ToString)
        MyBase.Render(writer)


    End Sub

    'Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)

    '    writer.AddStyleAttribute("SitoOspite", "0")

    '    MyBase.AddAttributesToRender(writer)
    'End Sub

#End Region

End Class
