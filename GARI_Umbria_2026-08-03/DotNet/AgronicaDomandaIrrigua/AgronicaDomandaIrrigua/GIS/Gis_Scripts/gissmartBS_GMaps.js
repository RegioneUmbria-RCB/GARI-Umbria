

// gissmartBS google maps

var map;

var poligoni = [];
var punti = [];

var posizione_DaMappa_Lat = "";
var posizione_DaMappa_Lng = "";

var Gis_initializeLatLon_Scala = 18;
var AnteprimaSuMappa_bodyHeightOffset = 125;

//per ora il solo layer degli impianti
var layers = [{
    nome: "IMPIANTI",
    id: "19",
    colore_1: "0000FF",
    colore_2: "",
    varianza: "1",
    trasparenza: "0.6",
    icona16: "../AB_Immagini/icone/Impianto16.png",
    icona32: "../AB_Immagini/icone/Impianto32.png",
    TipoNodoAlberoAnagrafe: "6,7,8,9,19,"
}];


function ApriGoogleMaps() {

    mappa.CoordDaSelezione();

    var gDestinazione = "";
    if ($("#lat_cerca").val() != "" && $("#long_cerca").val() != "") {
        gDestinazione = $("#lat_cerca").val() + MapsSeparator + $("#long_cerca").val();
    } else {
        gDestinazione = $("#address").val();
    }


    if (gDestinazione != "" && gDestinazione !== undefined && gDestinazione !== null) {

        var goURL = "";
        if (MapsSeparator == "_") {
            goURL = "https://www.bing.com/maps?rtp=~pos." + gDestinazione;
        } else {
            goURL = "https://www.google.it/maps/dir//" + gDestinazione + "/";
        }


        window.open(goURL);
    } else {
        alert("Selezionare l'impianto sulla mappa.");
    }


}

function AnteprimaImpostaTitolo() {

    //visualizzo area del poligono;
    var area = selectedShapeAreaCalcola();
    var testoArea = "";
    if (area > 0)
        testoArea = "<div style='color: blue'>Sup. Calcolata (Ha): " + area.toString() + "</div>";

    $("#gissmartBsAnteprimaSuMappa_Title").html("<div>" + ddl_Impianto.text() + "</div>" + testoArea);

}

function selectedShapeAreaCalcola() {

    if (shape.selectedShape === undefined || shape.selectedShape === null)
        return -1;

    var area = gMapsUtility.getArea("", undefined, shape.selectedShape);

    return area;
}


function Gis_initializeLatLon(Lat, Long, mapZoom, showOverviewControl) {

    if (map == null) {
        setupMap(Lat, Long, mapZoom, showOverviewControl);
    }

    map.setZoom(mapZoom);

}


function setupMap(lat, lng, mapZoom, showOverviewControl) {
    var mapLatlng = new google.maps.LatLng(lat, lng);
    var myOptions = {
        zoom: mapZoom,
        center: mapLatlng,
        overviewMapControl: showOverviewControl,
        zoomControl: true,
        streetViewControl: false,
        fullscreenControl: true,
        fullscreenControlOptions: {
            position: google.maps.ControlPosition.BOTTOM_RIGHT
        },
        zoomControlOptions: {
            style: google.maps.ZoomControlStyle.SMALL,
            position: google.maps.ControlPosition.LEFT_TOP
        },
        mapTypeId: google.maps.MapTypeId.SATELLITE
    };
    map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
    google.maps.event.trigger(map, 'resize');
}

