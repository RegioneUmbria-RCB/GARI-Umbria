

/* js_demo_tiles.js */




var rainfallOverlayToggle = 0;

var customMapOverlayBase = new Array();

var SatRispostaListaOverlayer = undefined;

//il formato "agronica" andrà dismesso..
var Enum_GmapTilesFormat = {
    Gdal2Tiles: { value: 0, name: "Gdal2Tiles", code: 0 },
    AgronicaOld: { value: 1, name: "AgronicaOld", code: 1 }
}


var Enum_TipoDiOverlay = {
    Sentinel2Agronica: { value: 0, name: "Agronica_Sentinel2", code: 0 },
    wms: { value: 1, name: "wms", code: 1 }
}

//impostare il formato dei dati per i tiles
var GmapTilesFormat = Enum_GmapTilesFormat.Gdal2Tiles;




var urlPathServizioTiles = "http://localhost/Sentinel2/";
var urlPfApi = "http://localhost:52579/WS_Mappe_2013/MappeApi.svc";


/**
 * Legge i dati dim un sensore partendo da un punto (in formato WKT)
 * @param {any} PoligonoWkt punto in formato WKT
 * @param {any} Zoom livello di zoom google
 * @param {any} Sensore sensore da leggere
 * @param {any} DataInizio Data iniziale per lettura
 * @param {any} DataFine Data Finale per lettura
 */
function LetturaDatiElaboratiSuSensore(PoligonoWkt, Zoom, Sensore, DataInizio, DataFine) {


    ajaxAgronica(urlPfApi + "/LetturaDatiElaboratiSuSensoreListaValori", JSON.stringify({ PoligonoWKT: PoligonoWkt, Zoom: Zoom, Sensore: Sensore, DataInizio: DataInizio, DataFine: DataFine }),
        function (risposta) {

            var elencoDati = JSON.parse(risposta.RispostaStringa);

            var categorie = [];
            var valori = [];

            var text = $("#PfDdlSensoreElaborazione").data("kendoDropDownList").value();

            var anno = 0;

            var minValue = 1000.5;
            var maxValue = -1000.5;
            var steps = elencoDati.length;
            var categoryStep = roundNumber(steps / 10, 0);
            if (categoryStep < 1) {
                categoryStep = 1;
            }

            elencoDati.sort(LetturaDatiElaboratiSuSensore_compare);

            for (var i = 0; i < elencoDati.length; i++) {

                var curStrData = elencoDati[i].DataRiferimento;

                //per questione di fuso GMT
                curStrData = curStrData.replace("T00:00:00.0000000", "T06:00:00.0000000");

                var dateObj = kendo.parseDate(curStrData);
                var month = dateObj.getUTCMonth() + 1; //months from 1-12
                var day = dateObj.getUTCDate();
                anno = dateObj.getUTCFullYear();
                categorie.push(day + "/" + month);
                valori.push(elencoDati[i].Valore);

                if (elencoDati[i].Valore > maxValue) {
                    maxValue = elencoDati[i].Valore;
                }

                if (elencoDati[i].Valore < minValue) {
                    minValue = elencoDati[i].Valore;
                }

            }

            var height = $("#pf_data").height() + "px";
            //var width = $("#pf_data").width() + "px";
            var width = "510px";

            if (steps > 6) {
                steps = 6;
            }

            var majorUnit = (maxValue - minValue) / (steps - 1);

            var tabstrip = $("#satTabStrip").data("kendoTabStrip");
            tabstrip.enable(tabstrip.tabGroup.children().eq(1), true);
            tabstrip.select(1);

            var kc = $("#satKendoMicroChart").data("kendoChart");

            if (kc !== undefined) {
                kc.options.categoryAxis.categories = categorie;
                kc.options.categoryAxis.labels.step = categoryStep;
                kc.options.series[0].name = text + " anno " + anno;

                var serieToEdit = kc.findSeriesByIndex(0);
                serieToEdit.data(valori);

                //il colore al momento è un po' HardCoded..
                if (text === "NDWI_GAO" || text === "NDWI_McFeeters") {
                    kc.options.series[0].color = "#0000FF";
                } else {
                    kc.options.series[0].color = "#ff6800";
                }


                kc.redraw();

            } else {

                $("#satKendoMicroChart").kendoChart({
                    render: function (e) {
                        var currentMin = 100000;
                        var currentMax = -100000;
                        var series = e.sender.options.series
                        for (let i = 0; i < series.length; i++) {
                            for (let k = 0; k < series[i].data.length; k++) {
                                if (series[i].data[k] < currentMin && series[i].visible == true) {
                                    currentMin = series[i].data[k]
                                }
                                if (series[i].data[k] > currentMax && series[i].visible == true) {
                                    currentMax = series[i].data[k]
                                }
                            }
                        }
                        var oldMin = e.sender.options.valueAxis.min;
                        var oldMax = e.sender.options.valueAxis.max;
                        if (oldMin != currentMin) {
                            e.sender.options.valueAxis.min = currentMin;
                            e.sender.redraw()
                        }
                        if (oldMax != currentMax) {
                            e.sender.options.valueAxis.max = currentMax;
                            e.sender.redraw()
                        }


                        var steps = series[0].data.length;
                        if (steps > 6) {
                            steps = 6;
                        }
                        var majorUnit = (currentMax - currentMin) / (steps);
                        e.sender.options.valueAxis.majorUnit = majorUnit;

                        console.log("majorUnit, steps: " + steps);
                        console.log("majorUnit ricalcolato: " + majorUnit);

                    },
                    title: {
                        text: ""
                    },
                    seriesDefaults: {
                        type: "line",
                        style: "smooth"
                    },
                    legend: {
                        position: "bottom"
                    },
                    chartArea: {
                        background: "",
                        width: width,
                        height: height
                    },
                    series: [{
                        name: text + " anno " + anno,
                        data: valori
                    }],
                    categoryAxis: {
                        labels: {
                            step: categoryStep
                        },
                        categories: categorie,
                        majorGridLines: {
                            visible: false
                        }
                    },
                    valueAxis: {
                        labels: {
                            format: "{0}",
                            skip: 1,
                            template: "#= kendo.toString(value, '0.000') #"
                        },
                        line: {
                            visible: false
                        },
                        axisCrossingValue: -Number.MAX_VALUE,
                        min: minValue,
                        max: maxValue,
                        majorUnit: majorUnit
                    },
                    tooltip: {
                        visible: true,
                        format: "{0}",
                        template: "#= series.name # (#= category #): #= kendo.toString(value, '0.000') #"
                    }
                });

                $("#satKendoMicroChart").data("kendoChart").refresh();


            }

        }, null);
}

