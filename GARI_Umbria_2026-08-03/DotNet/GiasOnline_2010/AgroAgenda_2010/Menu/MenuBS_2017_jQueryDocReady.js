var menuBS2017Resx;

$(document).ready(function () {

    var agronicaAgenda2010Resx = readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx", "MenuBS_2017_jQueryDocReady.js");

    // Gestione vecchia (attualmente utilizzata) con funzione Traduzione
    menuBS2017Resx = agronicaAgenda2010Resx;
    // Gestione nuova (attualmente non necessaria per questo aspx, in quanto necessita delle risorse del solo resx globale) con TraduzioneMultiResx, 
    // per utilizzarla è necessario sostituire i richiami alla funzione singola e dichiarare la variabile come array di oggetti
    //menuBS2017Resx.push(agronicaAgenda2010Resx);

    // La variabile datiMeteoResx, utilizzata per i js dei dati del meteo e degli indicatori DSS necessita anche delle traduzioni dal file AgronicaAgenda_2010
    if (!Array.isArray(datiMeteoResx)) {
        datiMeteoResx = [];
    }
    datiMeteoResx.push(agronicaAgenda2010Resx);

    // leggo il widget degli allarmi
    LeggiWidgetAllarmi();

    // leggo e scrivo il meteo
    getWeather("dashboard");

    // per lo scroll delle previsioni
    scrollInit();

    // apro il menu
    LeggiMacrosezioni();

    // aggiungi i preferiti
    LeggiPreferitiDash();

    $("#widget-meteo-2").RiepilogoMeteo({
        url_meteoWS: "../DataAnalisiBI/Meteo/MeteoWS.aspx",
        summary: true,
        msg_wait: Traduzione(menuBS2017Resx, "RiepilogoDatiMeteoInAttesaDiElaborazione", "Riepilogo dati meteo - In attesa di elaborazione..."),
        msg_na: Traduzione(menuBS2017Resx, "RiepilogoDatiMeteoNonDisponibili", "Riepilogo dati meteo non disponibili"),
        msg_err: Traduzione(menuBS2017Resx, "RiepilogoDatiMeteoErroreInElaborazione", "Riepilogo dati meteo - Errore in elaborazione")
    });

    $("#widget-gauges").DSS_Summary({
        url_meteoWS: "../DataAnalisiBI/Meteo/MeteoWS.aspx",
        msg_summary: Traduzione(menuBS2017Resx, "RiepilogoIndicatoriEmergenzeDSS", "Riepilogo indicatori emergenze DSS"),
        msg_wait: Traduzione(menuBS2017Resx, "IndicatoriEmergenzeDSSInAttesaDiElaborazione", "Indicatori emergenze DSS - In attesa di elaborazione..."),
        msg_na: Traduzione(menuBS2017Resx, "IndicatoriEmergenzeDSSNonDisponibili", "Indicatori emergenze DSS non disponibili"),
        msg_err: Traduzione(menuBS2017Resx, "IndicatoriEmergenzeDSSErroreInElaborazione", "Indicatori emergenze DSS - Errore in elaborazione"),
        gauge_click_callback: function (params) {

            let url = "../DataAnalisiBI/Meteo/DSS_Difesa.aspx?";
            url += "Parametri=" + encodeURIComponent(JSON.stringify(params));

            window.location = url;
        },
        date_range: SonoLoggatoComeSuperUser
        //,fun_callback: function (status) { if (status != 0) $("#widget_emergenze").show(); else $("#widget_emergenze").hide(); }
    });


    $("#widget-suolo").MonitorSuolo({
        url_meteoWS: "../DataAnalisiBI/Meteo/MeteoWS.aspx",
        //window_title: Stringa titolo finestra popup
        msg_wait: Traduzione(menuBS2017Resx, "MonitoraggioSuoloInAttesaDiElaborazione", "Monitoraggio suolo - In attesa di elaborazione..."),
        msg_na: Traduzione(menuBS2017Resx, "MonitoraggioSuoloNonDisponibile", "Monitoraggio suolo non disponibile"),
        msg_err: Traduzione(menuBS2017Resx, "MonitoraggioSuoloErroreInElaborazione", "Monitoraggio suolo - Errore in elaborazione"),
        fun_callback: function (status) {
            //Funzione chiamata al termine dell'elaborazione (parametro status: -1 errore, 0 nulla da visualizzare, 1 ok)
            if (status === 0) {
                $("#widget-suolo").addClass("elemento-nascosto");
            }
        }
    });

    if (pivaAziendaSelezionata !== "") {
        if ($("#widget-meteo-2").data("RiepilogoMeteo") !== undefined)
            $("#widget-meteo-2").data("RiepilogoMeteo").show(pivaAziendaSelezionata);
        if ($("#widget-gauges").data("DSS_Summary") !== undefined)
            $("#widget-gauges").data("DSS_Summary").show(pivaAziendaSelezionata);
        if ($("#widget-suolo").data("MonitorSuolo") !== undefined)
            $("#widget-suolo").data("MonitorSuolo").show(pivaAziendaSelezionata);
    } else {
        //$("#widget_meteo_2").hide();
        //$("#widget_emergenze").hide();
    }

});