var mappa_inizializzata = false;
function inizializzaMappa(impostaModifica) {

    if (mappa_inizializzata)
        return;



    mappa.inizializza();

    mappa.elemenotMappa = map;

    //' VAnni: 13/4/2017: nessuna inizializzazione..
    //mappa.initNewMap();

    if (mappa.drawingManager == undefined || mappa.drawingManager == null) {
        mappa.drawingManager = new google.maps.drawing.DrawingManager({
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
            polygonOptions: {
                fillColor: 'red',
                strokeWeight: 5,
                fillOpacity: 0.5,
                editable: true
            },
            map: mappa.elemenotMappa
        });
    }


    /*al completamento del poligono */
    google.maps.event.addListener(mappa.drawingManager, 'overlaycomplete', function (e) {

        statoDisegnoCorrenteGoogle = enumStatoDisegnoGoogle.Completato;


        PulsanteAnnullaNouvoDisegno(false);

        
        $("#btnGGDisegno").prop('disabled', false);
        $("#btnGGDisegno").text("Concludi Disegno");

        var newShape = e.overlay;
        newShape.type = e.type;

        mappa.drawingManager.setDrawingMode(null);

        setSelection(newShape, true);

        preparaXSalvataggioPoligoni();

        utility.log("overlaycomplete");


    });

    mappa_inizializzata = true;

}

/* Selezione dello shape */
function setSelection(shapeS, modificabile) {
    shape.selectedShape = shapeS;
    shape.selectedShape.set('fillColor', shape.selectedShape.selectedColor);

    shapeS.setEditable(modificabile);

    AnteprimaImpostaTitolo();

    if (modificabile) {
        $("#btnGGDisegno").text("Concludi Disegno");
        statoDisegnoCorrenteGoogle = enumStatoDisegnoGoogle.Completato;
    }

}

function preparaXSalvataggioPoligoni() {

    //imposto i punti selezionati come se avessi fatto click diverse vote sul pulsante "Registra nuovo punto"
    console.log("Overlay complete");
}


/**
 * Mostra o nasconde il pulsante per annullare il nuovo disegno
 * @param {boolean} mostra
 */
function PulsanteAnnullaNouvoDisegno(mostra) {
    if (mostra) {
        $("#rowGGAnnullaDisegno").show();
        $("#rowGGDisegno").removeClass("col-lg-4 col-md-4 col-sm-6 col-xs-12").addClass("col-lg-4 col-md-4 col-sm-6 col-xs-6");        

    } else {
        $("#rowGGAnnullaDisegno").hide();        
        $("#rowGGDisegno").removeClass("col-lg-4 col-md-4 col-sm-6 col-xs-6").addClass("col-lg-4 col-md-4 col-sm-6 col-xs-12");      
    }
}

function btnGGAnnullaDisegno() {

    switch (statoDisegnoCorrenteGoogle) {

        case enumStatoDisegnoGoogle.InCorso:
            console.log("Annulla Nuovo Disegno");            
            interfaccia.switchDrawingMode(null);
            $("#btnGGDisegno").text("Nuovo Disegno");
            statoDisegnoCorrenteGoogle = enumStatoDisegnoGoogle.Nessuno;
            pulisciMappa();
            PulsanteAnnullaNouvoDisegno(false);
            break;
    }

}

function btnGGIniziaDisegno() {

    PulsanteAnnullaNouvoDisegno(false);

    switch (statoDisegnoCorrenteGoogle) {

        case enumStatoDisegnoGoogle.Nessuno:
            console.log("Nuovo Disegno");
            interfaccia.switchDrawingMode(google.maps.drawing.OverlayType.POLYGON);
            $("#btnGGDisegno").prop('disabled', true);
            $("#btnGGDisegno").text("In Corso ...");
            statoDisegnoCorrenteGoogle = enumStatoDisegnoGoogle.InCorso;
            PulsanteAnnullaNouvoDisegno(true);
            break;


        case enumStatoDisegnoGoogle.InCorso:              
            break;

        case enumStatoDisegnoGoogle.Completato:
            
            $("#btnGGDisegno").text("Nuovo Disegno");

            //rimuovo un eventuale poligono sotto..
            RimuoviPoligonoInFaseDisegno();

            flagGps = enum_Gis_FlagGPS.DisegnatoSuCartografia.value;

            var MVCArray = shape.selectedShape.getPath().getArray();

            for (var i = 0; i < MVCArray.length; i++) {
                console.log(MVCArray[i]);

                posizione_DaMappa_Lat = MVCArray[i].lat();
                posizione_DaMappa_Lng = MVCArray[i].lng();

                PulsanteNuovoPunto(true);
            }

            $("#gissmartBsAnteprimaSuMappa").data("kendoDialog").close();
            $("#GisSmartBSctrl").data("kendoDialog").open();

            //rimuovo il poligono dalla mappa
            shape.selectedShape.setMap(null);

            //svuoto l'oggetto
            shape.selectedShape = undefined;

            statoDisegnoCorrenteGoogle = enumStatoDisegnoGoogle.Nessuno;

            ddlRecuperaReInizializza();

            break;


        default:

    }


}