function LetturaDatiElaboratiSuSensore_compare(a, b) {
    if (a.DataRiferimento < b.DataRiferimento)
        return -1;
    if (a.DataRiferimento > b.DataRiferimento)
        return 1;
    return 0;
}

function WmsUrlRER(bbox) {
    var url = "https://servizigis.regione.emilia-romagna.it/wms/suoli?SERVICE=WMS";
    url += "&VERSION=1.3.0";
    url += "&REQUEST=GetMap";
    url += "&BBOX=" + bbox;
    url += "&CRS=EPSG:6706";
    url += "&WIDTH=1089";
    url += "&HEIGHT=872";
    url += "&LAYERS=Carta_Suoli_50k";
    url += "&STYLES=&FORMAT=image/jpeg";
    url += "&DPI=96";
    url += "&MAP_RESOLUTION=96";
    url += "&FORMAT_OPTIONS=dpi:96";
    
    //console.log(url);
    return url;
}

function WmsUrlAE(bbox) {
    var url = "https://wms.cartografia.agenziaentrate.gov.it/inspire/wms/ows01.php?SERVICE=WMS";
    url += "&VERSION=1.3.0";
    url += "&REQUEST=GetMap";
    url += "&BBOX=" + bbox;
    url += "&CRS=EPSG:6706";
    url += "&WIDTH=1089";
    url += "&HEIGHT=872";
    url += "&LAYERS=CP.CadastralZoning,strade,acque,CP.CadastralParcel,fabbricati,vestizioni";
    url += "&STYLES=default";
    url += "&FORMAT=image/png";
    url += "&DPI=96";
    url += "&MAP_RESOLUTION=96";
    url += "&FORMAT_OPTIONS=dpi:96";
    url += "&TRANSPARENT=TRUE";

    return url;
}

function WmsGetFeatureInfoERTest(latLng) {
    
    var o = gMapsUtility.WMS_GetFeatureInfoGeoData(mappa.elemenotMappa, latLng);
   
    //WmsGetFeatureInfo_ChiamataAjax(url, latLng);

}

function WmsGetFeatureInfo(latLng) {

    var o = gMapsUtility.WMS_GetFeatureInfoGeoData(mappa.elemenotMappa, latLng);

    //base WMS URL
    var url = "https://wms.cartografia.agenziaentrate.gov.it/inspire/wms/ows01.php?language=ita"
    url += "&SERVICE=WMS";
    url += "&VERSION=1.3.0";
    url += "&REQUEST=GetFeatureInfo";
    url += "&BBOX=" + o.BBOX;
    url += "&CRS=EPSG:6706";
    url += "&WIDTH=" + o.WIDTH;
    url += "&HEIGHT=" + o.HEIGHT;
    url += "&LAYERS=CP.CadastralZoning,CP.CadastralParcel";
    url += "&STYLES=default";
    url += "&FORMAT=image/png";
    url += "&QUERY_LAYERS=CP.CadastralParcel";
    url += "&INFO_FORMAT=text/html";
    url += "&X=" + o.X;
    url += "&Y=" + o.Y;

    console.log(url);
    WmsGetFeatureInfo_ChiamataAjax(url, latLng);
}

//per chiamata Standard Ajax
function WmsGetFeatureInfo_ChiamataAjax(theUrl, coord) {
    ajaxAgronica(indirizzohttp + "/WmsGetFeatureInfo_httpGet", JSON.stringify({ url: theUrl }),
        function (risposta) {
            WmsGetFeatureInfo_popup(risposta.RispostaStringa, coord);
        }, null);
}

function WmsGetFeatureInfo_httpGet(theUrl, coord) {
    var xhttp;
    //istanza di una richiesta XHTTP
    xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {

            WmsGetFeatureInfo_popup(xhttp.responseText, coord);
        } else {
            kendo.alert("Si è verificato un errore.");
        }
    };
    xhttp.open("GET", theUrl, true);
    xhttp.send();
}

function WmsGetFeatureInfo_popup(testo, coord) {
    var infowindow = new google.maps.InfoWindow({
        content: testo,
        position: coord

    });
    infowindow.open(mappa.elemenotMappa);
}

function customMapOverlayBaseInizializzaCalendario() {

    if (!$("#satPfTools").is(":visible")) {
        return;
    }
    

    console.log("chiamo customMapOverlayBaseInizializzaCalendario");

    var windowWidgetPF = $("#satPfTools").data("kendoWindow");
    kendo.ui.progress(windowWidgetPF.element, true);


    //var dataRiferimento = kendo.toString($("#pf_data").data("kendoCalendar").selectDates()[0], "dd/MM/yyyy");
    var bounds = mappa.elemenotMappa.getBounds();
    var PoligonoWKT = "";
    if (bounds !== undefined) {
        PoligonoWKT = mappa.GoogleBoundsWKT();
    }

    ajaxAgronica(urlPfApi + "/customMapOverlayBaseInizializzaCalendario", JSON.stringify({ PoligonoWKT: PoligonoWKT }),
        function (risposta) {


            SatRispostaListaOverlayer = risposta.RispostaStringa.ListaOverlayer;
            var DateDaInserire = [];

            for (var i = 0; i < SatRispostaListaOverlayer.length; i++) {
                DateDaInserire.push(kendo.parseDate(SatRispostaListaOverlayer[i].DataRiferimento, "dd/MM/yyyy").getTime());
            }

            var dataSelezione = undefined;
            if (DateDaInserire.length > 0) {
                dataSelezione = kendo.parseDate(SatRispostaListaOverlayer[0].DataRiferimento, "dd/MM/yyyy");
            }

            var cal = $("#pf_data").data("kendoCalendar");
            if (cal !== undefined) {
                cal.destroy();
                $("#pf_data").html("");
            }

            //selectable: "multiple",
            $("#pf_data").kendoCalendar({
                value: dataSelezione,
                dates: DateDaInserire,
                change: pfDataChange,
                weekNumber: true,
                month: {
                    // template for dates in month view
                    content: '# if ($.inArray(data.date.getTime(), data.dates) != -1) { # ' +
                        '<div class="calendarImgPresente">#= data.value #</div>' +
                        '# } else { #' +
                        '<div class="calendarImgAssente">#= data.value #</div>' +
                        '# } #'
                },
                footer: false
            });


            $("#SatElaborazioneMsg").height($("#pf_data").height());

            customMapOverlayBaseInizializza();

            $("#SatElaborazioneMsg").MapProc_Message({ url: indirizzohttp });

            //un minimo di delay per dare l'idea che elabora .. 
            setTimeout(function () {
                var windowWidgetPF = $("#satPfTools").data("kendoWindow");
                kendo.ui.progress(windowWidgetPF.element, false);
            }, 1600);


        }, function (risposta) {
            $("#pf_data").hide();
            $("#SatElaborazioneMsg").css({ "padding": "10px", "text-align": "center" });
            $("#SatElaborazioneMsg").text("Errore di comunicazione con il server");
        },
        null,
        false
    );
}


