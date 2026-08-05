// -------------------------------------------------------
// ---------------- OPENWEATHERMAP KEY -------------------
// -------------------------------------------------------

// chiave per le previsioni in forma testuale
var OpenWeatherAppKey_text = "4347e6c6d761ee6cd0cc43e68a747058";
// chiave per le previsioni in forma grafica
var OpenWeatherAppKey_map = "185477576fdf6d45a5da2ed59a801fb3";

// -------------------------------------------------------
// ----------------- GEOLOCALIZZAZIONE -------------------
// -------------------------------------------------------
// variabili globali per i valori di longitudine e latitudine: possono arrivare dai campi di testo o dal gps
var AgroMeteoLatitudine = 0.00;
var AgroMeteoLongitudine = 0.00;
// variabile per l'indirizzo dell'azienda in questione
var AgroMeteoDescrizione = "";

// -------------------------------------------------------
// --------------------- DATI METEO ----------------------
// -------------------------------------------------------
var results;

// -------------------------------------------------------
// --------------- GLOBALI PER LA DASHBOARD --------------
// -------------------------------------------------------
// globali per la dashboard del meteo
var map;
var geoJSON;
var request;
var gettingData = false;
var infowindow;
var primaesecuzione = true;
var percorsoIcone = "";
var mmMax = 0.00;
var maxPressione = 0;
var MaxPressioneSuolo = 0;
var tempMaxMax = 0;
var tempMinMin = 100;
// globali per i grafici settimanali
var mmPioggiaOrarioCorrente = new Array;
var pressioneOrarioCorrente = new Array;
var pressioneAlSuoloOrarioCorrente = new Array;
var dataSettimana = new Array;
var orarioAttualeGrafico;
// globali per i grafici orari
var indicePerArrayOrari = 0;
var mmPioggiaSettimana = new Array;
var pressioneSettimana = new Array;
var pressioneAlSuoloSettimana = new Array;
var giornoSettimana = new Array;
var temperaturaSettimana = new Array;
var temperaturaMaxSettimana = new Array;
var temperaturaMinSettimana = new Array;

// -------------------------------------------------------
// ---------- MODALITA DI RICHIESTA DATI METEO -----------
// -------------------------------------------------------
function getWeather(modalita) {

    // variabile che ospita la scelta del metodo di inputo: come campi di testo o gps: di default è dai campi di testo
    var input_type = 'input';

    var dati;
    var recupero_da_gps;

    // se viene scelto il metodo di immissione di latitudine e longitudine da campi di testo...
    if (input_type == 'input') {
        // ...poi invio la richiesta e mi occupo della gestione della risposta e dei relativi dati...
        sendRequest(modalita);
    } else {
        // ...altrimenti leggo i dati dal gps del dispositivo in uso
        getPosition();
    }
    return false;
}

// -------------------------------------------------------
// ------------ WS PER RICEVERE DATI METEO ---------------
// -------------------------------------------------------
function sendRequest(modalita) {

    // variabile per settare la modalità di ricerca del clima: weather per le previsioni correnti, forecast per quelle future
    var searchmode = 'forecast';
    // imposto il sistema metrico decimale
    var units = '&units=metric';
    // imposto la lingua italiana per le comunicazioni del json
    var language = '&lang=it';
    // imposto i parametri di accuratezza della ricerca like è il risultato più vicino, accurate è quello esatto
    var accuracy = '&type=accurate';
    // se vuoto ritorna un Json altrimenti con &mode=xml ritorna un xml
    var return_mode = '';
    // limite delle righe del risultato: cnt è il numero di righe limite, &cnt=3 limita il risultato a 3 righe
    var result_limitation = '';

    if (AgroMeteoLatitudine == 0 && AgroMeteoDescrizione == "") {
        AgroMeteoLatitudine = 44.12;
        AgroMeteoLongitudine = 12.26;
        AgroMeteoDescrizione = "Cesena";
    }

    // recupero il valore dalle variabili globali relativo alla posizione..
    var latitude = AgroMeteoLatitudine;
    var longitude = AgroMeteoLongitudine;
    // e l'indirizzo della sede in questione
    var city = AgroMeteoDescrizione;

    // preparo la stringa con la richiesta
    var queryString = 'https://api.openweathermap.org/data/2.5/' + searchmode + '?q=' + city + '&lat=' + latitude + '&lon=' + longitude + units + language + '&APPID=' + OpenWeatherAppKey_text;
    
    $.getJSON(queryString, function (risultati) {
        // per la dashboard al fine di evitare ulteriori chiamate
        results = risultati;
        AgroMeteoLatitudine = results.city.coord.lat;
        AgroMeteoLongitudine = results.city.coord.lon;
        // attuale
        showWeatherDataLocation();
        // previsioni
        if (modalita == "dashboard") {
            showWeatherForecast();
        }      
    }).fail(function (jqXHR) {
        $('#weather-data').hide();
        $('#forecast_scroll').hide();
        $('#error-msg').show();
        // $('#error-msg').text("Errore nella ricezione dei dati. " + jqXHR.statusText);
    });
}

// -------------------------------------------------------
// ------------ WIDGET DASHBOARD DATA ODIERNA ------------
// -------------------------------------------------------
function showWeatherDataLocation() {

        // il vento arriva con intensita e direzione: la direzione è in gradi da Nord quindi normalizzo
        var direzioneVento = windIrection(results.list[0].wind.deg);
        // arrotondo la temperatura per non avere numeri con la virgola nel widget
        var temperaturaArrotondata = Math.round(results.list[0].main.temp);

        // composizione del widget
        $('#citta_corrente').text(results.city.name);
        $('#temperature').text(temperaturaArrotondata);
        $('#descriptions').text(results.list[0].weather[0].description);
        // percorso con le icone personalizzate
        var percorso = $("#weather-icon").attr("percorso");
        percorsoIcone = percorso + results.list[0].weather[0].icon + ".png";
        $("#weather-icon").attr("src", percorsoIcone); 
        // inserisco i mm di pioggia per l'orario dell'array
        if (results.list[0].rain == undefined || results.list[0].rain["3h"] == undefined) {
            mmPioggiaOrarioCorrente[0] = parseFloat("0");
        } else {
            mmPioggiaOrarioCorrente[0] = parseFloat(results.list[0].rain["3h"]).toFixed(2);
            if (mmPioggiaOrarioCorrente[0] == "NaN") {
                mmPioggiaOrarioCorrente[0] = parseFloat("0");
            }
        }
        // inserisco la pressione per l'orario dell'array
        pressioneOrarioCorrente[0] = results.list[0].main.pressure;
        pressioneAlSuoloOrarioCorrente[0] = results.list[0].main.grnd_level;
        // aggiungo la data per l'asse x
        dataSettimana[0] = "oggi";
}

