/* ClientJs/Mappa.js */

//-------------------------------------------------------------------------------------------------
// Per mostrare i tiles...
//-------------------------------------------------------------------------------------------------

//' VAnni: 28/10/2020: Obsoleta ..
//var GrigliaTiles_Sviluppo = false;

function CoordMapType_Tiles(tileSize) {
    this.tileSize = tileSize;
}

CoordMapType_Tiles.prototype.getTile = function (coord, zoom, ownerDocument) {

    let coord1 = coord;
    // tile range in one direction range is dependent on zoom level
    // 0 = 1 tile, 1 = 2 tiles, 2 = 4 tiles, 3 = 8 tiles, etc
    let tileRange = 1 << zoom;
    coord1.y = tileRange - 1 - coord.y;

    var div = ownerDocument.createElement('div');
    div.innerHTML = coord1;
    div.style.width = this.tileSize.width + 'px';
    div.style.height = this.tileSize.height + 'px';
    div.style.fontSize = '10';
    div.style.color = 'yellow';
    div.style.borderStyle = 'solid';
    div.style.borderWidth = '1px';
    div.style.borderColor = '#AAAAAA';
    return div;
};

/*
// Insert this overlay map type as the first overlay map type at
// position 0. Note that all overlay map types appear on top of
// their parent base map.
this.elemenotMappa.overlayMapTypes.insertAt(0, new CoordMapType_Tiles(new google.maps.Size(256, 256)));
*/

var TestoLivelloZoom = "Livello zoom: ";
var mappa_zoom_PassaSatellite = 16;

var Global_Entita_Cod = 0;