function customMapOverlayBaseInizializza() {

    var urlBase = "";
    urlBase = urlPathServizioTiles;



    var dataRiferimento = kendo.toString($("#pf_data").data("kendoCalendar").value(), "dd/MM/yyyy");

    if (dataRiferimento) {

        customMapOverlayBase = new Array();
        rainMapOverlayArray = new Array();

        PfDdlSensoreElaborazione_Change_valoreSelezionato = $("#PfDdlSensoreElaborazione").data("kendoDropDownList").value();
        var oSensore = new Object();
        for (var i = 0; i < SatRispostaListaOverlayer.length; i++) {

            var curOl = SatRispostaListaOverlayer[i];

            if (dataRiferimento === curOl.DataRiferimento) {

                for (var j = 0; j < curOl.Passaggi.length; j++) {

                    var curPass = curOl.Passaggi[j];

                    for (var z = 0; z < curPass.Sensore.length; z++) {

                        var curSensore = curPass.Sensore[z];

                        oSensore[curSensore.CodiceSensore] = curSensore.Descrizione;

                        var opacityValue = getOpacityValue("#satSliderTrasparenza");

                        var urlToInsert = urlBase + curPass.url + "/" + curSensore.CodiceSensore + "/";
                        customMapOverlayBase.push({
                            Tile: curPass.Tile.Tile,
                            SensoreElaborazione: curSensore.CodiceSensore,
                            DataRiferimento: dataRiferimento,
                            Descrizione: "Sentinel 2 - " + dataRiferimento + " - " + curSensore.Descrizione + "  ",
                            url: urlToInsert,
                            opacity: opacityValue
                        });

                        rainMapOverlayArray.push({ url: OverlayGenera(Enum_TipoDiOverlay.Sentinel2Agronica, 256, urlToInsert), toggle: 0 });

                    }

                }
            }

        }

        let keys = Object.keys(oSensore);
        let ds = new Array()
        for (k = 0; k < keys.length; k++) {
            ds.push({ text: oSensore[keys[k]], value: keys[k] });
        }
        $("#PfDdlSensoreElaborazione").data("kendoDropDownList").setDataSource(ds);

    }

}

var PfDdlSensoreElaborazione_Change_valoreSelezionato;
function pfDataChange() {
    customMapOverlayBaseInizializza();
    if (PfDdlSensoreElaborazione_Change_valoreSelezionato !== "Sel") {
        $("#PfDdlSensoreElaborazione").data("kendoDropDownList").value(PfDdlSensoreElaborazione_Change_valoreSelezionato);
    }
    OverlayRielabora(Enum_TipoDiOverlay.Sentinel2Agronica);
}

function PfDdlSensoreElaborazione_Change() {

    OverlayRielabora(Enum_TipoDiOverlay.Sentinel2Agronica);

}

function wmsDdlSensoreElaborazione_Change() {

    WmsOverlayReset();
    OverlayRielabora(Enum_TipoDiOverlay.wms);

}


function WmsOverlayReset() {

    customMapOverlayBase = new Array();
    rainMapOverlayArray = new Array();

    var opacityValue = getOpacityValue("#wmsSliderTrasparenza");

    var urlToInsert = "";
    customMapOverlayBase.push({
        Tile: "Wms - Catasto",
        SensoreElaborazione: "Wms - Catasto",
        DataRiferimento: "01/01/1900",
        Descrizione: "Wms - Catasto",
        url: urlToInsert,
        opacity: opacityValue
    });

    rainMapOverlayArray.push({ url: OverlayGenera(Enum_TipoDiOverlay.wms, 512, urlToInsert), toggle: 0 });
}


function OverlayRielabora(TipoDiOverlay) {

    switch (TipoDiOverlay) {
        case Enum_TipoDiOverlay.Sentinel2Agronica:
            var dataRiferimento = kendo.toString($("#pf_data").data("kendoCalendar").value(), "dd/MM/yyyy");
            var sensore = $("#PfDdlSensoreElaborazione").data("kendoDropDownList").value();

            if (dataRiferimento && sensore) {
                if (sensore !== "") {
                    attivaSatelliteNewSetOverlayInserisciPerDataSensore(dataRiferimento, sensore, "#satSliderTrasparenza");
                }
            }
            break;

        case Enum_TipoDiOverlay.wms:
            var sensore = $("#WmsDdlSensoreElaborazione").data("kendoDropDownList").value();
            attivaSatelliteNewSetOverlayInserisciPerDataSensore("01/01/1900", sensore, "#wmsSliderTrasparenza");
            break;
        default:

    }



}


function getOpacityValue(sliderJquerySelector, val) {

    if (typeof val !== "number") {
        val = $(sliderJquerySelector).data("kendoSlider").value() / 100.0;
    } else {
        val = val / 100.0;
    }
    return 1.0 - val;
}

function satSliderTrasparenza_OnChange(e) {

    if (e.value === 0) {
        $("#sat_lbl_trasparenza").text("Trasparenza:");
    } else {
        $("#sat_lbl_trasparenza").text(kendo.format("Trasparenza ({0}%):", e.value));
    }

    OverlayImpostaOpacity(getOpacityValue("#satSliderTrasparenza", e.value));
}

function OverlayImpostaOpacity(opacityValue) {


    var omap = mappa.elemenotMappa.overlayMapTypes.getAt(0);

    if (omap !== undefined) {
        omap.setOpacity(opacityValue);
    }
}


/**
 * Funzione di generazione di overlay
 * @param {any} TipoDiOverlay
 * @param {any} DimensioneTile
 * @param {any} url
 */