// -------------------------------------------------------
// ----------- WIDGET PREVISIONI PER DASHBOARD -----------
// -------------------------------------------------------
function showWeatherForecast() {

    // di default nascondo i div e li mostro solamente se il rilevamento è valido
    $('#previsioni_uno').hide();
    $('#previsioni_due').hide();
    $('#previsioni_tre').hide();
    $('#previsioni_quattro').hide();

    // variabile per ottenere l'orario corrente
    var today = new Date();
    var now = today.getHours();
    // per recuperare l'orario del rilevamento
    var orario_split;
    // supporto
    var temp_arrotondata;
    var perc;
    var orario_rilevamento_numerico;
    var orario_attuale_numerico;
    var data_conforme;

    var stringa_data = "";

    // dimensione delle soluzioni: quanti rilevamenti singoli sono effettuati
    var dimensione = results.list.length;
    // variabile che ospita data ed ora del rilevamento: da splittare perché nella forma
    // yyyy-mm-dd hh:mm:ss
    var tempo_temp;
    // variabili che ospitano i dati splittati dalla data di rilevamento di temporanei di giorno ed ora
    var data_temp;
    var ora_temp;
    // variabile per la differenza in giorni tra due date
    var diff;
    // per calcolare la differenza tra date prendo la data corrente in formato UTC
    var split_current_date = results.list[0].dt_txt.split(" ");
    var split_current = split_current_date[0].split("-");
    // nei mesi il -1 si mette perchè il metodo getMonth misura i mesi da 0 a 11
    var utc = Date.UTC(split_current[0], Number(split_current[1]) - 1, split_current[2]);
    var split_ril;
    // variabile per formattare la data di rilevamento in formato UTC
    var utc_conf;

    // salto il primo rilevamento perchè è quello già mostrato e vado a dividere gli altri a seconda della data
    for (var i = 1; i < dimensione; i++) {
        // spezzo il momento di rilevamento...
        tempo_temp = results.list[i].dt_txt.split(" ");
        // ... per ottenere data ed ora 
        data_temp = tempo_temp[0];
        ora_temp = tempo_temp[1];
        // prendo la data di rilevamento, divido anno mese e giorno e la normalizzo in utc
        split_ril = data_temp.split("-")
        utc_conf = Date.UTC(split_ril[0], Number(split_ril[1]) - 1, split_ril[2]);
        // eseguo la differenza, in millisecondi, tra la data di rilevamento e la data corrente
        diff = utc_conf - utc;

        // ora guardo quanti millisecondi intercorrono tra la data corrente e la data di rilevamento
        // 86400000 millisecondi corrispondono a 24 ore: previsioni del giorno dopo
        if (diff == 86400000) {
            // elaborazioni :
            // arrotondo la temperatura per comodita di lettura
            temp_arrotondata = Math.round(results.list[i].main.temp);
            orario_split = ora_temp.split(":");
            // per l'orario prendo solamente l'ora
            orario_rilevamento_numerico = parseInt(orario_split[0]);
            // orario attuale in forma numerica
            orario_attuale_numerico = parseInt(now);
            orarioAttualeGrafico = orario_attuale_numerico;
            // riordinamento della data nella forma dd/mm/yyyy
            data_conforme = parseInt(split_ril[2]) + "/" + parseInt(split_ril[1]) + "/" + split_ril[0];
            // se l'orario di rilevament - orario attuale da un risultato compreso da 0 e 3 allora mostro quel rilevamento
            if (((orario_attuale_numerico - orario_rilevamento_numerico) < 3) && ((orario_attuale_numerico - orario_rilevamento_numerico) >= 0)) {
                // scrittura dati
                $('#domani').text(data_conforme);
                $('#temperature-tomorrow').text(temp_arrotondata);
                $('#descriptions-tomorrow').text(results.list[i].weather[0].description);
                perc = $("#weather-icon-tomorrow").attr("percorso");
                $("#weather-icon-tomorrow").attr("src", perc + results.list[i].weather[0].icon + ".png");
                // mostro il div riempito
                $('#previsioni_uno').show();
                // inserisco i mm di pioggia per l'orario dell'array
                if (results.list[i].rain == undefined || results.list[i].rain["3h"] == undefined) {
                    mmPioggiaOrarioCorrente[1] = parseFloat("0");
                } else {
                    mmPioggiaOrarioCorrente[1] = parseFloat(results.list[i].rain["3h"]).toFixed(2);
                    if (mmPioggiaOrarioCorrente[1] == "NaN") {
                        mmPioggiaOrarioCorrente[1] = parseFloat("0");
                    }
                }

                // inserisco la pressione per l'orario dell'array
                pressioneOrarioCorrente[1] = results.list[i].main.pressure;
                pressioneAlSuoloOrarioCorrente[1] = results.list[i].main.grnd_level;
                // aggiungo la data per l'asse x
                dataSettimana[1] = data_conforme;
            }         
        }
        // 172800000 millisecondi corrispondono a 48 ore: previsioni di due dopo
        else if (diff == 172800000) {
            // elaborazioni:
            // arrotondo la temperatura per comodita di lettura
            temp_arrotondata = Math.round(results.list[i].main.temp);
            // per l'orario prendo solamente l'ora
            orario_split = ora_temp.split(":");
            orario_rilevamento_numerico = parseInt(orario_split[0]);
            // orario attuale in forma numerica
            orario_attuale_numerico = parseInt(now);
            // riordinamento della data nella forma dd/mm/yyyy
            data_conforme = parseInt(split_ril[2]) + "/" + parseInt(split_ril[1]) + "/" + split_ril[0];
            // se l'orario di rilevament - orario attuale da un risultato compreso da 0 e 3 allora mostro quel rilevamento
            if (((orario_attuale_numerico - orario_rilevamento_numerico) < 3) && ((orario_attuale_numerico - orario_rilevamento_numerico) >= 0)) {
                // scrittura dati
                $('#dopodomani').text(data_conforme);
                $('#temperature-tomorrowtwo').text(temp_arrotondata);
                $('#descriptions-tomorrowtwo').text(results.list[i].weather[0].description);
                perc = $("#weather-icon-tomorrowtwo").attr("percorso");
                $("#weather-icon-tomorrowtwo").attr("src", perc + results.list[i].weather[0].icon + ".png");
                // mostro il div riempito
                $('#previsioni_due').show();
                // inserisco i mm di pioggia per l'orario dell'array
                if (results.list[i].rain == undefined || results.list[i].rain["3h"] == undefined) {
                    mmPioggiaOrarioCorrente[2] = parseFloat("0");
                } else {
                    mmPioggiaOrarioCorrente[2] = parseFloat(results.list[i].rain["3h"]).toFixed(2);
                    if (mmPioggiaOrarioCorrente[2] == "NaN") {
                        mmPioggiaOrarioCorrente[2] = parseFloat("0");
                    }
                }

                // inserisco la pressione per l'orario dell'array
                pressioneOrarioCorrente[2] = results.list[i].main.pressure;
                pressioneAlSuoloOrarioCorrente[2] = results.list[i].main.grnd_level;
                dataSettimana[2] = data_conforme;
            }          
        }
        // 259200000 millisecondi corrispondono a 72 ore: previsioni di tre dopo
        else if (diff == 259200000) {
            // elaborazioni:
            // arrotondo la temperatura per comodita di lettura
            temp_arrotondata = Math.round(results.list[i].main.temp);
            // per l'orario prendo solamente l'ora
            orario_split = ora_temp.split(":");
            orario_rilevamento_numerico = parseInt(orario_split[0]);
            // orario attuale in forma numerica
            orario_attuale_numerico = parseInt(now);
            // riordinamento della data nella forma dd/mm/yyyy
            data_conforme = parseInt(split_ril[2]) + "/" + parseInt(split_ril[1]) + "/" + split_ril[0];
            // se l'orario di rilevament - orario attuale da un risultato compreso da 0 e 3 allora mostro quel rilevamento
            if (((orario_attuale_numerico - orario_rilevamento_numerico) < 3) && ((orario_attuale_numerico - orario_rilevamento_numerico) >= 0)) {
                // scrittura dati
                $('#dopoduegiorni').text(data_conforme);
                $('#temperature-tomorrowtre').text(temp_arrotondata);
                $('#descriptions-tomorrowtre').text(results.list[i].weather[0].description);
                perc = $("#weather-icon-tomorrowtre").attr("percorso");
                $("#weather-icon-tomorrowtre").attr("src", perc + results.list[i].weather[0].icon + ".png");
                // mostro il div riempito
                $('#previsioni_tre').show();
                // inserisco i mm di pioggia per l'orario dell'array
                if (results.list[i].rain == undefined || results.list[i].rain["3h"] == undefined) {
                    mmPioggiaOrarioCorrente[3] = parseFloat("0");
                } else {
                    mmPioggiaOrarioCorrente[3] = parseFloat(results.list[i].rain["3h"]).toFixed(2);
                    if (mmPioggiaOrarioCorrente[3] == "NaN") {
                        mmPioggiaOrarioCorrente[3] = parseFloat("0");
                    }
                }

                // inserisco la pressione per l'orario dell'array
                pressioneOrarioCorrente[3] = results.list[i].main.pressure;
                pressioneAlSuoloOrarioCorrente[3] = results.list[i].main.grnd_level;
                // aggiungo la data per l'asse x
                dataSettimana[3] = data_conforme;
            }
        }
        // 345600000 millisecondi corrispondono a 96 ore: previsioni di quattro dopo
        else if (diff == 345600000) {
            // elaborazioni:
            // arrotondo la temperatura per comodita di lettura
            temp_arrotondata = Math.round(results.list[i].main.temp);
            // per l'orario prendo solamente l'ora
            orario_split = ora_temp.split(":");
            orario_rilevamento_numerico = parseInt(orario_split[0]);
            // orario attuale in forma numerica
            orario_attuale_numerico = parseInt(now);
            // riordinamento della data nella forma dd/mm/yyyy
            data_conforme = parseInt(split_ril[2]) + "/" + parseInt(split_ril[1]) + "/" + split_ril[0];
            // se l'orario di rilevament - orario attuale da un risultato compreso da 0 e 3 allora mostro quel rilevamento
            if (((orario_attuale_numerico - orario_rilevamento_numerico) < 3) && ((orario_attuale_numerico - orario_rilevamento_numerico) >= 0)) {
                // scrittura dati
                $('#dopotregiorni').text(data_conforme);
                $('#temperature-tomorrowfour').text(temp_arrotondata);
                $('#descriptions-tomorrowfour').text(results.list[i].weather[0].description);
                perc = $("#weather-icon-tomorrowfour").attr("percorso");
                $("#weather-icon-tomorrowfour").attr("src", perc + results.list[i].weather[0].icon + ".png");
                // mostro il div riempito
                $('#previsioni_quattro').show();
                // inserisco i mm di pioggia per l'orario dell'array
                if (results.list[i].rain == undefined || results.list[i].rain["3h"] == undefined) {
                    mmPioggiaOrarioCorrente[4] = parseFloat("0");
                } else {
                    mmPioggiaOrarioCorrente[4] = parseFloat(results.list[i].rain["3h"]).toFixed(2);
                    if (mmPioggiaOrarioCorrente[4] == "NaN") {
                        mmPioggiaOrarioCorrente[4] = parseFloat("0");
                    }
                }

                // inserisco la pressione per l'orario dell'array
                pressioneOrarioCorrente[4] = results.list[i].main.pressure;
                pressioneAlSuoloOrarioCorrente[4] = results.list[i].main.grnd_level;
                // aggiungo la data per l'asse x
                dataSettimana[4] = data_conforme;
            }
        }
    }
}