var mappa = {
    elemenotMappa: null,
    geocoder: null,
    markerIndirizzo: null,
    markersMultipoint: null,
    Livelli: null,
    MarkGps: null,
    infowindow: null,
    cosaPossoDisegnare: new Array(),
    gisTipoOggettoXLayer: new Array(),
    drawingManager: null,
    isKws: false,
    markerClusterer: new Array(),
    mostraEtichette: true,

    polyOptions: {
        strokeWeight: 0,
        fillOpacity: 0.6,
        editable: true
    },
    inizializza: function () {

        utility.log("A G R O N I C A  --- Gis --- 2015");

        this.geocoder = new google.maps.Geocoder();

        this.cosaPossoDisegnare.push(google.maps.drawing.OverlayType.POLYGON);

        this.ImpostaLayer();

        this.inizializzaArrayCosaPossodisegnare();
    },
    initNewMap: function () {
        var latlngc = new google.maps.LatLng(44.168615, 12.269121);

        //Carico i layer
        this.elemenotMappa = new google.maps.Map(document.getElementById('map'), {
            zoom: 8,
            center: latlngc,
            mapTypeId: (GisMobileMode ? google.maps.MapTypeId.HYBRID : google.maps.MapTypeId.ROADMAP),
            streetViewControl: false,
            fullscreenControl: !GisMobileMode,
            mapTypeControl: !GisMobileMode,
            mapTypeControlOptions: {
                style: google.maps.MapTypeControlStyle.DROPDOWN_MENU,
                position: google.maps.ControlPosition.TOP_LEFT
            },
            zoomControl: !GisMobileMode,
            zoomControlOptions: {
                style: google.maps.ZoomControlStyle.SMALL,
                position: google.maps.ControlPosition.LEFT_CENTER
            },
            tilt: 0
        });

        this.drawingManager = new google.maps.drawing.DrawingManager({
            drawingControlOptions: {
                position: google.maps.ControlPosition.TOP_RIGHT,
                drawingModes: [google.maps.drawing.OverlayType.POLYGON, google.maps.drawing.OverlayType.MARKER]
            },
            markerOptions: {
                draggable: true
            },
            polylineOptions: {
                editable: true
            },
            drawingControl: false,
            polygonOptions: this.polyOptions,
            map: this.elemenotMappa
        });

        //GABRIELE (Mostra la griglia)
        //' VAnni: 28/10/2020: Ora uso la nuova variabile che arriva da impostazioni utente.
        if (Client_Flag_GrigliaTiles_Sviluppo) {
            this.elemenotMappa.overlayMapTypes.insertAt(0, new CoordMapType_Tiles(new google.maps.Size(256, 256)));
        }

        if (!GisMobileMode) {

            let zoomControlContenitore = document.createElement("div");
            let zoomControlDiv = document.createElement('div');
            zoomControlDiv.style.padding = '5px';
            zoomControlContenitore.appendChild(zoomControlDiv);
            // Set CSS for the control border.
            let controlUI = document.createElement('div');
            controlUI.id = "satZoomLevel";
            controlUI.style.backgroundColor = 'white';
            controlUI.style.borderRadius = '2px';
            //controlUI.style.borderStyle = 'solid';
            //controlUI.style.borderWidth = '2px';
            controlUI.style.boxShadow = 'rgba(0, 0, 0, 0.3) 0px 1px 4px -1px';
            controlUI.style.padding = '4px';
            controlUI.style.cursor = 'default';
            controlUI.style.textAlign = 'center';
            controlUI.style.zIndex = 10000;
            //controlUI.title = TestoLivelloZoom + mappa.elemenotMappa.zoom;
            zoomControlDiv.appendChild(controlUI);
            let controlText = document.createElement('div');
            controlText.id = "satZoomLevelTesto";
            controlText.style.fontFamily = 'Arial,sans-serif';
            controlText.style.fontSize = '12px';
            controlText.style.fontWeight = 'bold';
            //controlText.style.paddingLeft = '4px';
            //controlText.style.paddingRight = '4px';
            controlText.innerHTML = TestoLivelloZoom + this.elemenotMappa.zoom;
            controlUI.appendChild(controlText);

            this.elemenotMappa.controls[google.maps.ControlPosition.TOP_CENTER].push(zoomControlContenitore);

            google.maps.event.addListener(this.elemenotMappa, 'zoom_changed', function () {

                $("#satZoomLevelTesto").html(TestoLivelloZoom + this.zoom);

                // VAnni: 6/2/2018: oltre il livello di zoom passo al satellite in automatico
                if (this.getMapTypeId() != google.maps.MapTypeId.HYBRID && this.zoom >= mappa_zoom_PassaSatellite) {
                    this.setMapTypeId(google.maps.MapTypeId.HYBRID)
                    this.setTilt(0); // disable 45 degree imagery
                }
            });

        }
    },
    MetriXPixel: function () {

        //https://gis.stackexchange.com/questions/7430/what-ratio-scales-do-google-maps-zoom-levels-correspond-to;

        return 156543.03392 * Math.cos(this.elemenotMappa.getCenter().lat() * Math.PI / 180) / Math.pow(2, this.elemenotMappa.zoom);

    },
    AggiungiMarker: function (lat, lng, lbl, bzoom) {
        let circle = {
            //M (CX - R), CY  a R,R 0 1,0 (R * 2),0   a R,R 0 1,0 -(R * 2),0
            path: "M-10,0 a 10,10 0 1, 0 20,0 a 10,10 0 1,0 -20,0",
            fillColor: 'yellow',
            strokeColor: 'gold',
            fillOpacity: .8,
            strokeWeight: 2,
            scale: 1
        };

        let marker = new google.maps.Marker({
            position: new google.maps.LatLng(lat, lng),
            icon: circle,
            map: this.elemenotMappa
        });

        if (this.markersMultipoint === null) {
            this.markersMultipoint = new Array();
        }

        this.markersMultipoint.push(marker);

        if (lbl) {
            marker.setLabel("" + this.markersMultipoint.length);
        }

        if (bzoom) {

            this.elemenotMappa.setZoom(19);
            this.elemenotMappa.setCenter(marker.position);
        }
    },
    PulisciMarkers: function () {
        if (this.markersMultipoint === null) {
            return;
        }

        for (let i = 0; i < this.markersMultipoint.length; i++) {
            this.markersMultipoint[i].setMap(null);
        }
        this.markersMultipoint = new Array();
    },
    CercaCoordinate: function (lat, lng) {

        this.NascondiMarker();

        let lat_lng = new google.maps.LatLng(lat, lng);
        this.elemenotMappa.setZoom(15);
        this.markerIndirizzo = new google.maps.Marker({
            map: this.elemenotMappa,
            position: lat_lng
        });
        mappa.elemenotMappa.setCenter(lat_lng);
    },
    NascondiMarker: function () {
        if (this.markerIndirizzo == null) {
            return;
        }

        this.markerIndirizzo.setMap(null);
        this.markerIndirizzo = null;
    },
    ImpostaLayer: function () {
        this.Livelli = new Array();
        this.MarkGps = new Array();
        for (var i = 0; i < layers.length; i++) {
            //            var tilesCorretto = new Array();
            //            var j = 0;
            //            if (!!layers[i].tiles)
            //            for (j = 0; j < layers[i].tiles.length; j++)
            //                tilesCorretto.push(layers[i].nome + " " + layers[i].tiles[j]);


            var lTrasparenza = 0.6;
            if (layers[i].trasparenza !== "")
                lTrasparenza = parseFloat(layers[i].trasparenza.replace(",", "."));

            this.Livelli.push({
                nome: layers[i].nome,
                id: layers[i].id,
                colore_1: layers[i].colore_1,
                colore_2: layers[i].colore_2,
                varianza: layers[i].varianza,
                trasparenza: lTrasparenza,
                MostraDescrizioneAssociata: layers[i].MostraDescrizioneAssociata,
                TipoNodoAlberoAnagrafe: layers[i].TipoNodoAlberoAnagrafe,
                icona16: layers[i].icona16,
                icona32: layers[i].icona32,
                datiViste: layers[i].tiles,
                poligoni: new Array(),
                poligoni2 : new Array(),
                circle: new Array(),
                punti: new Array(),
                polyline: new Array(),
                ABLabel: new Array()
            });
        }
    }
    ,
    inizializzaArrayCosaPossodisegnare: function () {
        utility.warn("inizializzaArrayCosaPossodisegnare");
        // i poligono al momento sono ammessi ovunque. indico solo i punti, quindi
        this.gisTipoOggettoXLayer.push({
            layer: 13,
            GIS_TipoOggetto_Cod: google.maps.drawing.OverlayType.MARKER
        });
        this.gisTipoOggettoXLayer.push({
            layer: 50,
            GIS_TipoOggetto_Cod: google.maps.drawing.OverlayType.MARKER
        });
        this.gisTipoOggettoXLayer.push({
            layer: 63,
            GIS_TipoOggetto_Cod: google.maps.drawing.OverlayType.MARKER
        });
        this.gisTipoOggettoXLayer.push({
            layer: 67,
            GIS_TipoOggetto_Cod: google.maps.drawing.OverlayType.MARKER
        });

        utility.log("inizializzaArrayCosaPossodisegnare len= " + this.gisTipoOggettoXLayer.length);

    }
    //    ,
    //    colorOverlaysByID: function (id) {
    //        for (var i = 0; i < this.Livelli.length; i++) {
    //            if (this.Livelli[i].id == id) {
    //                clearSingleOverlay(null, this.Livelli[i].poligoni, true);
    //                clearSingleOverlay(null, this.Livelli[i].punti, true);
    //                clearSingleOverlay(null, this.Livelli[i].polyline, true);
    //                clearSingleOverlay(null, this.Livelli[i].ABLabel, true);
    //            }
    //        }
    //    }
    ,
    /* Ricerca x Indirizzo */
    ricercaIndirizzo: function (address, callback) {

        this.NascondiMarker();

        this.elemenotMappa.setZoom(zoomRicercaIndirizzo);

        let that = this;
        this.geocoder.geocode({
                'address': address
            },
            function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    that.elemenotMappa.setCenter(results[0].geometry.location);

                    if (typeof callback === "function") {

                        callback(results[0].geometry.location.lat(), results[0].geometry.location.lng());

                    }
                    // GABRIELE gestito nel callback
                    ////' VAnni: 13/3/2017: imposto anche le coordinate su casella di testo.
                    //$("#lat_cerca").val(results[0].geometry.location.lat());
                    //$("#long_cerca").val(results[0].geometry.location.lng());

                    that.markerIndirizzo = new google.maps.Marker({
                        map: that.elemenotMappa,
                        position: results[0].geometry.location
                    });

                } else {
                    alert("Geocode was not successful for the following reason: " + status);
                }
            }
        );
    },
    identificaIndirizziLatLong: function (__lat, __long, IDValoreRitorno) {

        if (isNaN(__lat) || isNaN(__long)) {
            return false;
        }

        var vapp = this;
        var latlng = new google.maps.LatLng(__lat, __long);
        vapp.geocoder.geocode({
            'latLng': latlng
        }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {

                var ValoreRitorno = "";
                for (var i = 0; i < results[0].address_components.length; i++) {
                    var shortname = results[0].address_components[i].short_name;
                    ValoreRitorno = ValoreRitorno + "," + shortname;
                }

                $(IDValoreRitorno).val(ValoreRitorno);

            }
        });

    },

    /* trova tutti i possibili indirizzi di un poligono */
    identificaIndirizzi: function (MVCArray) {
        var vapp = this;

        var __lat = 0;
        var __long = 0;
        var i = 0;
        var mArray = MVCArray.getArray();
        var conta = 0;
        var len = mArray.length;
        for (conta = 0; conta < len; conta++) {
            __lat = __lat + mArray[conta].lat();
            __long = __long + mArray[conta].lng();
        }
        __lat = __lat / len;
        __long = __long / len;

        $('#stringaIndirizzo').val("");

        vapp.geocoder.geocode({
            location: new google.maps.LatLng(__lat, __long)
        }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {

                let best_result = null;
                let found_street_address = false;

                $.each(results, function (i, elem) {

                    $.each(elem.types, function (j, t) {

                        if (t.toLowerCase() === "street_address") {

                            best_result = elem;
                            found_street_address = true;

                            return false;
                        } else {

                            if (t.toLowerCase() === "route") {

                                best_result = elem;
                            }
                        }
                    });

                    if (found_street_address) {

                        return false;
                    }
                });

                if (best_result === null) {

                    best_result = results[0];
                }

                var indirizzo = best_result.formatted_address;

                var stringona = "<br><br>";

                var via = "";
                var n_civico = 0;
                var citta = "";
                var localita = "";
                var provincia = "";
                var sigla = "";
                var cap = "";
                var stato = "";
                var ISO_Stato = "";

                $.each(best_result.address_components, function (i, comp) {
                    var shortname = comp.short_name;
                    var longname = comp.long_name;
                    var type = comp.types;
                    switch (type.toString()) {
                        case 'street_number':
                            n_civico = shortname;
                            break;
                        case 'route':
                            via = shortname;
                            break;
                        case 'sublocality,political':
                            localita = shortname;
                            break;
                        case 'locality,political':
                            citta = shortname;
                            break;
                        case 'administrative_area_level_2,political':
                            provincia = longname;
                            sigla = shortname;
                            break;
                        case 'administrative_area_level_3,political':
                            citta = longname;
                            break;
                        case 'postal_code':
                            cap = shortname;
                            break;
                        case 'country,political':
                            stato = longname + ", " + shortname;
                            ISO_Stato = shortname;
                            break;
                    }
                    //stringona = stringona + "t:" + type + "  sh:" + shortname + " lo:" + longname + "<br>";
                });

                //stringona = stringona +"<br><br>";

                if (localita == "")
                    localita = citta;

                impostaValoriIndirizzo(via, n_civico, localita, citta, provincia, sigla, stato, ISO_Stato);

                $('#stringaIndirizzo').val(indirizzo);
                //            $('#stringaIndirizzo').html(indirizzo);
            }
        });
    }
    ,
    SelezionaPoligono: function () {
        this.drawingManager.setDrawingMode(null);
    },
    CoordDaSelezione: function (callback) {

        //la selezione è un punto
        if (shape.selectedPointsArray !== undefined) {
            if (shape.selectedPointsArray.length > 0) {

                if (typeof callback === "function") {

                    var pp = shape.selectedPointsArray[shape.selectedPointsArray.length - 1].position;
                    let lat = pp.lat();
                    let lng = pp.lng();

                    callback(lat, lng);
                }

                return true;
            }
        }


        //if (CoordFromPointsMVCArray !== undefined && CoordFromPointsMVCArray !== null) {

        //    let tmpArrayPunti = CoordFromPointsMVCArray.b;

        //    if (tmpArrayPunti === undefined) {
        //        if (CoordFromPointsMVCArray.j !== undefined) {
        //            tmpArrayPunti = CoordFromPointsMVCArray.j;
        //        }
        //    }

        //    if (tmpArrayPunti !== undefined) {
        //        if (tmpArrayPunti.length > 0) {

        //            if (typeof callback === "function") {

        //                var pp = tmpArrayPunti[tmpArrayPunti.length - 1];
        //                let lat = pp.lat();
        //                let lng = pp.lng();

        //                callback(lat, lng);
        //            }

        //            return true;
        //        }
        //    }
        //}


        //la selezione è un poligono
        if (shape.selectedShape !== undefined && shape.selectedShape !== null) {

            if (typeof callback === "function") {

                var MVCArray = shape.selectedShape.getPath();
                var mArray = MVCArray.getArray();

                let lat = mArray[0].lat();
                let lng = mArray[0].lng();

                callback(lat, lng);
            }

            return true;
        }

        return false;


    },
    NascondiInfo: function () {
        if (this.infowindow != null) this.infowindow.close();
    },
    Info_Poligono: function () {

        this.NascondiInfo();

        var veg_cod = "";
        var sEntita_Cod = "";

        var lat;
        var lng;

        //GABRIELE -> Se i PUNTI selezionati sono > 1 posiziona la InfoWindow sul primo dell'elenco (bug)
        var hMpM = $("#hiddenMultipointModifica").val();
        var selectedShape_testo = "";

        if (hMpM != undefined && hMpM != '') {
            sEntita_Cod = hMpM;
            //var app = hMpM.split("§")[0].split("^")[1].split(",");

            let arr_hMpM = hMpM.split("§");
            var s_app = arr_hMpM.pop();
            while (s_app === "") {
                s_app = arr_hMpM.pop();
            }
            var app = s_app.split("^")[1].split(",");

            sEntita_Cod = s_app + "§";

            lat = app[0].replace("(", "");
            lng = app[1].replace(")", "").replace(" ", "");

        } else {
            if (shape.selectedShape != null) {

                sEntita_Cod = shape.selectedShape.Entita_Cod

                var MVCArray = shape.selectedShape.getPath();
                var mArray = MVCArray.getArray();


                //console.time('find point');
                let debug = polylabel(mArray);
                //console.timeEnd('find point');
                lat = debug.lat();
                lng = debug.lng();

                //lat = mArray[0].lat();
                //lng = mArray[0].lng();

                selectedShape_testo = shape.selectedShape.testo;

            }
        }
        if (sEntita_Cod !== "") {

            utility.log("infowindow = " + lat + " - " + lng);

            this.infowindow = new google.maps.InfoWindow();
            this.infowindow.setPosition(new google.maps.LatLng(lat, lng));

            this.getProprieta(veg_cod, sEntita_Cod, 2);

            this.infowindow.open(this.elemenotMappa);
        }
    }
    ,
    ImpostaValoriInfoNuovoImpianto: function (Entita_Cod, Veg_Cod, r, jQueryDiv) {
        $("#lbl_appezza_data_inizio").text(r.Appezzamento_Validita_Inizio);
        $("#lbl_appezza_data_fine").text(r.Appezzamento_Validita_Fine);
    }
    ,
    ImpostaValoriInfoAppezzamento: function (Entita_Cod, Veg_Cod, r, jQueryDiv) {
        //var msg = "<div style='width:250px;'>";
        var msg = "";
        if (r.hasOwnProperty("app_nome")) {
            msg = "<div style='font-size: larger; font-weight: bolder; margin-bottom: 5px;'>" + r.app_nome + "</div> ";
        }
        var selectedShape_testo = "";
        var area = 0;
        var r_grfi_des = "";

        if (shape.selectedShape !== undefined) {
            if (shape.selectedShape !== null) {
                if (shape.selectedShape.informazioni == 'True') {
                    msg += stringhe.formattaStringaInfo("Az", r.rag_soc);
                    msg += stringhe.formattaStringaInfo("Centro", r.sa_nome);
                    r_grfi_des = r.grfi_des
                }

                selectedShape_testo = shape.selectedShape.testo;
                var MVCArray = shape.selectedShape.getPath();
                area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);

                utility.log("ImpostaValoriInfoAppezzamento, area: " + area);
            }
        }

        //'  Vanni, 30/09/2015 12:23:35: Evito di mostrare la descrizione se contiene info che riguardano la PF o l'export SHP
        //' VAnni: 7/3/2017: mostro adeguatamente le informazioni da extra_info
        if (!selectedShape_testo.indexOf("§") > 0) {
            msg += stringhe.formattaStringaInfo("Descrizione", selectedShape_testo);
            msg += " <br/> ";
        } else {

            if (r.extra_info) {
                for (var curInfo in r.extra_info) {
                    //utility.log(curInfo);
                    msg += "<em>" + curInfo + ': ' + r.extra_info[curInfo] + "</em><br />";
                }

                msg += "<br />";
            }

        }


        switch (r.Flag_GPS) {
            case "1":
                //i18n__
                msg += "<u>Dati rilevati con GPS</u><br />";
                break;
            case "2":
                //i18n__
                msg += "<u>Dati Importati da fonte esterna a GIAS</u><br />";
                break;
            default:

        }

        msg += stringhe.formattaStringaInfo("Sup. Catastale", r.Superficie);

        if (area != 0)
            msg += stringhe.formattaStringaInfo("Sup. Calcolata [google]", area);


        msg += stringhe.formattaStringaInfo("Indirizzo", r.indirizzo);

        if (r.Progetto_Nome != "")
            msg += stringhe.formattaStringaInfo("Lotto", r.Progetto_Nome);

        msg += " <br/> ";

        let msgSpecie = Traduzione(AgronicaControlliGisResx, "jsLblSpecie");
        let msgVarieta = Traduzione(AgronicaControlliGisResx, "jsLblVarieta");
        let msgFinalita = Traduzione(AgronicaControlliGisResx, "jsLblFinalità");
        let msgTipologia = Traduzione(AgronicaControlliGisResx, "jsLblTipologia");
        let msgMostraDet = Traduzione(AgronicaControlliGisResx, "jsLblMostraDettagli");
        let msgDestUso = Traduzione(AgronicaControlliGisResx, "jsLblDestUso")

        msg += stringhe.formattaStringaInfo(msgSpecie, r.veg_des);
        msg += stringhe.formattaStringaInfo(msgVarieta, r.cul_des);
        msg += stringhe.formattaStringaInfo(msgTipologia, r.grva_des);

        if (r_grfi_des != "") {
            msg += stringhe.formattaStringaInfo(msgFinalita, r.grfi_des);
        }

        msg += stringhe.formattaStringaInfo(msgDestUso, r.dest_uso);

        let msgValDa = Traduzione(AgronicaControlliGisResx, "jsLblValiditaInizio");
        let msgValA = Traduzione(AgronicaControlliGisResx, "jsLblValiditaFine");


        msg += stringhe.formattaStringaInfo(msgValDa, r.Validita_Inizio);
        msg += stringhe.formattaStringaInfo(msgValA, r.Validita_Fine);

        if (r.Gias_Palm.Descrizione != "") {
            msg += stringhe.formattaStringaInfo("GPS: ", r.Gias_Palm.Descrizione +
                "(<span id='spanGiasPalmDettagli' style='color: blue' onclick = 'GiasPalmDettagli(\"" + r.Gias_Palm.piva + "\", \"" + r.Gias_Palm.sa_cod + "\", \"" + r.Gias_Palm.id + "\")'>" + msgMostraDet + "</span> )");
            msg += "<div style='display: none; height: 250px;overflow: scroll; ' id=\"divGiasPalmDettagli\"></div> "
        }
        msg += " <div>GIS: " + Entita_Cod +"</div>"
        msg += " </div> ";

        if (jQueryDiv) {
            $(jQueryDiv).html(msg);
        } else {

            this.infowindow.setContent(msg);
        }

    }
    , GiasPalmDettagli: function (piva, sa_cod, id) {

        //Vanni, 21/02/2020: ORRORE Di codifica, confrontare stringhe!!!
        if ($("#spanGiasPalmDettagli").text() != msgNascondiDet) {

            ajaxAgronica(indirizzohttp + "/GiasPalmDettagli", JSON.stringify({ piva: piva, sa_cod: sa_cod, id: id }),
                function (risposta) {

                    var msg = "";

                    var r = JSON.parse(risposta.RispostaStringa);

                    //informazioni Generali.
                    //i18n__
                    msg += "<br /><i aria-hidden='true' style = 'font-size: medium; font-weight: bold'>Dettagli Gias Palm:</i> <br /><br />";
                    msg += "<strong>Perimetro</strong>: " + r.giaspalm_dettagli.Perimetro + "<br />";
                    msg += "<strong>Sup. Rilevata in Campo</strong>: " + r.giaspalm_dettagli.Area + "<br />";

                    //per ogni elemento del vettore (punto)
                    var i = 0;
                    var det = r.giaspalm_dettagli.punti_dettagli;
                    for (var curInfo in det) {

                        if (i > 0)
                            msg += "<em>P. " + i.toString() + '</em>: <br />';

                        //info del punto
                        for (var curInfoPunto in det[curInfo]) {
                            if (curInfoPunto != "NMEA") {
                                msg += "<strong>" + curInfoPunto + '</strong>: ' + det[curInfo][curInfoPunto] + "<br />";
                            }
                        }
                        msg += "</i><br />"
                        i++;

                    }

                    let msgNascondiDet = Traduzione(AgronicaControlliGisResx, "jsLblNascondiDettagli");

                    $("#divGiasPalmDettagli").html(msg);
                    $("#divGiasPalmDettagli").show();
                    $("#spanGiasPalmDettagli").text(msgNascondiDet);



                }, null);

        } else {

            $("#spanGiasPalmDettagli").text(msgMostraDet);
            $("#divGiasPalmDettagli").hide();
            $("#divGiasPalmDettagli").html("");

        }


    },
    //Tipologia caricamento 1 --> caricamento della modifica appezzamento
    getProprieta: function (Veg_Cod, Entita_Cod, tipologiaCaricamento, usaChiaveAlbero, jQueryDiv) {
        var objA = this;
        var ritorno;
        var tipologiaCaricamentoClient = tipologiaCaricamento;
        if (tipologiaCaricamento == 3) {
            tipologiaCaricamento = 2;
        }

        if (!usaChiaveAlbero && stringhe.contains(Entita_Cod, "§")) {
            var app = Entita_Cod.split("§");
            Entita_Cod = app[app.length - 2].split("|")[1].split("^")[0];
        }

        var funzioneDaChiamare = "/getProprieta";
        var lData = "{ Entita_Cod: '" + Entita_Cod + "',tipologia: '" + tipologiaCaricamento + "' }";

        if (usaChiaveAlbero) {
            funzioneDaChiamare = "/getProprietaViaChiaveAlbero"
            lData = "{ ChiaveAlbero: '" + Entita_Cod + "',tipologia: '" + tipologiaCaricamento + "' }";
        }

        $.ajax({
            type: "POST",
            url: indirizzohttp + funzioneDaChiamare,
            data:lData,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var r = JSON.parse(msg.d);

                if (tipologiaCaricamentoClient == 1) {

                    objA.ImpostaValoriModifcaAppezzamento(Entita_Cod, Veg_Cod, r, usaChiaveAlbero);

                    if (shape.selectedShape.layerDiAppartenenza === "1") {

                        var area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);
                        $("#pop_up_sup_app_modifica").val(area);

                        let lblModificaAppezza = Traduzione(AgronicaControlliGisResx, "jsLblModificaAppezzamento");
                        $("#cmbAssocia_Modifica_GenerazionePoligoni_1").text(lblModificaAppezza);
                        $("#cmbAssocia_Modifica_GenerazionePoligoni_1").addClass("ModificaAppezzamento");

                        let lblNuovoImpianto = Traduzione(AgronicaControlliGisResx, "jsLblNuovoImpiantoColturale");
                        $("#cmbAssocia_Modifica_GenerazionePoligoni_2").text(lblNuovoImpianto);
                        $("#cmbAssocia_Modifica_GenerazionePoligoni_2").addClass("NuovoImpianto");

                        let lblNuovoImpModificaApp = Traduzione(AgronicaControlliGisResx, "jsLblNuovoImpiantoModificaAppezza");
                        $("#cmbAssocia_Modifica_GenerazionePoligoni_3").text(lblNuovoImpModificaApp);
                        $("#cmbAssocia_Modifica_GenerazionePoligoni_3").addClass("NuovoImpiantoModificaAppezzamento");

                        $("#pop_up_sup_app_modifica_divModificaDatiDiAnagrafica").hide();

                        var preparaXSalvataggioPoligoni_SavePlus;

                        if (preparaXSalvataggioPoligoni_SavePlus) {

                          preparaXSalvataggioPoligoni_SavePlus = false;
                            let lblNuovoImpSuApp = Traduzione(AgronicaControlliGisResx, "jsLblNuovoImpiantoSuAppezza");
                            preparaXSalvataggioPoligoniImpostaTitoloWin(lblNuovoImpSuApp);
                            $("#cmbAssocia_Modifica_GenerazionePoligoni").val("2").change();
                        }


                    }

                }

                if (tipologiaCaricamentoClient == 2) {
                    objA.ImpostaValoriInfoAppezzamento(Entita_Cod, Veg_Cod, r, jQueryDiv);
                }
                if (tipologiaCaricamentoClient == 3) {
                    objA.ImpostaValoriInfoNuovoImpianto(Entita_Cod, Veg_Cod, r, jQueryDiv);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
    ,
    CaricaCodiciAnagrafeLetti: undefined,
    CaricaCodiciAnagrafeRiportaCodiciSuTextbox: function (tbDoveScrivere) {

        $('input' + tbDoveScrivere).each(function () {

            mappa.CaricaCodiciAnagrafeRiportaCodice(tbDoveScrivere, $(this).attr("data-idCod"));

        });


    },
    CaricaCodiciAnagrafeRiportaCodice: function (tbDoveScrivere, id_cod) {

        for (var i = 0; i < mappa.CaricaCodiciAnagrafeLetti.length; i++) {
            if (parseInt(id_cod)=== mappa.CaricaCodiciAnagrafeLetti[i].id_cod) {
                $(tbDoveScrivere + '[data-idCod="' + mappa.CaricaCodiciAnagrafeLetti[i].id_cod.toString() + '"]').val(mappa.CaricaCodiciAnagrafeLetti[i].val_cod);
            }
        }

    },
    CaricaCodiciAnagrafe: function (tbDoveScrivere, Entita_Cod, gruppo, usaChiaveAlbero) {

        $(tbDoveScrivere).val("");

        var funzioneDaChiamare = "/CaricaCodiciAnagrafe";
        if (usaChiaveAlbero) {
            funzioneDaChiamare = "/CaricaCodiciAnagrafeViaChiaveAlbero";
        }

        ajaxAgronica(indirizzohttp + funzioneDaChiamare, JSON.stringify({ Entita_Cod: Entita_Cod, gruppo: gruppo }),
            function (risposta) {

                mappa.CaricaCodiciAnagrafeLetti = JSON.parse(risposta.RispostaStringa);

                mappa.CaricaCodiciAnagrafeRiportaCodiciSuTextbox(tbDoveScrivere);

                if (gruppo === "IMPIANTI") {
                    CaricaComboOrganismoReferente("#ddl_azienda_html", "#pop_up_app_m_OrganismoReferente");

                    let ca_DestUso = 0;
                    let i = 0;
                    while (ca_DestUso === 0 && i < mappa.CaricaCodiciAnagrafeLetti.length) {
                        if (mappa.CaricaCodiciAnagrafeLetti[i].id_cod >= 3000 && mappa.CaricaCodiciAnagrafeLetti[i].id_cod < 4000) {
                            ca_DestUso = mappa.CaricaCodiciAnagrafeLetti[i].id_cod;
                        }
                        i++;
                    }
                    if (ca_DestUso > 0) {
                        $("#pop_up_destuso_modifica").val(ca_DestUso);
                    }
                }


            }, null);

    }
    ,

    CaricaCodiceSocio: function (tbDoveScrivere, piva, id_cod) {

        ajaxAgronica(indirizzohttp + "/CaricaImpreseCodici", JSON.stringify({ id_cod: id_cod, piva: piva }),
            function (risposta) {
                $(tbDoveScrivere).val(risposta.RispostaStringa);
            }, null);

    }
    ,

    ImpostaValoriModifcaAppezzamento: function (Entita_Cod, Veg_Cod, r, usaChiaveAlbero) {

        CaricaTipologia(Veg_Cod, "#pop_up_tipologia_modifica", r.grva_cod);
        CaricaVarieta("#pop_up_varieta_modifica", r.cul_cod);
        CaricaFinalita("#pop_up_finalita_modifica", r.grfi_cod);

        mappa.CaricaCodiceSocio("#txt_m_CodiceSocio", r.piva, 1033);

        //vanni, per retro-compatibilità effettuo chiamata solo se ci sono codici da impostare .. :
        if ($(".m_CodiceAnagrafe").length > 0) {
            mappa.CaricaCodiciAnagrafe(".m_CodiceAnagrafe", Entita_Cod, "IMPIANTI", usaChiaveAlbero)
            mappa.CaricaCodiciAnagrafe(".m_CodiceAnagrafe", Entita_Cod, "APPEZZA", usaChiaveAlbero)
        }


        $('#pop_up_sup_precedente').val(r.Superficie);
        $('#pop_up_sup_app_modifica').val(r.Superficie);
        $('#via_stringa_modifica').val(r.indirizzo);

        $('#pop_up_m_lotto').val(r.Progetto_Nome);

        if (mappa.isKws) {
            var splitLotto = $('#pop_up_m_lotto').val().split("-")
            $('#pop_up_m_app_originale').val(splitLotto[0]);
            $('#pop_up_app_m_n_appezza').val(splitLotto[1]);
        }

        if (r.Validita_Inizio != '01/01/1900') {

            $('#pop_up_m_data_inizio').val(r.Validita_Inizio);
        }
        else {
            $('#pop_up_m_data_inizio').val("");
        }

        if (r.Validita_Fine != '31/12/2100') {
            $('#pop_up_m_data_fine').val(r.Validita_Fine);
        }
        else {
            $('#pop_up_m_data_fine').val("");
        }

        interfaccia.loading(false);

    }
    ,
    isNuovoElemento: function () {
        var rval = true;
        if (shape.selectedShape != undefined)
            rval = (shape.selectedShape.html == undefined);

        utility.log("isNuovoElemento: " + rval);
        return rval;
    }
    , RicreaPannelloColore: function () {

        if (shape === undefined) {
            setTimeout(this.RicreaPannelloColore, 500);
            return;
        }

        if (shape.glayerDoveDisegno == -1)
            return 0;

        var l_place = place.length;
        var l_livelli = this.Livelli.length;
        //Identifico i Min e Max

        for (var i = 0; i < l_place; i++) {
            if (placeLivelli[place[i].layer].datiViste != undefined) {
                precision.SettaMaxMin(place[i].AppIdRate, placeLivelli[place[i].layer].datiViste, placeLivelli[place[i].layer].nome);
            }

        } //next i




        PopolaComboPannelloColore(xCreazioneCombo, xComboCorrenti);

        if (placeLivelli[shape.glayerDoveDisegno] !== undefined) {
            interfaccia.CreaPannelloColore('abc', placeLivelli[shape.glayerDoveDisegno].datiViste);
        }


    },
    riselezionaShape: function () {
        var shapePrecedente = shape.selectedShape;
        if (!shapePrecedente)
            return 0;
        if (!shapePrecedente.layerDiAppartenenza)
            return 0;
        var id_livelloOK = 0;
        var layerDiAppartenenza;
        var i;
        var LivelloCorrente;
        for (i = 0; i < this.Livelli.length; i++) {
            LivelloCorrente = this.Livelli[i];
            if (LivelloCorrente.id == shapePrecedente.layerDiAppartenenza) {
                layerDiAppartenenza = LivelloCorrente;
                break;
            }
        }

        if (layerDiAppartenenza === undefined) {
            return;
        }

        var trovato = this.cercaEdImposta(layerDiAppartenenza.poligoni, shapePrecedente);
        if (trovato == false) {
            trovato = this.cercaEdImposta(layerDiAppartenenza.polyline, shapePrecedente);
        }
        if (trovato == false) {
            trovato = this.cercaEdImposta(layerDiAppartenenza.punti, shapePrecedente);
        }
        if (trovato == false) {
            trovato = this.cercaEdImposta(layerDiAppartenenza.circle, shapePrecedente);
        }
        if (trovato == false) {
            trovato = this.cercaEdImposta(layerDiAppartenenza.ABLabel, shapePrecedente);
        }

        shape.selectedShape.setEditable(shapePrecedente.editable);
    }
    ,
    cercaEdImposta: function (arr, shapePrecedente) {
        var trovato = false;
        var i;
        for (i = 0; i < arr.length; i++) {
            var poligono = arr[i];
            if (poligono.Entita_Cod == shapePrecedente.Entita_Cod) {
                trovato = true;
                shape.selectedShape = poligono;
                break;
            }
        }
        return trovato;
    }
    ,
    clearArrayForme: function (arr) {
        var l = arr.length;
        for (var i = 0; i < l; i++) {
            arr[i].setMap(null);
        }
    }
    ,
    clearAllShape: function () {
        var l_livelli = this.Livelli.length;
        for (var i = 0; i < l_livelli; i++) {
            this.clearArrayForme(this.Livelli[i].poligoni);
            this.clearArrayForme(this.Livelli[i].poligoni2);
            this.clearArrayForme(this.Livelli[i].punti);
            this.clearArrayForme(this.Livelli[i].circle);
            this.clearArrayForme(this.Livelli[i].polyline);
            this.clearArrayForme(this.Livelli[i].ABLabel);

            this.Livelli[i].poligoni = new Array();
            this.Livelli[i].poligoni2 = new Array();
            this.Livelli[i].punti = new Array();
            this.Livelli[i].circle = new Array();
            this.Livelli[i].polyline = new Array();
            this.Livelli[i].ABLabel = new Array();
        }
    },
    LayerNascondiSeNonCiSonoDati: function (TipologiaLayer) {

        $(".chkLayerRow").show();

        var l_livelli = this.Livelli.length;
        for (var i = 0; i < l_livelli; i++) {
            if (!this.LivelloContieneDati(i)) {
                let idLiv = this.Livelli[i].id;
                switch (TipologiaLayer) {

                    case Enum_tipologia_layer.Gruppo_Colturale.value:
                        if (idLiv !== "-1") {
                            $("#chkLayerRow_" + idLiv).hide();
                        }
                        break;

                    default:
                        //$("#chkLayerRow_" + idLiv).hide();
                        break;
                }

            }
        }
    },
    LivelloContieneDati: function (i) {
        if (this.Livelli[i].poligoni.length > 0) {
            return true;
        }
        if (this.Livelli[i].poligoni2.length > 0) {
            return true;
        }
        if (this.Livelli[i].punti.length > 0) {
            return true;
        }
        if (this.Livelli[i].circle.length > 0) {
            return true;
        }
        if (this.Livelli[i].polyline.length > 0) {
            return true;
        }
        if (this.Livelli[i].ABLabel.length > 0) {
            return true;
        }
        return false;
    },
    ImpostaShape: function (riCreaPannelloColore, autoFit) {

        var zLevel = this.elemenotMappa.zoom;
        var p = Math.pow(2, (21 - zLevel));



        if (autoFit === undefined) {
            autoFit = true;
        }

        this.clearAllShape();

        var kk = 0;

        console.warn('ImpostaShape');
        if (!place)
            return 0;
        var l_place = place.length;
        var l_livelli = this.Livelli.length;


        var placeLivelli = new Array();
        for (var i = 0; i < l_livelli; i++) {
            placeLivelli[this.Livelli[i].id] = this.Livelli[i];
        }

        let ozindex = -1;
        let idxGlayerDoveDisegno = -1;
        for (var i = 0; i < layers.length; i++) {
            if (layers[i].id === shape.glayerDoveDisegno) {
                idxGlayerDoveDisegno = i;
            }
            let lc = -2;
            try {
                lc = parseInt(layers[i].zindex);
            } catch (e) {
            }
            if (ozindex < lc) {
                ozindex = lc;
            }
        }

        if (riCreaPannelloColore == true) {
            this.RicreaPannelloColore();
        }


        //trovo i limiti estremi della mappa
        var latlngAutoFit = new google.maps.LatLngBounds();

        let saveShape = null;

        //per tutti i punti/poligoni
        for (var i = 0; i < l_place; i++) {
            var polygonPaths = [];

            //var lat_centro = 0;
            //var lng_centro = 0;

            var p_place = place[i];

            let pl = new PolyLabel();//per posizionamento etichetta


            //per autoFIT
            var l_place_vertici = p_place.vertici.length;
            for (var j = 0; j < l_place_vertici; j++) {
                var l_at = p_place.vertici[j].lat;
                var l_ong = p_place.vertici[j].long;

                let ll = new google.maps.LatLng(l_at, l_ong);
                pl.add(ll);

                polygonPaths.push(ll);
                //lat_centro += parseFloat(l_at);
                //lng_centro += parseFloat(l_ong);
                latlngAutoFit.extend(ll);//new google.maps.LatLng(l_at, l_ong));
            }

            //lat_centro = lat_centro / j;
            //lng_centro = lng_centro / j;

            let pos = pl.position();
            let lat_centro = pos.lat();
            let lng_centro = pos.lng();


            var k_livelli = placeLivelli[place[i].layer];
            //if (k_livelli.id == place[i].layer) {

            var colore_poligon;
            var selectedColor = '#222222';

            var v_min = 0.0;
            var v_max = 0.0;
            var colore_1 = "";
            var colore_2 = "";
            var varianza = "";
            var circle_radius = p * 1.5;
            var circle_opacity = k_livelli.trasparenza;

            //caso precision Farming o avversità
            if (k_livelli.datiViste != undefined) {
                if (p_place.layer == shape.glayerDoveDisegno) {
                    var curAppIdRate = precision.GETvaloreAppIdRateDaStringa(place[i].AppIdRate, $("#cmbViste option:selected").text(), placeLivelli[place[i].layer].nome);

                    colore_poligon = interfaccia.leggiColoreDaDiv($("#cmbViste option:selected").text(), curAppIdRate);
                    //circle_radius = interfaccia.leggiRaggioDaDiv($("#cmbViste option:selected").text(), curAppIdRate);
                    //circle_radius = circle_radius * 1.3;
                } else {
                    colore_poligon = "000000";
                    if (shape.glayerDoveDisegno != -1) {
                        circle_opacity = 0;
                    }
                }
            } else {
                //prendo il colore del layer originale
                if (k_livelli.v_min === "undefined") {
                    $("#scala_colori_contenitore").hide();
                }
                v_min = k_livelli.v_min;
                v_max = k_livelli.v_max;
                colore_1 = k_livelli.colore_1;
                colore_2 = k_livelli.colore_2;
                varianza = k_livelli.varianza;
                colore_poligon = colori.calcolaColore(v_min, v_max, curAppIdRate, colore_1, colore_2, varianza);
            }

            if (p_place.TipologiaGML == 'Polygon') {
                let DataRaccolta = precision.GETvaloreAppIdRateDaStringa(p_place.AppIdRate, "Data Raccolta", "");
                if (DataRaccolta !== undefined) {
                    p_place.TipologiaGML = 'PolygonFill';

                    console.log("PolygonFill");
                }

                if (p_place.InOsservazione !== undefined && p_place.InOsservazione === 1) {

                    p_place.TipologiaGML = 'PolygonFill';

                }
            }


            if (k_livelli.id == shape.glayerDoveDisegno) {
                //GABRIELE -> porto in primo piano i poligoni selezionati
                p_place.zindex = parseInt(ozindex) + 1;
                if (idxGlayerDoveDisegno >= 0) {
                    layers[idxGlayerDoveDisegno].zindex = p_place.zindex;
                }
            }

            ShapeAggiungiOggetti(p_place, k_livelli, colore_poligon, polygonPaths, lat_centro, lng_centro, circle_radius, circle_opacity);

            if (p_place.Entita_Cod == Global_Entita_Cod) {

                if (p_place.TipologiaGML === "Polygon") {

                    saveShape = k_livelli.poligoni[k_livelli.poligoni.length - 1];
                }

                Global_Entita_Cod = 0;
            }

            p_place.zindex = ozindex;


            kk = l_livelli;
            // } //for kk
            //} //for i


            polygonPaths = null;
            //lat_centro = null;
            //lng_centro = null;
            p_place = null;

        }
        // next per tutti i punti/poligoni

        this.RenderizzaShape();

        //per ora commento...
        this.GestioneCluster();

        if (autoFit)
            this.elemenotMappa.setCenter(latlngAutoFit.getCenter(), this.elemenotMappa.fitBounds(latlngAutoFit));


        //' VAnni: 25/3/2019: sulle tipologie di layer selezionate occorre nascondere quelle che non servono
        mappa.LayerNascondiSeNonCiSonoDati(parseInt(TipologiaLayer()));


        this.riselezionaShape();
        interfaccia.loading(false);

        if (saveShape !== null) {

            let bounds = new google.maps.LatLngBounds();
            saveShape.getPath().forEach(function (v) {
                bounds.extend(v);
            });

            this.elemenotMappa.setCenter(bounds.getCenter(), this.elemenotMappa.fitBounds(bounds));

            //e lo seleziono...
            setSelection(saveShape);
        }

    }
    , EseguiGisInizializzazioneDaParametriCartografici: function (parametri) {



        var latlngBounds = new google.maps.LatLngBounds();

        var p = JSON.parse(parametri);
        var vCenter = p.Center.split(" ");
        var latlngCenter = new google.maps.LatLng( vCenter[1], vCenter[0]);



        var p_arr = p.BBox.split(" ");

        for (let v = 0; v < p_arr.length-1; v+=2) {
            latlngBounds.extend(new google.maps.LatLng(p_arr[v + 1], p_arr[v]));
        }


        this.elemenotMappa.setCenter(latlngCenter);
        this.elemenotMappa.setZoom(15);
        let in_ne = this.elemenotMappa.getBounds().contains(latlngBounds.getNorthEast());
        let in_sw = this.elemenotMappa.getBounds().contains(latlngBounds.getSouthWest());
        if (!in_ne || !in_sw) {
            this.elemenotMappa.fitBounds(latlngBounds);
        }
    }
    , RenderizzaShape: function () {



        var arrChk = this.getLayerCheccati();
        var l_livelli = this.Livelli.length;

        var Zoom = this.elemenotMappa.zoom;
        var RenderizzaShape_LivelloZoom = Zoom;

        var nElementiNascosti = 0;


        for (var i = 0; i < l_livelli; i++) {

            if (arrChk["ID_" + this.Livelli[i].id] == true) {

                ImpostaMappa(this.Livelli[i].poligoni, true, false, 'poly', false);
                ImpostaMappa(this.Livelli[i].poligoni2, true, false, 'poly2', false);

                //' VAnni: 3/3/2017: valutare Gestione con plugin
                if (TipoRender == Enum_TipoRender.Parziale) {
                    if (Zoom > mappa_zoom_mostraPunti)
                        ImpostaMappa(this.Livelli[i].punti, true, true, 'point', true); //vanni, 17/04/2013 per selezione markers
                    else {
                        this.clearArrayForme(this.Livelli[i].punti);
                        nElementiNascosti = nElementiNascosti + this.Livelli[i].punti.length;
                    }

                } else {
                    ImpostaMappa(this.Livelli[i].punti, true, true, 'point', true);
                }

                ImpostaMappa(this.Livelli[i].circle, true, true, 'circle', false);
                ImpostaMappa(this.Livelli[i].polyline, false, false, 'poly', false);

                //' VAnni: 3/3/2017: valutare Gestione con plugin
                if (TipoRender == Enum_TipoRender.Parziale) {
                    if (Zoom > mappa_zoom_mostraEtichette)
                        ImpostaMappa(this.Livelli[i].ABLabel, false, false, 'poly', false);
                    else {
                        this.clearArrayForme(this.Livelli[i].ABLabel);
                        nElementiNascosti = nElementiNascosti + this.Livelli[i].ABLabel.length;
                    }
                } else {
                    ImpostaMappa(this.Livelli[i].ABLabel, false, false, 'poly', false);
                }

            }

        }

        if (nElementiNascosti > 0) {
            $("#lblMessaggiFiltri").html("Sono stati nascosti " + nElementiNascosti.toString() + " elementi perchè il livello di zoom è troppo ampio.");
        } else {
            $("#lblMessaggiFiltri").html("");
        }

    }
    , markerClusterer_clear: function () {
        let markerClusterer_len = this.markerClusterer.length;
        if (markerClusterer_len > 0) {
            for (var i = 0; i < markerClusterer_len; i++) {
                this.markerClusterer[i].clearMarkers();
            }
            this.markerClusterer = new Array();
        }
    }
    , ShowHideMarkerClusters: function (mostra) {
        var MarkerClusterer_ZoomLevel;
        if (mostra) {
            MarkerClusterer_ZoomLevel = 2000;
        } else {
            MarkerClusterer_ZoomLevel = 1;
        }
        mappa.GestioneCluster();
    }
    , GestioneCluster: function () {

        //' VAnni: 3/3/2017: Gestione dei marker con plugin
        this.markerClusterer_clear();

        var obj_chk = this.getLayerCheccati();

        //todo: leggere da impostazioni
        var zoom = MarkerClusterer_ZoomLevel;
        var size = -1;
        var style = 1;
        zoom = zoom == -1 ? null : zoom;
        size = size == -1 ? null : size;
        style = style == -1 ? null : style;

        let objMC = null;

        let that = this;

        $.each(this.Livelli, function (iLiv, livello) {

            if (obj_chk["ID_" + livello.id] == true) {

                //GABRIELE: i punti li clusterizzo comunque (???)

                if (objMC === null) {
                    objMC = new MarkerClusterer(that.elemenotMappa, livello.punti, {
                        maxZoom: zoom,
                        gridSize: size,
                        styles: markerClusterer_styles[style]
                        //imagePath: MarkerClusterer_Path + '/images/m'
                    });

                    that.markerClusterer.push(objMC);

                } else {

                    $.each(livello.punti, function (i, pt) {
                        objMC.addMarker(pt, true);
                    });
                }

                if (livello.MostraDescrizioneAssociata === "1") {

                    $.each(livello.poligoni, function (idx, elem) {

                        if (typeof elem.etichetta === "object") {
                            if (elem.etichetta !== null) {

                                objMC.addMarker(elem.etichetta, true);
                            }
                        }
                    });


                    $.each(livello.punti, function (idx, elem) {

                        if (typeof elem.etichetta === "object") {
                            if (elem.etichetta !== null) {

                                objMC.addMarker(elem.etichetta, true);
                            }
                        }
                    });

                }
            }

        });



        this.checkVisibleMarkerLabels();


    },
    checkVisibleMarkerLabels: function() {
        var bounds = mappa.elemenotMappa.getBounds();

        var mostraMarkerClusterFlag = $("#chk_mostra_markerClusters").is(':checked');
        let m = 0;
        if (!mostraMarkerClusterFlag) {
            for (m = 0; m < this.markerClusterer.length; m++) {
                this.markerClusterer[m].setMap(null);
            }
        }

        for (m = 0; m < this.markerClusterer.length; m++) {
            mappa.checkVisibleElements(mappa.markerClusterer[m].markers_, bounds);
        }
    },
    checkVisibleElements: function (elementsArray, bounds) {

        var mostraMarkerClusterFlag = $("#chk_mostra_markerClusters").is(':checked');

        //checks if marker is within viewport and displays the marker accordingly - triggered by google.maps.event "idle" on the map Object
        elementsArray.forEach(function (item) {
            //If the item is within the bounds of the viewport
            if (bounds.contains(item.position) && mappa.mostraEtichette && !mostraMarkerClusterFlag && mappa.elemenotMappa.zoom >= mappa_zoom_mostraEtichette) {
                //If the item isn't already being displayed
                if (item.map != mappa.elemenotMappa) {
                    item.setMap(mappa.elemenotMappa);
                }
            } else {
                item.setMap(null);
            }
        });
    },

    getLayerCheccati: function () {
        var risp = new Object();
        $('.chkLayer').each(function () {
            let id = $(this).attr('id');
            let chk = this.checked;
            risp["ID_" + id] = chk;
        });
        return risp;
    },

    GoogleBoundsWKT: function () {


        var bounds = this.elemenotMappa.getBounds();

        var neLat = bounds.getNorthEast().lat();
        var neLng = bounds.getNorthEast().lng();
        var swLat = bounds.getSouthWest().lat();
        var swLng = bounds.getSouthWest().lng();

        return "POLYGON((" +
            swLng + " " + swLat + "," +
            neLng + " " + swLat + "," +
            neLng + " " + neLat + "," +
            swLng + " " + neLat + "," +
            swLng + " " + swLat +
            "))";



    },

    //GABRIELE 10 04 2019
    ShowHideEtichette: function (showFlag) {

        this.mostraEtichette = showFlag;

        for (let l = 0; l < this.Livelli.length; l++) {
            let liv = this.Livelli[l];

            let p = 0;
            //punti
            for (p = 0; p < liv.punti.length; p++) {
                let pp = liv.punti[p];
                if (typeof pp.etichetta === "object") {
                    if (pp.etichetta !== null) {

                        pp.etichetta.setVisible(showFlag);
                    }
                }
            }

            //poligoni
            for (p = 0; p < liv.poligoni.length; p++) {
                let pol = liv.poligoni[p];
                if (typeof pol.etichetta === "object") {
                    if (pol.etichetta !== null) {

                        pol.etichetta.setVisible(showFlag);
                    }
                }
            }
        }

        for (let m = 0; m < this.markerClusterer.length; m++) {

            if (showFlag) {
                this.markerClusterer[m].setMap(this.elemenotMappa);
            } else {
                this.markerClusterer[m].setMap(null);
            }

        }
    }

}

//****************************************************************************************************

function CellQueue() {
    if (!(this instanceof CellQueue)) return new CellQueue();

    this.data = [];
    this.length = this.data.length;
}

CellQueue.prototype = {

    push: function (item) {
        this.data.push(item);
        this.length++;
        this._up(this.length - 1);
    },

    pop: function () {
        if (this.length === 0) return undefined;

        var top = this.data[0];
        this.length--;

        if (this.length > 0) {
            this.data[0] = this.data[this.length];
            this._down(0);
        }
        this.data.pop();

        return top;
    },

    peek: function () {
        return this.data[0];
    },
    _compare: function (a, b) {
        return b.max - a.max;
    },
    _up: function (pos) {
        var data = this.data;
        var compare = this._compare;
        var item = data[pos];

        while (pos > 0) {
            var parent = (pos - 1) >> 1;
            var current = data[parent];
            if (compare(item, current) >= 0) break;
            data[pos] = current;
            pos = parent;
        }

        data[pos] = item;
    },

    _down: function (pos) {
        var data = this.data;
        var compare = this._compare;
        var halfLength = this.length >> 1;
        var item = data[pos];

        while (pos < halfLength) {
            var left = (pos << 1) + 1;
            var right = left + 1;
            var best = data[left];

            if (right < this.length && compare(data[right], best) < 0) {
                left = right;
                best = data[right];
            }
            if (compare(best, item) >= 0) break;

            data[pos] = best;
            pos = left;
        }

        data[pos] = item;
    }
};

function Cell(lat, lng, h, polygon) {
    this.lat = lat; // cell center lat
    this.lng = lng; // cell center lng
    this.h = h; // half the cell size

    // signed distance from point to polygon outline (negative if point is outside)
    let inside = false;
    let minDistSq = Infinity;
    for (let i = 0, len = polygon.length, j = len - 1; i < len; j = i++) {
        let a = polygon[i];
        let b = polygon[j];

        if ((a.lng() > lng !== b.lng() > lng) &&
            (lat < (b.lat() - a.lat()) * (lng - a.lng()) / (b.lng() - a.lng()) + a.lat())) inside = !inside;

        // get squared distance from a point to a segment
        let x = a.lat();
        let y = a.lng();
        let dx = b.lat() - x;
        let dy = b.lng() - y;

        if (dx !== 0 || dy !== 0) {

            let t = ((lat - x) * dx + (lng - y) * dy) / (dx * dx + dy * dy);

            if (t > 1) {
                x = b.lat();
                y = b.lng();
            } else if (t > 0) {
                x += dx * t;
                y += dy * t;
            }
        }
        dx = lat - x;
        dy = lng - y;

        minDistSq = Math.min(minDistSq, dx * dx + dy * dy);
    }

    this.d = (inside ? 1 : -1) * Math.sqrt(minDistSq); // distance from cell center to polygon
    this.max = this.d + this.h * Math.SQRT2; // max distance to polygon within a cell
}

function PolyLabel() {
    if (!(this instanceof PolyLabel)) return new PolyLabel();

    this.vertex = [];
    this.minLat = 0;
    this.minLng = 0;
    this.maxLat = 0;
    this.maxLng = 0;
}

PolyLabel.prototype = {

    add: function (LatLng) {
        this.vertex.push(LatLng);
        if (this.vertex.length > 1) {
            let lat = parseFloat(LatLng.lat());
            let lng = parseFloat(LatLng.lng());
            if (lat < this.minlat) this.minlat = lat;
            if (lng < this.minlng) this.minlng = lng;
            if (lat > this.maxlat) this.maxlat = lat;
            if (lng > this.maxlng) this.maxlng = lng;
        } else {
            this.minlat = parseFloat(LatLng.lat());
            this.minlng = parseFloat(LatLng.lng());
            this.maxlat = this.minlat;
            this.maxlng = this.minlng;
        }
    },

    position: function (precision) {
        precision = precision || 0.000001;

        let width = this.maxlng - this.minlng;
        let height = this.maxlat - this.minlat;
        let cellSize = Math.min(width, height);
        let h = cellSize / 2;

        if (cellSize === 0)
            return new google.maps.LatLng(this.minlat, this.minlng);

        // take centroid as the first best guess
        let area = 0;
        let lat = 0;
        let lng = 0;
        // get polygon centroid
        let bestCell;
        for (let i = 0, len = this.vertex.length, j = len - 1; i < len; j = i++) {
            let a = this.vertex[i];
            let b = this.vertex[j];
            let f = a.lat() * b.lng() - b.lat() * a.lng();
            lat += (a.lat() + b.lat()) * f;
            lng += (a.lng() + b.lng()) * f;
            area += f * 3;
        }
        if (area === 0) {
            bestCell = new Cell(this.vertex[0].lat(), this.vertex[0].lng(), 0, this.vertex);
        } else {
            bestCell = new Cell(lat / area, lng / area, 0, this.vertex);
        }

        // special case for rectangular polygons
        let bboxCell = new Cell(this.minlat + height / 2, this.minlng + width / 2, 0, this.vertex);
        if (bboxCell.d > bestCell.d)
            bestCell = bboxCell;

        // a priority queue of cells in order of their "potential" (max distance to polygon)
        //let cellQueue = new TinyQueue(null, compareMax);
        let cellQueue = new CellQueue();

        // cover polygon with initial cells
        for (lng = this.minlng; lng < this.maxlng; lng += cellSize) {
            for (lat = this.minlat; lat < this.maxlat; lat += cellSize) {
                cellQueue.push(new Cell(lat + h, lng + h, h, this.vertex));
            }
        }

        while (cellQueue.length) {
            // pick the most promising cell from the queue
            let cell = cellQueue.pop();

            // update the best cell if we found a better one
            if (cell.d > bestCell.d)
                bestCell = cell;

            // do not drill down further if there's no chance of a better solution
            if (cell.max - bestCell.d <= precision)
                continue;

            // split the cell into four cells
            h = cell.h / 2;
            cellQueue.push(new Cell(cell.lat - h, cell.lng - h, h, this.vertex));
            cellQueue.push(new Cell(cell.lat + h, cell.lng - h, h, this.vertex));
            cellQueue.push(new Cell(cell.lat - h, cell.lng + h, h, this.vertex));
            cellQueue.push(new Cell(cell.lat + h, cell.lng + h, h, this.vertex));
        }

        return new google.maps.LatLng(bestCell.lat, bestCell.lng);

    }
};

function polylabel(polygon, precision) {
    let pl = new PolyLabel();
    for (let i = 0; i < polygon.length; i++) {
        pl.add(polygon[i]);
    }
    return pl.position(precision);
}



//****************************************************************************************************************



//Start custom poly fill code
PolyLineFill.prototype = new google.maps.OverlayView();

function PolyLineFill(cfgObj) {
    this.div_ = null;
    this.poly_ = cfgObj.poly;
    this.polysvg_ = null;
    this.fill_ = cfgObj.fill;
    this.stroke_ = cfgObj.stroke || "#000";
    this.opacity_ = cfgObj.opacity !== undefined ? cfgObj.opacity : "1";
    this.zIndex_ = cfgObj.zIndex || "999";
    this.calcBounds();
}

PolyLineFill.prototype.onAdd = function () {

    let layer = this.getPanes().overlayLayer;//overlayMouseTarget;//

    let nId = 0;
    $(layer.childNodes).each(function (index) {
        let cId = parseInt($(layer.childNodes[index]).attr('attr-id'));
        if (cId >= nId) {
            nId = cId + 1;
        }
    });

    let lineFill_id = "lineFill-" + nId;

    // Create the DIV and set some basic attributes.
    let div = document.createElement('div');
    div.style.borderStyle = 'none';
    div.style.borderWidth = '0px';
    div.style.position = 'absolute';
    div.style.zIndex = this.zIndex_;
    div.setAttribute('attr-id', nId);

    //https://www.w3schools.com/graphics/svg_reference.asp
    //create the svg element
    let svgns = "http://www.w3.org/2000/svg";
    let svg = document.createElementNS(svgns, "svg");
    svg.setAttributeNS(null, "height", "100%");
    svg.setAttributeNS(null, "width", "100%");
    svg.setAttributeNS(null, "preserveAspectRatio", "xMidYMid meet");

    //A container for referenced elements
    let def = document.createElementNS(svgns, "defs");

    //create the pattern fill
    let pattern = document.createElementNS(svgns, "pattern");
    pattern.setAttributeNS(null, "id", lineFill_id);
    pattern.setAttributeNS(null, "patternUnits", "userSpaceOnUse");
    pattern.setAttributeNS(null, "patternTransform", "rotate(-45)");
    pattern.setAttributeNS(null, "height", "5");
    pattern.setAttributeNS(null, "width", "5");
    def.appendChild(pattern);

    let rect = document.createElementNS(svgns, "rect");
    rect.setAttributeNS(null, "id", "rectFill");
    rect.setAttributeNS(null, "fill", this.fill_);
    rect.setAttributeNS(null, "fill-opacity", this.opacity_);
    rect.setAttributeNS(null, "stroke", this.stroke_);
    rect.setAttributeNS(null, "stroke-opacity", 1);//this.opacity_);
    rect.setAttributeNS(null, "stroke-dasharray", "5,5");
    rect.setAttributeNS(null, "width", "5");
    rect.setAttributeNS(null, "height", "5");
    pattern.appendChild(rect);

    svg.appendChild(def);

    //add polygon to the div
    let p = document.createElementNS(svgns, "polygon");
    p.setAttributeNS(null, 'fill', 'url(#' + lineFill_id + ')');
    p.setAttributeNS(null, 'stroke', this.fill_);
    p.setAttributeNS(null, 'stroke-width', '0'); //'1');
    p.setAttributeNS(null, 'pointer-events', 'all');
    //set a reference to this element;
    this.polysvg_ = p;
    svg.appendChild(p);

    div.appendChild(svg);

    // Set the overlay's div_ property to this DIV
    this.div_ = div;

    // We add an overlay to a map via one of the map's panes.
    // We'll add this overlay to the overlayLayer pane.
    layer.appendChild(this.div_);

    //// onclick listener solo se il layer sarà overlayMouseTarget e non overlayLayer
    //google.maps.event.addDomListener(this.polysvg_, 'click', function () {
    //    alert("click");
    //});
}

PolyLineFill.prototype.AdjustPoints = function () {
    //adjust the polygon points based on the projection.
    let proj = this.getProjection();
    let sw = proj.fromLatLngToDivPixel(this.bounds_.getSouthWest());
    let ne = proj.fromLatLngToDivPixel(this.bounds_.getNorthEast());

    let points = "";
    for (let i = 0; i < this.poly_.length; i++) {
        let point = proj.fromLatLngToDivPixel(this.poly_.getAt(i));
        if (i === 0) {
            points += (point.x - sw.x) + ", " + (point.y - ne.y);
        } else {
            points += " " + (point.x - sw.x) + ", " + (point.y - ne.y);
        }
    }
    return points;
}

PolyLineFill.prototype.draw = function () {
    // Size and position the overlay. We use a southwest and northeast
    // position of the overlay to peg it to the correct position and size.
    // We need to retrieve the projection from this overlay to do this.
    let overlayProjection = this.getProjection();

    // Retrieve the southwest and northeast coordinates of this overlay
    // in latlngs and convert them to pixels coordinates.
    // We'll use these coordinates to resize the DIV.
    let sw = overlayProjection.fromLatLngToDivPixel(this.bounds_.getSouthWest());
    let ne = overlayProjection.fromLatLngToDivPixel(this.bounds_.getNorthEast());

    // Resize the image's DIV to fit the indicated dimensions.
    let div = this.div_;
    div.style.left = sw.x + 'px';
    div.style.top = ne.y + 'px';
    div.style.width = (ne.x - sw.x) + 'px';
    div.style.height = (sw.y - ne.y) + 'px';

    this.polysvg_.setAttributeNS(null, "points", this.AdjustPoints());
}

PolyLineFill.prototype.onRemove = function () {

    this.div_.parentNode.removeChild(this.div_);
    this.div_ = null;
}

PolyLineFill.prototype.calcBounds = function () {
    this.bounds_ = new google.maps.LatLngBounds();
    for (let i = 0; i < this.poly_.length; i++) {
        this.bounds_.extend(this.poly_.getAt(i));
    }
}

PolyLineFill.prototype.setPath = function (index, obj, op) {
    if (op === 'replace') {
        this.poly_.setAt(index, obj);// = obj;
    } else if (op === 'insert') {
        this.poly_.splice(index, 0, obj);
    }
    this.calcBounds();
    this.draw();
}


window.BW = {};
window.BW.PolyLineFill = PolyLineFill;
//end poly fill code

export  { polylabel, PolyLabel, PolyLineFill, mappa };