function OverlayGenera(TipoDiOverlay, tileDimension, url) {

    var selettoreOpacity;
    if (TipoDiOverlay === Enum_TipoDiOverlay.Sentinel2Agronica) {
        selettoreOpacity = "#satSliderTrasparenza";
    } else {
        selettoreOpacity = "#wmsSliderTrasparenza"
    }

    var opacityValue = getOpacityValue(selettoreOpacity);

    switch (TipoDiOverlay) {

        case Enum_TipoDiOverlay.Sentinel2Agronica:

            return new google.maps.ImageMapType({
                getTileUrl: function (tile, zoom) {


                    if (GmapTilesFormat == Enum_GmapTilesFormat.Gdal2Tiles) {

                        //formato di cartelle file system restuito da GDAL2TILES
                        var ymax = 1 << zoom;
                        var y = ymax - tile.y - 1;
                        return url + zoom + "/" + tile.x + "/" + y + ".png";

                    } else {

                        //formato "agronica"
                        return url + coord.x + '_' + coord.y + '_' + zoom + '.png';
                    }

                },
                tileSize: new google.maps.Size(tileDimension, tileDimension),
                maxZoom: 15,
                minZoom: 3,
                opacity: opacityValue
            });

        case Enum_TipoDiOverlay.wms:
            return new google.maps.ImageMapType({
                getTileUrl: function (coord, zoom) {
                    var proj = mappa.elemenotMappa.getProjection();
                    var zfactor = Math.pow(2, zoom);
                    // get Long Lat coordinates
                    var top = proj.fromPointToLatLng(new google.maps.Point(coord.x * tileDimension / zfactor, coord.y * tileDimension / zfactor));
                    var bot = proj.fromPointToLatLng(new google.maps.Point((coord.x + 1) * tileDimension / zfactor, (coord.y + 1) * tileDimension / zfactor));

                    //corrections for the slight shift of the SLP (mapserver)
                    var deltaX = 0.0;
                    var deltaY = 0.0;

                    //create the Bounding box string
                    //var bbox = (top.lng() + deltaX) + "," +
                    //    (bot.lat() + deltaY) + "," +
                    //    (bot.lng() + deltaX) + "," +
                    //    (top.lat() + deltaY);

                    var bbox = (bot.lat() + deltaX) + "," +
                        (top.lng() + deltaY) + "," +
                        (top.lat() + deltaX) + "," +
                        (bot.lng() + deltaY);

                    //base WMS URL
                    //var url = WmsUrlRER(bbox); //Prototipo Regione ER
                    var url = WmsUrlAE(bbox);

                    if (Debug_Mode === Enum_debugMode.Soft) {
                        console.log(url);
                    }

                    return url;                 // return URL for the tile

                },
                tileSize: new google.maps.Size(tileDimension, tileDimension),
                isPng: true,
                minZoom: 17,
                maxZoom: 20
            });
            
        default: break;
    }

}






/**
 * funzione di partenza
 * */
function DemoGis() {

    //verifico in base a questo elemento se sono nella nuova versione o in quella precedente .

    if ($("#satPFToolsNavBar").length) {

        //inizializza da configurazioni
        urlPathServizioTiles = Gis_Global_ws_mappe_2013_basePath + "Sentinel2/";
        urlPfApi = indirizzohttp;

        customMapOverlayBaseInizializzaCalendario();
    } else {

        var homeControlContenitore = document.createElement("div");

        for (var i = 0; i < customMapOverlayBase.length; i++) {

            mappa.elemenotMappa.overlayMapTypes.insertAt(
                i, new CoordMapType(new google.maps.Size(256, 256)));

            var homeControlDiv = document.createElement('div');
            homeControlContenitore.appendChild(homeControlDiv);
            var homeControl = new HomeControl(homeControlDiv, map, customMapOverlayBase[i], i);

            homeControlDiv.index = i + 1;
            mappa.elemenotMappa.controls[google.maps.ControlPosition.TOP_RIGHT].push(homeControlContenitore);
        }


    }
}


function attivaSatelliteNewSetOverlayInserisciPerDataSensore(dataRiferimento, sensore, sliderJquerySelector) {

    attivaSatelliteRipulisiciOverlays();

    for (var i = 0; i < customMapOverlayBase.length; i++) {

        var cOver = customMapOverlayBase[i];
        if (cOver.SensoreElaborazione === sensore && cOver.DataRiferimento === dataRiferimento) {
            attivaSatelliteNewSetOverlayInserisciPerIndice(i, sliderJquerySelector);
        }

    }

}

function attivaSatelliteRipulisiciOverlays() {


    for (var i = 0; i < rainMapOverlayArray.length; i++) {        
        rainMapOverlayArray[i].toggle = 0;

        for (var j = 0; j < mappa.elemenotMappa.overlayMapTypes.length; j++) {
            if (mappa.elemenotMappa.overlayMapTypes.getAt(j).constructor.name !== "CoordMapType_Tiles") {
                mappa.elemenotMappa.overlayMapTypes.removeAt(j, rainMapOverlayArray[i].url);
            }
        }
    }

}

function attivaSatelliteNewSetOverlayInserisciPerIndice(indice, sliderJquerySelector) {

    //Overlays the rainfall map on top of the Google map    
    mappa.elemenotMappa.overlayMapTypes.insertAt(0, rainMapOverlayArray[indice].url);

    var opacityValue = getOpacityValue(sliderJquerySelector);

    if (opacityValue < 1) {
        OverlayImpostaOpacity(opacityValue);
    }

    rainMapOverlayArray[indice].toggle = 1

}



function attivaSatelliteNewSetOverlayInserisci(indice) {


    //Overlays the rainfall map on top of the Google map
    mappa.elemenotMappa.overlayMapTypes.insertAt(0, rainMapOverlayArray[indice].url);
    //Show the weather key.
    //mappa.elemenotMappa.controls[google.maps.ControlPosition.TOP_LEFT].push(weatherKeyDiv);
    rainMapOverlayArray[indice].toggle = 1;
    document.getElementById("sat_" + indice).style.backgroundColor = 'yellow';

}

function attivaSatelliteNewSetOverlayRimuovi(indice) {

    //remove the overlay map.
    mappa.elemenotMappa.overlayMapTypes.removeAt(0, rainMapOverlayArray[indice].url);
    //remove the weather key
    //mappa.elemenotMappa.controls[google.maps.ControlPosition.TOP_LEFT].pop(weatherKeyDiv);
    rainMapOverlayArray[indice].toggle = 0;
    document.getElementById("sat_" + indice).style.backgroundColor = 'white';



}


var rainMapOverlayArray = new Array();

function attivaSatelliteNew(indice) {

    var inserire = (rainMapOverlayArray[indice].toggle === 0);

    for (var i = 0; i < rainMapOverlayArray.length; i++) {
        attivaSatelliteNewSetOverlayRimuovi(i);
    }

    //If the rainfall map is NOT showing aleady then show it ...
    if (inserire) {
        attivaSatelliteNewSetOverlayInserisci(indice);
    }

}