// -------------------------------------------------------
// ------------- ELABORAZIONE DATI DEL VENTO -------------
// -------------------------------------------------------
function windIrection(direzioneVento) {

    if (direzioneVento >= 0 && direzioneVento <= 22.5) {
        direzioneVento = 'Nord (Tramontana)';
    }
    else if (direzioneVento <= 67.5 && direzioneVento > 22.5) {
        direzioneVento = 'Nord-Est (Grecale)';
    }
    else if (direzioneVento <= 112.5 && direzioneVento > 67.5) {
        direzioneVento = 'Est (Levante)';
    }
    else if (direzioneVento <= 157.5 && direzioneVento > 112.5) {
        direzioneVento = 'Sud-Est (Scirocco)';
    }
    else if (direzioneVento <= 202.5 && direzioneVento > 157.5) {
        direzioneVento = 'Sud (Ostro)';
    }
    else if (direzioneVento <= 247.5 && direzioneVento > 202.5) {
        direzioneVento = 'Sud-Ovest (Libeccio)';
    }
    else if (direzioneVento <= 292.5 && direzioneVento > 247.5) {
        direzioneVento = 'Ovest (Ponente)';
    }
    else if (direzioneVento <= 337.5 && direzioneVento > 292.5) {
        direzioneVento = 'Nord-Ovest (Maestrale)';
    }
    else if (direzioneVento <= 360 && direzioneVento > 337.5) {
        direzioneVento = 'Nord-Ovest (Maestrale)';
    }
    else {
        direzioneVento = 'Valore della direzione ERRATO';
    }
    return direzioneVento;
}

// -------------------------------------------------------
// ---------- POSIZIONE GPS LUOGO DI INTERESSE -----------
// -------------------------------------------------------
function getPosition() {

    var options = {
        enableHighAccuracy: true,
        maximumAge: 3600000
    }

    var watchID = navigator.geolocation.getCurrentPosition(onSuccess, onError, options);

    function onSuccess(position) {
        // recupero la posizione dal dispositivo...
        latitude = position.coords.latitude;
        longitude = position.coords.longitude;
        // ... ed invio la richiesta secondo la posizione corrente ed interpreto la risposta
        // la richiesta al webservice e la relativa trattazione dei dati di risposta viene fatta qui perché possono altrimenti sorgere dei problemi dovuti al ritardo della lettura della posizione
        sendRequest();
    }

    function onError(error) {
        $('#error-msg').show();
        $('#error-msg').text("Errore nei dati GPS: " + error.code + "-" + error.message);
    }
}