function ripulisciImmaginiCosaDisegno() {

}


function EditViaMappa() {

    var t = $("#txt_descrizione").val();
    if (t == undefined || t == "") {
        alert("Descrizione obbligatoria");
        return;
    }

    //$("#btn_nuovoPunto").hide();

    AnteprimaGisSmartBS(true);

}

function AnteprimaGisSmartBS(impostaModifica) {

    $("#GisSmartBSctrl").data("kendoDialog").close();

    $("#gissmartBsAnteprimaSuMappa").data("kendoDialog").open();

     WaitFrame.show();

    setTimeout(function () {

        if (google) {


            var sLatLon = $(".CurGPS_Pos").html();
            var sLat = sLatLon.split("|")[0];
            var sLong = sLatLon.split("|")[1];

            var lat = parseFloat(sLat);
            var long = parseFloat(sLong);

            if (isNaN(lat) || isNaN(long)) {
                lat = 44.1682308;
                long = 12.267000184;
            }

            Gis_initializeLatLon(lat, long, Gis_initializeLatLon_Scala, false);

            pulisciMappa();

            if (ddl_Recupera.value() != "") {

                var xidx = parseInt(ddl_Recupera.value()) - 1;
                $('#hidden_ID_poligono_x_salvataggio').val(xidx.toString());

                var DisegnoCorrente = HiddenPuntiNuovo_GetFromArrayPoligoni();
                var vDisegnoCorrente = DisegnoCorrente.split(":");

                PulsanteAnnullaNouvoDisegno(false);

                if (vDisegnoCorrente.length > 3) {
                    var polyCoords = JSON.parse(DisegnoCorrente);
                    Gis_Poligono(polyCoords);
                } else {
                    Gis_Marker(lat, long);
                }
            }


            AnteprimaImpostaTitolo();            


            if (impostaModifica) {
                $("#btnGGDisegno").show();
            } else {
                $("#btnGGDisegno").hide();
            }


            inizializzaMappa(impostaModifica);

        } else {

            alert("Impossibile trovare le API di Google Maps");

        }

        WaitFrame.hide();

    }, aggiornaTimeoutInizializza);
}


function EditViaPunto() {

    //$("#btn_EditViaMappa").hide();
    $("#btnGGDisegno").hide();

}

function EditTipo_Azzera() {

    flagGps = enum_Gis_FlagGPS.MisuratoSuSmartPhone.value;

    $("#btn_nuovoPunto").show();
    $("#btn_EditViaMappa").show();
    $("#btnGGDisegno").show();
}