//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

var TileCoveringAlg = function (TileSize) {

    var TILE_SIZE = TileSize;
    var SCALE = 1;
    var TILE_SIDES = [
        { p0: new google.maps.Point(), p1: new google.maps.Point(), incr_x: -1, incr_y: 0, skip: 2 },   //left -> Skip right
        { p0: new google.maps.Point(), p1: new google.maps.Point(), incr_x: 0, incr_y: -1, skip: 3 },   //top -> Skip bottom
        { p0: new google.maps.Point(), p1: new google.maps.Point(), incr_x: 1, incr_y: 0, skip: 0 },    //right -> Skip left
        { p0: new google.maps.Point(), p1: new google.maps.Point(), incr_x: 0, incr_y: 1, skip: 1 }     //bottom -> Skip top
    ];

    var WorldCoordinate = function (latlng) {
        let siny = Math.sin(latlng.lat() * Math.PI / 180);
        // Truncating to 0.9999 effectively limits latitude to 89.189.
        // This is about a third of a tile past the edge of the world tile.
        siny = Math.min(Math.max(siny, -0.9999), 0.9999);
        let wCoord = new google.maps.Point(
            TILE_SIZE * (0.5 + latlng.lng() / 360),
            TILE_SIZE * (0.5 - Math.log((1 + siny) / (1 - siny)) / (4 * Math.PI))
        );
        return wCoord;
    }

    var TileFromWorldCoordinate = function (wCoord) {
        //tile -> topleft corner
        let fact = SCALE / TILE_SIZE;
        return new google.maps.Point(Math.floor(wCoord.x * fact), Math.floor(wCoord.y * fact));
    }

    var Segment = function (v0, v1) {

        var orientation = function (p, q, r) {
            // To find orientation of ordered triplet (p, q, r).
            // The function returns following values
            // 0 --> p, q and r are colinear
            // 1 --> Clockwise
            // 2 --> Counterclockwise
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ for details of below formula.
            let val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
            if (val === 0) return 0;  // colinear 
            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }

        var onSegment = function (p, q, r) {
            if (q.x <= Math.max(p.x, r.x) && q.x >= Math.min(p.x, r.x) &&
                q.y <= Math.max(p.y, r.y) && q.y >= Math.min(p.y, r.y))
                return true;
            return false;
        }

        return {
            v0: v0,
            v1: v1,

            Intersect: function (p2, q2) {
                //intersezione tra segmento [p1, q1] e segmento [p2, q2]

                var p1 = this.v0;
                var q1 = this.v1;

                // Find the four orientations needed for general and special cases
                let o1 = orientation(p1, q1, p2);
                let o2 = orientation(p1, q1, q2);
                let o3 = orientation(p2, q2, p1);
                let o4 = orientation(p2, q2, q1);

                // General case
                if (o1 != o2 && o3 != o4) {
                    return true;
                }

                //Special cases

                // p1, q1 and p2 are colinear and p2 lies on segment p1q1
                if (o1 == 0 && onSegment(p1, p2, q1)) {
                    return true;
                }
                // p1, q1 and q2 are colinear and q2 lies on segment p1q1
                if (o2 == 0 && onSegment(p1, q2, q1)) {
                    return true;
                }
                // p2, q2 and p1 are colinear and p1 lies on segment p2q2
                if (o3 == 0 && onSegment(p2, p1, q2)) {
                    return true;
                }
                // p2, q2 and q1 are colinear and q1 lies on segment p2q2
                if (o4 == 0 && onSegment(p2, q1, q2)) {
                    return true;
                }
                return false;
            }
        }
    }

    var SidesForTile = function (tile) {
        //left
        TILE_SIDES[0].p0 = new google.maps.Point(tile.x, tile.y);
        TILE_SIDES[0].p1 = new google.maps.Point(tile.x, tile.y + 1);
        //top
        TILE_SIDES[1].p0 = new google.maps.Point(tile.x, tile.y);
        TILE_SIDES[1].p1 = new google.maps.Point(tile.x + 1, tile.y);
        //right
        TILE_SIDES[2].p0 = new google.maps.Point(tile.x + 1, tile.y);
        TILE_SIDES[2].p1 = new google.maps.Point(tile.x + 1, tile.y + 1);
        //bottom
        TILE_SIDES[3].p0 = new google.maps.Point(tile.x, tile.y + 1);
        TILE_SIDES[3].p1 = new google.maps.Point(tile.x + 1, tile.y + 1);
    }

    var jagArray = function () {
        return {
            array: new Array(),
            add: function (liv1, liv2) {
                let len = this.array.length;
                let idx = 0;
                let found = false;
                while (!found && idx < len) {
                    if (this.array[idx].liv1 === liv1) {
                        if (liv2 < this.array[idx].liv2_min) {
                            this.array[idx].liv2_min = liv2;
                        } else if (liv2 > this.array[idx].liv2_max) {
                            this.array[idx].liv2_max = liv2;
                        }
                        found = true;
                    } else {
                        if (this.array[idx].liv1 > liv1) {
                            this.array.splice(idx, 0, { liv1: liv1, liv2_min: liv2, liv2_max: liv2 });
                            found = true;
                        }
                    }
                    idx++;
                }
                if (!found) {
                    this.array.push({ liv1: liv1, liv2_min: liv2, liv2_max: liv2 });
                }
            }
        }
    }

    return {
        Calc: function (polygon, zoom) {
            SCALE = 1 << zoom;

            let fact = TILE_SIZE / SCALE;

            let coveringTiles_cols = new jagArray();
            let coveringTiles_rows = new jagArray();

            let p0 = WorldCoordinate(polygon[0]);

            let currentTile = TileFromWorldCoordinate(p0);
            SidesForTile(currentTile);

            coveringTiles_cols.add(currentTile.x, currentTile.y);
            coveringTiles_rows.add(currentTile.y, currentTile.x);

            let idx = 1;
            let len = polygon.length;
            let seg = new Segment(p0, WorldCoordinate(polygon[idx]));
            let skip_side = -1;
            while (idx <= len) {

                //Cerco da quale lato del tile "esce" il segmento del poligono... (nel caso ignoro quello da cui sono entrato)
                let next_seg = true;
                let i_side = 0;
                while (i_side < TILE_SIDES.length) {

                    if (i_side !== skip_side) {

                        let side_p0 = new google.maps.Point(TILE_SIDES[i_side].p0.x * fact, TILE_SIDES[i_side].p0.y * fact);
                        let side_p1 = new google.maps.Point(TILE_SIDES[i_side].p1.x * fact, TILE_SIDES[i_side].p1.y * fact);

                        if (seg.Intersect(side_p0, side_p1)) {

                            skip_side = TILE_SIDES[i_side].skip;
                            currentTile = new google.maps.Point(currentTile.x + TILE_SIDES[i_side].incr_x, currentTile.y + TILE_SIDES[i_side].incr_y);
                            SidesForTile(currentTile);

                            coveringTiles_cols.add(currentTile.x, currentTile.y);
                            coveringTiles_rows.add(currentTile.y, currentTile.x);

                            let chkTile = TileFromWorldCoordinate(seg.v1);
                            if (currentTile.x !== chkTile.x || currentTile.y !== chkTile.y) {
                                next_seg = false;
                            }

                            i_side = TILE_SIDES.length;
                        }
                    }
                    i_side++;
                }

                if (next_seg) {
                    idx++;
                    if (idx <= len) {
                        seg.v0 = seg.v1;
                        if (idx < polygon.length) {
                            seg.v1 = WorldCoordinate(polygon[idx]);
                        } else {
                            seg.v1 = p0;
                        }
                        skip_side = -1;
                    }
                }
            }

            let aTiles = new Array();
            let iCol = 0;
            let LCol = coveringTiles_cols.array.length;
            let iRow;
            let LRow = coveringTiles_rows.array.length;

            while (iCol < LCol) {
                let liv1 = coveringTiles_cols.array[iCol].liv1;
                let liv2 = coveringTiles_cols.array[iCol].liv2_min;
                while (liv2 <= coveringTiles_cols.array[iCol].liv2_max) {

                    iRow = 0;
                    while (iRow < LRow) {
                        if (coveringTiles_rows.array[iRow].liv1 === liv2) {
                            if (coveringTiles_rows.array[iRow].liv2_min <= liv1 && liv1 <= coveringTiles_rows.array[iRow].liv2_max) {
                                aTiles.push(new google.maps.Point(liv1, liv2));
                            }
                            iRow = LRow + 1;
                        } else {
                            if (coveringTiles_rows.array[iRow].liv1 > liv2) {
                                iRow = LRow + 1;
                            }
                        }
                        iRow++;
                    }

                    liv2++;
                }
                iCol++;
            }

            return aTiles;
        }
    }
}