// -------------------------------------------------------
// ------------- SCROLL DEL METEO FORECAST ---------------
// -------------------------------------------------------
function scrollInit() {

    var scroll_enable = true;
    var scroll_move = 1;
    var scroll_time = 100;

    var obj = document.getElementById("forecast_scroll");

    if (scroll_enable) {
        if ((obj.scrollTop + scroll_move) < (obj.scrollHeight - obj.clientHeight))
            obj.scrollTop += scroll_move;
        else
            obj.scrollTop = 0;
    }
    setTimeout(scrollInit, scroll_time);
}

// -------------------------------------------------------
// -------------------------------------------------------
// ------------------ DASHBOARD METEO --------------------
// -------------------------------------------------------
// -------------------------------------------------------

// -------------------------------------------------------
// ------------- GOOGLE MAPS CON OPEN WEATHER ------------
// -------------------------------------------------------

// ascoltatore del pulsante in cui preparo i div per la dashboard del meteo
function apriMeteoLarge() {     

    // chiudo il div con i widget
    $("#widget").hide();
    // chiudo il div della dasahboard
    $("#dashboard").hide();

    // aggiungo la dashboard del meteo al contenitore principale
    $("#contenitore_principale").append("<div id='meteo_dashboard' class='row'>");

    // $("#meteo_dashboard").append("<div id='googlemap_forecast' class='col-xs-9' style='height: 80vh !important;'></div>");
    $("#meteo_dashboard").append("<div id='city_forecast' class='col-lg-3 col-sm-4 col-xs-12'></div>");
    $("#meteo_dashboard").append("<div id='graph_dashboard' class='col-lg-9 col-sm-8 col-xs-12'>");

    // mappa interattiva
    // $("#contenitore_principale").append("<div id='mappa_interattiva' style='height: 80vh; padding-right: 0; padding-left: 0;' class='row'></div>");

    // grafico temperature orarie
    $("#graph_dashboard").append("<div id='meteo_dashboard_graph_temp'>");
    $("#meteo_dashboard_graph_temp").append("<div id='temp_hour_graph' class='row' style='margin-bottom:15px;'></div>");
    $("#graph_dashboard").append("</div>");
    
    // grafici settimanali
    $("#graph_dashboard").append("<div id='meteo_dashboard_graph'>");
    // grafico pioggia settimana
    $("#meteo_dashboard_graph").append("<div id='mmrain_same_graph' class='row' style='margin-bottom:15px;'></div>");
    // grafico umidita settimana
    $("#meteo_dashboard_graph").append("<div id='umidita_same_graph' class='row' style='margin-bottom:15px;'></div>");
    $("#graph_dashboard").append("</div>");

    // grafici orari
    // grafico pioggia oraria
    $("#graph_dashboard").append("<div id='meteo_dashboard_graph_hour'>");
    $("#meteo_dashboard_graph_hour").append("<div id='mmrain_hour_graph' class='row' style='margin-bottom:15px;'></div>");
    $("#graph_dashboard").append("</div>");

    // grafico umidita oraria
    $("#graph_dashboard").append("<div id='meteo_dashboard_umm_hour'>"); 
    $("#meteo_dashboard_umm_hour").append("<div id='umm_hour_graph' class='row' style='margin-bottom:15px;'></div>");
    $("#graph_dashboard").append("</div>");

    $("#meteo_dashboard").append("</div>");

    $("#contenitore_principale").append("</div>");

    // css per la mappa
    $(".gm-style-iw").css("text-align", "center");

    // -------------------------------------------------------
    // ---------------- MAPPA METEO CORRENTE -----------------
    // -------------------------------------------------------
    // google maps
    // initialize();    

    // -------------------------------------------------------
    // ------------------ PANNELLO LATERALE ------------------
    // -------------------------------------------------------
    creaPrevisioniLatoMappa();

    // -------------------------------------------------------
    // ------------------ MAPPA INTERATTIVA ------------------
    // -------------------------------------------------------
    // initMap(AgroMeteoLatitudine, AgroMeteoLongitudine, percorsoIcone);

    // -------------------------------------------------------
    // ----------------------- GRAFICI -----------------------
    // -------------------------------------------------------
    graficoPioggiaSettimanale();
    graficoPressioneSettimanale();  
}

// inizializzazione del pannello meteo
function initialize() {

    infowindow = new google.maps.InfoWindow();

    // zoom e centro della mappa
    var mapOptions = {
        zoom: 11,
        mapTypeId: google.maps.MapTypeId.HYBRID,
        center: new google.maps.LatLng(AgroMeteoLatitudine, AgroMeteoLongitudine)
    };

    // posiziono la mappa nell'apposito div
    map = new google.maps.Map(document.getElementById('googlemap_forecast'), mapOptions);
    primaesecuzione = false;

    // Add interaction listeners to make weather requests
    google.maps.event.addListener(map, 'idle', checkIfDataRequested);
}

function checkIfDataRequested() {
    while (gettingData === true) {
        request.abort();
        gettingData = false;
    }
    getCoords();
}
function getCoords() {
    var bounds = map.getBounds();
    var NE = bounds.getNorthEast();
    var SW = bounds.getSouthWest();
    //getWeather(NE.lat(), NE.lng(), SW.lat(), SW.lng());

    gettingData = true;
    var requestString = "https://api.openweathermap.org/data/2.5/box/city?bbox="
        + SW.lng() + "," + NE.lat() + "," //left top
        + NE.lng() + "," + SW.lat() + "," //right bottom
        + map.getZoom()
        + "&cluster=yes&format=json"
        + "&APPID=" + OpenWeatherAppKey_map;
    request = new XMLHttpRequest();
    request.onload = proccessResults;
    request.open("get", requestString, true);
    request.send();
}

// processamento dei risultati del meteo
function proccessResults() {
    if (primaesecuzione == false) {
        console.log(this);

        var results = JSON.parse(this.responseText);

        if (results.list.length > 0) {
            resetData();
            for (var i = 0; i < results.list.length; i++) {
                geoJSON.features.push(jsonToGeoJson(results.list[i]));
            }
            drawIcons(geoJSON);
        }
    }      
}
// materiale per il meteo
function jsonToGeoJson(weatherItem) {
    var feature = {
        type: "Feature",
        properties: {
            city: weatherItem.name,
            weather: weatherItem.weather[0].main,
            temperature: weatherItem.main.temp,
            min: weatherItem.main.temp_min,
            max: weatherItem.main.temp_max,
            humidity: weatherItem.main.humidity,
            pressure: weatherItem.main.pressure,
            windSpeed: weatherItem.wind.speed,
            windDegrees: weatherItem.wind.deg,
            windGust: weatherItem.wind.gust,
            icon: percorsoIcone.substring(0, percorsoIcone.length - 7)
            + weatherItem.weather[0].icon + ".png",
            coordinates: [weatherItem.coord.Lon, weatherItem.coord.Lat]
        },
        geometry: {
            type: "Point",
            coordinates: [weatherItem.coord.Lon, weatherItem.coord.Lat]
        }
    };
    map.data.setStyle(function (feature) {
        return {
            icon: {
                url: feature.getProperty('icon'),
                anchor: new google.maps.Point(25, 25)
            }
        };
    });
    return feature;
}
// per disegnare le icone sulla mappa
function drawIcons(weather) {
    map.data.addGeoJson(geoJSON);
    gettingData = false;
}

