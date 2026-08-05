Imports System.Text
Imports AgronicaCoreVarieDAL

Public Class AgroMeteo
    Inherits AgroControlliCommons

    Private _meteo_headerPlaceHeader As PlaceHolder

    Public Property modalita As String = ""
    Public Property altezza As String = "100px"

    Public Property Meteo_headerPlaceHeader As PlaceHolder
        Get
            Return _meteo_headerPlaceHeader
        End Get
        Set(value As PlaceHolder)
            _meteo_headerPlaceHeader = value
        End Set
    End Property

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        Dim IDDiv As String = "AgroMeteo" & Me.ClientID

        'aggiungo il css
        Dim provahtml As New StringBuilder

        Dim str As String = ""
        str = str & "<script type='text/javascript' src='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/index.js'></script>"
        'per la mappa nuova
        str = str & "<script type='text/javascript' src='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/leaflet-openweathermap-master/example/leaflet/leaflet.js'></script>"
        str = str & "<script type='text/javascript' src='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/leaflet-openweathermap-master/example/leaflet/Permalink.js'></script>"
        str = str & "<script type='text/javascript' src='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/leaflet-openweathermap-master/example/leaflet/Permalink.Layer.js'></script>"
        str = str & "<script type='text/javascript' src='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/leaflet-openweathermap-master/example/leaflet/Permalink.Overlay.js'></script>"
        str = str & "<script type='text/javascript' src='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/leaflet-openweathermap-master/example/leaflet/leaflet-flattrbutton.js'></script>"
        str = str & "<script type='text/javascript' src='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/leaflet-openweathermap-master/leaflet-openweathermap.js'></script>"
        str = str & "<script type='text/javascript' src='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/leaflet-openweathermap-master/example/leaflet/leaflet-languageselector.js'></script>"
        str = str & "<script type='text/javascript' src='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/leaflet-openweathermap-master/example/files/map_i18n.js'></script>"
        str = str & "<script type='text/javascript' src='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/leaflet-openweathermap-master/example/files/map.js'></script>"
        ' per la previsioni
        str = str & "<script type='text/javascript' src='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/weather.js'></script>"

        'per le mappe di google
        str = str & "<script type='text/javascript' src='https://maps.googleapis.com/maps/api/js?key=AIzaSyA2cJIYR1WcTQgp1VudoETCNvbfVIoG-ZQ'></script>"

        Dim ComponentePrincipale As New StringBuilder

        ComponentePrincipale.Clear()

        'per discriminare il meteo nella versione dashboard o nella versione classica
        If modalita = "dashboard" Then
            ' -------------------------------------
            ' -------------------------- previsioni
            ' -------------------------------------
            ComponentePrincipale.Append("<div class='row'>")
            ' -------------------------------------
            ' ------------------ previsioni odierne
            ' -------------------------------------
            ComponentePrincipale.Append("<div id='weather-data' Class='col-xs-6' style='cursor: pointer; height: " & altezza & " !important; max-height: " & altezza & " !important; filter: alpha(opacity=80); border-radius: 10px 0 0 10px; border-left: solid 1px; border-bottom: solid 1px; border-top: solid 1px; border-right: solid 1px; border-color: #008CBA;' onClick='javascript:apriDashboardMeteo();'>")
            ComponentePrincipale.Append("<img id='weather-icon' style='position: absolute; margin: auto; top: 0; width: auto; height: 90px;' runat='server' alt='img_weather' title='imgweather' percorso='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/AgroMeteoIcons/'>")
            ComponentePrincipale.Append("<b><span id='summaryup' style='position: absolute; margin-top: 0; top: 0;'><span id='citta_corrente'></span></b>")
            ComponentePrincipale.Append("<b><span id='summarydown' style='position: absolute; margin-bottom: 0; bottom: 0;'><span id='temperature'></span>°C - <span id='descriptions'></span></span></b>")
            'chiudo div previsioni odierne
            ComponentePrincipale.Append("</div>")
            ' -------------------------------------
            ' ------------------- previsioni future
            ' -------------------------------------
            ComponentePrincipale.Append("<div id='forecast_scroll' class='col-xs-6' id='forecast-data' style='overflow: hidden; height: " & altezza & " !important; max-height: " & altezza & " !important; filter: alpha(opacity=80); border-radius: 0 10px 10px 0; border-right: solid 1px; border-bottom: solid 1px; border-top: solid 1px; border-color: #008CBA;' onmouseover='scroll_enable=false' onmouseout='scroll_enable=true'>")
            'div pad
            ComponentePrincipale.Append("<div id='pad' style='margin-top: 100px; margin-bottom: 100px;'>")
            'giorno dopo
            ComponentePrincipale.Append("<div id='previsioni_uno' style='display: none;'>")
            ComponentePrincipale.Append("<b><span id='domani'></span>: </b><img id='weather-icon-tomorrow' style='position: relative;' runat='server' alt='img_weather' title='imgweather' percorso='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/AgroMeteoIcons/' height='20px' width='auto'>")
            ComponentePrincipale.Append("<span id='summary-tomorrow'> <span id='temperature-tomorrow'></span>°C con <span id='descriptions-tomorrow'></span></span><br>")
            ComponentePrincipale.Append("<br>")
            ComponentePrincipale.Append("</div>")
            'due giorni dopo
            ComponentePrincipale.Append("<div id='previsioni_due' style='display: none;'>")
            ComponentePrincipale.Append("<b><span id='dopodomani'></span>: </b><img id='weather-icon-tomorrowtwo' style='position: relative;' runat='server' alt='img_weather' title='imgweather' percorso='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/AgroMeteoIcons/' height='20px' width='auto'>")
            ComponentePrincipale.Append("<span id='summary-tomorrowtwo'> <span id='temperature-tomorrowtwo'></span>°C con <span id='descriptions-tomorrowtwo'></span></span><br>")
            ComponentePrincipale.Append("<br>")
            ComponentePrincipale.Append("</div>")
            'tre giorni dopo
            ComponentePrincipale.Append("<div id='previsioni_tre' style='display: none;'>")
            ComponentePrincipale.Append("<b><span id='dopoduegiorni'></span>: </b><img id='weather-icon-tomorrowtre' style='position: relative;' runat='server' alt='img_weather' title='imgweather' percorso='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/AgroMeteoIcons/' height='20px' width='auto'>")
            ComponentePrincipale.Append("<span id='summary-tomorrowtre'> <span id='temperature-tomorrowtre'></span>°C con <span id='descriptions-tomorrowtre'></span></span><br>")
            ComponentePrincipale.Append("<br>")
            ComponentePrincipale.Append("</div>")
            'quattro giorni dopo 
            ComponentePrincipale.Append("<div id='previsioni_quattro' style='display: none;'>")
            ComponentePrincipale.Append("<b><span id='dopotregiorni'></span>: </b><img id='weather-icon-tomorrowfour' style='position: relative;' runat='server' alt='img_weather' title='imgweather' percorso='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/AgroMeteoIcons/' height='20px' width='auto'>")
            ComponentePrincipale.Append("<span id='summary-tomorrowfour'> <span id='temperature-tomorrowfour'></span>°C con <span id='descriptions-tomorrowfour'></span></span><br>")
            ComponentePrincipale.Append("<br>")
            ComponentePrincipale.Append("</div>")
            'chiudo il div pad
            ComponentePrincipale.Append("</div>")
            'chiudo div previsioni future
            ComponentePrincipale.Append("</div>")
            ' -------------------------------------
            ' ------------------- chiudo previsioni
            ' -------------------------------------
            ComponentePrincipale.Append("</div>")
        Else
            ComponentePrincipale.Append("<img id='weather-icon' runat='server' alt='img_weather' title='imgweather' percorso='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/AgroMeteoIcons/' height='35px' width='auto'><span id='summary'><span id='temperature'></span></span> °C con <span id='descriptions'></span>")
        End If


        Dim app As String = Utilita_Compressione.RemoveWhitespaceFromHtml(ComponentePrincipale.ToString() & str)

        writer.Write(app)
        MyBase.Render(writer)
    End Sub

    Private Sub AgroMeteo_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        inizializza()

        'Questo CSS del meteo era precedentemente incluso direttamente sulla pagina, ma l'inclusione era fatta male,
        'pertanto in produzione non è mai stato caricato.
        'Per questioni storiche non rimuovo del tutto il file, ma lascio l'inclusione fatta bene e commentata
        'AppendCssToHeader("", "?", PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/AgroMeteo.css", _meteo_headerPlaceHeader)
        
        AppendCssToHeader("", "?", PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/leaflet-openweathermap-master/example/leaflet/leaflet.css", _meteo_headerPlaceHeader)
        AppendCssToHeader("", "?", PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/leaflet-openweathermap-master/leaflet-openweathermap.css", _meteo_headerPlaceHeader)
        AppendCssToHeader("", "?", PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/leaflet-openweathermap-master/example/files/map.css", _meteo_headerPlaceHeader)
        AppendCssToHeader("", "?", PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AgroMeteo/leaflet-openweathermap-master/example/leaflet/leaflet-languageselector.css", _meteo_headerPlaceHeader)

    End Sub

End Class