function ElaboraMappaDettagliataSuSelezione_Click() {

    if (shape.selectedShape === undefined || shape.selectedShape === null) {
        return;
    }

    let proj = mappa.elemenotMappa.getProjection();

    let tca = new TileCoveringAlg(256);

    let MVCArray = shape.selectedShape.getPath();

    let tiles = new Array();

    //' VAnni: 4/11/2019: parto dal livello 14 a seguito di rimozione di tale livello per via dello spazio a disposizione..
    let zoom = 14;

    if (false) {

        while (zoom > 0) {

            let aTiles = tca.Calc(MVCArray.getArray(), zoom);

            if (aTiles.length === 1) {

                let scale = 1 << zoom;
                let fact = 256 / scale;

                let ne = proj.fromPointToLatLng(new google.maps.Point((aTiles[0].x + 1) * fact, aTiles[0].y * fact));
                let sw = proj.fromPointToLatLng(new google.maps.Point(aTiles[0].x * fact, (aTiles[0].y + 1) * fact));

                tiles.push({
                    zoom: zoom,
                    xTile: aTiles[0].x,
                    yTile: scale - 1 - aTiles[0].y,
                    n: ne.lng(),
                    e: ne.lat(),
                    s: sw.lng(),
                    w: sw.lat()
                });

                zoom = 0;

            }
            zoom--;
        }

    } else {

        //' VAnni: 17/01/2020: arrivo al livello 15 (non più 17) per via dello spazio a disposizione e del numero di record su database..
        while (zoom <= 15) {

            let aTiles = tca.Calc(MVCArray.getArray(), zoom);

            let scale = 1 << zoom;
            let fact = 256 / scale;

            let t = 0;
            while (t < aTiles.length) {

                let ne = proj.fromPointToLatLng(new google.maps.Point((aTiles[t].x + 1) * fact, aTiles[t].y * fact));
                let sw = proj.fromPointToLatLng(new google.maps.Point(aTiles[t].x * fact, (aTiles[t].y + 1) * fact));

                tiles.push({
                    zoom: zoom,
                    xTile: aTiles[t].x,
                    yTile: scale - 1 - aTiles[t].y,
                    n: ne.lng(),
                    e: ne.lat(),
                    s: sw.lng(),
                    w: sw.lat()
                });

                t++;
            }
            zoom++;
        }

    }


    /*
    livello 16
    
    tile
     {x: 34976, y: 23596} scale - 1 - y:41939
    
    
    scale = 1 << zoom
    
    
    
    n   ne.lng()
    12.1343994140625
    e   ne.lat()
    44.91813929958515
    
    s   sw.lng()
    12.12890625
    w   sw.lat()
    44.914249368747086
    
    
    nw						ne				se				sw
    
    12.1343994140625 44.914249368747086, 12.1343994140625 44.91813929958515, 12.12890625 44.91813929958515, 12.12890625 44.914249368747086, 12.1343994140625 44.914249368747086    */

    //let tile = aTiles[0];

    ////function tileCoordsToBBox(map, coord, zoom, tileWidth, tileHeight) {
    //// scale is because the number of tiles shown at each zoom level double.
    //let fact = 256 / (1 << zoom);
    //// A point is created for the north-east and south-west corners, calculated
    //// by taking the tile coord and multiplying it by the tile's width and the map's scale.
    //var ne = proj.fromPointToLatLng(new google.maps.Point( (tile.x + 1) * fact, tile.y * fact));
    //var sw = proj.fromPointToLatLng(new google.maps.Point( tile.x * fact, (tile.y + 1) * fact));

    //if (mappa.elemenotMappa.overlayMapTypes.j.length > 1) {
    //    mappa.elemenotMappa.overlayMapTypes.j.splice(1, 1);
    //}
    //mappa.elemenotMappa.overlayMapTypes.push(new CoordMapType_Tiles_Debug(new google.maps.Size(256, 256), aTiles));


    ajaxAgronicaSync(urlPfApi + "/ElaboraMappaDettagliataSuPoligono",
        JSON.stringify({ Tiles: JSON.stringify(tiles), EntitaCod: shape.selectedShape.Entita_Cod }),
        false,
        function (risposta) {

            let oRisp = JSON.parse(risposta.RispostaStringa);

            let win_el = document.createElement("div");
            document.body.appendChild(win_el);
            let $win_el = $(win_el);

            $win_el.kendoDialog({
                title: "Elaborazione mappa dettagliata",
                closable: false,
                modal: true,
                visible: false,
                content: oRisp.OutMsg,
                actions: [
                    { text: 'Ok' }
                ],
                close: function (e) {
                    this.destroy();

                    let tl = $("#SatElaborazioneMsg").data('MapProc_Message');
                    tl.AccodaNuoveConfigurazioni(oRisp.TilesCodes);
                    tl.IniziaPolling();
                }
            });

            $win_el.data("kendoDialog").open();

        }, null);
}


