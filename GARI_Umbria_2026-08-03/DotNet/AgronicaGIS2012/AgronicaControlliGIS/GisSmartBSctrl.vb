Imports System.IO
Imports System.Text
Imports System.Web
Imports AgronicaCoreVarieDAL

Public Class GisSmartBSctrl
    Inherits AgronicaControlli_2010.AgroControlliCommons
    Implements AgronicaControlli_2010.iAgronicaControlliCommons

    Private _pivaSelezionataDaMenu As String = ""
    Private _ragSocSelezionataDaMenu As String = ""
    Private _MapsSeparator As String = "+"

    Private _IntegrazionePaginaGis As Boolean

    Public Property IntegrazionePaginaGis As Boolean
        Get
            Return _IntegrazionePaginaGis
        End Get
        Set(value As Boolean)
            _IntegrazionePaginaGis = value
        End Set
    End Property

    Public Property Attivo As Boolean
        Get
            Return _Attivo
        End Get
        Set(value As Boolean)
            _Attivo = value
        End Set
    End Property

    Public Property AnagraficaAttiva As Boolean
        Get
            Return _AnagraficaAttiva
        End Get
        Set(value As Boolean)
            _AnagraficaAttiva = value
        End Set
    End Property

    Public Property absolutePath As String
    Private _Attivo As Boolean

    Private _AnagraficaAttiva As Boolean

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)


        Dim str As New StringBuilder

        If Not Attivo Then
            str.Append("<div id = 'GisSmartBSctrl'style='display:none' aria-attivo='false'>NON ATTIVO</div>")

            writer.Write(str.ToString)
            MyBase.Render(writer)
            Exit Sub
        End If

        inizializza()

        Dim PuntoInterrogativo As String = "?"

        If GiasVersioneCorrente.StartsWith("?") Then
            PuntoInterrogativo = ""
        End If

        Dim ComponentePrincipale As String = ""

        'aggiungo il css
        Dim provahtml As New StringBuilder

        Dim myCSS As String = "css_GisSmartBS.css"

        Dim appCSS As String = "<style>@import url('" & Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlliGIS." & myCSS) & "');</style>"
        str.Append(appCSS)

        getResource("AgronicaControlliGIS.GisSmartBS.htm", ComponentePrincipale)



        If String.IsNullOrEmpty(absolutePath) Then
            absolutePath = Path.GetDirectoryName(HttpContext.Current.Request.Url.AbsolutePath)
        End If


        ComponentePrincipale = ComponentePrincipale.Replace("__basePath__", absolutePath)

        ComponentePrincipale = ComponentePrincipale.Replace("__pivaSelezionataDaMenu__", _pivaSelezionataDaMenu)
        ComponentePrincipale = ComponentePrincipale.Replace("ragSocSelezionataDaMenu__", _ragSocSelezionataDaMenu)
        ComponentePrincipale = ComponentePrincipale.Replace("__MapsSeparator__", _MapsSeparator)
        ComponentePrincipale = ComponentePrincipale.Replace("__formGisSmartBsAnagraficaAttiva__", _AnagraficaAttiva.ToString.ToLower)




        Dim agronicaScripts As New List(Of String)


        str.AppendLine("<script type='text/javascript' src='" + PATH_GIASBASE + "agronica/Scripts/jquery.cookie.js'></script>")

        agronicaScripts.Add("gissmartBS_Localization.js")
        agronicaScripts.Add("gissmartCommons.js")
        agronicaScripts.Add("gissmartBS.js")
        agronicaScripts.Add("gissmartBS_jQuery_DocReady.js")
        agronicaScripts.Add("gissmartBS_KendoUI.js")

        'questi riferimenti esistono già se siamo in pagina GIS
        If Not IntegrazionePaginaGis Then
            agronicaScripts.Add("Costanti_Gis.js")
            agronicaScripts.Add("Utility.js")
            agronicaScripts.Add("Mappa.js")
            agronicaScripts.Add("Shape.js")
            agronicaScripts.Add("Interfaccia.js")
            agronicaScripts.Add("Numeri.js")
            agronicaScripts.Add("gMapsUtility.js")
        End If

        agronicaScripts.Add("gissmartBS_GMaps.js")


        For Each s In agronicaScripts
            str.AppendLine("<script type='text/javascript' src='" & absolutePath & "/Gis_Scripts/" & s & "?" & GiasVersioneCorrente & "'></script>")
        Next

        Dim app As String = Utilita_Compressione.RemoveWhitespaceFromHtml(ComponentePrincipale & str.ToString)

        writer.Write(app)
        MyBase.Render(writer)


    End Sub


    Private Shared Sub getResource(ByVal risorsa As String, ByRef str_e As String)

        'Dim readStream  New FileStream("c:\testBinary.dat", FileMode.Open)
        Dim readBinary As New BinaryReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(risorsa))

        str_e = readBinary.ReadChars(1)
        str_e = ""

        Dim length As Integer = readBinary.BaseStream.Length
        Dim allData As Byte() = readBinary.ReadBytes(length)

        str_e = System.Text.Encoding.UTF8.GetString(allData)

        readBinary.Close()

    End Sub

    Private Sub GisSmartBSctrl_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If Not IntegrazionePaginaGis Then

            Dim awc As New AgronicaCoreGestioneRichieste.AgroWebConfig

            Dim gMaps As String = "https://maps.googleapis.com/maps/api/js?key=&sensor=false&libraries=drawing,geometry"
            Dim gMapsApi As String = "https://www.google.com/jsapi"

            If awc.GoogleMaps <> "" Then
                gMaps = awc.GoogleMaps
            End If

            If awc.googlemaps_jsapi <> "" Then
                gMapsApi = awc.googlemaps_jsapi
            End If

            If Debugger.IsAttached Then
                gMaps = "https://maps.googleapis.com/maps/api/js?v=3.exp&client=gme-addictive&sensor=false&libraries=drawing,geometry"
            End If

            'Page.ClientScript.RegisterClientScriptInclude("googlemaps", "https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&libraries=drawing,geometry")
            Page.ClientScript.RegisterClientScriptInclude("googlemaps", gMaps)
            Page.ClientScript.RegisterClientScriptInclude("jsapi", gMapsApi)
        End If
    End Sub
End Class