// per pulire il layer
function resetData() {
    geoJSON = {
        type: "FeatureCollection",
        features: []
    };
    map.data.forEach(function (feature) {
        map.data.remove(feature);
    });
}

// -------------------------------------------------------
// ---------- PREVISIONI A DESTRA DELLA MAPPA ------------
// -------------------------------------------------------

function creaPrevisioniLatoMappa() {

         // se faccio la chiamata significa che devo popolare il div quindi faccio ricominciare il contatore da 0
        indicePerArrayOrari = 0;
        // anche la temperatura viene settata a zero
        mmMax = 0.00;
        // anche la pressione va messa a zero
        maxPressione = 0;
        MaxPressioneSuolo = 0;
        // ed anche le temperature vanno messe a default
        tempMaxMax = 0;
        tempMinMin = 100;

        // città
        var citta = results.city.name;
        // icona: nel percorso rimane il nome doppio del png quindi cavo il nome, che è di 3 caratteri, ed il ".png"
        var icona = percorsoIcone.substring(0, percorsoIcone.length - 7) + results.list[0].weather[0].icon;
        // vento in nodi
        var nodi = Math.round(results.list[0].wind.speed);
        // il vento arriva con intensità e direzione: la direzione è in gradi da Nord quindi normalizzo
        var direzioneVento = windIrection(results.list[0].wind.deg);
        // arrotondo la temperatura per non avere numeri con la virgola nel widget
        var temperaturaArrotondata = Math.round(results.list[0].main.temp);
        // descrizione
        var descrizione = results.list[0].weather[0].description;
        // tasso umidità
        var umidità = results.list[0].main.humidity;
        // mm di pioggia
        var mm_pioggia = parseFloat("0");
        if (results.list[0].rain == undefined || results.list[0].rain["3h"] == undefined) {
            mm_pioggia = parseFloat("0");
        } else {
            mm_pioggia = parseFloat(results.list[0].rain["3h"]).toFixed(2);
            if (mm_pioggia == "NaN") {
                mm_pioggia = parseFloat("0");
            }
        }

        // -------------------------------------------------------
        // -------------- RILEVAMENTI AL MOMENTO -----------------
        // -------------------------------------------------------
        $("#city_forecast").append("<h4>" + citta + "</h4>");
        $("#city_forecast").append("<img id='weather-icon' style='margin: auto; top: 0; left: 0; right: 0; ' runat='server' alt='img_weather' title='imgweather' src='" + icona + ".png' width='70%' height='auto';><br><br>");
        $("#city_forecast").append("<span><h5> • " + temperaturaArrotondata + "°C con " + descrizione + "</h5></span>");
        $("#city_forecast").append("<span><h5> • tasso di umidità pari al " + umidità + "%</h5></span>");
        $("#city_forecast").append("<span><h5> • " + mm_pioggia + "mm di pioggia nelle ultime 3 ore</h5></span>");
        $("#city_forecast").append("<span><h5> • vento: " + nodi + " nodi da " + direzioneVento + "</h5></span><br>");

        // -------------------------------------------------------
        // -------------------- PREVISIONI -----------------------
        // -------------------------------------------------------
        $("#city_forecast").append("<div id='dashboard_forecast'>");
        $("#city_forecast").append("</div>");
        // primo giorno
        $("#dashboard_forecast").append("<b><span id='title_first'></span></b>");
        $("#dashboard_forecast").append("<ul id='forecast-data_first' data-role='listview' data-inset='true' class='ui-listview ui-listview-inset ui-corner-all ui-shadow not-displayed'></ul>");
        // secondo giorno
        $("#dashboard_forecast").append("<b><span id='title_second'></span>");
        $("#dashboard_forecast").append("<ul id='forecast-data_second' data-role='listview' data-inset='true' class='ui-listview ui-listview-inset ui-corner-all ui-shadow not-displayed'></ul>");     
        // terzo giorno
        $("#dashboard_forecast").append("<b><span id='title_third'></span></b>");
        $("#dashboard_forecast").append("<ul id='forecast-data_third' data-role='listview' data-inset='true' class='ui-listview ui-listview-inset ui-corner-all ui-shadow not-displayed'></ul>");
        // quarto giorno
        $("#dashboard_forecast").append("<b><span id='title_fourth'></span></b>");
        $("#dashboard_forecast").append("<ul id='forecast-data_fourth' data-role='listview' data-inset='true' class='ui-listview ui-listview-inset ui-corner-all ui-shadow not-displayed'></ul>");  
        // quinto giorno
        $("#dashboard_forecast").append("<b><span id='title_five'></span></b>");
        $("#dashboard_forecast").append("<ul id='forecast-data_five' data-role='listview' data-inset='true' class='ui-listview ui-listview-inset ui-corner-all ui-shadow not-displayed'></ul>"); 

        // -------------------------------------------------------
        // --------------------- ELABORAZIONI --------------------
        // -------------------------------------------------------
        var stringa_data = "";
        // dimensione delle soluzioni: quanti rilevamenti singoli sono effettuati
        var dimensione = results.list.length;
        // variabile che ospita data ed ora del rilevamento: da splittare perchè nella forma
        // yyyy-mm-dd hh:mm:ss
        var tempo_temp;
        // variabili che ospitano i dati splittati dalla data di rilevamento di temporanei di giorno ed ora
        var data_temp;
        var ora_temp;
        var ora;
        // variabile per la differenza in giorni tra due date
        var diff;
        // per calcolare la differenza tra date prendo la data corrente in formato UTC
        var split_current_date = results.list[0].dt_txt.split(" ");
        var split_current = split_current_date[0].split("-");
        // nei mesi il -1 si mette perché il metodo getMonth misura i mesi da 0 a 11
        var utc = Date.UTC(split_current[0], Number(split_current[1]) - 1, split_current[2]);
        var split_ril;
        // variabile per formattare la data di rilevamento in formato UTC
        var utc_conf;
        // variabile per la data in formato dd/mm/yyyy
        var data_conforme;

        // salto il primo rilevamento perché è quello già mostrato e vado a dividere gli altri a seconda della data
        for (var i = 1; i < dimensione; i++) {
            // spezzo il momento di rilevamento...
            tempo_temp = results.list[i].dt_txt.split(" ");
            // ... per ottenere data ed ora 
            data_temp = tempo_temp[0];
            ora_temp = tempo_temp[1];
            // prendo la data di rilevamento, divido anno mese e giorno e la normalizzo in utc
            split_ril = data_temp.split("-")
            utc_conf = Date.UTC(split_ril[0], Number(split_ril[1]) - 1, split_ril[2]);
            // eseguo la differenza, in millisecondi, tra la data di rilevamento e la data corrente
            diff = utc_conf - utc;

            // dell'orario tengo conto solamente dell'ora tanto non ho mai minuti e secondi
            ora = ora_temp.split(":");

            // metto la data nel formato dd/mm/yyyy
            data_conforme = parseInt(split_ril[2]) + "/" + parseInt(split_ril[1]) + "/" + split_ril[0];

            // ora guardo quanti millisecondi intercorrono tra la data corrente e la data di rilevamento
            // 86400000 millisecondi corrispondono a 24 ore: previsioni del giorno dopo
            if (diff == 86400000) {
                $('#weather-data_second').show();
                $('#forecast-data_second').show();
                $('#title_second').text("Previsioni per il " + data_conforme);
                $('#forecast-data_second').append('<li><b> Ore ' + ora[0] + ':</b> <img id="weather-icon" style="position: relative;" runat="server" alt="img_weather" title="imgweather" src="' + percorsoIcone.substring(0, percorsoIcone.length - 7) + results.list[i].weather[0].icon + '.png" height="20px" width="auto"> ' + Math.round(results.list[i].main.temp) + '°C con ' + results.list[i].weather[0].description + ', ' + Math.round(results.list[i].wind.speed) + ' nodi di vento da ' + windIrection(results.list[i].wind.deg) + '</li>');
             
                // per i grafici orari della settimana
                if (results.list[i].rain == undefined || results.list[i].rain["3h"] == undefined) {
                    mmPioggiaSettimana[indicePerArrayOrari] = parseFloat("0");
                } else {
                    mmPioggiaSettimana[indicePerArrayOrari] = parseFloat(results.list[i].rain["3h"]).toFixed(2);
                    if (mmPioggiaSettimana[indicePerArrayOrari] == "NaN") {
                        mmPioggiaSettimana[indicePerArrayOrari] = parseFloat("0");
                    }
                }
                // il nuovo max per i mm di pioggia settimanali: limite superiore del grafico
                if (mmPioggiaSettimana[indicePerArrayOrari] > mmMax) {
                    mmMax = mmPioggiaSettimana[indicePerArrayOrari];
                }
                pressioneSettimana[indicePerArrayOrari] = results.list[i].main.pressure;
                // il nuovo max per la pressione atmosferica: limite superiore del grafico
                if (pressioneSettimana[indicePerArrayOrari] > maxPressione) {
                    maxPressione = pressioneSettimana[indicePerArrayOrari];
                }
                pressioneAlSuoloSettimana[indicePerArrayOrari] = results.list[i].main.grnd_level;
                giornoSettimana[indicePerArrayOrari] = parseInt(split_ril[2]) + "/" + parseInt(split_ril[1]) + " \n " + ora[0];
                temperaturaSettimana[indicePerArrayOrari] = results.list[i].main.temp;
                temperaturaMaxSettimana[indicePerArrayOrari] = results.list[i].main.temp_max;
                temperaturaMinSettimana[indicePerArrayOrari] = results.list[i].main.temp_min;
                if (temperaturaMaxSettimana[indicePerArrayOrari] > tempMaxMax) {
                    tempMaxMax = temperaturaMaxSettimana[indicePerArrayOrari];
                }
                if (tempMinMin > temperaturaMinSettimana[indicePerArrayOrari]) {
                    tempMinMin = temperaturaMinSettimana[indicePerArrayOrari];
                }
                indicePerArrayOrari++;
            }
            // 172800000 millisecondi corrispondono a 48 ore: previsioni di due dopo
            else if (diff == 172800000) {
                $('#weather-data_third').show();
                $('#forecast-data_third').show();
                $('#title_third').text("Previsioni per il " + data_conforme);
                $('#forecast-data_third').append('<li><b> Ore ' + ora[0] + ':</b> <img id="weather-icon" style="position: relative;" runat="server" alt="img_weather" title="imgweather" src="' + percorsoIcone.substring(0, percorsoIcone.length - 7) + results.list[i].weather[0].icon + '.png" height="20px" width="auto"> ' + Math.round(results.list[i].main.temp) + '°C con ' + results.list[i].weather[0].description + ', ' + Math.round(results.list[i].wind.speed) + ' nodi di vento da ' + windIrection(results.list[i].wind.deg) + '</li>');

                // per i grafici orari della settimana
                if (results.list[i].rain == undefined || results.list[i].rain["3h"] == undefined) {
                    mmPioggiaSettimana[indicePerArrayOrari] = parseFloat("0");
                } else {
                    mmPioggiaSettimana[indicePerArrayOrari] = parseFloat(results.list[i].rain["3h"]).toFixed(2);
                    if (mmPioggiaSettimana[indicePerArrayOrari] == "NaN") {
                        mmPioggiaSettimana[indicePerArrayOrari] = parseFloat("0");
                    }
                }
                // il nuovo max per i mm di pioggia settimanali: limite superiore del grafico
                if (mmPioggiaSettimana[indicePerArrayOrari] > mmMax) {
                    mmMax = mmPioggiaSettimana[indicePerArrayOrari];
                }
                pressioneSettimana[indicePerArrayOrari] = results.list[i].main.pressure;
                // il nuovo max per la pressione atmosferica: limite superiore del grafico
                if (pressioneSettimana[indicePerArrayOrari] > maxPressione) {
                    maxPressione = pressioneSettimana[indicePerArrayOrari];
                }
                pressioneAlSuoloSettimana[indicePerArrayOrari] = results.list[i].main.grnd_level;
                giornoSettimana[indicePerArrayOrari] = parseInt(split_ril[2]) + "/" + parseInt(split_ril[1]) + " \n " + ora[0];
                temperaturaSettimana[indicePerArrayOrari] = results.list[i].main.temp;
                temperaturaMaxSettimana[indicePerArrayOrari] = results.list[i].main.temp_max;
                temperaturaMinSettimana[indicePerArrayOrari] = results.list[i].main.temp_min;
                if (temperaturaMaxSettimana[indicePerArrayOrari] > tempMaxMax) {
                    tempMaxMax = temperaturaMaxSettimana[indicePerArrayOrari];
                }
                if (tempMinMin > temperaturaMinSettimana[indicePerArrayOrari]) {
                    tempMinMin = temperaturaMinSettimana[indicePerArrayOrari];
                }
                indicePerArrayOrari++;
            }
            // 259200000 millisecondi corrispondono a 72 ore: previsioni di tre dopo
            else if (diff == 259200000) {
                $('#weather-data_fourth').show();
                $('#forecast-data_fourth').show();
                $('#title_fourth').text("Previsioni per il " + data_conforme);
                $('#forecast-data_fourth').append('<li><b> Ore ' + ora[0] + ':</b> <img id="weather-icon" style="position: relative;" runat="server" alt="img_weather" title="imgweather" src="' + percorsoIcone.substring(0, percorsoIcone.length - 7) + results.list[i].weather[0].icon + '.png" height="20px" width="auto"> ' + Math.round(results.list[i].main.temp) + '°C con ' + results.list[i].weather[0].description + ', ' + Math.round(results.list[i].wind.speed) + ' nodi di vento da ' + windIrection(results.list[i].wind.deg) + '</li>');

                // per i grafici orari della settimana
                if (results.list[i].rain == undefined || results.list[i].rain["3h"] == undefined) {
                    mmPioggiaSettimana[indicePerArrayOrari] = parseFloat("0");
                } else {
                    mmPioggiaSettimana[indicePerArrayOrari] = parseFloat(results.list[i].rain["3h"]).toFixed(2);
                    if (mmPioggiaSettimana[indicePerArrayOrari] == "NaN") {
                        mmPioggiaSettimana[indicePerArrayOrari] = parseFloat("0");
                    }
                }
                // il nuovo max per i mm di pioggia settimanali: limite superiore del grafico
                if (mmPioggiaSettimana[indicePerArrayOrari] > mmMax) {
                    mmMax = mmPioggiaSettimana[indicePerArrayOrari];
                }
                pressioneSettimana[indicePerArrayOrari] = results.list[i].main.pressure;
                // il nuovo max per la pressione atmosferica: limite superiore del grafico
                if (pressioneSettimana[indicePerArrayOrari] > maxPressione) {
                    maxPressione = pressioneSettimana[indicePerArrayOrari];
                }
                pressioneAlSuoloSettimana[indicePerArrayOrari] = results.list[i].main.grnd_level;
                giornoSettimana[indicePerArrayOrari] = parseInt(split_ril[2]) + "/" + parseInt(split_ril[1]) + " \n " + ora[0];
                temperaturaSettimana[indicePerArrayOrari] = results.list[i].main.temp;
                temperaturaMaxSettimana[indicePerArrayOrari] = results.list[i].main.temp_max;
                temperaturaMinSettimana[indicePerArrayOrari] = results.list[i].main.temp_min;
                if (temperaturaMaxSettimana[indicePerArrayOrari] > tempMaxMax) {
                    tempMaxMax = temperaturaMaxSettimana[indicePerArrayOrari];
                }
                if (tempMinMin > temperaturaMinSettimana[indicePerArrayOrari]) {
                    tempMinMin = temperaturaMinSettimana[indicePerArrayOrari];
                }
                indicePerArrayOrari++;
            }
            // 345600000 millisecondi corrispondono a 96 ore: previsioni di quattro dopo
            else if (diff == 345600000) {
                $('#weather-data_five').show();
                $('#forecast-data_five').show();
                $('#title_five').text("Previsioni per il " + data_conforme);
                $('#forecast-data_five').append('<li><b> Ore ' + ora[0] + ':</b> <img id="weather-icon" style="position: relative;" runat="server" alt="img_weather" title="imgweather" src="' + percorsoIcone.substring(0, percorsoIcone.length - 7) + results.list[i].weather[0].icon + '.png" height="20px" width="auto"> ' + Math.round(results.list[i].main.temp) + '°C con ' + results.list[i].weather[0].description + ', ' + Math.round(results.list[i].wind.speed) + ' nodi di vento da ' + windIrection(results.list[i].wind.deg) + '</li>');      

                // per i grafici orari della settimana
                if (results.list[i].rain == undefined || results.list[i].rain["3h"] == undefined) {
                    mmPioggiaSettimana[indicePerArrayOrari] = parseFloat("0");
                } else {
                    mmPioggiaSettimana[indicePerArrayOrari] = parseFloat(results.list[i].rain["3h"]).toFixed(2);
                    if (mmPioggiaSettimana[indicePerArrayOrari] == "NaN") {
                        mmPioggiaSettimana[indicePerArrayOrari] = parseFloat("0");
                    }
                }
                // il nuovo max per i mm di pioggia settimanali: limite superiore del grafico
                if (mmPioggiaSettimana[indicePerArrayOrari] > mmMax) {
                    mmMax = mmPioggiaSettimana[indicePerArrayOrari];
                }
                pressioneSettimana[indicePerArrayOrari] = results.list[i].main.pressure;
                // il nuovo max per la pressione atmosferica: limite superiore del grafico
                if (pressioneSettimana[indicePerArrayOrari] > maxPressione) {
                    maxPressione = pressioneSettimana[indicePerArrayOrari];
                }
                pressioneAlSuoloSettimana[indicePerArrayOrari] = results.list[i].main.grnd_level;
                giornoSettimana[indicePerArrayOrari] = parseInt(split_ril[2]) + "/" + parseInt(split_ril[1]) + " \n " + ora[0];
                temperaturaSettimana[indicePerArrayOrari] = results.list[i].main.temp;
                temperaturaMaxSettimana[indicePerArrayOrari] = results.list[i].main.temp_max;
                temperaturaMinSettimana[indicePerArrayOrari] = results.list[i].main.temp_min;
                if (temperaturaMaxSettimana[indicePerArrayOrari] > tempMaxMax) {
                    tempMaxMax = temperaturaMaxSettimana[indicePerArrayOrari];
                }
                if (tempMinMin > temperaturaMinSettimana[indicePerArrayOrari]) {
                    tempMinMin = temperaturaMinSettimana[indicePerArrayOrari];
                }
                indicePerArrayOrari++;
            }
            else if (diff >= 0 && diff < 86400000) {
                // la data è quella odierna
                $('#weather-data_first').show();
                $('#forecast-data_first').show();
                $('#title_first').text("Previsioni per la giornata odierna");
                $('#forecast-data_first').append('<li><b> Ore ' + ora[0] + ':</b> <img id="weather-icon" style="position: relative;" runat="server" alt="img_weather" title="imgweather" src="' + percorsoIcone.substring(0, percorsoIcone.length - 7) + results.list[i].weather[0].icon + '.png" height="20px" width="auto"> ' + Math.round(results.list[i].main.temp) + '°C con ' + results.list[i].weather[0].description + ', ' + Math.round(results.list[i].wind.speed) + ' nodi di vento da ' + windIrection(results.list[i].wind.deg) + '</li>');

                // per i grafici orari della settimana
                if (results.list[i].rain == undefined || results.list[i].rain["3h"] == undefined) {
                    mmPioggiaSettimana[indicePerArrayOrari] = parseFloat("0");
                } else {
                    mmPioggiaSettimana[indicePerArrayOrari] = parseFloat(results.list[i].rain["3h"]).toFixed(2);
                    if (mmPioggiaSettimana[indicePerArrayOrari] == "NaN") {
                        mmPioggiaSettimana[indicePerArrayOrari] = parseFloat("0");
                    }
                }
                // il nuovo max per i mm di pioggia settimanali: limite superiore del grafico
                if (mmPioggiaSettimana[indicePerArrayOrari] > mmMax) {
                    mmMax = mmPioggiaSettimana[indicePerArrayOrari];
                }
                pressioneSettimana[indicePerArrayOrari] = results.list[i].main.pressure;
                // il nuovo max per la pressione atmosferica: limite superiore del grafico
                if (pressioneSettimana[indicePerArrayOrari] > maxPressione) {
                    maxPressione = pressioneSettimana[indicePerArrayOrari];
                }
                pressioneAlSuoloSettimana[indicePerArrayOrari] = results.list[i].main.grnd_level;
                giornoSettimana[indicePerArrayOrari] = parseInt(split_ril[2]) + "/" + parseInt(split_ril[1]) + " \n " + ora[0];
                temperaturaSettimana[indicePerArrayOrari] = results.list[i].main.temp;
                temperaturaMaxSettimana[indicePerArrayOrari] = results.list[i].main.temp_max;
                temperaturaMinSettimana[indicePerArrayOrari] = results.list[i].main.temp_min;
                if (temperaturaMaxSettimana[indicePerArrayOrari] > tempMaxMax) {
                    tempMaxMax = temperaturaMaxSettimana[indicePerArrayOrari];
                }
                if (tempMinMin > temperaturaMinSettimana[indicePerArrayOrari]) {
                    tempMinMin = temperaturaMinSettimana[indicePerArrayOrari];
                }
                indicePerArrayOrari++;
            }

            // -------------------------------------------------------
            // ----------------- GRAFICI DATI ORARI ------------------
            // -------------------------------------------------------
            graficoTemperatureOrario();
            graficoPioggiaOraria();
            graficoPressioneOraria();
        }    
}