//-------------------------------------------------------------------------------------------------
//SOLO PER DEBUG ----------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
/*
function CoordMapType_Tiles_Debug(tileSize, aTiles) {
    this.tileSize = tileSize;
    let sortedArr = aTiles.sort(function (a, b) {
        if (a.x < b.x) {
            return -1;
        } else if (a.x > b.x) {
            return 1;
        } else {
            if (a.y < b.y) {
                return -1;
            } else if (a.y > b.y) {
                return 1;
            }
        }
        return 0;
    });

    this.redTiles = new Array();
    let l = sortedArr.length;
    for (let i = 0; i < l; i++) {
        for (let j = i + 1; j < l; j++) {
            // If a[i] is found later in the array
            if (sortedArr[i].x === sortedArr[j].x && sortedArr[i].y === sortedArr[j].y)
                j = ++i;
        }
        this.redTiles.push(sortedArr[i]);
    }
}


CoordMapType_Tiles_Debug.prototype.getTile = function (coord, zoom, ownerDocument) {

    let coord1 = coord;
    // tile range in one direction range is dependent on zoom level
    // 0 = 1 tile, 1 = 2 tiles, 2 = 4 tiles, 3 = 8 tiles, etc
    //let tileRange = 1 << zoom;
    //coord1.y = tileRange - 1 - coord.y;

    let len = this.redTiles.length;
    let idx = 0;
    let div = null;
    while (idx < len) {
        if (this.redTiles[idx].x === coord.x) {
            if (this.redTiles[idx].y === coord.y) {

                div = ownerDocument.createElement('div');
                div.style.width = this.tileSize.width + 'px';
                div.style.height = this.tileSize.height + 'px';
                div.style.backgroundColor = '#FF0000';
                div.style.opacity = 0.5;

                idx = len + 1;
            }
        }
        idx++;
    }
    return div;
};
*/
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------



function CoordMapType(tileSize) {
    this.tileSize = tileSize;
}

CoordMapType.prototype.getTile = function (coord, zoom, ownerDocument) {
    var div = ownerDocument.createElement('div');
    //div.innerHTML = coord;
    //div.style.width = this.tileSize.width + 'px';
    //div.style.height = this.tileSize.height + 'px';
    //div.style.fontSize = '10';
    //div.style.borderStyle = 'solid';
    //div.style.borderWidth = '1px';
    //div.style.borderColor = '#AAAAAA';
    return div;
};


function HomeControl(controlDiv, map, cfg, indice) {

    // Set CSS styles for the DIV containing the control
    // Setting padding to 5 px will offset the control
    // from the edge of the map.
    controlDiv.style.padding = '5px';

    // Set CSS for the control border.
    var controlUI = document.createElement('div');
    controlUI.id = "sat_" + indice;
    controlUI.style.backgroundColor = 'white';
    controlUI.style.borderStyle = 'solid';
    controlUI.style.borderWidth = '2px';
    controlUI.style.cursor = 'pointer';
    controlUI.style.textAlign = 'center';
    controlUI.style.zIndex = 10000;
    controlUI.title = cfg.Descrizione;
    controlDiv.appendChild(controlUI);

    // Set CSS for the control interior.
    var controlText = document.createElement('div');
    controlText.style.fontFamily = 'Arial,sans-serif';
    controlText.style.fontSize = '12px';
    controlText.style.paddingLeft = '4px';
    controlText.style.paddingRight = '4px';
    controlText.innerHTML = '<strong>' + cfg.Descrizione + '</strong>';
    controlUI.appendChild(controlText);

    rainMapOverlayArray.push({ url: OverlayGenera(Enum_TipoDiOverlay.Sentinel2Agronica, 256, cfg.url), toggle: 0 });

    // Setup the click event listeners: simply set the map to Chicago.
    google.maps.event.addDomListener(controlUI, 'click', function () {
        attivaSatelliteNew(indice);
    });
}


//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

//$(...).MapProc_Message({ Configurazione json });
//
//Configurazione:
//      url: "Gis.aspx" (Obbligatorio)