function ImpiantoCaricaDati() {

    var xChiaveAlbero = ddl_Impianto.value();

    var vChiaveAlbero = xChiaveAlbero.split("§");

    var piva = "";
    var saCod = 0;
    var appezza = 0;
    var idReg = 0;

    piva = vChiaveAlbero[1];
    saCod = parseInt(vChiaveAlbero[2]);
    appezza = parseInt(vChiaveAlbero[4]);
    idReg = parseInt(vChiaveAlbero[5]);

    ajaxAgronica(gisSmartIndirizzoHttp + "/CaricaSingoloOggetto_Impianto"
        , JSON.stringify({ piva: piva, saCod: saCod, appezza: appezza, idReg: idReg }),
        function (risposta) {

            var res = JSON.parse(risposta.RispostaStringa);

            $("#lbl_msgImpianti").html(res.messaggio);
            entitaCod = res.entitaCod;


            //questo chiude eventuali disegni in corso... sarà re-impostato in seguito...
            statoDisegnoCorrente = statoDisegno.Nessuno;

            var chiaveAlbero = ddl_Impianto.value().toString().replace(/\\/g, '\\\\');
            var risultatiQ;

            if (res.dati.length > 0) {

                ImpiantoCaricaDatiMsgTrovati();

                risultatiQ = poligonoCacheEntitaCodRicerca(entitaCod, chiaveAlbero);

                //decido se caricare da cache oppure impostare i dati da server
                if (risultatiQ.length == 0) {
                    ImpiantoCaricaDatiElaboraPunti(res.dati[0].vertici);
                    memorizzaPoligonoCacheEntitaCod(entitaCod);
                } else {
                    ddl_Recupera.value(risultatiQ[0].poligonoCorrente + 1);
                    RecuperaPoligono();
                    ImpiantoCaricaDatiElaboraPunti_Pulisci = true;
                }


            } else {

                //reupero per chive albero eventuali elementi da cache
                ImpiantoCaricaDatiMsgNonTrovati();
                PoligonoInFaseDisegnoNascondi();

                risultatiQ = poligonoCacheEntitaCodRicercaChiaveAlbero(chiaveAlbero);
                if (risultatiQ.length > 0) {
                    ddl_Recupera.value(risultatiQ[0].poligonoCorrente + 1);
                    RecuperaPoligono();
                    ImpiantoCaricaDatiElaboraPunti_Pulisci = true;
                }

            }

        }, null);
}

function PoligonoInFaseDisegnoNascondi() {

    var poligonoCorrente = PoligonoInFaseDisegno();

    if (poligonoCorrente != -1) {
        ddl_Recupera.value("");
        RecuperaPoligono();
    }

}


function RimuoviPoligonoInFaseDisegno() {

    var poligonoCorrente = PoligonoInFaseDisegno();
    if (poligonoCorrente != -1) {
        Cancellapoligono($("#Cancellapoligono_" + poligonoCorrente));
    }

}


function PoligonoInFaseDisegno() {
    var sel = ddl_Recupera.value();
    if (sel != undefined && sel != "") {
        var poligonoCorrente = parseInt(sel);
        poligonoCorrente = poligonoCorrente - 1;
        return poligonoCorrente;
    }

    return -1;
}

function poligonoCacheEntitaCodRicercaChiaveAlbero(chiaveAlbero) {
    if (poligonoCacheEntitaCod.length == 0) {
        return poligonoCacheEntitaCod;
    } else {
        return poligonoCacheEntitaCod.filter(function (x) { return (x.chiaveAlbero == chiaveAlbero) });
    }


}
function poligonoCacheEntitaCodRicerca(entitaCod, chiaveAlbero) {

    if (poligonoCacheEntitaCod.length == 0) {
        return poligonoCacheEntitaCod;
    } else {
        return poligonoCacheEntitaCod.filter(function (x) { return (x.entitaCod == entitaCod && x.chiaveAlbero == chiaveAlbero) });
    }


}


function poligonoCacheEntitaCodRicercaXNumero(poligonoCorrente) {

    if (poligonoCacheEntitaCod.length == 0) {
        return poligonoCacheEntitaCod;
    } else {
        return poligonoCacheEntitaCod.filter(function (x) { return (x.poligonoCorrente == poligonoCorrente) });
    }


}

function memorizzaPoligonoCacheEntitaCod(entitaCodImposta) {

    var poligonoCorrente = numeroPoligoni;
    poligonoCorrente = poligonoCorrente - 1;

    if (poligonoCorrente >= 0) {

        var chiaveAlbero = ddl_Impianto.value().toString().replace(/\\/g, '\\\\');

        //se sono offLine allora non devo legare il poligono ad un elemento.
        if (!LavoraOnLine)
            chiaveAlbero = "";

        var risultatiQ = poligonoCacheEntitaCodRicerca(entitaCodImposta, chiaveAlbero);

        if (risultatiQ.length == 0) {

            var p = {
                entitaCod: entitaCodImposta,
                poligonoCorrente: poligonoCorrente,
                chiaveAlbero: chiaveAlbero,
                modificato: false
            }

            poligonoCacheEntitaCod.push(p);

            cookiePoligonoCacheEntitaCod();

        }

    }

}