// -------------------------------------------------------
// ----------------------- GRAFICI -----------------------
// -------------------------------------------------------

// -------------------------------------------------------
// ------------------ PIOGGIA SETTIMANALE ----------------
// -------------------------------------------------------
function graficoPioggiaSettimanale() {

    var titoloGrafico = "Previsioni per i mm di pioggia \n per le ore " + orarioAttualeGrafico;

    var series = [{
        name: "mm di pioggia",
        data: mmPioggiaOrarioCorrente,

        // Line chart marker type
        markers: { type: "square" }
    }];

    // mmrain_same_graph
    $("#mmrain_same_graph").kendoChart({
        title: {
            text: titoloGrafico
        },
        legend: {
            position: "bottom"
        },
        seriesDefaults: {
            type: "line",
            color: "#1E90FF"
        },
        series: series,
        valueAxis: {
            line: {
                visible: false
            }
        },
        categoryAxis: {
            categories: dataSettimana,
            majorGridLines: {
                visible: false
            }
        },
        tooltip: {
            visible: true,
            format: "{0}"
        }
    });
}

// -------------------------------------------------------
// ------------------ UMIDITA SETTIMANALE ----------------
// -------------------------------------------------------
function graficoPressioneSettimanale() {

    var titoloGrafico = "Previsioni per la pressione atmosferica \n per le ore " + orarioAttualeGrafico;

    // mmrain_same_graph
    $("#umidita_same_graph").kendoChart({
        title: {
            text: titoloGrafico
        },
        legend: {
            position: "bottom"
        },
        seriesDefaults: {
            type: "column",
            stack: false         
        },
        series: [{
            name: "pressione hPa",
            data: pressioneOrarioCorrente,
            color: "#1E90FF",
            // Line chart marker type
            markers: { type: "square" }
        }, {
            name: "pressione al suolo nPa",
            data: pressioneAlSuoloOrarioCorrente,
            color: "#00008B",
            // Line chart marker type
            markers: { type: "square" }
        }],
        valueAxis: {
            line: {
                visible: false
            }
        },
        categoryAxis: {
            categories: dataSettimana,
            majorGridLines: {
                visible: false
            }
        },
        tooltip: {
            visible: true,
            format: "{0}"
        }
    });
}