(function ($) {

    $.MapProc_Message = function (elem, options) {

        // to avoid confusions, use "plugin" to reference the current instance of the object
        var plugin = this;
        // this will hold the merged default, and user-provided options plugin's properties will be available through this object like:
        // plugin.settings.propertyName from inside the plugin or element.data('pluginName').settings.propertyName from outside the plugin,
        // where "element" is the element the plugin is attached to;
        plugin.settings = {}
        plugin.$element = $(elem);  // reference to the jQuery version of DOM element
        plugin.element = elem;      // reference to the actual DOM element
        plugin.wrapper = null;

        // plugin's default options this is private property and is accessible only from inside the plugin
        let defaults = {
        }

        // the plugin's final properties are the merged default and user-provided options (if any)
        plugin.settings = $.extend({}, defaults, options);

        var ws_params = {
            FiltroElaborazioni: -1,
            ListaConfigurazioni: "",
            DataRiferimentoElaborazioni: ""
        };

        var _polling = false;

        var arr_msgs = new Array();

        //-------------------------------------------------------------------------------------------------
        // public methods
        //-------------------------------------------------------------------------------------------------

        plugin.AccodaNuoveConfigurazioni = function (configurazioni) {
            if (ws_params.ListaConfigurazioni === "") {
                ws_params.ListaConfigurazioni = configurazioni;
            } else {
                ws_params.ListaConfigurazioni = ws_params.ListaConfigurazioni + "," + configurazioni;
            }

        }
        plugin.IniziaPolling = function () {

            console.log("IniziaPolling");

            ws_params.FiltroElaborazioni = -1;

            //TODO: 
            // Se sto già facendo polling???
            // La chiamata a questa funzione avviene in corrispondenza della pressione del pulsante "Elabora mappa dettagliata su poligono"
            // per cui si generano nuovi codici in aggiunta a ListaConfigurazioni e la DataRiferimentoElaborazioni potrebbe essere 
            // diversa da quella letta in precedenza...

            _polling = true;

            setTimeout(_pollingFunc, 1000);
        }

        plugin.FinePolling = function () {
            console.log("FinePolling");
            _polling = false;
        }

        //-------------------------------------------------------------------------------------------------
        // private methods
        //-------------------------------------------------------------------------------------------------

        var _generaRigaTabella = function (Testo, Colore) {

            if (plugin.wrapper === null) {
                return;
            }

            let tr = document.createElement("tr");
            let td1 = document.createElement("td");
            let spn = document.createElement("span")
            spn.textContent = Testo;
            spn.style.setProperty("color", Colore);
            td1.appendChild(spn)

            tr.appendChild(td1);

            plugin.wrapper.appendChild(tr);
        }


        var _showMsgs = function (DataRiferimentoElaborazione) {
            if (plugin.wrapper === null) {
                return;
            }

            let len = arr_msgs.length;
            let idx = 0;
            let conteggioNonElaborati = 0;
            let conteggioElaboratiDataPrecedente = 0;
            let conteggioElaboratiDataSuccessiva = 0;

            while (idx < len) {

                switch (arr_msgs[idx].stato) {
                    case 0:
                        conteggioNonElaborati++;
                        break;
                    case 1:
                        conteggioElaboratiDataPrecedente++;
                        break;
                    case 2:
                        conteggioElaboratiDataSuccessiva++;
                        break;
                }


                idx++;
            }

            if (conteggioNonElaborati === 0) {
                plugin.FinePolling();
            }

            //pulizia...
            plugin.wrapper.innerHTML = "";

            //i18n__
            if (conteggioNonElaborati > 0) {
                _generaRigaTabella("Elementi in attesa di elaborazione: " + conteggioNonElaborati.toString(), "Red");
            } else {
                _generaRigaTabella("Non ci sono elementi in attesa di elaborazione.", "Black");
            }

            //if (conteggioElaboratiDataPrecedente > 0) 
            //_generaRigaTabella("Elementi Elaborati: " + conteggioElaboratiDataPrecedente.toString(), "black");

            if (conteggioElaboratiDataSuccessiva > 0) {
                _generaRigaTabella("Elaborati " + conteggioElaboratiDataSuccessiva + " nuovi elementi dopo il  " + DataRiferimentoElaborazione + ".", "Blue");
            } else {
                _generaRigaTabella("Nono ci sono nuovi elementi elaborati dopo il  " + DataRiferimentoElaborazione + ".", "Black");
            }



        }

        var _pollingFunc = function () {

            plugin.wrapper.innerHTML = "";

            //se ci sono configurazioni impostate
            if (ws_params.ListaConfigurazioni !== "") {

                //i18n__
                _generaRigaTabella("Aggiornamento in corso...", "Blue");

                ajaxAgronica(plugin.settings.url + "/VerificheSuElaborazioniConfigurazioneUtente",
                    JSON.stringify(ws_params),
                    function (risposta) {

                        let oRisp = JSON.parse(risposta.RispostaStringa);

                        let len = oRisp.ListaElaborazioni.length;
                        let idx = 0;
                        arr_msgs = new Array();
                        while (idx < len) {

                            arr_msgs.push({
                                codice: oRisp.ListaElaborazioni[idx].Gis_Sat_Sentinel_User_Config_COD,
                                stato: oRisp.ListaElaborazioni[idx].StatoElaborazione,
                                data: oRisp.ListaElaborazioni[idx].DataRiferimentoElaborazione,
                                nelab: oRisp.ListaElaborazioni[idx].NumeroElaborazioni
                            });

                            idx++;
                        }

                        _showMsgs(ws_params.DataRiferimentoElaborazioni);

                        if (_polling) {

                            setTimeout(_pollingFunc, 30000);

                        }
                    },
                    function (risposta) {
                        //_show_msg(plugin.settings.msg_err);

                        //if (typeof plugin.settings.fail_callback === 'function') {
                        //    plugin.settings.fail_callback();
                        //}
                    },
                    null,
                    false
                );

            } else {

                //i18n__
                _generaRigaTabella("Nessuna Richiesta impostata...", "Black");
                _generaRigaTabella("Per ottenere elaborazioni di mappe satellitari occorre selezionare un poligono e fare click sul pulsante sotto 'Elabora Mappa'", "Black");
            }

        }

        //-----------------------------------------------------------------------------------------
        // fire up the plugin! 
        //-----------------------------------------------------------------------------------------
        let container = document.createElement("div");
        //container.className = "";
        container.style.cssText = "margin:5px;";
        //container.id = "";
        plugin.element.appendChild(container);
        let title = document.createElement("div");
        title.style.cssText = "text-align:center; font-weight:bolder; margin-bottom:5px;";
        title.textContent = "Notifiche elaborazioni mappe";
        container.appendChild(title);
        plugin.wrapper = document.createElement("table");
        plugin.wrapper.style.cssText = "width:100%;";
        container.appendChild(plugin.wrapper);

        //Faccio la prima richiesta al server e devo ricavare
        //  ws_params.ListaConfigurazioni = 
        //  ws_params.DataRiferimentoElaborazioni =

        ajaxAgronica(plugin.settings.url + "/LeggiElaborazioniInSospesoMappaDettagliata",
            "",
            function (risposta) {

                let oRisp = JSON.parse(risposta.RispostaStringa);

                ws_params.ListaConfigurazioni = oRisp.TilesCodes;
                ws_params.DataRiferimentoElaborazioni = oRisp.DataRif;

                //Al WS richiedo lo stato attuale delle elaborazioni e lo mostro...
                _pollingFunc();

            },
            function (risposta) {
            },
            null,
            false
        );

        //-----------------------------------------------------------------------------------------


    } // MapProc_Message

    //Add the plugin to the jQuery.fn object
    $.fn.MapProc_Message = function (options) {

        //Controllo che siano stati passati le impostazioni obbligatorie...
        if (typeof options.url !== "string" || $.trim(options.url) === "") {
            return;
        }

        // iterate through the DOM elements we are attaching the plugin to
        return this.each(function () {
            // if plugin has not already been attached to the element
            if (undefined == $(this).data('MapProc_Message')) {
                // create a new instance of the plugin
                // pass the DOM element and the user-provided options as arguments
                var plugin = new $.MapProc_Message(this, options);

                // in the jQuery version of the element store a reference to the plugin object
                // you can later access the plugin and its methods and properties like
                // element.data('pluginName').publicMethod(arg1, arg2, ... argn) or
                // element.data('pluginName').settings.propertyName
                $(this).data('MapProc_Message', plugin);
            }
        });

    }

})(jQuery);