var ImpiantoCaricaDatiElaboraPunti_Pulisci = false;
function ImpiantoCaricaDatiElaboraPunti(vertici) {

    //rimuovo eventualmente il poligono in fase di disegno.
    PoligonoInFaseDisegnoNascondi();

    //carico tutti i punti
    for (var i = 0; i < vertici.length; i++) {
        console.log(vertici[i]);

        posizione_DaMappa_Lat = vertici[i].lat;
        posizione_DaMappa_Lng = vertici[i].long;

        PulsanteNuovoPunto(true);
    }

    ImpiantoCaricaDatiElaboraPunti_Pulisci = true;

}

function ImpiantoCaricaDatiMsgTrovatiClear() {
    $("#lbl_msgImpianti").css("font-size", "");
    $("#lbl_msgImpianti").css("font-weight", "");
    $("#lbl_msgImpianti").css("color", "");
}

function ImpiantoCaricaDatiMsgTrovati() {

    ImpiantoCaricaDatiMsgTrovatiClear();

    $("#lbl_msgImpianti").css("font-size", "14px");
    $("#lbl_msgImpianti").css("font-weight", "bold");
    $("#lbl_msgImpianti").css("color", "green");
}

function ImpiantoCaricaDatiMsgNonTrovati() {

    ImpiantoCaricaDatiMsgTrovatiClear();

    $("#lbl_msgImpianti").css("font-size", "12px");
    $("#lbl_msgImpianti").css("font-weight", "bold");
    $("#lbl_msgImpianti").css("color", "orange");
}



function Gis_Marker(Lat, Long) {

    pulisciMappa();

    var quakeEventLatlng = new google.maps.LatLng(Lat, Long);
    var marker = createQuakeEventMarker(quakeEventLatlng);
    marker.setAnimation(google.maps.Animation.DROP);

    punti.push(marker);

    var latlngAutoFit = new google.maps.LatLngBounds();
    latlngAutoFit.extend(new google.maps.LatLng(Lat, Long));

    map.setCenter(latlngAutoFit.getCenter(), map.fitBounds(latlngAutoFit));
    map.setZoom(16);

}


function createQuakeEventMarker(quakeEventLatlng) {
    return new google.maps.Marker({ position: quakeEventLatlng, map: map });
}

function Gis_Poligono(polyCoords) {


    var latlngAutoFit = new google.maps.LatLngBounds();
    for (cCoord in polyCoords) {
        latlngAutoFit.extend(new google.maps.LatLng(polyCoords[cCoord].lat, polyCoords[cCoord].lng));
    }

    try {

        // Construct the polygon.
        var abc = new google.maps.Polygon({
            paths: polyCoords,
            strokeColor: "#" + layers[0].colore_1,
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: "#" + layers[0].colore_1,
            fillOpacity: 0.35
        });

        //evento di selezione
        google.maps.event.addListener(abc, 'click', function () {
            setSelection(this, true);
        });

        poligoni.push(abc);
        abc.setMap(map);


        map.setCenter(latlngAutoFit.getCenter(), map.fitBounds(latlngAutoFit));
        map.setZoom(16);

    } catch (e) {

        alert("Impossibile visualizzare un'anteprima del poligono.");

    }


}


function pulisciMappa() {

    if (shape.selectedShape !== null && shape.selectedShape !== undefined)
        shape.selectedShape.setMap(null);

    cancellaOggettiGrafici(poligoni);
    cancellaOggettiGrafici(punti);
}


function cancellaOggettiGrafici(oggetti) {
    for (var i = 0; i < oggetti.length; i++) {
        oggetti[i].setMap(null);
    }
    oggetti.length = 0;
}