// -------------------------------------------------------
// ------------------- PIOGGIA AD ORE --------------------
// -------------------------------------------------------
function graficoPioggiaOraria() {
    var series = [{
        name: "mm di pioggia",
        data: mmPioggiaSettimana,

        // Line chart marker type
        markers: { type: "square" }
    }];

    $("#mmrain_hour_graph").kendoChart({
        title: {
            text: "Previsioni settimanali per mm di pioggia \n rilevamenti ogni 3 ore"
        },
        legend: {
            position: "bottom"
        },
        seriesDefaults: {
            type: "line",
            color: "#1E90FF"
        },
        series: series,
        valueAxis: {
            max: mmMax,
            line: {
                visible: false
            }
        },
        categoryAxis: {
            categories: giornoSettimana,
            majorGridLines: {
                visible: false
            }
        },
        tooltip: {
            visible: true,
            format: "{0}"
        }
    });
}

// -------------------------------------------------------
// ------------------- UMIDITA AD ORE --------------------
// -------------------------------------------------------
function graficoPressioneOraria() {

    $("#umm_hour_graph").kendoChart({
        title: {
            text: "Previsioni settimanali per la pressione atmosferica \n rilevamenti ogni 3 ore"
        },
        legend: {
            position: "bottom"
        },
        seriesDefaults: {
            type: "column",
            stack: false        
        },
        series: [{
            name: "pressione hPa",
            data: pressioneSettimana,
            color: "#1E90FF",
            // Line chart marker type
            markers: { type: "square" }
        }, {
            name: "pressione al suolo hPa",
            data: pressioneAlSuoloSettimana,
            color: "#00008B",
            // Line chart marker type
            markers: { type: "square" }
        }],
        valueAxis: {
            max: maxPressione,
            line: {
                visible: false
            }
        },
        categoryAxis: {
            categories: giornoSettimana,
            majorGridLines: {
                visible: false
            }
        },
        tooltip: {
            visible: true,
            format: "{0}"
        }
    });
}

// -------------------------------------------------------
// ---------------- TEMPERATURE AD ORE -------------------
// -------------------------------------------------------
function graficoTemperatureOrario() {

    $("#temp_hour_graph").kendoChart({
        title: {
            text: "Previsioni settimanali per la temperatura °C \n rilevamenti ogni 3 ore"
        },
        legend: {
            position: "bottom"
        },
        seriesDefaults: {
            type: "line"
        },
        series: [{
            name: "temperatura minima °C",
            data: temperaturaMinSettimana,
            color: "#1E90FF",
            // Line chart marker type
            markers: { type: "square" }
        }, {
            name: "temperatura massima °C",
            data: temperaturaMaxSettimana,
            color: "#FF0000",
            // Line chart marker type
            markers: { type: "square" }
        }, {
            name: "temperatura °C",
            data: temperaturaSettimana,
            color: "#000000",
            // Line chart marker type
            markers: { type: "square" }
        }],
        valueAxis: {
            max: tempMaxMax,
            min: tempMinMin,
            line: {
                visible: false
            }
        },
        categoryAxis: {
            categories: giornoSettimana,
            majorGridLines: {
                visible: false
            }
        },
        tooltip: {
            visible: true,
            format: "{0}"
        }
    });
}