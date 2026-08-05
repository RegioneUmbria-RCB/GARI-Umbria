

/* scriptMaps_DI_PARTENZA.js */


//Variabili Globali

var Debug_Mode = Enum_debugMode.Off;


//indica il tipo di render 
var Enum_TipoRender = {
    Completa: { value: 0, name: "Completa", code: 0 },
    Parziale: { value: 1, name: "Parziale", code: 1 }
};


//tipologia dei layer
var Enum_tipologia_layer = {

    Standard_Entità: { value: 1, name: "Standard_Entità", code: 1 },
    Gruppo_Colturale: { value: 5, name: "Gruppo_Colturale", code: 5 },
    Avversità: { value: 7, name: "Avversità", code: 7 },
    Fenologia: { value: 8, name: "Fenologia", code: 8 },
    Rilievi_Vegeto_Produttivi: { value: 9, name: "Rilievi_Vegeto_Produttivi", code: 9 },
    Analisi_Cronologia_Agenda: { value: 11, name: "Analisi_Cronologia_Agenda", code: 11 },
    Cultivar: { value: 15, name: "Cultivar", code: 15 },
    Organizzazione_di_appartenenza: { value: 100, name: "Organizzazione_di_appartenenza", code: 100 }

};

var TipoRender = Enum_TipoRender.Completa;

var nElementiNascosti = 0;

var response_ok;
var geocoder;
//var Livelli;
//var mappa.MarkGps;
var Testi = new Array();
//var infowindow;
//var markerIndirizzo;
//var map;
var w;
var h;
//var drawingManager;
//var shape.selectedShape;
//var shape.selectedShapeArray = new Array();
//var shape.selectedShape_Copia;
//var selectedColor;
var colorButtons = {};

var markerClusterer = new Array();


var GiasBase_Path = GiasBase_Domain + PATH_GIASBASE + "agronica/scripts/";
var GoogleUtility_Path = GiasBase_Path + "GoogleMaps/v3-utility-library/";
var MarkerClusterer_Path = GoogleUtility_Path + "markerclusterer"



//var cosaStoDisegnando = google.maps.drawing.OverlayType.POLYGON;
//var shape.cosaPossoDisegnare = new Array();
var gisTipoOggettoXLavCod = new Array();
//var gisTipoOggettoXLayer = new Array();
var placeLivelli = new Array();

var layerNonAlbero = new Array(
    2,
    4,
    5,
    6,
    7,
    8,
    9,
    10,
    11,
    12,
    14,
    15,
    16,
    17,
    18,
    33,
    51
);


var AttivaSelezioneMultipla = true;

var place;

var Sementieri_Sportello_Configurazione_cod = -1;
var isSementi = false;
var ModalitaBootstrap = false;
var AggiornaDatiGiasAlarm = false;
var CiSonoVecchiDatiNonImportati = false;
//var glayerDoveDisegno = "-1";
var gOperazioneDiAgendaDoveDisegno = "-1";
var gNuovo = false;
var isDebug = false;
var AbilitaPF = false;
var Codice_Fiscale_Tecnico = "";

var overlay;

var CoordFromPoints;

var indirizzohttp = location.pathname;
indirizzohttp = indirizzohttp.split("/")[indirizzohttp.split("/").length - 1];

var maxH;

var ctrlPressed = false;

var AperturaLayer = false;


var OffSet_contenitore_Tool = 20;
var mappa_zoom_mostraEtichette = 16;
var mappa_zoom_mostraPunti = 16;
var bAbilitaRenderPerEventoZoom = true;
var bAbilitaRenderPerEventoDrag = true;

//delay in millisecondi
var mappa_drag_delay = 2500;
var mappa_zoom_delay = 1000;

var richiediClickSuAggiornaFinestra = false;

//fine variabili globali

function cacheIt(event) {
    ctrlPressed = event.ctrlKey && AttivaSelezioneMultipla;
}
document.onkeydown = cacheIt;
document.onkeyup = cacheIt;

//jQuery.logThis = function (text) {
//    if ((window['console'] != undefined)) {
//        console.log(text);
//    }
//}

// 










function selezionaIndiceLayerDoveScrivoDaAlbero(livello, chiaveAlbero) {
    utility.warn('selezionaIndiceLayerDoveScrivoDaAlbero(livello:=$(' + livello + "), chiaveAlbero:=" + chiaveAlbero);

    var chiaviAlberoLayer = livello.TipoNodoAlberoAnagrafe.split(",");
    //    var chiaveID = chiaveAlbero.split("\\")[0];
    var chiaveID = chiaveAlbero.split(separatoreChiaveAlbero)[0]; //todo, verifica separatoratore
    for (var i = 0; i < chiaviAlberoLayer.length; i++) {
        if (chiaviAlberoLayer[i] == chiaveID) {
            shape.settaLayerDoveDisegnare(livello.id, livello.icona32, livello.nome);
            return true;
        }
    }
    return false
}






function cmbViste_change() {
    utility.warn('cmbViste_change');
    interfaccia.loading(true);
    interfaccia.selezionaDivVista();
    clearOverlaysByID(ID_Overlay);
    mappa.ImpostaShape(false);
    interfaccia.loading(false);
}





function indiceCorrenteDivVista(datiViste) {
    for (var i = 0; i < datiViste.length; i++)
        if (datiViste[i].nome == $("#cmbViste option:selected").text())
            return i;

    return -1;
}


function PopolaComboPannelloColore(dati, datiValidi) {

    var html = "";
    var vDati = dati.split("|");

    var aggiunti = new Array();


    for (var i = 0; i < vDati.length; i++) {
        var vLabels = vDati[i].split("§");

        if (stringhe.contains(datiValidi, vLabels[0] + '§') && $.inArray(vLabels[0], aggiunti) == -1) {

            aggiunti.push(vLabels[0]);

            html = html + "<option value='" + vLabels[0] + '§' + "'";
            if (vLabels[0] == "")
                html = html + " selected='selected' ";
            html = html + ">" + vLabels[0] + "</option>";

        }
    }

    $("#cmbViste").html(html);
}














/********************************************************************/
/********************** INIT ****************************************/
/********************************************************************/





/* inizializzazione della mappa */
function initialize() {
    interfaccia.initDatePiker();
    //CreaPannelloColore(50,160,"#E01B6A","#1B8EE0",5);
    mappa.isKws = isKws();
    mappa.inizializza();

    //    shape.cosaPossoDisegnare.push(google.maps.drawing.OverlayType.POLYGON);

    //    ImpostaLayer();

    //    inizializzaArrayCosaPossodisegnare();

    mappa.initNewMap();



    /*demo*/

    /* demo */



    //    var polyOptions = {
    //        strokeWeight: 0,
    //        fillOpacity: 0.6,
    //        editable: true
    //    };

    //per la creazione dell'interfaccia di edit dei poligoni
    //    drawingManager = new google.maps.drawing.DrawingManager({
    //        drawingControlOptions: {
    //            position: google.maps.ControlPosition.TOP_RIGHT,
    //            drawingModes: [google.maps.drawing.OverlayType.POLYGON, google.maps.drawing.OverlayType.MARKER]
    //        },
    //        markerOptions: {
    //            draggable: true
    //        },
    //        polylineOptions: {
    //            editable: true
    //        },
    //        drawingControl: false,
    //        polygonOptions: mappa.polyOptions,
    //        map: mappa.elemenotMappa
    //    });

    $(document).on("click", "#LayerSelezionaTutto", function () {
        clickLayerSelezionaTutto(true);
    });

    $(document).on("click", "#LayerDeSelezionaTutto", function () {
        clickLayerSelezionaTutto(false);
    });

    $(document).on("click", "#selimg_multipoint", function () {
        interfaccia.switchDrawingMode(google.maps.drawing.OverlayType.MARKER);
    });


    $(document).on("click", "#selimg_poligono", function () {
        interfaccia.switchDrawingMode(google.maps.drawing.OverlayType.POLYGON);
    });

    /*al completamento del poligono */
    google.maps.event.addListener(mappa.drawingManager, 'overlaycomplete', function (e) {

        utility.log("overlaycomplete");

        var newShape = e.overlay;
        newShape.type = e.type;

        if (e.type != google.maps.drawing.OverlayType.MARKER) {

            mappa.drawingManager.setDrawingMode(null);
            google.maps.event.addListener(newShape, 'click', function () {
                setSelection(newShape);
            });
            setSelection(newShape);
            if (shape.glayerDoveDisegnoSuTipologiaStandard !== "-1" && shape.glayerDoveDisegnoSuTipologiaStandard !== undefined) {
                preparaXSalvataggioPoligoni();
            } else {
                alert("non è stato selezionato alcun layer dove disegnare. Selezionare un layer facendo click sulla relativa icona nell elenco dei layer.");

                if (shape.selectedShape != undefined) {
                    shape.selectedShape.setMap(null);
                }
                clearSelectionBoth();

            }

        } else {

            utility.log("addMarkerMultiPoint");
            addMarkerMultiPoint($("#chkMP"), $("#hiddenMultipoint"), null, newShape.getPosition());

        }


    });

    //Eventi Google Maps
    // Clear the current selection when the drawing mode is changed, or when the
    // map is clicked.
    google.maps.event.addListener(mappa.drawingManager, 'drawingmode_changed', clearSelectionBoth);

    google.maps.event.addListener(mappa.elemenotMappa, 'zoom_changed', mappa_zoom_changed);
    google.maps.event.addListener(mappa.elemenotMappa, 'bounds_changed', mappa_bounds_changed);
    google.maps.event.addListener(mappa.elemenotMappa, 'dragend', mappa_dragend);
    google.maps.event.addListener(mappa.elemenotMappa, 'tilesloaded', mappa_tilesloaded);

    google.maps.event.addListener(mappa.elemenotMappa, 'mousemove', mappa_mousemove);
    google.maps.event.addListener(mappa.elemenotMappa, 'mouseover', mappa_mouseover);
    google.maps.event.addListener(mappa.elemenotMappa, 'mouseout', mappa_mouseout);

    google.maps.event.addListener(mappa.elemenotMappa, 'idle', mappa_idle);



    //Eventi Agronica
    //pulisco la selezione eventuale di elementi
    google.maps.event.addListener(mappa.elemenotMappa, 'click', clearSelection);

    google.maps.event.addDomListener(document.getElementById('delete-button'), 'click', deleteSelectedShape);
    google.maps.event.addDomListener(document.getElementById('save-button'), 'click', preparaXSalvataggioPoligoni);


    google.maps.event.addDomListener(document.getElementById('disenga_poligono'), 'click', CreaPoligono);

    //seleziono un poligono
    google.maps.event.addDomListener(document.getElementById('select_poligono'), 'click', SelezionaPoligono);

    google.maps.event.addDomListener(document.getElementById('info_appezzamento'), 'click', Info_Poligono);
    google.maps.event.addDomListener(document.getElementById('PianoRateoVariabile'), 'click', dialogRateo);
    google.maps.event.addDomListener(document.getElementById('GeneraPlanning'), 'click', dialogGeneraPlanning);
    google.maps.event.addDomListener(document.getElementById('BufferZone'), 'click', dialogBufferZoneIntersection);
    google.maps.event.addDomListener(document.getElementById('AnalisiMeteo'), 'click', dialogAnalisiMeteo);

    makeToolBar();
}


//#Region "Eventi google maps"

function mappa_tilesloaded() {

}

function mappa_bounds_changed() {
    utility.log("mappa_bounds_changed");
}

function mappa_dragend() {

    if (TipoRender == Enum_TipoRender.Parziale) {


        delay_KeyUp(function () {

            utility.log("mappa_dragend (in)..." + mappa.elemenotMappa.zoom.toString());

            //in questo caso nessuna renderizzaione è necessaria, si rimanda..
            if (RenderizzaShape_LivelloZoom == 0) {
                RenderizzaShape_LivelloZoom = mappa.elemenotMappa.zoom;
            } else {

                if (bAbilitaRenderPerEventoDrag) {
                    Renderizza();
                }
            }

        }, mappa_drag_delay);
    }
    //utility.log("mappa_drag");
}

function mappa_mousemove() {
    bAbilitaRenderPerEventoZoom = false;
    //attenzione a non sovraccaricare la funzione..
    //utility.log("mappa_mousemove");
}

function mappa_mouseover() {
    utility.log("mappa_mouseover");
}

function mappa_mouseout() {
    utility.log("mappa_mouseout");
}

function mappa_idle() {

    utility.log("mappa_idle");
    bAbilitaRenderPerEventoZoom = true;
}

var RenderizzaShape_LivelloZoom = 0;

function mappa_zoom_devoRenderizzare(NuovoZoom, ZoomRichiesto_Soglia) {

    var rval = false;
    if (NuovoZoom > ZoomRichiesto_Soglia) {
        rval = !(RenderizzaShape_LivelloZoom > ZoomRichiesto_Soglia);
    } else {
        rval = !(RenderizzaShape_LivelloZoom < ZoomRichiesto_Soglia);
    }

    return rval;


}

function mappa_zoom_changed() {

    if (TipoRender == Enum_TipoRender.Parziale) {

        utility.log("mappa_zoom_changed (out)..." + mappa.elemenotMappa.zoom.toString());

        //' VAnni: 6/3/2017: nonostante la chiamata sul wait questa non viene eseguita correttamente .. in attesa di risolvere commento.
        //interfaccia.loading(true);

        delay_KeyUp(function () {

            utility.log("mappa_zoom_changed (in)..." + mappa.elemenotMappa.zoom.toString());

            //in questo caso nessuna renderizzaione è necessaria, si rimanda..
            if (RenderizzaShape_LivelloZoom == 0) {
                RenderizzaShape_LivelloZoom = mappa.elemenotMappa.zoom;
            } else {
                var b_mappa_zoom_devoRenderizzare = false;
                b_mappa_zoom_devoRenderizzare = mappa_zoom_devoRenderizzare(mappa.elemenotMappa.zoom, mappa_zoom_mostraPunti);

                //se l'ultimo livello di zoom raggiunto determina un ricaricaricamento allo procedo, altrimenti no...
                if (b_mappa_zoom_devoRenderizzare && bAbilitaRenderPerEventoZoom) {
                    Renderizza();
                }
            }

        }, mappa_zoom_delay);

    }


}

function Renderizza() {

    utility.log("mappa_zoom_changed (esegue render)..." + mappa.elemenotMappa.zoom.toString());

    bAbilitaRenderPerEventoZoom = false;
    mappa.RenderizzaShape();
    RenderizzaShape_LivelloZoom = mappa.elemenotMappa.zoom;

    utility.log("mappa_zoom_changed (in..fine)..." + mappa.elemenotMappa.zoom.toString());

}
//#End Region "Eventi google maps"




function CreaPoligono() {
    interfaccia.CreaPoligono();
}

function SelezionaPoligono() {
    mappa.SelezionaPoligono();
}

function Info_Poligono() {
    mappa.Info_Poligono();
}



function InizializzaMenuCosaDisegno() {

    utility.log("inizializza Long Press!");
    $("#disenga_poligono").mouseup(function () {
        clearTimeout(pressTimer)
        // Clear timeout
        utility.log("mouseup Long Press!");
        return false;
    }).mousedown(function () {
        // Set timeout
        pressTimer = window.setTimeout(function () { mostraMenuCosaDisegno() }, 1000)
        return false;
    });
}

function mostraMenuCosaDisegno() {
    utility.log("mousedown Long Press!");
    shape.mostraMenuCosaDisegnoPreparaStrumenti();
}

function nascondoMenuCosaDisegno() {
    shape.nascondoMenuCosaDisegno();
}





function verificaSePossoDisegnareQuesto(cosaVoglioDisegnare) {
    for (var i = 0; i < mappa.cosaPossoDisegnare.length; i++) {
        if (mappa.cosaPossoDisegnare[i] == cosaVoglioDisegnare)
            return true;
    }
    return false;
}

function ripulisciImmaginiCosaDisegno() {
    $("#img_poligono").hide();
    $("#img_multipoint").hide();
}













function ShapeAggiungiOggetti(p_place, k_livelli, colore_poligon, polygonPaths, lat_centro, lng_centro, circle_radius) {

    var xEtichetta = "";

    //TODO: Rendere configurabile fillOpacity
    var trasparenza = k_livelli.trasparenza;
    if (trasparenza === undefined) {
        trasparenza = 0.6;
    }

    switch (p_place.TipologiaGML) {
        case 'Point':
            var punto;

            if (p_place.tipoicona == "Trattore") {

                punto = new google.maps.Marker({
                    html: p_place.id,
                    position: polygonPaths[0],
                    draggable: false,
                    icon: trattore
                });
            }
            else {

                var isDraggable;
                isDraggable = !(p_place.layer == 55)

                var ImpostaEtichetta = true;

                if (p_place.Testo.indexOf("§") > 0) {
                    ImpostaEtichetta = false;
                }

                xEtichetta = "";
                if (ImpostaEtichetta) {
                    xEtichetta = p_place.Testo;
                }
                punto = new google.maps.Marker({
                    html: p_place.id,
                    position: polygonPaths[0],
                    draggable: isDraggable,
                    icon: k_livelli.icona16,
                    label: { text: xEtichetta, color: "yellow" },
                    layerDiAppartenenza: p_place.layer,
                    chiavealbero: utility.chiaveAlbero_ridotta_to_big(p_place.chiavealbero)
                });
            }


            //'  Vanni, 23/08/2016 14:54:14: Evito di mostrare elementi di Precision Farming...

            //var ImpostaEtichetta = true;

            //if (p_place.Testo.indexOf("§") > 0) {
            //    ImpostaEtichetta = false;
            //}

            //if (ImpostaEtichetta) {
            //    var lbl;
            //    lbl = new Label({ text: p_place.Testo, html: p_place.id });
            //    lbl.bindTo('position', punto, 'position');
            //}
            //k_livelli.punti.push(punto);

            //if (ImpostaEtichetta) {
            //    k_livelli.ABLabel.push(lbl);
            //}

            k_livelli.punti.push(punto);
            break;

        case 'LineString':
            if (p_place.vertici.length == 2) {
                k_livelli.polyline.push(new google.maps.Polyline({
                    html: p_place.id,
                    path: polygonPaths,
                    zIndex: 100000,
                    layerDiAppartenenza: p_place.layer,
                    icona32: k_livelli.icona32,
                    layerDiAppartenenza_nome: k_livelli.nome,
                    flag_gps: p_place.flag_gps
                }));
            }
            else {
                //Linestring
                k_livelli.polyline.push(new google.maps.Polyline({
                    html: p_place.id,
                    path: polygonPaths,
                    strokeColor: '#' + colore_poligon,
                    fillOpacity: 1.0,
                    strokeWeight: 1,
                    zIndex: 100000,
                    layerDiAppartenenza: p_place.layer,
                    icona32: k_livelli.icona32,
                    layerDiAppartenenza_nome: k_livelli.nome,
                    flag_gps: p_place.flag_gps
                }));
            }
            break;
        case 'Circle':
            //circle
            colore_poligon = aggiungiCancellettoSeNonEsiste(colore_poligon);
            console.info(circle_radius);
            k_livelli.circle.push(new google.maps.Circle({
                html: p_place.id,
                vegcod: p_place.veg_cod,
                inserimento: p_place.inserimento,
                Entita_Cod: p_place.Entita_Cod,
                modifica: p_place.modifica,
                cancellazione: p_place.cancellazione,
                informazioni: p_place.informazioni,
                chiavealbero: utility.chiaveAlbero_ridotta_to_big(p_place.chiavealbero),
                AppIdRate: p_place.AppIdRate,
                testo: p_place.Testo,
                center: polygonPaths[0],
                radius: circle_radius,
                strokeColor: colore_poligon,
                fillOpacity: trasparenza,
                strokeWeight: 1,
                fillColor: colore_poligon,
                selectedColor: '#7d7d7d',
                colore_precedente: colore_poligon,
                zIndex: 10000000000, //p_place.zindex
                layerDiAppartenenza: p_place.layer,
                icona32: k_livelli.icona32,
                layerDiAppartenenza_nome: k_livelli.nome,
                flag_gps: p_place.flag_gps
            }));
            break;
        case 'Polygon':
            //Poligoni
            colore_poligon = aggiungiCancellettoSeNonEsiste(colore_poligon);

            //vanni, 27/02/2018, se richiesto imposto un'etichetta ... 
            if ($("#chk_mostra_descrizioneImpianto").is(':checked')  && k_livelli.MostraDescrizioneAssociata === "1") {
                var etichetta = $("#" + utility.chiaveAlbero_ridotta_to_big(p_place.chiavealbero)).text();
                if (etichetta !== "") {

                    var myLatlngCentro = new google.maps.LatLng(lat_centro, lng_centro);
                    var pEtichetta = new google.maps.Marker({
                        html: p_place.id,
                        position: myLatlngCentro,
                        draggable: isDraggable,
                        icon: k_livelli.icona16,
                        label: { text: etichetta, color: "yellow" },
                        layerDiAppartenenza: p_place.layer,
                        chiavealbero: utility.chiaveAlbero_ridotta_to_big(p_place.chiavealbero)
                    });
                    k_livelli.punti.push(pEtichetta);
                }
            }

            k_livelli.poligoni.push(new google.maps.Polygon({
                html: p_place.id,
                vegcod: p_place.veg_cod,
                inserimento: p_place.inserimento,
                Entita_Cod: p_place.Entita_Cod,
                modifica: p_place.modifica,
                cancellazione: p_place.cancellazione,
                informazioni: p_place.informazioni,
                chiavealbero: utility.chiaveAlbero_ridotta_to_big(p_place.chiavealbero),
                AppIdRate: p_place.AppIdRate,
                testo: p_place.Testo,
                paths: polygonPaths,
                strokeColor: colore_poligon,
                fillOpacity: trasparenza,
                strokeWeight: 1,
                fillColor: colore_poligon,
                selectedColor: '#7d7d7d',
                colore_precedente: colore_poligon,
                zIndex: p_place.zindex,
                layerDiAppartenenza: p_place.layer,
                icona32: k_livelli.icona32,
                layerDiAppartenenza_nome: k_livelli.nome,
                flag_gps: p_place.flag_gps
            }));
            break;
        case 'MultiSurface':
            //Poligoni
            colore_poligon = aggiungiCancellettoSeNonEsiste(colore_poligon);

            k_livelli.poligoni.push(new google.maps.Polygon({
                html: p_place.id,
                vegcod: p_place.veg_cod,
                inserimento: p_place.inserimento,
                Entita_Cod: p_place.Entita_Cod,
                modifica: p_place.modifica,
                cancellazione: p_place.cancellazione,
                informazioni: p_place.informazioni,
                chiavealbero: utility.chiaveAlbero_ridotta_to_big(p_place.chiavealbero),
                AppIdRate: p_place.AppIdRate,
                testo: p_place.Testo,
                paths: polygonPaths,
                strokeColor: colore_poligon,
                fillOpacity: trasparenza,
                strokeWeight: 1,
                fillColor: colore_poligon,
                selectedColor: '#7d7d7d',
                colore_precedente: colore_poligon,
                zIndex: p_place.zindex,
                layerDiAppartenenza: p_place.layer,
                icona32: k_livelli.icona32,
                layerDiAppartenenza_nome: k_livelli.nome,
                flag_gps: p_place.flag_gps
            }));
            break;
        default:
            alert(p_place.TipologiaGML);
    }

    if (p_place.flag_gps == '1') {
        //calcolo il baricentro del poligono
        //aggiungo l'icona
        var myLatlng = new google.maps.LatLng(lat_centro, lng_centro);

        if ($('#chk_mostra_GPS').is(':checked') == true) {
            mappa.MarkGps.push(new google.maps.Marker({
                position: myLatlng,
                map: mappa.elemenotMappa,
                icon: variabile_gps
            }));
        }
    }
}




function aggiungiCancellettoSeNonEsiste(colore) {
    if (!stringhe.startsWith(colore, "#"))
        colore = "#" + colore;
    return colore
}

function ImpostaMappa(dati, setClick, setdblClick, functSelect, setMap_isInBound) {
    for (var kk = 0; kk < dati.length; kk++) {

        if (setClick) {

            google.maps.event.addListener(dati[kk], 'mouseup', function () {
                if (functSelect != 'poly' && functSelect != 'circle') {
                    setSelectionPointMouseUp(this);
                }
            });
            google.maps.event.addListener(dati[kk], 'click', function () {
                if (functSelect == 'poly' || functSelect == 'circle') {
                    if (functSelect == 'poly') {
                        if (ctrlPressed)
                            setMultiSelection(this);
                        else
                            setSelection(this);
                    }
                    else {
                        setSelectionCircle(this);
                    }
                }
                else
                    setSelectionPoint(this);
            });
        }

        if (setMap_isInBound) {
            if (Bound_isMarkerIn(dati[kk])) {
                dati[kk].setMap(mappa.elemenotMappa);
            }
        } else {
            dati[kk].setMap(mappa.elemenotMappa);
        }

    }

}

function Bound_isMarkerIn(marker) {

    return mappa.elemenotMappa.getBounds().contains(marker.getPosition());

    //var rval = false;
    //try {
    //    rval = mappa.elemenotMappa.getBounds().contains(marker.getPosition());
    //} catch (e) {

    //}
    //return rval;
}


/********************************************************************/
/********************** Tool Selezione e Creazione  *****************/
/*******************************************************************/







/********************************************************************/
/********************** LABEL **************************************/
/*******************************************************************/

function catasto() {


    var ritorno;

    var listaEntita = EntitaCod_MultiSelezionate();

    if (listaEntita == "") {
        if (shape.selectedShape != null)
            listaEntita = shape.selectedShape.Entita_Cod;
    }

    var indirizzohttpCatasto = '';
    indirizzohttpCatasto = 'RipartoCatasto.aspx?entita=' + listaEntita;


    //utility.log(indirizzohttp);

    $("#frameCatasto").attr("src", indirizzohttpCatasto);
    $("#pop_up_Catasto").dialog("open");

}

function EntitaCod_MultiSelezionate() {

    var vApp = new Array();
    for (var i = 0; i < shape.selectedShapeArray.length; i++) {
        vApp.push(shape.selectedShapeArray[i].Entita_Cod);
    }

    return vApp.join(",");

}

function copiaIncollaLogThis(ChiaveAlbero) {
    //log    
    utility.log("copia/incolla chiave albero = " + ChiaveAlbero);


    if (shape.selectedShape_Copia == undefined) {
        utility.log("copia/incolla selected shape copiato = undefined ");
    }
    else {
        utility.log("copia/incolla selected shape copiato = " + shape.selectedShape_Copia.getPath().getArray().toString());
    }

    if (shape.selectedShape == undefined) {
        utility.log("copia/incolla selected shape attuale = undefined ");
    }
    else {
        utility.log("copia/incolla selected shape attuale = " + shape.selectedShape.getPath().getArray().toString());
    }
    //fine log
}

function copiaIncolla() {

    //gestione dei potenziali errori

    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString(); //.replace(/\\/g, '\\\\'); ;

    if (ChiaveAlbero == '') {

        //log
        copiaIncollaLogThis(ChiaveAlbero);
        alert("Nessun elemento selezionato.");
        return 1;
    }

    if (shape.selectedShape_Copia == undefined) {
        if (ChiaveAlbero != '' && shape.selectedShape == undefined) {

            //log
            copiaIncollaLogThis(ChiaveAlbero);
            alert("Nessun disegno per l'elemento selezionato.");
            return 1;
        }
    }

    //fine gestione potenziali errori


    if (shape.selectedShape_Copia == undefined) {

        //copia
        shape.selectedShape_Copia = shape.selectedShape;
        alert("Selezionare un elemento dall'albero di sinistra e fare di nuovo click sul pulsante per incollare.");
    }
    else {

        //incolla

        if (shape.selectedShape == undefined) {
            shape.selectedShape = shape.selectedShape_Copia;

            var MVCArray = shape.selectedShape.getPath();
            $('#hiddenPunti_Nuovo').val(MVCArray.getArray().toString());

            var area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);

            interfaccia.loading(true);
            salvataggioDirettoConAppezza("", shape.selectedShape.flag_gps); //' VAnni: 23/3/2017: non passo l'area perchè la funzione lato server re-imposta l'area sull'impianto e non deve...
            shape.selectedShape_Copia = undefined;
            shape.selectedShape = undefined;

            //LOG
            copiaIncollaLogThis(ChiaveAlbero);

        }
        else {
            //log
            copiaIncollaLogThis(ChiaveAlbero);
            alert("Esiste già un elemento grafico e quindi non è possibile incollare.");
        }


        //log
        copiaIncollaLogThis(ChiaveAlbero);
    }

}

function editPunti() {


    //gestione dei potenziali errori

    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString(); //.replace(/\\/g, '\\\\'); ;
    if (ChiaveAlbero == '' && CoordFromPoints != '') {
        alert("Nessun elemento selezionato.");
        return 1;
    }


    if (ChiaveAlbero != '' && CoordFromPoints == '' && shape.selectedShape == undefined) {
        alert("Nessun disegno per l'elemento selezionato.");
        return 1;
    }

    if (CoordFromPoints != '' && shape.selectedShape != undefined) {
        alert("Esiste già un disegno per l'elemento selezionato.");
        return 1;
    }
    //fine gestione dei potenziali errori


    if (CoordFromPoints != '') {
        CoordFromPoints = CoordFromPoints.substring(0, CoordFromPoints.length - 2);

        // (lat,long),(lat,long),(lat,long) --> length = 6
        var ll = CoordFromPoints.split(",").length;
        utility.log("--> length =" + ll);

        if (ll < 5) {
            alert("Selezionare almeno 3 vertici.");
            return 1;
        }

        $('#hiddenPunti_Nuovo').val(CoordFromPoints);
        $('#dialogAllertOperazioni_hidden').val("1");
        $('#dialogConfermaAssociazione').dialog("open");
        return 1;
    }




    interfaccia.loading(true);




    $.ajax({
        type: "POST",
        url: indirizzohttp + "/ScomponiPoligono",
        data: "{ ChiaveAlbero: '" + ChiaveAlbero + "', entita_cod: '" + shape.selectedShape.Entita_Cod + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                if (msg.d == "ok") {
                    interfaccia.loading(false);
                    AggiornaTutto();
                    $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val("");
                }
                else
                    alert("Si è verificato un problema durante la scomposizione. - " + msg.d);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });

}






/*******************************************************************/
/******************* PANNELLO DI SALVATAGGIO ***********************/
/*******************************************************************/

function AggiungiOptionInSelect(selectName, OptionValue, OptionText) {
    if (!EsisteOptionInSelect(selectName, OptionValue))
        $(selectName).append(new Option(OptionText, OptionValue, false, true));
}

function EsisteOptionInSelect(selectName, OptionValue) {
    return ($(selectName + "[value='" + OptionValue + "']").length > 0);
}

function DammiOptionValueInSelect(selectName) {
    return $(selectName).find(":selected").val();
}

function DammiOptionTextInSelect(selectName) {
    return $(selectName).find(":selected").text();
}

function ImpostaOptionInSelect(selectName, OptionValue) {
    $(selectName).val(OptionValue);
}

/******************* NUOVA CENTRO ************************/
/* funzione per la creazione di un nuovo centro  */
function InviaDatiNuovoCentro() {
    var MVCArray = shape.selectedShape.getPath();
    var coord = {
        "sLat": 0,
        "sLong": 0
    };

    getGLatLong(MVCArray, coord);


    var oggetto = {
        "piva": $('#pop_up_impianto_azienda').val(),
        "ragione_sociale": $('#txt_sa_nome').val(),
        "sLat": coord.sLat,
        "sLong": coord.sLong,
        "provincia": $('#ddl_Provincia_' + centro_ID_ind).val(),
        "comune": $('#ddl_Comune_' + centro_ID_ind).val(),
        "via": $('#txt_Via_' + centro_ID_ind).val(),
        "frazione": $('#txt_Frazione_' + centro_ID_ind).val(),
        "civico": $('#pop_up_finalita').val(),
        "stato": $('#txt_ISO_Stato_' + centro_ID_ind).val(),
        "note": $('#txt_Note_' + centro_ID_ind).val()
    };


    //se sono tutte valorizzate allora posso procedere
    var blocca = false;

    if (oggetto.piva == "" || oggetto.ragione_sociale == "") {
        blocca = true;
    }


    if (blocca) {
        alert("completare i campi obbligatori");
        return false;
    }

    interfaccia.loading(true);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/btn_SalvaNuoovoCentroAziendale_Click",
        data: "{ piva: '" + oggetto.piva + "', ragione_sociale: '" + oggetto.ragione_sociale + "', sLat: '" + oggetto.sLat + "', sLong: '" + oggetto.sLong + "', provincia: '" + oggetto.provincia + "', comune: '" + oggetto.comune + "', via: '" + oggetto.via + "', frazione: '" + oggetto.frazione + "', civico: '" + oggetto.civico + "', stato: '" + oggetto.stato + "', note: '" + oggetto.note + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {


            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                var sa_cod = msg.d;
                sa_cod = sa_cod.split('=')[1];

                $('#popup_nuovo_centro').dialog("close");

                var descriz = "";
                descriz = oggetto.ragione_sociale;

                if (isSementi)
                    descriz = descriz + " (Lat.: " + oggetto.sLat + " - Long.: " + oggetto.sLong + ")";

                AggiungiOptionInSelect('#pop_up_centro', sa_cod, descriz + '|' + oggetto.via + ' ' + oggetto.civico + ' ' + oggetto.comune);
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
            alert(xhr.status);
            alert(thrownError);
        }
    });

}

/******************* NUOVA AZIENDA ************************/

/* default */
function popup_nuova_azienda_open() {
    ImpostaAziendaPadreDefault();
}

function ImpostaAziendaPadreDefault() {
    interfaccia.loading(true);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/ImpostaAziendaPadreDefault",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                if (msg.d.toString() != '') {
                    var AziendaPadre = msg.d.split('|');
                    AggiungiOptionInSelect("#Cmb_Imprese_Padre", AziendaPadre[0], AziendaPadre[1]);
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
            alert(xhr.status);
            alert(thrownError);
        }
    });

}

/* funzione per la creazione di una nuova azienda  */
function InviaDatiNuovaAzienda() {
    var MVCArray = shape.selectedShape.getPath();
    var coord = {
        "sLat": 0,
        "sLong": 0
    };

    getGLatLong(MVCArray, coord);


    var oggetto = {
        "piva": $('#txt_Piva').val(),
        "codice_socio": $('#txt_CodiceSocio').val(),
        "piva_padre": $('#Cmb_Imprese_Padre').val(),
        "sLat": coord.sLat,
        "sLong": coord.sLong,
        "ragione_sociale": $('#txt_rag_Soc').val(),
        "provincia": $('#ddl_Provincia_' + imprese_ID_ind).val(),
        "comune": $('#ddl_Comune_' + imprese_ID_ind).val(),
        "via": $('#txt_Via_' + imprese_ID_ind).val(),
        "frazione": $('#txt_Frazione_' + imprese_ID_ind).val(),
        "civico": $('#pop_up_finalita').val(),
        "stato": $('#txt_ISO_Stato_' + imprese_ID_ind).val(),
        "note": $('#txt_Note_' + imprese_ID_ind).val()
    };


    //se sono tutte valorizzate allora posso procedere
    var blocca = false;

    if (oggetto.piva == "" || oggetto.ragione_sociale == "") {
        blocca = true;
    }

    if (blocca) {
        alert("completare i campi obbligatori");
        return false;
    }

    interfaccia.loading(true);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/btn_Salva_Azienda_Click",
        data: "{ piva: '" + oggetto.piva + "', Codice_Socio: '" + oggetto.codice_socio + "', piva_padre: '" + oggetto.piva_padre + "', ragione_sociale: '" + oggetto.ragione_sociale + "', sLat: '" + oggetto.sLat + "', sLong: '" + oggetto.sLong + "', provincia: '" + oggetto.provincia + "', comune: '" + oggetto.comune + "', via: '" + oggetto.via + "', frazione: '" + oggetto.frazione + "', civico: '" + oggetto.civico + "', stato: '" + oggetto.stato + "', note: '" + oggetto.note + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            interfaccia.loading(false);
            $('#popup_nuova_azienda').dialog("close");

            AggiungiOptionInSelect('#pop_up_impianto_azienda', oggetto.piva, oggetto.ragione_sociale);
            CaricaCentroAziendale();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
            alert(xhr.status);
            alert(thrownError);

        }
    });


}


function ImpostaPulsantiNuovoImpianto() {
    $.ajax({
        type: "POST",
        url: indirizzohttp + "/ImpostaPulsantiNuovoImpianto",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                if (msg.d != "True")
                    $('#righelloSpecie').html("");
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function CaricaDateDefaultDaVegCod(dataInizio, dataFine) {
    var veg_cod = $('#pop_up_specie').val();
    $.ajax({
        type: "POST",
        url: indirizzohttp + "/CaricaDateDefaultDaVegCod",
        data: "{Veg_Cod :'" + veg_cod + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                if (msg.d == '-1') { }
                else {

                    var DataInizioFine = msg.d.split('|');

                    $(dataInizio).val(DataInizioFine[0]);
                    $(dataFine).val(DataInizioFine[1]);
                    if ($(dataFine).val() == "") {
                        $(dataFine).removeAttr("disabled");
                    }
                }
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function CaricaDateDefault(dataInizio, dataFine) {
    $.ajax({
        type: "POST",
        url: indirizzohttp + "/CaricaDateDefault",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                if (msg.d == '-1') { }
                else {

                    var DataInizioFine = msg.d.split('|');

                    $(dataInizio).val(DataInizioFine[0]);
                    $(dataFine).val(DataInizioFine[1]);
                    if ($(dataFine).val() == "") {
                        $(dataFine).removeAttr("disabled");
                    }
                }
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

/******************* Carica AZIENDA ********************************/
/* funzione per il caricamento tramite ajax delle aziende */
function CaricaAzienda() {
    $.ajax({
        type: "POST",
        url: indirizzohttp + "/CaricaAzienda",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                $('#pop_up_impianto_azienda').html(msg.d);
                CaricaCentroAziendale();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

/********************* INDIRIZZI ***********************************/
/* nascondi marlker Indirizzo */
function nascondiAddress() {
    mappa.markerIndirizzo.setMap(null);
    mappa.markerIndirizzo = null;
}


function ricercaCoordinate() {
    if (mappa.markerIndirizzo != null) {
        nascondiAddress();
    }
    var lat = $('#lat_cerca').val().replace(",", ".");
    var long = $('#long_cerca').val().replace(",", ".");

    var ok = true;
    if (parseFloat(lat) == null)
        ok = false;

    if (parseFloat(long) == null)
        ok = false;

    if (ok == true) {
        mappa.elemenotMappa.setZoom(15)
        mappa.markerIndirizzo = new google.maps.Marker({
            map: mappa.elemenotMappa,
            position: new google.maps.LatLng(lat, long)
        });
        mappa.elemenotMappa.setCenter(new google.maps.LatLng(lat, long));
    } else {
        alert('i campi Lat e Long non contengono valori corretti');
    }
}


function getGLatLong(MVCArray, coord) {
    var mArray = MVCArray.getArray();
    var conta = 0;
    var len = mArray.length;
    for (conta = 0; conta < len; conta++) {
        coord.sLat = coord.sLat + mArray[conta].lat();
        coord.sLong = coord.sLong + mArray[conta].lng();
    }
    coord.sLat = coord.sLat / len;
    coord.sLong = coord.sLong / len;
}



function objToString(obj) {
    var str = '';
    for (var p in obj) {
        if (obj.hasOwnProperty(p)) {
            str += p + '::' + obj[p] + '\n';
        }
    }
    return str;
}





/******************* Carica CentroAziendale ************************/
/* funzione per il caricamento tramite ajax dei centri aziendali salvati su l'azienda selezionata */
function CaricaCentroAziendale() {
    var azienda_selezionata = $('#pop_up_impianto_azienda').val();
    if (azienda_selezionata != "") {
        $.ajax({
            type: "POST",
            url: indirizzohttp + "/CaricaCentroAziendale",
            data: "{Piva: '" + azienda_selezionata + "', isSementi: '" + isSementi.toString() + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {

                if (msg.d == 'SessioneScaduta') {
                    $('#dialogSessioneScaduta').dialog("open");
                }
                else {
                    $('#pop_up_centro').html(msg.d);

                    if ($(".ddl_Sa_Cod_sx").length > 0) {

                        $('#pop_up_centro').val($('.ddl_Aziende').val() + "|" + $('.ddl_Sa_Cod_sx').val());
                    }
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
}

/******************* Carica Specie ************************/
/* funzione per il caricamento tramite ajax delle specie  */
function CaricaSpecie(combo_specie, veg_cod) {
    $.ajax({
        type: "POST",
        url: indirizzohttp + "/CaricaSpecie",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                $(combo_specie).html(msg.d);
                if (veg_cod != '') {
                    $(combo_specie).val(veg_cod)
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

/******************* Carica Tipologia ************************/
/* funzione per il caricamento tramite ajax delle specie  */
function CaricaTipologia(Veg_Cod, combo_tipologia, valoreSelezionato) {
    $.ajax({
        type: "POST",
        url: indirizzohttp + "/CaricaTipologia",
        data: "{ Veg_Cod: '" + Veg_Cod + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            r_ok();
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                $(combo_tipologia).html(msg.d);
            }
            if (valoreSelezionato != "")
                $(combo_tipologia).val(valoreSelezionato);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

/******************* Carica Varieta ************************/
/* funzione per il caricamento tramite ajax delle specie  */
function CaricaVarieta(combo_varieta, cul_cod) {
    var Veg_Cod;

    if (combo_varieta == '#pop_up_varieta') {
        Veg_Cod = $('#pop_up_specie').val();
    } else {
        //        pop_up_varieta_modifica
        Veg_Cod = $('#pop_up_specie_modifica').val();
    }

    utility.log("Veg_CodVerieta :" + Veg_Cod);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/CaricaVarieta",
        data: "{ Veg_Cod: '" + Veg_Cod + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            r_ok();
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                //alert(msg.d);
                $(combo_varieta).html(msg.d);
                if (cul_cod != '') {
                    $(combo_varieta).val(cul_cod)
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function r_ok() {
    response_ok = response_ok - 1;
    if (response_ok == 0) {
        interfaccia.loading(false);
    }
}

/******************* Carica Disciplinare ************************/
/* funzione per il caricamento tramite ajax delle specie  */
function CaricaDisciplinare() {
    var Veg_Cod = $('#pop_up_specie').val();
    interfaccia.loading(true);
    $.ajax({
        type: "POST",
        url: indirizzohttp + "/CaricaDisciplinare",
        data: "{ Veg_Cod: '" + Veg_Cod + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            r_ok();
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                $('#pop_up_disciplinare').html(msg.d);

            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

/******************* Carica Finalita ************************/
/* funzione per il caricamento tramite ajax delle specie  */
function CaricaFinalita(combo_finalita, finalita) {
    if (combo_finalita == '#pop_up_finalita') {
        var Veg_Cod = $('#pop_up_specie').val();
    } else {
        //        pop_up_varieta_modifica
        var Veg_Cod = $('#pop_up_specie_modifica').val();
    }

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/CaricaFinalita",
        data: "{ Veg_Cod: '" + Veg_Cod + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            r_ok();
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                $(combo_finalita).html(msg.d);
                if (finalita != '') {
                    $(combo_finalita).val(finalita)
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}


/******************* NUOVO IMPIANTO ************************/

/* verifica della correttezza di un poligono */
function VerificaPoligono(Poligono, EliminaPoligono) {
    interfaccia.loading(true);
    var bTestPoligono = false;
    $.ajax({
        async: false,
        type: "POST",
        url: indirizzohttp + "/VerificaPoligono",
        data: "{ Poligono: '" + Poligono + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                if (msg.d != 'Ok') {
                    if (EliminaPoligono) {
                        if (shape.selectedShape != undefined)
                            shape.selectedShape.setMap(null);
                    }
                }
                else {
                    bTestPoligono = true;
                }

            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });

    return bTestPoligono;
}


//Chiede via WS il codice Fiscale Tecnico..
function getCodiceFiscaleTecnico() {

    interfaccia.loading(true);
    ajaxAgronicaSync(indirizzohttp + "/getCodiceFiscaleTecnico", null, false,
        function (risposta) {

            Codice_Fiscale_Tecnico = risposta.RispostaStringa;

        }, null);


    interfaccia.loading(false);

}


function PredisponiPerSalvataggio_Lotto(isModifica) {

    if (Codice_Fiscale_Tecnico == '13171470159') {

        var appOriginale = '#pop_up_app_originale';
        var nAppezza = '#pop_up_app_n_appezza';
        var nLotto = '#pop_up_lotto';

        if (isModifica) {
            appOriginale = '#pop_up_m_app_originale';
            nAppezza = '#pop_up_app_m_n_appezza';
            nLotto = "#pop_up_m_lotto";
        }

        $(nLotto).val(
            $(appOriginale).val().toString().replace(/-/g, "") + '-' +
            $(nAppezza).val().toString().replace(/-/g, "")
        );
    }
}


function SementiMappaturaLiberaSpecieVegetalePermessa(veg_cod) {
    let result = true;
    if (veg_cod !== undefined && veg_cod !== "") {

        $.ajax({
            type: "POST",
            async: false,
            url: indirizzohttp + "/SementiMappaturaLiberaSpecieVegetalePermessa",
            data: "{ veg_cod: '" + veg_cod + "' }",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                result = (msg.d === "true");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
                result = false;
            }
        });

    }
    return result;
}

/* funzione per la creazione di un nuvo impianto  */
function InviaDatiNuovoImpianto() {

    let veg_cod = $('#pop_up_specie').val();
    if (!SementiMappaturaLiberaSpecieVegetalePermessa(veg_cod)) {
        alert("Specie vegetale soggetta a sportello...\r\nSalvataggio non consentito!");
        return false;
    }

    PredisponiPerSalvataggio_Lotto(false);

    interfaccia.loading(true);
    var hiddenPunti_Nuovo = $('#hiddenPunti_Nuovo').val();
    var oggetto = {
        "piva": $('#pop_up_impianto_azienda').val(),
        "rag_soc": $('#pop_up_impianto_azienda').find(":selected").text(),
        "sa_cod": $('#pop_up_centro').val(),
        "veg_cod": $('#pop_up_specie').val(),
        "sup_imp": $('#pop_up_sup_app').val(),
        "data_inizio": $('#pop_up_data_inizio').val(),
        "data_fine": $('#pop_up_data_fine').val(),
        "varieta": $('#pop_up_varieta').val(),
        "finalita": $('#pop_up_finalita').val(),
        "disciplinare": $('#pop_up_disciplinare').val(),
        "tipologia": $('#pop_up_tipologia').val(),
        "nome": $('#pop_up_nome').val(),
        "lotto": $('#pop_up_lotto').val(),
        "data_semina": $('#pop_up_d_semina').val(),
        "data_raccolta": $('#pop_up_d_raccolta').val(),
        "via_stringa": $('#stringaIndirizzo').val(),
        "hiddenPunti_Nuovo": hiddenPunti_Nuovo
    };

    if (oggetto.finalita == null) {
        oggetto.finalita = "";
    }
    if (oggetto.disciplinare == null) {
        oggetto.disciplinare = "";
    }
    if (oggetto.tipologia == null) {
        oggetto.tipologia = "";
    }


    //se sono tutte valorizzate allora posso procedere
    var blocca = false;



    if (shape.glayerDoveDisegnoSuTipologiaStandard != "19") {

        if (oggetto.piva == "" || oggetto.sa_cod == "") {
            blocca = true;
        }

    } else {


        if (oggetto.piva == "" || oggetto.sa_cod == "" || oggetto.veg_cod == "" || oggetto.sup_imp == "") {
            blocca = true;
        }

        if (isSementi == true && (oggetto.data_inizio == "" || oggetto.data_fine == "" || oggetto.tipologia == "")) {
            blocca = true;
        }

        // controllo che nono siano nulle
        if (oggetto.piva == null || oggetto.sa_cod == null || oggetto.veg_cod == null || oggetto.sup_imp == null) {
            alert("completare i null");
            return false;
        }

    }

    if (blocca) {
        alert("completare i campi obbligatori");
        interfaccia.loading(false);
        return false;
    }

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/SalvaNuovoImpianto",
        data: "{ piva: '" + oggetto.piva + "', sa_cod: '" + oggetto.sa_cod + "', veg_cod: '" + oggetto.veg_cod + "', sup_imp: '" + oggetto.sup_imp + "', piva: '" + oggetto.piva + "', data_inizio: '" + oggetto.data_inizio + "', data_fine: '" + oggetto.data_fine + "', varieta: '" + oggetto.varieta + "', finalita: '" + oggetto.finalita + "', disciplinare: '" + oggetto.disciplinare + "', tipologia: '" + oggetto.tipologia + "', nome: '" + oggetto.nome + "', lotto: '" + oggetto.lotto + "', data_semina: '" + oggetto.data_semina + "', data_raccolta: '" + oggetto.data_raccolta + "', via_stringa: '" + oggetto.via_stringa.replace("'", "`") + "', hiddenPunti_Nuovo: '" + oggetto.hiddenPunti_Nuovo + "', layer_cod: '" + shape.glayerDoveDisegnoSuTipologiaStandard + "', elementografico_des: '" + $("#txtElementoGrafico_Des").val() + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            $('#responseInterferenze').width('0px');
            $('#responseInterferenze').html('');
            $("#pop_up_impianto").dialog("close");

            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {

                utility.log('SalvaNuovoImpianto, esito = ' + msg.d);

                //'  Vanni, 05/06/2014 09:42:52: verifico se e come ricaricare...
                //RicaricaAziende(oggetto.piva, oggetto.rag_soc);

                var app = msg.d.split(".");
                if (app.length > 1) {
                    RicaricaSituazioneMappa(app[1], "", oggetto);
                }



            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function RicaricaSituazioneMappa(id, chiavealbero, oggetto) {
    var ricaricaNecessario = !stringhe.contains(layerNonAlbero, shape.glayerDoveDisegnoSuTipologiaStandard);

    if (ricaricaNecessario) {

        utility.log("Ricarica necessario");

        if (shape.selectedShape != undefined)
            shape.selectedShape.setMap(null);

        RicaricaAziende(oggetto.piva, oggetto.rag_soc, true);

        //' VAnni: 7/3/2017: Forza esecuzione del click
        //$("#AggiornaFiltro_Client").click();
    }
    else {
        utility.log("Ricarica non necessario");
        GestisciShapeAppenaCreatoClient(id, chiavealbero);
    }
}



function GestisciShapeAppenaCreatoClient(id, chiavealbero) {

    var ids = id.split(",");

    utility.log("GestisciShapeAppenaCreatoClient()");
    utility.log(shape.selectedShape);

    var col = "000000"
    var circle_radius = 100;
    for (var i = 0; i < ids.length; i++) {

        var lat_centro = 0;
        var lng_centro = 0;

        var p_place = place[i];

        var l_place_vertici = p_place.vertici.length;
        for (var j = 0; j < l_place_vertici; j++) {
            var l_at = p_place.vertici[j].lat;
            var l_ong = p_place.vertici[j].long;
            lat_centro += parseFloat(l_at);
            lng_centro += parseFloat(l_ong);
        }

        lat_centro = lat_centro / j;
        lng_centro = lng_centro / j;

        pushNewPlace(ids[i], chiavealbero);

        ShapeAggiungiOggetti(place[place.length - 1], placeLivelli[shape.glayerDoveDisegnoSuTipologiaStandard], col, shape.selectedShape.getPath(), lat_centro, lng_centro, circle_radius);
        shape.selectedShape.set('fillColor', "#" + col);

        //' VAnni: 3/3/2017: i punti saranno gestiti in seguito..
        if (p_place.TipologiaGML != 'Point') {
            shape.selectedShape.setMap(mappa.elemenotMappa);
        }


        setSelection(shape.selectedShape);
    }

    mappa.GestioneCluster();


}

function pushNewPlace(id, chiavealbero) {

    utility.log("pushNewPlace();");

    var entita_cod = id.split("|")[1];

    place.push({
        layer: shape.glayerDoveDisegnoSuTipologiaStandard,
        id: id,
        tipoicona: "",
        zindex: 10000,
        Entita_Cod: entita_cod,
        veg_cod: -1,
        inserimento: true,
        flag_gps: 0,
        modifica: true,
        cancellazione: true,
        informazioni: "",
        chiavealbero: chiavealbero,
        Testo: $("#txtElementoGrafico_Des").val(),
        AppIdRate: " ",
        TipologiaGML: "Polygon",
        vertici: shape.selectedShape.getPath().getArray()
    });
}



function ConfermaGestioneColoriLayer() {
    //alert('ConfermaGestioneColoriLayer');
    var tabella = $('#place_tabella table tr');

    var stringa_dati = "";
    var conta = 0;
    $('#place_tabella table tr').each(function () {
        if (conta == 0) { //salto la prima riga
            conta = 1;
        }
        else {
            var id = $(this).find(".chiave").val();
            var primario = $(this).find(".primario").val();
            var secondario = $(this).find(".secondario").val();
            var varianza = $(this).find(".varianza").val();
            var trasparenza = $(this).find(".trasparenza").val();
            var zindex = $(this).find(".zindex").val();
            var visible = $(this).find(".flag_visibile").is(":checked");
            var mostraDescrizioneAssociata = $(this).find(".MostraDescrizioneAssociata").is(":checked");
            var tipologiaLayer = $("#ddlTipologiaLayer").val();
            if (primario == null)
                primario = "";
            if (secondario == null)
                secondario = "";
            if (varianza == null)
                varianza = "";
            if (zindex == null)
                zindex = "";
            if (trasparenza == null || trasparenza === undefined || trasparenza == "") {
                stringa_dati = stringa_dati + id + "|" + primario + "|" + secondario + "|" + varianza + "|" + zindex + "|" + visible + "|" + tipologiaLayer + "$";
            } else {
                if (mostraDescrizioneAssociata == null || mostraDescrizioneAssociata === undefined || mostraDescrizioneAssociata == "") {
                    stringa_dati = stringa_dati + id + "|" + primario + "|" + secondario + "|" + varianza + "|" + trasparenza + "|" + zindex + "|" + visible + "|" + tipologiaLayer + "$";
                } else {
                    stringa_dati = stringa_dati + id + "|" + primario + "|" + secondario + "|" + varianza + "|" + trasparenza + "|" + mostraDescrizioneAssociata +  "|" + zindex + "|" + visible + "|" + tipologiaLayer + "$";

                }
            }

        }
    });
    var hiddenPrincipale_1_Dettagli_2 = $('#hiddenPrincipale_1_Dettagli_2').val();
    $.ajax({
        type: "POST",
        url: indirizzohttp + "/SalvaColoriLayer",
        data: "{ Tipologia:'" + hiddenPrincipale_1_Dettagli_2 + "', Dati: '" + stringa_dati + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            interfaccia.loading(false);

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                alert("Colori Impostati, verrà ricaricata la pagina");
                location.href = indirizzohttp;
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });

}


function ConfermaBufferZoneIntersection() {

}


function ConfermaSalvataggioAB() {
    interfaccia.loading(true);

    var hiddenPunti_A = '(' + $('#hiddenA').val() + ')';
    var hiddenPunti_B = '(' + $('#hiddenB').val() + ')';
    var hiddenPunti_AB = $('#hiddenCurrentSelectedAB').val();

    $('#hiddenCurrentSelectedAB').val('');
    $('#hiddenA').val('');
    $('#hiddenB').val('');

    //    utility.log('A = ' + hiddenPunti_A);
    //    utility.log('B = ' + hiddenPunti_B);
    //    utility.log('AB = ' + hiddenPunti_AB);

    //se sono tutte valorizzate allora posso procedere
    var blocca = false;

    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString(); //.replace(/\\/g, '\\\\'); ;


    //utility.log(ChiaveAlbero);

    //se sono tutte valorizzate allora posso procedere
    if (ChiaveAlbero == "") {
        alert("nessun impianto o pianificazione selezionata.");
        return false;
    }

    if (blocca) {
        alert("completare i campi obbligatori");
        return false;
    }

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/SalvaNuovoAB",
        data: "{ chiaveAlbero: '" + ChiaveAlbero + "', hiddenPunti_A: '" + hiddenPunti_A + "', hiddenPunti_B: '" + hiddenPunti_B + "', hiddenPunti_AB: '" + hiddenPunti_AB + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                //utility.log(msg.d);
                if (msg.d == 'Ok') {
                    $("#dialogAB").dialog("close");
                    AggiornaTutto();
                }
                else {
                    alert(msg.d);
                }
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

//function ImpostaAziendaAppenaSalvataSuCombo(piva, rag_soc) {

////    if (DammiOptionValueInSelect("#ddl_Aziende") != piva) {
////        $("#ddl_Aziende_toAdd").val(piva + '|' + rag_soc);
////        setTimeout('__doPostBack(\'ddl_Aziende\',\'\')', 0);
////    }

//}



/******************* ELIMINA ************************/

function EliminaImpiantoPuntiScomposti() {

    if ($("#ckConfermaEliminaPuntiScomposti").is(':checked') == false) {
        alert("per procedere spuntare prima di confermare, altrimenti fare click su annulla.");
        return false;
    }

    var piva = $('#ddl_azienda_html').val();
    var sa_cod = $('#ddl_Sa_Cod_html').val();

    interfaccia.loading(true);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/EliminaImpiantoPuntiScomposti",
        data: JSON.stringify({ piva: piva, sa_cod: sa_cod }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            $("#dialogEliminaImpiantoPuntiScomposti").dialog("close");
            $("#dialogScomponiPunti").dialog("close");

            interfaccia.loading(false);


            if (msg.d == "SessioneScaduta") {
                $('#dialogSessioneScaduta').dialog("open");
            } else {
                if (msg.d != "ok")
                    alert(msg.d);
                else
                    alert("Operezione eseguita correttamente.");
            }

            if ($('#AggiornaFiltro').length > 0) {
                $('#AggiornaFiltro').click();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });

}

function resettaCheckElimina() {


    $("#ckEliminaGrafica").removeAttr('checked');
    $("#ckEliminaPrecision").removeAttr('checked');
    $("#ckEliminaPrecisionABLine").removeAttr('checked');
    $("#ckEliminaImpianto").removeAttr('checked');
    $("#ckEliminaDatoPalm").removeAttr('checked');
    $("#ckConfermaElimina").removeAttr('checked');

}

function dialogEliminaImpiantoOnOpen() {

    var Entita_Cod = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();

    utility.log("dialogEliminaImpiantoOnOpen = " + Entita_Cod);

    resettaCheckElimina();

    //verifica se posso eliminare i planning.. lo faccio se: non sono sementi, si tratta di un planning.
    if (!isSementi && Entita_Cod != undefined) {
        if (stringhe.startsWith(Entita_Cod, chiaveAlberoPlanning)) {
            interfaccia.loading(true);

            $.ajax({
                type: "POST",
                url: indirizzohttp + "/VerificaCancella",
                data: "{ Entita_Cod: '" + Entita_Cod + "' }",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {

                    interfaccia.loading(false);

                    if (msg.d == "SessioneScaduta") {
                        $('#dialogSessioneScaduta').dialog("open");
                    } else {
                        if (msg.d == "true") {
                            $("#divEliminaEntitaGIAS").show();
                        }
                        else {
                            $("#divEliminaEntitaGIAS").hide();

                        }
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }

}


//#region cancellazione

/* funzione per l eliminazione di un impianto esistente */
function EliminaMultipointSelezionati() {

    if ($("#ckConfermaEliminaMultipoint").is(':checked') == false) {
        alert("per procedere spuntare prima di confermare, altrimenti fare click su annulla.");
        return false;
    }

    interfaccia.loading(true);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/CancellaMultipoint",
        data: "{ hiddenPunti_M: '" + $("#hiddenMultipointModifica").val() + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            interfaccia.loading(false);
            $("#dialogEliminaMultipoint").dialog('close');
            $("#hiddenMultipointModifica").val('');


            if (msg.d.Esito == true) {
                alert(msg.d.StringaRisposta);
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                alert(msg.d.StringaRisposta);
                if (shape.selectedShape != undefined)
                    shape.selectedShape.setMap(null);
            }

            if ($('#AggiornaFiltro').length > 0) {
                $('#AggiornaFiltro').click();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });


}


/* funzione per l eliminazione di un impianto esistente */
function EliminaImpianto() {
    $('#hiddenID').val(shape.selectedShape.html);

    var eliminaImpinato = $('#ckEliminaImpianto').is(':checked');
    var eliminaGrafica = $('#ckEliminaGrafica').is(':checked');
    var EliminaDatoGiasPalm = $('#ckEliminaDatoPalm').is(':checked');
    var EliminaPrecisionFarming = $('#ckEliminaPrecision').is(':checked');
    var EliminaPrecisionFarmingABLine = $('#ckEliminaPrecisionABLine').is(':checked');

    var pivasuperuser = $('#hiddenID').val().split("|")[0];
    var Entita_Cod = $('#hiddenID').val().split("|")[1];

    //se sono tutte valorizzate allora posso procedere
    if (pivasuperuser == "" || Entita_Cod == "") {
        alert("nessun poligono selezionato.");
        return false;
    }

    if ($("#ckConfermaElimina").is(':checked') == false) {
        alert("per procedere spuntare prima di confermare, altrimenti fare click su annulla.");
        return false;
    }

    interfaccia.loading(true);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/Cancella",
        data: "{ entita_cod: '" + Entita_Cod + "', EliminaGrafica: '" + eliminaGrafica + "', EliminaImpianto: '" + eliminaImpinato + "', EliminaPrecisionFarming: '" + EliminaPrecisionFarming + "', EliminaPrecisionFarmingABLine: '" + EliminaPrecisionFarmingABLine + "', EliminaDatoGiasPalm: '" + EliminaDatoGiasPalm + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            $("#dialogEliminaImpianto").dialog("close");
            interfaccia.loading(false);

            if (msg.d.Esito == true) {
                alert(msg.d.StringaRisposta);
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                alert(msg.d.StringaRisposta);
                if (shape.selectedShape != undefined)
                    shape.selectedShape.setMap(null);
            }

            if ($('#AggiornaFiltro').length > 0) {
                $('#AggiornaFiltro').click();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });

}

function isNuovoElemento() {

    return mappa.isNuovoElemento();

}


/******************* MODIFICA IMPIANTO ************************/
/* funzione per la modifica di un impianto esistente */
function ModificaImpianto() {

    let veg_cod = $('#pop_up_specie_modifica').val();
    if (!SementiMappaturaLiberaSpecieVegetalePermessa(veg_cod)) {
        alert("Specie vegetale soggetta a sportello...\r\nSalvataggio non consentito!");
        return false;
    }

    interfaccia.loading(true);

    PredisponiPerSalvataggio_Lotto(true);

    var pivasuperuser = $('#hiddenID').val().split("|")[0];
    var Entita_Cod = $('#hiddenID').val().split("|")[1];

    var nomeAppezzamento = $('#pop_up_nome_appezza_modifica').val();
    var specie = $('#pop_up_specie_modifica').val();
    var varieta = $('#pop_up_varieta_modifica').val();
    var finalita = $('#pop_up_finalita_modifica').val();
    var tipologia = $('#pop_up_tipologia_modifica').val();
    if (tipologia == null)
        tipologia = "0";
    var sup_google = $('#pop_up_sup_google_modifica').val();
    var sup_impianto = $('#pop_up_sup_app_modifica').val();
    var hiddenPunti_modifica = $('#hiddenPunti_modifica').val();
    var mData_Inizio = $('#pop_up_m_data_inizio').val();
    var mData_Fine = $('#pop_up_m_data_fine').val();
    var stop = false;

    //se sono tutte valorizzate allora posso procedere
    if (pivasuperuser == "" || Entita_Cod == "" || specie == "" || varieta == "" || finalita == "" || tipologia == "" || sup_google == "" || sup_impianto == "" || hiddenPunti_modifica == "") {
        stop = true;
    }
    if (isSementi && (mData_Inizio == "" || mData_Fine == "")) {
        stop = true;
    }

    if (stop) {
        alert("Completare i dati obbligatori.");
        interfaccia.loading(false);
        return false;
    }

    // controllo se devo associare la superficie a impianto o appezzamento
    if ($('#opt_associa_modifica').val() == '1') {
        //posso fare il semplice salvataggio dei dati senza modificare la superficie
        ModificaDatiImpianto();
    }
    else {
        var MVCArray = shape.selectedShape.getPath();
        var area = sup_impianto;

        interfaccia.loading(false);

        //        var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString().replace(/\\/g, '\\\\'); ;
        var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();
        if ($('#opt_associa_modifica').val() == '2') {
            //voglio associare la superficie solamente all'impianto
            // devo controllare se ci sono delle operazioni registrate su questo impianto
            $.ajax({
                type: "POST",
                url: indirizzohttp + "/ControllaSeEsistonoOperazioni",
                data: "{ ChiaveAlbero: '" + ChiaveAlbero + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    if (msg.d == "SessioneScaduta") {
                        $('#dialogSessioneScaduta').dialog("open");
                    } else {
                        if (msg.d == "true") {
                            //non ho alcuna operazione, posso procedere con la modifica della superficie
                            ModificaDatiImpianto();

                            //vanni, 16/01/2015, test senza chiamata a salvataggio diretto                            
                            //salvataggioDiretto(area);

                            $('#pop_up_modificaImpianto').dialog('close');
                        } else {
                            //attenzione
                            $('#lbl_alert_operazioni').html(msg.d);
                            $('#dialogAllertOperazioni_hidden').val(2);
                            $('#dialogAllertOperazioni').dialog("open");
                        }
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
        else {
            //voglio associare la superficie a impianto e appezzamento
            //devo controllare se ci sono operazione registrate sull'impianto e se l'appezzamento ha collegato il catasto
            $.ajax({
                type: "POST",
                url: indirizzohttp + "/ControllaSeEsisteCatasto",
                data: "{ ChiaveAlbero: '" + ChiaveAlbero + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    if (msg.d == "SessioneScaduta") {
                        $('#dialogSessioneScaduta').dialog("open");
                    } else {
                        if (msg.d == "true") {
                            alert("L'appezzamento è collegato con il catasto, in questo caso la modifica della superficie è da effettuare a mano. Andare sull'anagrafica dell'appezzamento se si vuole variare la superficie dell'appezzamento..");
                            $('#opt_associa_modifica').val(2);
                        } else {
                            // se non ho il catasto collegato controllo se esistono operazioni 
                            $.ajax({
                                type: "POST",
                                url: indirizzohttp + "/ControllaSeEsistonoOperazioni",
                                data: "{ ChiaveAlbero: '" + ChiaveAlbero + "'}",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (msg) {
                                    if (msg.d == "SessioneScaduta") {
                                        $('#dialogSessioneScaduta').dialog("open");
                                    } else {
                                        if (msg.d == "true") {
                                            //non ho alcuna operazione, posso procedere con la modifica della superficie

                                            //vanni, 16/01/2015, test senza chiamata a salvataggio diretto, commentata chiamata a ModificaDatiImpianto
                                            if (isNuovoElemento()) {
                                                area = salvataggioDirettoConAppezzaAreaCalcola(area);
                                                salvataggioDirettoConAppezza(area);
                                            }
                                            else {
                                                ModificaDatiImpianto();
                                            }


                                            $('#pop_up_modificaImpianto').dialog('close');
                                        } else {
                                            //attenzione
                                            $('#lbl_alert_operazioni_Appezza').html(msg.d);
                                            $('#dialogAllertOperazioni_hidden').val(2);
                                            $('#dialogAllertOperazioniAppezza').dialog("open");
                                        }
                                    }
                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    alert(xhr.status);
                                    alert(thrownError);
                                }
                            });
                        }
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }

}

function ModificaDatiImpianto() {
    interfaccia.loading(true);
    var pivasuperuser = $('#hiddenID').val().split("|")[0];
    var Entita_Cod = $('#hiddenID').val().split("|")[1];

    var via_stringa = $('#via_stringa_modifica').val();

    var nome_appezza = $('#pop_up_nome_appezza_modifica').val();
    var specie = $('#pop_up_specie_modifica').val();
    var varieta = $('#pop_up_varieta_modifica').val();
    var finalita = $('#pop_up_finalita_modifica').val();
    var tipologia = $('#pop_up_tipologia_modifica').val();
    var lotto = $('#pop_up_m_lotto').val();
    if (tipologia == null)
        tipologia = "0";
    var sup_google = $('#pop_up_sup_google_modifica').val();

    var sup_impianto;
    if ($('#opt_associa_modifica').val() == 1) {
        sup_impianto = 0;
    }
    else {
        sup_impianto = $('#pop_up_sup_app_modifica').val();
    }

    var hiddenPunti_modifica = $('#hiddenPunti_modifica').val();

    var mData_Inizio = $('#pop_up_m_data_inizio').val();
    var mData_Fine = $('#pop_up_m_data_fine').val();
    var codice_socio = $('#txt_m_CodiceSocio').val();
    var stop = false;

    //se sono tutte valorizzate allora posso procedere
    if (pivasuperuser == "" || Entita_Cod == "" || specie == "" || varieta == "" || finalita == "" || sup_google == "" || hiddenPunti_modifica == "") {
        stop = true;
    }

    if (isSementi && (mData_Inizio == "" || mData_Fine == "")) {
        stop = true;
        alert("2");
    }

    if (stop) {
        alert("Completare i dati obbligatori.");
        interfaccia.loading(false);
        return false;
    }

    //Vanni, 16/01/2017 16:09:24: ricondotto a chiamata standard ...
    ajaxAgronica(indirizzohttp + "/ModificaImpianto",
        "{ entita_cod: '" + Entita_Cod + "', nome_appezza: '" + nome_appezza + "', sup_imp: '" + sup_impianto + "', data_inizio: '" + mData_Inizio + "', data_fine: '" + mData_Fine + "', specie: '" + specie + "', varieta: '" + varieta + "', finalita: '" + finalita + "', tipologia: '" + tipologia + "' , hiddenPunti_modifica: '" + hiddenPunti_modifica + "' , via_stringa: '" + via_stringa + "' , lotto: '" + lotto + "' , codice_socio: '" + codice_socio + "' }",
        function (risposta) {

            interfaccia.loading(false);


            if (risposta.RispostaOK) {

                alert(risposta.RispostaStringa);

                //16/01/2015, ripulisco la selezione.
                $('#hiddenPunti_modifica').val("");

                AggiornaTutto();
                $("#pop_up_modificaImpianto").dialog("close");

            } else {

                alert(risposta.Errore);

            }

        }, null);

    //$.ajax({
    //    type: "POST",
    //    url: indirizzohttp + "/ModificaImpianto",
    //    data: "{ entita_cod: '" + Entita_Cod + "', nome_appezza: '" + nome_appezza + "', sup_imp: '" + sup_impianto + "', data_inizio: '" + mData_Inizio + "', data_fine: '" + mData_Fine + "', specie: '" + specie + "', varieta: '" + varieta + "', finalita: '" + finalita + "', tipologia: '" + tipologia + "' , hiddenPunti_modifica: '" + hiddenPunti_modifica + "' , via_stringa: '" + via_stringa + "' , lotto: '" + lotto + "' , codice_socio: '" + codice_socio + "' }",
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    success: function (msg) {

    //        if (msg.d == 'SessioneScaduta') {
    //            $("#pop_up_modificaImpianto").dialog("close");
    //            interfaccia.loading(false);
    //            $('#dialogSessioneScaduta').dialog("open");
    //        }
    //        else {
    //            alert("salvataggio avvenuto con successo");

    //            //16/01/2015, ripulisco la selezione.
    //            $('#hiddenPunti_modifica').val("");

    //            AggiornaTutto();
    //            $("#pop_up_modificaImpianto").dialog("close");
    //        }
    //    },
    //    error: function (xhr, ajaxOptions, thrownError) {
    //        alert(xhr.status);
    //        alert(thrownError);
    //    }
    //});
}

//per chiamata Standard Ajax
function ChiamataAjax() {
    ajaxAgronica("/url", JSON.stringify({ Parametro: "Parametro" }),
        function (risposta) {
            console.log(risposta);
            alert(risposta);
        }, null);
}

function AggiornaTutto() {

    CoordFromPoints = "";
    CoordFromPointsMVCArray = new google.maps.MVCArray();

    AggiornaLayer();
    Inizializza();
    redimAlbero();
    GestisciStrumentoScomponiRicomponi();
}

function GestisciStrumentoScomponiRicomponi() {

    var piva = $('#ddl_azienda_html').val();
    var sa_cod = $('#ddl_Sa_Cod_html').val();

    //mantenere questo if per retro-compatibilità su progetto AgronicaSementi_5
    if (piva === undefined || sa_cod === undefined) {
        return;
    }

    ajaxAgronica(
        indirizzohttp + "/GestisciStrumentoScomponiRicomponi",
        JSON.stringify({ piva: piva, sa_cod: sa_cod }),
        function (risposta) {

            if (risposta.RispostaStringa.Ricarica) {

                alert("Lo strumento di scomposizione ha attivato i layer necessari, sarà ricaricata la pagina.");
                window.location.href = indirizzohttp;

            } else {

                if (risposta.RispostaStringa.Mostra) {
                    $("#dialogScomponiPunti").dialog("open");
                }

            }

        }, null);
}


/*******************************************************************/
/********************* SHAPE ***************************************/
/*******************************************************************/
/* tolgo l'editing sullo shape */
function clearSelection() {
    if (shape.selectedShape) {

        bAbilitaRenderPerEventoDrag = true;

        utility.warn("clearSelection()");

        shape.selectedShape.set('fillColor', shape.selectedShape.colore_precedente);
        shape.selectedShape.setEditable(false);

        shape.selectedShape = null;
        $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val('');

        $('.jstree-clicked').removeClass('jstree-clicked');
        if (gNuovo)
            $('#disenga_poligono').show();

        //nascondo se c'è la popup info
        mappa.NascondiInfo();
    }
}

function clearSelectionArray() {

    if (shape.selectedShapeArray) {

        utility.warn("clearSelectionArray()");

        for (i = 0; i < shape.selectedShapeArray.length; i++) {
            var ss = shape.selectedShapeArray[i];
            ss.set('fillColor', ss.colore_precedente);
            ss.setEditable(false);
        }

        shape.selectedShapeArray = new Array();
        $("#ChiaveAlberoMultiSelezione").val("");
        ShapeAddedd = false;
    }
}

function clearSelectionMultipoint() {
    $("#hiddenMultipoint").val("");
    $("#hiddenMultipointModifica").val("");
}

function clearSelectionBoth() {
    utility.warn("clearSelectionBoth()");
    clearSelection();
    clearSelectionArray();
    clearSelectionMultipoint();
}




/* Evento di Selezione di un ab-line */
function setSelectionAB(shapeS) {
    $('#hiddenCurrentSelectedAB').val(shapeS.html);
    $("#chkAB").prop("checked", "checked");
    addAB($("#chkAB"), $('#hiddenA'), $('#hiddenB'), null, null, null, null);
    return;
}

function getBounds(obj) {
    var bounds = new google.maps.LatLngBounds();
    var paths = obj.getPaths();
    var path;
    for (var p = 0; p < paths.getLength(); p++) {
        path = paths.getAt(p);
        for (var i = 0; i < path.getLength(); i++) {
            bounds.extend(path.getAt(i));
        }
    }
    return bounds;
}

/* Evento di Selezione di un punto */
function setSelectionPointMouseUp(shapeS) {

    utility.log('setSelectionPointMouseUp: ' + shapeS.html);
    utility.log('lat: ' + shapeS.getPosition());

    addMarkerMultiPointModifica(shapeS.html, shapeS.getPosition());

    setSelectionAlbero(shapeS);
}


var PuntiSelezionati_Scomposti = new Array();
var UltimoPuntoSelezionato = undefined;
var UltimoPuntoSelezionato_Icona = undefined;

function setSelectionPoint(shapeS) {

    if (shapeS.getIcon() != flgSelezionato) {

        UltimoPuntoSelezionato = shapeS;
        UltimoPuntoSelezionato_Icona = shapeS.icon;
        PuntiSelezionati_Scomposti.push(shapeS);

        CoordFromPoints = CoordFromPoints + shapeS.getPosition() + ",";
        CoordFromPointsMVCArray.push(shapeS.getPosition());
        utility.log('test: ' + shapeS.html);
        utility.log('lat: ' + shapeS.getPosition());


        shapeS.setIcon(flgSelezionato);

        //'  Vanni, 17/02/2016 15:48:46: imposto la selezione dell'albero
        setSelectionAlbero(shapeS);
    }

}

var ShapeAddedd = false;
/* Evento di Selezione nultipla dello shape */
function setMultiSelection(shapeS) {

    var oldShape = shape.selectedShape;

    utility.log("setMultiSelection chiamata!");

    setSelection(shapeS);
    shapeS.set('fillColor', shape.selectedShape.selectedColor);
    var idNoA = ""

    //la prima selezione la aggiungo..

    if (oldShape && !ShapeAddedd) {
        shape.selectedShapeArray.push(oldShape);
        ShapeAddedd = true;
        if (oldShape.chiavealbero != null) {
            idNoA = oldShape.chiavealbero.toString().replace(' ', '');
            $("#ChiaveAlberoMultiSelezione").val($("#ChiaveAlberoMultiSelezione").val() + idNoA + "|");
        }
    }

    //aggiungo la selezione corrente
    if (ShapeAddedd) {
        if (shapeS.chiavealbero != null) {
            idNoA = shapeS.chiavealbero.toString().replace(' ', '');

            if (stringhe.contains($("#ChiaveAlberoMultiSelezione").val(), idNoA)) {

                utility.log("deseleziona: 1. " + $("#ChiaveAlberoMultiSelezione").val());
                var valToReplace = $("#ChiaveAlberoMultiSelezione").val().replace(idNoA + "|", "");
                utility.log("deseleziona: 2. " + valToReplace);

                $("#ChiaveAlberoMultiSelezione").val(
                    valToReplace
                );

                utility.log("deseleziona: 3. " + $("#ChiaveAlberoMultiSelezione").val());

                shape.selectedShapeArray = jQuery.removeFromArray(shapeS, selectedShapeArray);
                clearSelection();
            }
            else {
                shape.selectedShapeArray.push(shapeS);
                $("#ChiaveAlberoMultiSelezione").val($("#ChiaveAlberoMultiSelezione").val() + idNoA + "|");

            }
        }

    }
}
/* Evento di Selezione dello shape */




function setSelectionCircle(shapeS) {

    mappa.elemenotMappa.setCenter(shapeS.getCenter());
    var zoom = mappa.elemenotMappa.getZoom() - 1;
    mappa.elemenotMappa.setZoom(zoom);

    if (!ctrlPressed)
        clearSelectionBoth();

    shape.selectedShape = shapeS;


    shape.selectedShape.set('fillColor', shape.selectedShape.selectedColor);

    //permessi
    if (shape.selectedShape.modifica == 'True') {
        shapeS.setEditable(true);
        $('#save-button').show();
    }
    else {
        shapeS.setEditable(false);
        $('#save-button').hide();
    }

    if (shape.selectedShape.cancellazione == 'True') {
        shapeS.setEditable(true);
        $('#delete-button').show();
    }
    else {
        shapeS.setEditable(false);
        $('#delete-button').hide();
    }

    if (shape.selectedShape.informazioni == 'True') {
        shapeS.setEditable(true);
        //mostrerò ttutte le informanzioni
        $('#info_appezzamento').show();
    }
    else {
        shapeS.setEditable(false);
        //mostro poche informazioni
        $('#info_appezzamento').show();
    }

    $('#disenga_poligono').hide();
    //fine permessi

    if (shapeS.chiavealbero != null) {
        setSelectionAlbero(shapeS);
    }


}


/* Evento di Selezione dello shape */
function setSelection(shapeS) {


    bAbilitaRenderPerEventoDrag = false;

    mappa.elemenotMappa.setCenter(getBounds(shapeS).getCenter(), mappa.elemenotMappa.fitBounds(getBounds(shapeS)));
    var zoom = mappa.elemenotMappa.getZoom() - 1;
    mappa.elemenotMappa.setZoom(zoom);

    if (!ctrlPressed)
        clearSelectionBoth();

    shape.selectedShape = shapeS;


    shape.selectedShape.set('fillColor', shape.selectedShape.selectedColor);

    //permessi
    if (shape.selectedShape.modifica == 'True') {
        shapeS.setEditable(true);
        $('#save-button').show();
    }
    else {
        shapeS.setEditable(false);
        $('#save-button').hide();
    }

    if (shape.selectedShape.cancellazione == 'True') {
        shapeS.setEditable(true);
        $('#delete-button').show();
    }
    else {
        shapeS.setEditable(false);
        $('#delete-button').hide();
    }

    if (shape.selectedShape.informazioni == 'True') {
        //shapeS.setEditable(true);
        //mostrerò ttutte le informanzioni
        $('#info_appezzamento').show();
    }
    else {
        //shapeS.setEditable(false);
        //mostro poche informazioni
        $('#info_appezzamento').show();
    }

    $('#disenga_poligono').hide();
    //fine permessi

    if (shapeS.chiavealbero != null) {
        setSelectionAlbero(shapeS);
    }

}




function setSelectionAlbero(shapeS) {
    var idNoA = shapeS.chiavealbero.toString().replace(' ', '');
    //    console.log('-----------' + idNoA);
    var id = 'a' + idNoA;
    $('.jstree-clicked').removeClass('jstree-clicked');
    $('#' + id).addClass('jstree-clicked');

    //    utility.log(id);
    //    utility.log($('#' + id).attr('href'));

    //myScrollTop(id, "#spazio_Albero");
    myScrollTop(id, "#albero");


    $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val(idNoA);

    shape.selezionaIndiceLayerDoveScrivoDaOggettoGrafico();


    var selezionatoLayerDataChiaveAlbero = false;
    for (var i = 0; i < mappa.Livelli.length; i++) {
        if (!selezionatoLayerDataChiaveAlbero)
            selezionatoLayerDataChiaveAlbero = selezionaIndiceLayerDoveScrivoDaAlbero(mappa.Livelli[i], shapeS.chiavealbero.toString());
    }
}


function myScrollTop(id, divToScroll) {

    var p;
    try {
        //        console.log('ID= ' + id + ' divToScroll: ' + divToScroll);
        p = $("#treeAlberoAnagraficaAlberoAnagrafica").offset().top;


        //x 4 livelli
        var objcorrente = $(document.getElementById(id));
        for (a = 0; a <= 10; a++) {
            objcorrente = $(objcorrente).parent()
            if ($(objcorrente).html() === undefined)
                break;

            $(objcorrente).removeClass("jstree-closed").addClass("jstree-open");

        }




        let elem = document.getElementById(id);
        if (elem != null) {

            var p2 = $(elem).position().top;

            var goToScroll = -p + p2 + offSetScrollTop;

            utility.log("spazio_Albero = " + $(divToScroll).height() + " - goToScroll = " + goToScroll);


            $(divToScroll).scrollTop(goToScroll);
        }

    }
    catch (e) {
        utility.log("catch! myScrollTop(" + id + ", " + divToScroll + ")");
        utility.log(e);
    }
}





/* Eliminazione dello shape */
function deleteSelectedShape() {
    if (shape.selectedShape) {
        $("#dialogEliminaImpianto").dialog("open");
        return;
    }

    if ($("#hiddenMultipointModifica")) {
        $("#dialogEliminaMultipoint").dialog("open");
        return;
    }

}

function dialogAB() {
    $("#dialogAB").dialog("open");
}


function dialogAnalisiMeteo() {

    var CoordOk = false;
    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();

    if ($("#lat_cerca").val() == "") {
        CoordOk = mappa.CoordDaSelezione();
    } else {
        CoordOk = true;
    }

    if (!CoordOk) {
        $("#espandi_indirizzo").click();

        if (ChiaveAlbero == "") {
            alert("Occorre selezionare un elemento grafico.");
            return false;
        }
    }

    var lat = $("#lat_cerca").val();
    var lng = $("#long_cerca").val();


    $.ajax({
        type: "POST",
        url: indirizzohttp + "/AnalisiMeteo",
        data: "{ lat: '" + lat + "', lng: '" + lng + "', ChiaveAlbero: '" + ChiaveAlbero + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {

                if (stringhe.startsWith(msg.d, 'errore')) {
                    alert(msg.d);
                } else {
                    $("#framepop_up_Meteo").attr("src", "about:blank");
                    $("#framepop_up_Meteo").attr("src", msg.d);
                    utility.log("redir to: " + msg.d);
                    $("#pop_up_Meteo").dialog("open");
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function dialogBufferZoneIntersection() {


    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();

    //se sono tutte valorizzate allora posso procedere
    if (ChiaveAlbero == "") {
        alert("nessun Elemento selezionato.");
        interfaccia.loading(false);
        return false;
    }

    if (ChiaveAlbero != '' && CoordFromPoints == '' && shape.selectedShape == undefined) {
        alert("Nessun disegno per l'elemento selezionato.");
        return false;
    }


    //verificare se selezionato appezzamento.
    $("#tool_bar").hide();
    $("#dialogBufferZoneIntersection").dialog("open");
}



function BufferZoneIntersection_salva() {

    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();

    //per chiamata Standard Ajax
    ajaxAgronica(indirizzohttp + "/BufferZone_Aggiorna",
        JSON.stringify({
            ChiaveAlbero: ChiaveAlbero,
            txtDistBZ_CorpiIdrici: $("#txtDistBZ_CorpiIdrici").val(),
            txtDistBZ_AreeResPub: $("#txtDistBZ_AreeResPub").val(),
            txtDistBZ_Allevamenti: $("#txtDistBZ_Allevamenti").val(),
            txtDistBZ_VegNatNonColt: $("#txtDistBZ_VegNatNonColt").val(),
            txtSupBZ_Riduzione: $("#txtSupBZ_Riduzione").val()
        }),
        function (risposta) {

            var risp = JSON.parse(risposta.RispostaStringa);
            window.alert(risp);

        }, null);

}


//#Region "Scomposizione per punti"
function dialogScomponiPunti_Open() {

}

function DialogScomponiPunti_AnnullaVtx() {

    if (UltimoPuntoSelezionato !== undefined) {

        //Rimuovo da insieme coordinate
        CoordFromPointsMVCArray.pop();
        UltimoPuntoSelezionato = PuntiSelezionati_Scomposti.pop();


        //Rimuovo da stringa Coord.
        DialogScomponiPunti_AnnullaVtx_RimuoviCoordFromPoints();

        //re-imposta icona precedente
        UltimoPuntoSelezionato.setIcon(UltimoPuntoSelezionato_Icona);

    }

}

function DialogScomponiPunti_AnnullaVtx_RimuoviCoordFromPoints() {

    // (44.51269641967218, 11.784321516752243),(44.511961949708976, 11.785501688718796),
    var App = new Array();
    App = CoordFromPoints.split(",");

    if (App.length == 3) {
        CoordFromPoints = "";
    } else {

        App.splice([App.length - 3], 3);

        CoordFromPoints = App.join(",") + ",";
    }
}

function DialogScomponiPunti_AnnullaTutti() {

    if (UltimoPuntoSelezionato !== undefined) {
        CoordFromPoints = "";
        for (var cPunto in PuntiSelezionati_Scomposti) {
            cPunto.setIcon(UltimoPuntoSelezionato_Icona);
        }

        CoordFromPointsMVCArray = new Array();
        PuntiSelezionati_Scomposti = new Array();

    }

}


function DialogScomponiPunti_Associa() {

    editPunti();

}

function DialogScomponiPunti_Termina() {

    $('#dialogEliminaImpiantoPuntiScomposti').dialog("open");
    return 1;

}

function dialogScomponiPunti_onClose() {

}
//#End Region "Scomposizione per punti"


function dialogBufferZoneIntersection_Open() {


    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();
    var Entita_Cod = shape.selectedShape.Entita_Cod

    //per chiamata Standard Ajax
    ajaxAgronica(indirizzohttp + "/BufferZone_Leggi",
        JSON.stringify({ Entita_Cod: Entita_Cod, ChiaveAlbero: ChiaveAlbero }),
        function (risposta) {

            var dati = JSON.parse(risposta.RispostaStringa);
            var msg = "";
            if (dati.DatoLetto) {

                msg = "Dati già impostati sull'appezzamento."

                $("#txtDistBZ_CorpiIdrici").val(dati.txtDistBZ_CorpiIdrici);
                $("#txtDistBZ_AreeResPub").val(dati.txtDistBZ_AreeResPub);
                $("#txtDistBZ_Allevamenti").val(dati.txtDistBZ_Allevamenti);
                $("#txtDistBZ_VegNatNonColt").val(dati.txtDistBZ_VegNatNonColt);

                $("#txtSupBZ_Riduzione").val(dati.txtSupBZ_Riduzione);

            } else {
                msg = "Nessun dato impostato sull'appezzamento."
            }

            $("#dialogBufferZoneIntersection_DatoLetto").html(msg);
            $("#txtSupBZ_Riduzione_Ricalcolata").html(dati.txtSupBZ_Riduzione_Ricalcolata);

        }, null);

}




function dialogBufferZoneIntersection_onClose() {
    $("#tool_bar").show();
}

function dialogMultipoint() {
    $("#dialogMultipoint").dialog("open");
}


function dialogRateo() {
    $("#dialogRateo").dialog("open");
}

function dialogGeneraPlanning() {
    $("#dialogGeneraPlanning").dialog("open");
}

function ClearSelect(obj) {
    $('.toolSelezionato').removeClass('toolSelezionato');
    obj.addClass('toolSelezionato');
    obj.effect("bounce", { direction: 'down', times: 5 }, 300);
}

function BlinkFast(obj) {
    $('.toolSelezionato').removeClass('toolSelezionato');
    obj.addClass('toolSelezionato toolSelezionatoEvidenziato');
    //    obj.effect("bounce", { direction: 'down', times: 30, distance: 50, mode: 'effect' }, 300);
    obj.effect("bounce", { direction: 'down', times: 5 }, 300);
}

function predisponiLayoutDialogImpianto() {
    if (isSementi) {
        $("#lblpop_up_centro").html("Campo");
        $("#lblpop_up_data_inizio").html("Data Inserimento");
        $("#lblpop_up_data_fine").html("Data Raccolto");
        $("#lblpop_up_m_data_inizio").html("Data Inserimento");
        $("#lblpop_up_m_data_fine").html("Data Raccolto");
        $("#pop_up_data_inizio").attr("disabled", "disabled");
        $("#pop_up_m_data_inizio").attr("disabled", "disabled");
        $("#pop_up_data_fine").attr("disabled", "disabled");
        $("#pop_up_m_data_fine").attr("disabled", "disabled");
    }
    else {
        $("#righelloCampoVicino").hide();
    }
}







/* Salvataggio dello Shape */
function preparaXSalvataggioPoligoni() {


    if ($("#hiddenMultipointModifica").val() != "" && shape.selectedShape) {
        alert("modifica di elementi multipli non consentita.");
    }


    if ($("#hiddenMultipointModifica").val() != "") {

        interfaccia.loading(true);

        //carica App_nome o Programmazione_des
        $.ajax({
            type: "POST",
            url: indirizzohttp + "/ModificaMultipoint",
            data: "{ hiddenPunti_M: '" + $("#hiddenMultipointModifica").val() + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d == "SessioneScaduta") {
                    $('#dialogSessioneScaduta').dialog("open");
                } else {

                    interfaccia.loading(false);
                    $("#hiddenMultipointModifica").val('');

                    if (msg.d == "Ok") {
                        alert("salvataggio avvenuto con successo");
                    }
                    else {
                        alert(msg.d);
                    }

                    if ($('#AggiornaFiltro').length > 0) {
                        $('#AggiornaFiltro').click();
                    }
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
        return;
    }


    if (shape.selectedShape) {
        //i punti Salvati
        var MVCArray = shape.selectedShape.getPath();

        var test;
        test = MVCArray.getArray().toString();
        utility.log("PIPOPPOO:");


        //controllo se è un nuovo shape o la modifica di uno vecchio
        if (shape.selectedShape.html == undefined) {
            if (VerificaPoligono(test, true) == false) {
                return false;
            }


            $('#hiddenPunti_Nuovo').val(MVCArray.getArray().toString());

            var area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);

            //aggiungo l'infobox
            $('#pop_up_sup_app').val(area);
            $('#pop_up_sup_google').val(area);


            predisponiLayoutDialogImpianto();

            var salvataggio_diretto = false;
            if ($("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val() != '') {
                salvataggio_diretto = true;
            } else {
                //se ho il valore 
            }

            utility.log("salvataggio_diretto:" + salvataggio_diretto);

            if (salvataggio_diretto == true) {
                $('#lblDialogSup').html(area);
                $('#dialogConfermaAssociazione').dialog("open");

            }
            else {


                $("#pop_up_impianto").dialog("open");
                ImpostaVisibilita_pop_up_impianto();

                interfaccia.initDatePiker();


                interfaccia.loading(true);

                CaricaSpecie('#pop_up_specie', '');
                CaricaAzienda();

                if (isSementi == false) {
                    CaricaDateDefault('#pop_up_data_inizio', '#pop_up_data_fine');
                }
                ImpostaPulsantiNuovoImpianto();
                interfaccia.loading(false);
                mappa.identificaIndirizzi(MVCArray);
                //'  Vanni, 03/12/2015 09:51:53: vavava                
                inizializzaDDLProvincie('ddl_Provincia_', imprese_ID_ind);
                inizializzaDDLProvincie('ddl_Provincia_', centro_ID_ind);

                //GABRIELE 26 03 2019
                $('#pop_up_specie').trigger("change");
            }
            shape.selectedShape.setEditable(false);

        } else {

            if (VerificaPoligono(test, false) == false) {
                return false;
            }

            //            utility.log("shape.selectedShape.html:" + shape.selectedShape.html);

            predisponiLayoutDialogImpianto();

            $('#hiddenPunti_modifica').val(MVCArray.getArray().toString());
            $('#hiddenID').val(shape.selectedShape.html);

            var area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);

            $('#pop_up_sup_google_modifica').val(area);
            //            $('#pop_up_sup_app_modifica').val(area);

            ImpostaVisibilita_pop_up_modificaImpianto();
            $("#pop_up_modificaImpianto").dialog("open");

            interfaccia.loading(true);


            //carica App_nome o Programmazione_des
            $.ajax({
                type: "POST",
                url: indirizzohttp + "/CaricaAppNomeProgrammazione_des",
                data: "{ ChiaveAlbero: '" + shape.selectedShape.chiavealbero + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    if (msg.d == "SessioneScaduta") {
                        $('#dialogSessioneScaduta').dialog("open");
                    } else {
                        $('#pop_up_nome_appezza_modifica').val(msg.d);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });



            setTimeout(function () {
                var Veg_Cod = shape.selectedShape.vegcod;
                CaricaSpecie("#pop_up_specie_modifica", Veg_Cod);
                var Entita_Cod = shape.selectedShape.Entita_Cod;
                mappa.getProprieta(Veg_Cod, Entita_Cod, 1);
                if (isSementi == false) {
                    CaricaDateDefault('#pop_up_m_data_inizio', '#pop_up_m_data_fine');
                }
                shape.selectedShape.setEditable(false);
                interfaccia.loading(false);
            }, 100);
        }

    } else {
        alert("non è stato selezionato nulla");
    }
}

function ImpostaVisibilita_pop_up_modificaImpianto() {
    utility.log("ImpostaVisibilita_pop_up_modificaImpianto, glayerDoveDisegnoSuTipologiaStandard = " + shape.glayerDoveDisegnoSuTipologiaStandard);

    //'  Vanni, 28/09/2015 10:03:43: personalizzazione KWS, un po' Hardcoded ma appena c'è tempo configuriamo la cosa.
    settaVisibilitaKWS("#kws_m_appezza", "#div_m_lotto", "#pop_up_impiantoxDescr");

    if (shape.glayerDoveDisegnoSuTipologiaStandard != "19") {
        $("#pop_up_modificaImpianto_DatiImp").hide();
        $("#Dati_data_inizio_fine_modifica").hide();

    } else {
        $("#pop_up_modificaImpianto_DatiImp").show();
        $("#Dati_data_inizio_fine_modifica").show();
    }

}

function isKws() {
    getCodiceFiscaleTecnico();

    return (Codice_Fiscale_Tecnico == "13171470159")

}

function settaVisibilitaKWS(kws, lotto, pop_up_impiantoxDescr) {

    getCodiceFiscaleTecnico();

    if (Codice_Fiscale_Tecnico != "13171470159") {
        //nascondo i campoi codice originale e numero appezzamento..:        
        $(kws).hide();
        $(lotto).show();
        $(pop_up_impiantoxDescr).show();
    } else {
        $(kws).show();
        $(lotto).hide();
        $(pop_up_impiantoxDescr).hide();
    }
}

function ImpostaVisibilita_pop_up_impianto() {

    utility.log("ImpostaVisibilita_pop_up_impianto, glayerDoveDisegnoSuTipologiaStandard = " + shape.glayerDoveDisegnoSuTipologiaStandard);

    //'  Vanni, 28/09/2015 10:03:43: personalizzazione KWS, un po' Hardcoded ma appena c'è tempo configuriamo la cosa.
    settaVisibilitaKWS("#kws_appezza", "#div_lotto", "#pop_up_impiantoxDescr");

    if (shape.glayerDoveDisegnoSuTipologiaStandard != "19") {
        $("#PopupNuovaImpresa-Button").hide();
        $("#PopupNuovoSa-Button").hide();
        $("#divpop_up_specie").hide();
        $("#placeTipologia").hide();
        $("#divOpzioni_pop_up_impianto").hide();
    } else {
        $("#PopupNuovaImpresa-Button").show();
        $("#PopupNuovoSa-Button").show();
        $("#divpop_up_specie").show();
        $("#placeTipologia").show();
        $("#divOpzioni_pop_up_impianto").show();

    }

}


/**
 * assegna l'area ad un oggetto json in base a cosa selezionato nella combo indicata nel parametro.
 * @param {number} area Area da assegnare
 */
function salvataggioDirettoConAppezzaAreaCalcola(area) {

    var rval = "";
    var comboJQuerySelector = ""

    if ($("#pop_up_modificaImpianto").dialog('isOpen')) {
        comboJQuerySelector = "#opt_associa_modifica";
    }

    if ($("#dialogConfermaAssociazione").dialog('isOpen')) {
        comboJQuerySelector = "#option_associa";
    }

    if (comboJQuerySelector == "") {
        return "";
    }

    if ($(comboJQuerySelector).val() == '2') {
        rval = "{ 'AreaAppezzamento': " + area.toString() + ", 'AreaImpianto': 0 }";
    }


    if ($(comboJQuerySelector).val() == '3') {
        rval = "{ 'AreaAppezzamento': " + area.toString() + ", 'AreaImpianto': " + area.toString() + " }";
    }

    return rval;

}



function ControllaSeEsisteCatasto(area) {
    //    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString().replace(/\\/g, '\\\\'); ;
    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();
    $.ajax({
        type: "POST",
        url: indirizzohttp + "/ControllaSeEsisteCatasto",
        data: "{ ChiaveAlbero: '" + ChiaveAlbero + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == "SessioneScaduta") {
                $('#dialogSessioneScaduta').dialog("open");
            } else {
                if (msg.d == "true") {
                    alert("L'appezzamento è collegato con il catasto, in questo caso la modifica della superficie è da effettuare a mano. Andare sull'anagrafica dell'appezzamento se si vuole variare la superficie dell'appezzamento..");
                    $('#option_associa').val(2);
                } else {
                    // se non ho il catasto collegato controllo se esistono operazioni 
                    $.ajax({
                        type: "POST",
                        url: indirizzohttp + "/ControllaSeEsistonoOperazioni",
                        data: "{ ChiaveAlbero: '" + ChiaveAlbero + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (msg) {
                            if (msg.d == "SessioneScaduta") {
                                $('#dialogSessioneScaduta').dialog("open");
                            } else {
                                if (msg.d == "true") {
                                    //non ho alcuna operazione, posso procedere con la modifica della superficie
                                    var area = salvataggioDirettoConAppezzaAreaCalcola(area)
                                    salvataggioDirettoConAppezza(area);

                                } else {
                                    //attenzione
                                    $('#lbl_alert_operazioni_Appezza').html(msg.d);
                                    $('#dialogAllertOperazioniAppezza').dialog("open");
                                }
                            }
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.status);
                            alert(thrownError);
                        }
                    });
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}


function ControllaSeEsistonoOperazioni(area) {

    //    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString().replace(/\\/g, '\\\\'); ;
    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/ControllaSeEsistonoOperazioni",
        data: "{ ChiaveAlbero: '" + ChiaveAlbero + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == "SessioneScaduta") {
                $('#dialogSessioneScaduta').dialog("open");
            } else {
                if (msg.d == "true") {
                    //non ho alcuna operazione, posso procedere con la modifica della superficie
                    area = salvataggioDirettoConAppezzaAreaCalcola(area);
                    salvataggioDiretto(area);
                } else {
                    //attenzione
                    $('#lbl_alert_operazioni').html(msg.d);
                    $('#dialogAllertOperazioni_hidden').val(1);
                    $('#dialogAllertOperazioni').dialog("open");
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}




function salvataggioDirettoConAppezza(area, flag_gps) {
    //    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString().replace(/\\/g, '\\\\'); ;
    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();
    var hiddenPunti_Nuovo = $('#hiddenPunti_Nuovo').val();

    //vanni, 16/01/2015, per baco su grafica duplicata
    var hiddenPunti_modifica = $('#hiddenPunti_modifica').val();

    var hiddenPunti_daSalvare;

    if (hiddenPunti_modifica != "")
        hiddenPunti_daSalvare = hiddenPunti_modifica;
    else
        hiddenPunti_daSalvare = hiddenPunti_Nuovo;


    if (flag_gps === undefined)
        flag_gps = "0";

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza",
        data: "{ ChiaveAlbero: '" + ChiaveAlbero + "', hiddenPunti_Nuovo: '" + hiddenPunti_daSalvare + "', Area: '" + area + "', flag_gps: '" + flag_gps + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $('#responseInterferenze').width('0px');
            $('#responseInterferenze').html('');
            $("#pop_up_impianto").dialog("close");



            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {

                //16/01/2015, azzero oggetti.
                if (shape.selectedShape != undefined)
                    shape.selectedShape.setMap(null);
                $('#hiddenPunti_modifica').val("");

                AggiornaTutto();
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}




function salvataggioDiretto(area) {

    //    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString().replace(/\\/g, '\\\\');  
    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();
    var hiddenPunti_Nuovo = $('#hiddenPunti_Nuovo').val();

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/SalvaNuovoElementoGraficoDaChiaveAlbero",
        data: "{ ChiaveAlbero: '" + ChiaveAlbero + "', hiddenPunti_Nuovo: '" + hiddenPunti_Nuovo + "', Area: '" + area + "', ElementoGrafico_Des: '' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $('#responseInterferenze').width('0px');
            $('#responseInterferenze').html('');
            $("#pop_up_impianto").dialog("close");

            if (shape.selectedShape != undefined)
                shape.selectedShape.setMap(null);

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                AggiornaTutto();
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}


/*******************************************************************/
/********************* LAYER ***************************************/
/*******************************************************************/

function clickLayerSelezionaTutto(selezionaSeTrue) {

    utility.log("clickLayerSelezionaTutto");

    if (selezionaSeTrue) {
        $(".chkLayer").prop("checked", "checked");
    }
    else {
        $(".chkLayer").removeAttr("checked");
    }

    for (var i = 0; i < mappa.Livelli.length; i++) {
        clearSingleOverlay(null, mappa.Livelli[i].poligoni, !selezionaSeTrue);
        clearSingleOverlay(null, mappa.Livelli[i].punti, !selezionaSeTrue);

        clearSingleOverlay(null, mappa.Livelli[i].circle, !selezionaSeTrue);

        clearSingleOverlay(null, mappa.Livelli[i].polyline, !selezionaSeTrue);
        clearSingleOverlay(null, mappa.Livelli[i].ABLabel, !selezionaSeTrue);
    }
}

/* se il layer è checked = true lo visualizzo altimenti non lo presento nella mappa */
function clearOverlaysByID(id) {

    for (var i = 0; i < mappa.Livelli.length; i++) {
        if (mappa.Livelli[i].id == id) {

            clearSingleOverlay(null, mappa.Livelli[i].poligoni, true);
            clearSingleOverlay(null, mappa.Livelli[i].punti, true);
            clearSingleOverlay(null, mappa.Livelli[i].circle, true);
            clearSingleOverlay(null, mappa.Livelli[i].polyline, true);
            clearSingleOverlay(null, mappa.Livelli[i].ABLabel, true);

        }
    }
}

/* se il layer è checked = true lo visualizzo altimenti non lo presento nella mappa */
function clearOverlays(obj) {
    var valore = obj.val();
    for (var i = 0; i < mappa.Livelli.length; i++) {
        if (mappa.Livelli[i].id == obj.attr("id")) {

            clearSingleOverlay(obj, mappa.Livelli[i].poligoni, false);
            clearSingleOverlay(obj, mappa.Livelli[i].punti, false);
            clearSingleOverlay(obj, mappa.Livelli[i].circle, false);
            clearSingleOverlay(obj, mappa.Livelli[i].polyline, false);
            clearSingleOverlay(obj, mappa.Livelli[i].ABLabel, false);

        }
    }


    mappa.GestioneCluster();

    interfaccia.chekLayers(false);
}

function clearSingleOverlay(obj, oggetti, forzaClear) {
    for (var kk = 0; kk < oggetti.length; kk++) {

        if (obj == null) {
            if (forzaClear) {
                oggetti[kk].setMap(null);
            }
            else {
                oggetti[kk].setMap(mappa.elemenotMappa);
            }
        }
        else {
            if (obj.is(':checked') == false || forzaClear == true) {
                oggetti[kk].setMap(null);
            } else {
                oggetti[kk].setMap(mappa.elemenotMappa);
            }
        }
    }

}

/*******************************************************************/
/********************* TOOL BOX ************************************/
/*******************************************************************/
/* riposizionamento del tool a destra */
function PosizionaTool() {
    //gestione del pannello
    $("#sidebar").css({
        width: 55
    });
    $("#contenitore_tool").css({
        width: 100
    });


    $("#contenitore_tool").css({
        position: 'absolute',
        top: parseInt((h - $('#contenitore_tool').height()) / 2) + OffSet_contenitore_Tool,
        left: (w - $("#sidebar").width()) - 47
    }).show();


    var pp = $("#map").offset();

    $("#tool_bar").css({
        position: 'absolute',
        top: parseInt(pp.top + 5)
    });

    $("#scala_colori_contenitore").css({
        position: 'absolute',
        top: pp.top + $("#map").height() - 70,
        left: pp.left
    });
}

/* tool per la ricerca dell'indirizzo  */
function PosizionaRicercaIndirizzo() {
    //gestione del pannello
    $("#dialog_Ricerca").css({
        position: 'absolute',
        bottom: 0,
        right: 5
    }).show();
}


function makeToolBar() {

    console.warn("makeToolBar");

    var checklayer = $('#check-layer');
    checklayer.html('');



    for (var i = 0; i < layers.length; ++i) {

        var chiamataFunzione = 'shape.settaLayerDoveDisegnare(' + layers[i].id + ', \'' + layers[i].icona32 + '\', \'' + layers[i].nome + '\')';

        //utility.log("chiamataFunzione:" + chiamataFunzione);

        var box_img = '<img src="' + layers[i].icona16 + '" onclick="' + chiamataFunzione + '");" />';
        var box = '<span class="color-button" style="background-color:#' + layers[i].colore_1 + '" alt="' + layers[i].colore_1 + '">' + layers[i].nome + '</span>';
        var box_1 = '<span class="color-button" style="background-color:#' + layers[i].colore_2 + '" alt="' + layers[i].colore_2 + '">' + layers[i].nome + '</span>';

        var html = '<input type="checkbox" class="chkLayer" onclick="clearOverlays($(this));" checked="checked" id="' + layers[i].id + '"/><label for="' + layers[i].id + '">' + layers[i].nome + '</label>';
        checklayer.append("<div id='chkLayerRow_" + layers[i].id + "' style='float:left;'>" + box_img + " " + box + " " + box_1 + html + "</div><div style='clear:both'></div>");
    }



}




function isLayerVisualizzato(idL) {
    var trovato = false;
    for (var i = 0; i < layers.length; ++i) {
        if (layers[i].id == idL)
            trovato = true;
    }

    return trovato;
}




function selezioneDaIdAlbero(id, multi) {

    utility.log('Da modificare per selezione ');


    var trovato = false;
    var selezionatoLayerDataChiaveAlbero = false;
    //scorro tutti i layer e tutti i path per identificare quello attivo
    for (var i = 0; i < mappa.Livelli.length; i++) {

        if (!selezionatoLayerDataChiaveAlbero)
            selezionatoLayerDataChiaveAlbero = selezionaIndiceLayerDoveScrivoDaAlbero(mappa.Livelli[i], id);

        if (trovato == false) {
            utility.log('trovato = true');
            for (var kk = 0; kk < mappa.Livelli[i].poligoni.length; kk++) {

                if (mappa.Livelli[i].poligoni[kk].chiavealbero == id) {
                    trovato = true;
                    if (!multi) {
                        setSelection(mappa.Livelli[i].poligoni[kk]);
                        break;
                    } else {
                        ctrlPressed = true;
                        setMultiSelection(mappa.Livelli[i].poligoni[kk]);
                        ctrlPressed = false;
                        break;
                    }

                }
            }
        }
        else {
            i = mappa.Livelli.length;
        }
    }
    if (!multi) {
        if (trovato == false) {
            clearSelectionBoth();
            $('#disenga_poligono').show();
        }
    }
}

$(window).resize(function () {
    var p = $("#map").offset();

    h = ($("#map").height() + p.top);
    w = ($("#map").width() + p.left);


    if (mappa.Livelli != null) {
        for (var j = 0; j < mappa.Livelli.length; j++) {
            interfaccia.CreaPannelloColore('abc', mappa.Livelli[j].datiViste);
        }
    }

    PosizionaRicercaIndirizzo();
    PosizionaTool();
});

function pop_up_impianto_close(toClose) {

    $('#responseInterferenze').width('0px');
    $('#responseInterferenze').html('');
    if (toClose != null)
        $(toClose).dialog("close");

    //elimino lo shape
    if (shape.selectedShape != undefined)
        shape.selectedShape.setMap(null);

}



function VerificaLayerConfigurati() {

    if (layers.length == 0) {
        AperturaLayer = true;
        GestisciLayerPrincipale();

    }
}


/*******************************************************************/
/********************* DOCUMENT READY ******************************/
/*******************************************************************/
$(document).ready(function () {

    //    $("#arrow").on("click", function () {
    $(document).on("click", "#arrow", function () {
        $('#pop_up_sup_app_modifica').val($("#pop_up_sup_google_modifica").val());
    });

    $(document).on("click", "#arrowBufferZone", function () {
        $('#txtSupBZ_Riduzione').val($("#txtSupBZ_Riduzione_Ricalcolata").html());
    });

    InizializzaMenuCosaDisegno();



    //utility.log("$(document).ready");

    $(document).on("click", ".jstree-clicked", function () {
        //    $(".jstree-clicked").on("click", function () {
        //utility.log("live click jstree-clicked");
        var id = $(this).attr('id');
        id = id.substring(1);
        id = id.replace(/\\/g, '\\\\');

        utility.log('id: ' + id);


        selezioneDaIdAlbero(id, false);
    });

    $('#select_poligono').click(function () {
        ClearSelect($(this));
    });
    $('#disenga_poligono').click(function () {
        ClearSelect($(this));
    });
    $('#select_poligono').click(function () {
        ClearSelect($(this));
    });
    $('#delete-button').click(function () {
        ClearSelect($(this));
    });
    $('#save-button').click(function () {
        ClearSelect($(this));
    });
    $('#import-button').click(function () {
        ClearSelect($(this));
    });
    $('#export-button').click(function () {
        ClearSelect($(this));
    });
    $('#righello-button').click(function () {
        ClearSelect($(this));
    });
    $('#info_appezzamento').click(function () {
        ClearSelect($(this));
    });


    //àààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààà
    $.ajax({
        type: "POST",
        url: indirizzohttp + "/IsSementieri",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        async: false,
        dataType: "json",
        success: function (msg) {
            if (msg.d == "true")
            { isSementi = true; }
            else {
                isSementi = false;
            }


        },
        error: function (xhr, ajaxOptions, thrownError) {
            isSementi = false;
            alert(xhr.status);
            alert(thrownError);
        }
    });


    var p = $("#map").offset();
    h = ($("#map").height() + p.top);
    w = ($("#map").width() + p.left);

    maxH = $("#map").height();

    InizializzaDatepicker();

    if (isSementi == false) {
        try {
            $('#pop_up_finalita').datepicker({
                dateFormat: 'dd/mm/yy',
                disabled: false,
                changeMonth: true,
                changeYear: true
            });

            $('#pop_up_disciplinare').datepicker({
                dateFormat: 'dd/mm/yy',
                disabled: false,
                changeMonth: true,
                changeYear: true
            });
            $.datepicker.regional['it'];
        } catch (e) {

        }

    }


    initialize();

    PosizionaTool();

    VerificaLayerConfigurati();

    PosizionaRicercaIndirizzo();

    //àààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààààà

    $('.bottone').button();
    $('#CercaIndirizzo').click(function () {
        $('#NascondiMarkerIndirizzo').show();
    });
    $('#NascondiMarkerIndirizzo').click(function () {
        $('#NascondiMarkerIndirizzo').hide();
    });

    $('#aggiorna_date').click(function () {
        var limite_inferiore = $('input[name=limite_inferiore]:checked').val();
        var limite_superiore = $('input[name=limite_superiore]:checked').val();




        $.ajax({
            type: "POST",
            url: indirizzohttp + "/AggiornaFinestraTemporale",
            data: "{da: '" + $('#limita_data_da').val() + "', limite_inferiore:'" + limite_inferiore + "', a: '" + $('#limita_data_a').val() + "', limite_superiore: '" + limite_superiore + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d == 'ok') {

                    //CVDOCG: All'aggiornamento del filtro temporale l'albero mostra un azienda indefinita ed i dati provenienti da ricette non vengono visualizzati
                    var val = "";
                    val = $("#hidden_azienda").val();
                    if (val == "-1")
                        val = "";

                    $("#hidden_azienda").val(val);
                    $('#AggiornaFiltro').click();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                interfaccia.loading(false);
                alert(xhr.status);
                alert(thrownError);
            }
        });
    });


    $("#print").click(function () {
        $('#gisMenuStrumenti-menu').hide();
        window.print();
    });

    $("#espandi_contenuti").click(function () {
        clickPiu();
    });
    $("#espandi_indirizzo").click(function () {
        clickPiu_Indirizzi();
    });
    $("#gestisci_layer_principale").click(function () {
        GestisciLayerPrincipale();
    });
    $("#gestisci_dettagli_layer").click(function () {
        GestisciLayerDettagli();
    });

    $("#tipologia_layer").change(function () {

        //change della combo sul pannello dei layers

        ColorazioneAutomatica($('#tipologia_layer').val());
        AggiornaElencoTipologie();


    });

    /* -- popup Salvatagggio */
    $('#pop_up_impianto_azienda').change(function () {
        CaricaCentroAziendale();
    });

    $('#pop_up_specie').change(function () {
        response_ok = 4;
        if (isSementi == false) {
            CaricaDateDefaultDaVegCod("#pop_up_data_inizio", "#pop_up_data_fine");
        }
        CaricaDisciplinare();

        var Veg_Cod = $('#pop_up_specie').val();
        CaricaTipologia(Veg_Cod, "#pop_up_tipologia", "");

        CaricaVarieta("#pop_up_varieta", '');
        CaricaFinalita("#pop_up_finalita", '');

    });

    $('#pop_up_specie_modifica').change(function () {
        response_ok = 4;
        //            CaricaDisciplinare();

        var Veg_Cod = $('#pop_up_specie_modifica').val();
        CaricaTipologia(Veg_Cod, "#pop_up_tipologia_modifica", "");

        CaricaVarieta("#pop_up_varieta_modifica", '');
        CaricaFinalita("#pop_up_finalita_modifica", '');

    });

    try {

        $('#pop_up_d_semina').datepicker({
            dateFormat: 'dd/mm/yy',
            disabled: false,
            changeMonth: true,
            changeYear: true
        });

        $('#pop_up_d_raccolta').datepicker({
            dateFormat: 'dd/mm/yy',
            disabled: false,
            changeMonth: true,
            changeYear: true
        });
        $.datepicker.regional['it'];
    } catch (e) {

    }

    try {


        /*POP UP */
        $("#pop_up_impianto").dialog({
            autoOpen: false,
            height: maxH / 2,
            maxHeight: maxH,
            width: 'auto',
            modal: true,
            close: function (event, ui) {
                pop_up_impianto_close(null);
            },
            buttons: {
                "Salva Nuovo Elemento": function () {
                    InviaDatiNuovoImpianto();
                },
                "Annulla": function () {
                    pop_up_impianto_close(this);
                }
            }
        });

        $("#pop_up_modificaImpianto").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: 'auto',
            modal: true,
            buttons: {
                "Modifica Impianto": function () {
                    ModificaImpianto();
                },
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#dialogRateo").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: 'auto',
            modal: true,
            buttons: {
                "Conferma": function () {
                    precision.pfRateo();
                },
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#dialogGeneraPlanning").dialog({
            autoOpen: false,
            open: function () {
                precision.GeneraDescrizionePlanning();
            },
            height: 'auto',
            maxHeight: maxH,
            width: 'auto',
            modal: true,
            buttons: {
                "Verifica Aggregazione Poligono": function () {
                    precision.clickGeneraPlanning(true);
                }, "Conferma": function () {
                    precision.clickGeneraPlanning(false);
                },
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#dialogMultipoint").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: 'auto',
            modal: true,
            buttons: {
                "Conferma": function () {
                    ConfermaSalvataggioMultipoint();
                },
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#dialogAB").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: 'auto',
            modal: true,
            buttons: {
                "Conferma": function () {
                    ConfermaSalvataggioAB();
                },
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#dialogBufferZoneIntersection").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: 'auto',
            modal: false,
            open: function (event, ui) {
                dialogBufferZoneIntersection_Open();
            },
            close: function (event, ui) {
                dialogBufferZoneIntersection_onClose();
            },
            buttons: {
                "Annulla": function () {
                    dialogBufferZoneIntersection_onClose();
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#dialogScomponiPunti").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: 'auto',
            modal: false,
            open: function (event, ui) {
                dialogScomponiPunti_Open();
            },
            close: function (event, ui) {
                dialogScomponiPunti_onClose();
            },
            buttons: {
                "Annulla": function () {
                    dialogScomponiPunti_onClose();
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#dialogAllertOperazioniAppezza").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: '300',
            modal: true,
            buttons: {
                "Avanti": function () {

                    var area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);

                    area = salvataggioDirettoConAppezzaAreaCalcola(area);

                    if ($('#dialogAllertOperazioni_hidden').val() == 1) {

                        salvataggioDirettoConAppezza(area);
                    }
                    else {
                        ModificaDatiImpianto();
                        salvataggioDirettoConAppezza(area);
                        $('#pop_up_modificaImpianto').dialog('close');
                    }
                    $(this).dialog("close");
                },
                "Annulla": function () {
                    alert('Impianto e Appezzamento non Modificati');
                    AggiornaTutto();
                    $(this).dialog("close");
                }
            }
        });

        $("#dialogAllertOperazioni").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: '300',
            modal: true,
            buttons: {
                "Avanti": function () {
                    var area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);
                    if ($('#dialogAllertOperazioni_hidden').val() == 1) {
                        area = salvataggioDirettoConAppezzaAreaCalcola(area);
                        salvataggioDiretto(area);
                    }
                    else {
                        ModificaDatiImpianto();
                        area = salvataggioDirettoConAppezzaAreaCalcola(area);
                        salvataggioDiretto(area);
                        $('#pop_up_modificaImpianto').dialog('close');
                    }
                    $(this).dialog("close");
                },
                "Annulla": function () {
                    alert('Impianto non Modificato');
                    AggiornaTutto();
                    $(this).dialog("close");
                }
            }
        });

        $("#dialogConfermaAssociazione").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: '300',
            modal: true,
            buttons: {
                "Avanti": function () {

                    if ($('#option_associa').val() == '1') {
                        //posso fare il semplice salvataggio dei dati senza modificare la superficie
                        salvataggioDiretto('');
                    }
                    else {

                        area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);

                        if ($('#option_associa').val() == '2') {
                            //voglio associare la superficie solamente all'impianto
                            // devo controllare se ci sono delle operazioni registrate su questo impianto
                            ControllaSeEsistonoOperazioni(area);
                        }
                        else {
                            //voglio associare la superficie a impianto e appezzamento
                            //devo controllare se ci sono operazione registrate sull'impianto e se l'appezzamento ha collegato il catasto
                            ControllaSeEsisteCatasto(area);
                        }
                    }
                    $(this).dialog("close");
                }
            }
        });
        $("#pop_up_opAgenda").dialog({
            autoOpen: false,
            height: centoH,
            width: centoW,
            modal: true,
            buttons: {
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });
        $("#pop_up_Meteo").dialog({
            autoOpen: false,
            height: centoH,
            width: centoW,
            modal: true,
            buttons: {
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });
        $("#pop_up_visite").dialog({
            autoOpen: false,
            height: centoH,
            width: centoW,
            modal: true,
            buttons: {
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });
        $("#pop_up_opAgenda_Preselezione").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: 'auto',
            modal: true,
            buttons: {
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#dialogGestioneColoriLayer").dialog({
            autoOpen: false,            
            height: 'auto',
            maxHeight: maxH,
            width: 'auto',
            modal: true,
            buttons: {
                "Conferma": function () {
                    $('#dialogGestioneColoriLayer').dialog("close");
                    ConfermaGestioneColoriLayer();

                },
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#dialogSessioneScaduta").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: 'auto',
            modal: true,
            open: function (event, ui) {
                impostaRedirectStart();
            }
        });

        $("#dialogEliminaImpianto").dialog({
            autoOpen: false,
            open: function () {
                dialogEliminaImpiantoOnOpen();
            },
            height: 'auto',
            maxHeight: maxH,
            width: '300',
            modal: true,
            buttons: {
                "Elimina": function () {
                    EliminaImpianto();
                },
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#pop_up_export_shape").dialog({
            autoOpen: false,
            height: centoH,
            width: centoW,
            modal: true,
            buttons: {
                "Chiudi": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#pop_up_Ricette").dialog({
            autoOpen: false,
            height: centoH,
            width: centoW,
            modal: true,
            buttons: {
                "Chiudi": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#pop_up_Catasto").dialog({
            autoOpen: false,
            height: centoH,
            width: centoW,
            modal: true,
            buttons: {
                "Chiudi": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#pop_up_import_shape").dialog({
            autoOpen: false,
            height: centoH,
            width: centoW,
            modal: true,
            buttons: {

                "Annulla": function () {
                    CiSonoVecchiDatiNonImportati = false;
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#pop_up_Ritaglia").dialog({
            autoOpen: false,
            height: centoH,
            width: centoW,
            modal: true,
            buttons: {
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#dialogEliminaMultipoint").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: 'auto',
            modal: true,
            open: function (event, ui) {
                popup_nuova_azienda_open();
            },
            buttons: {
                "Elimina": function () {
                    EliminaMultipointSelezionati();
                    //elimino lo shape salvato
                },
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#dialogEliminaImpiantoPuntiScomposti").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: 'auto',
            modal: true,
            open: function (event, ui) {
                popup_nuova_azienda_open();
            },
            buttons: {
                "Conferma": function () {
                    EliminaImpiantoPuntiScomposti();
                    //elimino lo shape salvato
                },
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#popup_nuova_azienda").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: 'auto',
            modal: true,
            open: function (event, ui) {
                popup_nuova_azienda_open();
            },
            buttons: {
                "Salva Nuova Azienda": function () {
                    InviaDatiNuovaAzienda();
                    //elimino lo shape salvato
                },
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#popup_nuovo_centro").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: 'auto',
            modal: true,
            buttons: {
                "Salva Nuovo Centro": function () {
                    InviaDatiNuovoCentro();
                    //elimino lo shape salvato
                },
                "Annulla": function () {
                    $(this).dialog("close");
                    //elimino lo shape salvato
                }
            }
        });

        $("#dialogCultivarRicette").dialog({
            autoOpen: false,
            height: 'auto',
            maxHeight: maxH,
            width: 'auto',
            modal: true
        });

        $("#PopupNuovaImpresa-Button").click(function () {
            $('#popup_nuova_azienda').dialog("open");
        });

        $("#PopupNuovoSa-Button").click(function () {
            $('#popup_nuovo_centro').dialog("open");
        });

        $("#import-button").click(function () {
            var url = "../CaricaShape.aspx";
            if (CiSonoVecchiDatiNonImportati) {
                url = url + "?isFromAlert=1";
                $("#pop_up_import_shape").dialog({
                    height: centoH_xs,
                    width: centoW_xs
                });

            }
            else {
                $("#pop_up_import_shape").dialog({
                    height: centoH,
                    width: centoW
                });


            }

            $("#frameShape").attr("src", url);

            $("#pop_up_import_shape").dialog("open");
        });

        $("#nuova_ricetta").click(function () {
            precision.clickRicetta();
        });

        $("#selectPerSpecie").click(function () {
            precision.clickselectPerSpecie();
        });

        $("#ripartoCatasto-button").click(function () {
            catasto();
        });

        $("#nuova_agenda").click(function () {
            apriPreselezioneAgenda();
        });

        $("#nuova_visita").click(function () {
            apriPreselezioneVisita();
        });

        $("#ritaglio-edit-punti-gps-button").click(function () {
            editPunti();
        });

        $("#CopiaOggetto").click(function () {
            copiaIncolla();
        });

        $("#ritaglio-sfondo").click(function () {

            //mappa.elemenotMappasetZoom(zoomRicercaIndirizzoxRitaglio);

            interfaccia.loading(true);
            var Piva;

            var LatLong1;
            var LatLong2;

            var bounds = mappa.elemenotMappa.getBounds();
            //        alert(bounds);
            var AmaxX = bounds.getSouthWest().lng().toString();
            var AmaxY = bounds.getNorthEast().lat().toString();
            var AminX = bounds.getNorthEast().lng().toString();
            var AminY = bounds.getSouthWest().lat().toString();

            //controllo la chiave dell'albero selezionata

            var Entita_Cod = "";
            Entita_Cod = $('#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica').val().toString();  //.replace(/\\/g, '\\\\'); //shape.selectedShape.chiavealbero.toString();

            if (Entita_Cod == "") {
                alert("selezionare un centro aziendale a cui associare il ritaglio della mappa");
                interfaccia.loading(false);
                return false;
            }

            if (Entita_Cod.split("§")[0] != "3") {
                alert("selezionare il centro aziendale");
                interfaccia.loading(false);
                return false;
            }


            $.ajax({
                type: "POST",
                url: indirizzohttp + "/getUrlRitaglio",
                data: "{AmaxX: '" + AmaxX + "', AmaxY: '" + AmaxY + "', AminX: '" + AminX + "', AminY: '" + AminY + "', Entita_Cod: '" + Entita_Cod + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    if (msg.d == 'errore') {
                        alert("Selezionare un'azienda");
                    }
                    else {
                        //                    window.open(msg.d, "", "scrollbars=yes,resizable=yes,width=" + centoW + ",height=" + centoH + ",top=" + 0 + ",left=" + 0);
                        alert('ritaglio di mappa associato al centro aziendale');
                        interfaccia.loading(false);
                        //                    $("#frameRitaglia").attr("src", msg.d);
                        //                    $('#pop_up_Ritaglia').dialog("open");
                        //                    $('#pop_up_Ritaglia').parent().appendTo($('form:first'));

                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    interfaccia.loading(false);
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        });

        $("#nuovapiva-button").click(function () {
            interfaccia.loading(true);
            $.ajax({
                type: "POST",
                url: indirizzohttp + "/GetRandomPiva",
                data: "{ }",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    interfaccia.loading(false);
                    if (msg.d == 'SessioneScaduta') {
                        $('#dialogSessioneScaduta').dialog("open");
                    }
                    else {
                        $('#txt_Piva').val(msg.d);

                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    interfaccia.loading(false);
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        });

        $("#righelloSpecie").click(function () {
            verificaInterferenze("VerificaInterferenze");
        });

        $("#righelloCampoVicino").click(function () {
            verificaInterferenze("MostraCampoPiuVicino");
        });

        $("#export-button").click(function () {

            var Entita_Cod = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();
            var ePiva = "";
            var eSa_cod = "-1";
            var eApp = Entita_Cod.split("§")

            if (eApp[0] == "3") {

                ePiva = eApp[1];
                eSa_cod = eApp[2];

            }

            $("#pop_up_export_shape").dialog("open");
            $("#frameShapePF").attr("src", "../EsportaPF.aspx?piva=" + ePiva + "&sa_cod=" + eSa_cod);

        });

    } catch (e) {

    }

});




function verificaInterferenze(webService) {
    var Veg_Cod = $('#pop_up_specie').val()
    if (Veg_Cod == "") alert("selezionare una specie");
    else {

        var hiddenPunti_Nuovo = $('#hiddenPunti_Nuovo').val();
        var myData_Inizio = $('#pop_up_data_inizio').val();
        var myData_Fine = $('#pop_up_data_fine').val();
        var azienda_selezionata = $('#pop_up_impianto_azienda').val();
        var pop_up_tipologia = $('#pop_up_tipologia').val();

        interfaccia.loading(true);

        $.ajax({
            type: "POST",
            url: indirizzohttp + "/" + webService,
            data: "{ veg_cod: '" + Veg_Cod + "', grva_cod:'" + pop_up_tipologia + "', data_inizio: '" + myData_Inizio + "', data_fine: '" + myData_Fine + "', hiddenPunti_Nuovo: '" + hiddenPunti_Nuovo + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {

                interfaccia.loading(false);
                if (msg.d == 'SessioneScaduta') {
                    $('#dialogSessioneScaduta').dialog("open");
                }
                else {

                    $('#responseInterferenze').html(msg.d);
                    $('#responseInterferenze').width('300px');
                    $('#pop_up_impianto').width($('#pop_up_impianto').width() + 20);
                    //$('#pop_up_specie').html(msg.d);
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                interfaccia.loading(false);
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }

}


function exportConfirm() {
    var id_agenda = 21720;
    var id_formato = $('#cmbFormatoExport').val();
    var filename = "";
    $.ajax({
        type: "POST",
        url: indirizzohttp + "/EsportaShp",
        data: "{ id_agenda: '" + id_agenda + "', filename: '" + filename + "', id_formato: '" + id_formato + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                $('#txt_linkScaricaShape').html(msg.d);

            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}


/*************X SHJAPE **************/
function ChiudiImportaShape() {
    $("#pop_up_import_shape").dialog("close");
}

function NascondiRighelloSpecie() {
    $('#righelloSpecie').hide();
}

function InizializzaDatepickerUI() {

    try {

        $('#pop_up_data_inizio').datepicker({
            dateFormat: 'dd/mm/yy',
            disabled: false,
            changeMonth: true,
            changeYear: true
        });

        $('#pop_up_data_fine').datepicker({
            dateFormat: 'dd/mm/yy',
            disabled: false,
            changeMonth: true,
            changeYear: true
        });

        $('#pop_up_m_data_inizio').datepicker({
            dateFormat: 'dd/mm/yy',
            disabled: false,
            changeMonth: true,
            changeYear: true
        });

        $('#pop_up_m_data_fine').datepicker({
            dateFormat: 'dd/mm/yy',
            disabled: false,
            changeMonth: true,
            changeYear: true
        });
        $.datepicker.regional['it'];
    } catch (e) {

    }
}

function InizializzaDatepicker() {

    var appDatiTipologia;
    var appData_Inizio_Fine;

    appDatiTipologia = $('#DatiTipologia').html();
    appData_Inizio_Fine = $('#Dati_data_inizio_fine').html();

    $('#DatiTipologia').html('');
    $('#placeTipologia').append(appDatiTipologia);

    $('#Dati_data_inizio_fine').html('')
    $('#placeData_Inizio_Fine').append(appData_Inizio_Fine);


    //se sono in modalità sementi

    if (isSementi == true) {

        $('#pop_up_m_data_inizio').val(data_inizio_sportello);
        $('#pop_up_m_data_fine').val(data_fine_sportello);

        $('#pop_up_data_fine').val(data_fine_sportello);
        var m = new Date();
        var month = m.getMonth() + 1
        var day = m.getDate()
        var year = m.getFullYear()
        $('#pop_up_data_inizio').val(day + "/" + month + "/" + year);

    }

    if (!ModalitaBootstrap)
        InizializzaDatepickerUI();
}


//Log di quanto Inizializzato
function InizializzaProprietajsExtracted_LOG() {
    utility.warn("InizializzaProprietajs (inizializzazione da server) ");
    utility.log("AggiornaDatiGiasAlarm = " + AggiornaDatiGiasAlarm.toString());
    utility.log("isSementi = " + isSementi.toString());
    utility.log("Sementieri_Sportello_Configurazione_cod = " + Sementieri_Sportello_Configurazione_cod.toString());
    utility.log("CiSonoVecchiDatiNonImportati = " + CiSonoVecchiDatiNonImportati.toString());
    utility.log("AbilitaPF = " + AbilitaPF.toString());
    utility.log("Codice_Fiscale_Tecnico = " + Codice_Fiscale_Tecnico.toString());
}


//inizializzazione da server
//function InizializzaProprietajs(serverLeggiDatiGIASAlarm, serverSementieri, iSementieri_Sportello_Configurazione_cod, serverCiSonoVecchiDatiNonImportati, serverAbilitaPF, lCodice_Fiscale_Tecnico, sFinestraTemporale_GIS_Inizio, sFinestraTemporale_GIS_Fine, bModalitaBootstrap) {
function InizializzaProprietajs(serverLeggiDatiGIASAlarm, serverCiSonoVecchiDatiNonImportati, serverAbilitaPF, lCodice_Fiscale_Tecnico, sFinestraTemporale_GIS_Inizio, sFinestraTemporale_GIS_Fine, bModalitaBootstrap) {
    AggiornaDatiGiasAlarm = serverLeggiDatiGIASAlarm;
    //isSementi = serverSementieri;
    ModalitaBootstrap = bModalitaBootstrap;
    //Sementieri_Sportello_Configurazione_cod = iSementieri_Sportello_Configurazione_cod;
    CiSonoVecchiDatiNonImportati = serverCiSonoVecchiDatiNonImportati;
    AbilitaPF = serverAbilitaPF;

    if (lCodice_Fiscale_Tecnico != '')
        Codice_Fiscale_Tecnico = lCodice_Fiscale_Tecnico;


    InizializzaProprietajsExtracted_LOG();

    if (CiSonoVecchiDatiNonImportati && !AperturaLayer) {
        AperturaLayer = false;
        BlinkFast($('#import-button'));
        $("#import-button").click();
    }


    utility.log("abilitaPF - " + AbilitaPF);
    if (!AbilitaPF) {
        $('#nuova_ricetta').hide();
        $('#PianoRateoVariabile').hide();
        $('#ab-button').hide();
        $('#img_tool').hide();
        $('#scala_colori_contenitore').hide();

    }


    if (sFinestraTemporale_GIS_Inizio != '') {
        $("#limita_data_da").val(sFinestraTemporale_GIS_Inizio);
        richiediClickSuAggiornaFinestra = true;
    }

    if (sFinestraTemporale_GIS_Fine != '') {
        $("#limita_data_a").val(sFinestraTemporale_GIS_Fine);
        richiediClickSuAggiornaFinestra = true;
    }

    //if (richiediClickSuAggiornaFinestra) {
    //    $("#aggiorna_date").click();
    //}

    //    if (isSementi) {
    //        $('#CopiaOggetto').hide();
    //        $('#GeneraPlanning').hide();
    //        $('#selectPerSpecie').hide();
    //        //$('#import-button').hide();
    //        $('#export-button').hide();
    //        $('#nuova_ricetta').hide();
    //        $('#img_tool').hide();
    //        $('#ripartoCatasto-button').hide();
    //        $('#scala_colori_contenitore').hide();
    //        $('#PianoRateoVariabile').hide();

    //        $('#divEliminaEntitaGIAS').show();

    //    }
    //    else {
    //        $('#divEliminaEntitaGIAS').show();
    //        //$('#divEliminaEntitaGIAS').hide();
    //    }


}


function MostraNascondiPulsanti(bool_catasto, bool_precision, bool_esportazione, bool_bufferzone) {

    if (bool_bufferzone == 'False') {
        $("#BufferZone").hide();
    } else {
        $("#BufferZone").show();
    }

    if (bool_catasto == 'False') {
        $('#ripartoCatasto-button').hide();
    }
    else {
        $('#ripartoCatasto-button').show();
    }

    if (bool_precision == 'False') {
        $('#ab-button').hide();
        $('#nuova_ricetta').hide();
        $('#img_tool').hide();
        $('#PianoRateoVariabile').hide();
        $('#GeneraPlanning').hide();
        $('#divEliminaPrecision').hide();

        $('#divEliminaPrecision').hide();
        $('#divEliminaPrecisionAB').hide();
    }
    else {
        $('#ab-button').show();
        $('#nuova_ricetta').show();
        $('#img_tool').show();
        $('#PianoRateoVariabile').show();
        $('#GeneraPlanning').show();

        $('#divEliminaPrecision').show();
        $('#divEliminaPrecisionAB').show();
    }

    if (bool_esportazione == 'False')
    { $('#export-button').hide(); }
    else {
        $('#export-button').show();
    }

}

function ImpostaVisibilitaPulsanti(nuovo, salva, elimina, esporta, importa, ab, meteo, visite) {

    gNuovo = nuovo;

    if (nuovo)
        $('#disenga_poligono').show();
    else
        $('#disenga_poligono').hide();

    if (salva)
        $('#save-button').show();
    else
        $('#save-button').hide();

    if (elimina)
        $('#delete-button').show();
    else
        $('#delete-button').hide();

    if (importa)
        $('#import-button').show();
    else
        $('#import-button').hide();

    if (esporta)
        $('#export-button').show();
    else
        $('#export-button').hide();

    if (ab)
        $('#ab-button').show();
    else
        $('#ab-button').hide();

    if (meteo)
        $("#AnalisiMeteo").show();
    else
        $("#AnalisiMeteo").hide();

    if (visite)
        $("#nuova_visita").show();
    else
        $("#nuova_visita").hide();

}



function GestisciLayerDettagli() {
    interfaccia.loading(true);

    $('#hiddenPrincipale_1_Dettagli_2').val(2);

    var tipologia_layer = $("#tipologia_layer").val();

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/Carica_ddlTipologiaLayerDettagli",
        data: "{ tipologia_layer: '" + tipologia_layer + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            interfaccia.loading(false);

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                $('#dialogGestioneColoriLayer').dialog("open");
                if (msg.d.toString() != '') {
                    $("#ddlTipologiaLayer").html(msg.d);
                    Caricaplace_tabella_Dettagli($("#ddlTipologiaLayer").val());
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function Caricaplace_tabella_Dettagli(valoreSelezionato) {
    interfaccia.loading(true);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/Caricaplace_tabella_Dettagli",
        data: "{ valoreSelezionato: '" + valoreSelezionato + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            interfaccia.loading(false);

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                if (msg.d.toString() != '') {
                    $("#place_tabella").html(msg.d);


                    SliderjQueryUI();
                    ColorPickerJQueryUI();

                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
            alert(xhr.status);
            alert(thrownError);
        }
    });

}



//#Region "Gestione della tipologia Layer"


function Cambia_ddlTipologiaLayer() {

    //change sulla combo della tabella popup dei layers

    if ($('#hiddenPrincipale_1_Dettagli_2').val() == '1') {

        ColorazioneAutomatica($("#ddlTipologiaLayer").val());
        Caricaplace_tabella($("#ddlTipologiaLayer").val());

    } else {
        Caricaplace_tabella_Dettagli($("#ddlTipologiaLayer").val());

    }


}




function GestisciLayerPrincipale() {
    interfaccia.loading(true);

    $('#hiddenPrincipale_1_Dettagli_2').val(1);


    $.ajax({
        type: "POST",
        url: indirizzohttp + "/Carica_ddlTipologiaLayer",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            interfaccia.loading(false);

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                $('#dialogGestioneColoriLayer').dialog("open");
                if (msg.d.toString() != '') {

                    $("#ddlTipologiaLayer").html(msg.d);
                    $('#ddlTipologiaLayer').find('option[value="' + $("#tipologia_layer").val() + '"]').attr("selected", true);
                    Caricaplace_tabella($("#ddlTipologiaLayer").val());
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function Caricaplace_tabella(valoreSelezionato) {
    interfaccia.loading(true);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/Caricaplace_tabella",
        data: "{ valoreSelezionato: '" + valoreSelezionato + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            interfaccia.loading(false);

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {

                if (msg.d.toString() != '') {
                    $("#place_tabella").html(msg.d);

                    SliderjQueryUI();
                    ColorPickerJQueryUI();

                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
            alert(xhr.status);
            alert(thrownError);
        }
    });

}



function ColorazioneAutomatica(Layer_Selezionato) {




    interfaccia.loading(true);


    $.ajax({
        type: "POST",
        url: indirizzohttp + "/ColorazioneAutomatica",
        data: "{ TipologiaLayer_cod: '" + Layer_Selezionato + "' }",
        dataType: "json",
        async: true,
        contentType: "application/json; charset=utf-8",
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        },
        success: function (msg) {
            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                if (msg.d.substring(0, 2) == "ok") {

                    var msg_d = msg.d.substring(2, msg.d.length - 1);

                    //codice success


                }
            }

        }
    });
}

function AggiornaElencoTipologie() {
    console.log('AggiornaElencoTipologie');
    var Layer_Selezionato;
    Layer_Selezionato = $('#tipologia_layer').val();
    //alert("l:" + Layer_Selezionato);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/AggiornaElencoTipologie",
        data: "{ Layer_Selezionato: '" + Layer_Selezionato + "', Sementieri_Sportello_Configurazione_cod: '" + Sementieri_Sportello_Configurazione_cod + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {


                svuotaTutto();

                //reinizilaizzo i poligoni
                //                layers = eval('(' + msg.d + ')');

                //utility.log("AggiornaElencoTipologie, layers da server:");
                //utility.log(msg.d);
                layers = JSON.parse(msg.d);

                mappa.ImpostaLayer();
                makeToolBar();

                AggiornaLayer();

                interfaccia.chekLayers();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

//#End Region "Gestione della tipologia Layer"

function svuotaTutto_nuoviVettori() {
    var i;

    if (mappa.MarkGps !== null) {
        if (mappa.MarkGps != undefined) {
            for (i = 0; i < mappa.MarkGps.length; i++) {
                mappa.MarkGps[i].setMap(null);
            }
        }
    }
    mappa.MarkGps = new Array();



    if (mappa.Livelli !== null) {
        if (mappa.Livelli != undefined) {
            for (i = 0; i < mappa.Livelli.length; i++) {
                for (var kk = 0; kk < mappa.Livelli[i].poligoni.length; kk++) {
                    mappa.Livelli[i].poligoni[kk].setMap(null);
                }
                for (var kk = 0; kk < mappa.Livelli[i].punti.length; kk++) {
                    mappa.Livelli[i].punti[kk].setMap(null);
                }
                for (var kk = 0; kk < mappa.Livelli[i].circle.length; kk++) {
                    mappa.Livelli[i].circle[kk].setMap(null);
                }
                for (var kk = 0; kk < mappa.Livelli[i].polyline.length; kk++) {
                    mappa.Livelli[i].polyline[kk].setMap(null);
                }
                for (var kk = 0; kk < mappa.Livelli[i].ABLabel.length; kk++) {
                    mappa.Livelli[i].ABLabel[kk].setMap(null);
                }

                mappa.Livelli[i].poligoni = new Array();
                mappa.Livelli[i].punti = new Array();
                mappa.Livelli[i].circle = new Array();
                mappa.Livelli[i].polyline = new Array();
                mappa.Livelli[i].ABLabel = new Array();
            }
        }
    }
}
function svuotaTutto() {

    for (var i = 0; i < mappa.Livelli.length; i++) {
        for (var kk = 0; kk < mappa.Livelli[i].poligoni.length; kk++) {
            mappa.Livelli[i].poligoni[kk].setMap(null);
        }
        for (var kk = 0; kk < mappa.Livelli[i].punti.length; kk++) {
            mappa.Livelli[i].punti[kk].setMap(null);
        }
        for (var kk = 0; kk < mappa.Livelli[i].circle.length; kk++) {
            mappa.Livelli[i].circle[kk].setMap(null);
        }
        for (var kk = 0; kk < mappa.Livelli[i].polyline.length; kk++) {
            mappa.Livelli[i].polyline[kk].setMap(null);
        }
        for (var kk = 0; kk < mappa.Livelli[i].ABLabel.length; kk++) {
            mappa.Livelli[i].ABLabel[kk].setMap(null);
        }
    }

}


//#region "per sessione"
var sec = 7;
var cdID;

function impostaRedirect() {
    var m = "<p>Pagina di login ... (" + sec.toString() + ")</p>";
    $('#lblAutoLogin').html(m);
    if (sec == 0) {
        window.location = "../GST_Autenticazione/Autenticazione.aspx";
        window.clearInterval(cdID);
    }

    --sec;

}

function impostaRedirectStart() {
    cdID = window.setInterval('impostaRedirect();', 1000);
}

//# end region "per sessione"


function coordinateFromViaCentro() {
    var Layer_Selezionato;
    Layer_Selezionato = $('#tipologia_layer').val();
    $.ajax({
        type: "POST",
        url: indirizzohttp + "/coordinateFromViaCentro",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                $('#address').val(msg.d);
                $('#CercaIndirizzo').click();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });

}


function AggiornaLayer(autoFit, callback) {
    if (mappa.Livelli !== null)
        svuotaTutto_nuoviVettori();

    var Layer_Selezionato;
    Layer_Selezionato = $('#tipologia_layer').val();

    interfaccia.loading(true);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/AggiornaLayer",
        data: "{ Layer_Selezionato: '" + Layer_Selezionato + "', AggiornaDatiGiasAlarm: '" + AggiornaDatiGiasAlarm.toString() + "', Sementieri_Sportello_Configurazione_cod: '" + Sementieri_Sportello_Configurazione_cod.toString() + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                utility.warn("AggiornaLayer");
                if (msg.d == "[]") {
                    //mi posiziono sul centro dell'indirizzo
                    coordinateFromViaCentro();
                    interfaccia.loading(false);
                }
                else {

                    var start;
                    var diff;

                    start = (new Date).getTime();

                    //parte core: popolo oggetto place da lettura server.
                    place = JSON.parse(msg.d);                    

                    diff = (new Date).getTime() - start;
                    utility.log("spacchetta= " + diff + ")");

                    var l_place = place.length;
                    var l_livelli = mappa.Livelli.length;
                    start = (new Date).getTime();
                    mappa.ImpostaShape(true, autoFit);
                    diff = (new Date).getTime() - start;
                    utility.log("tutto impostashape= " + diff + ")");

                    GestisciStrumentoScomponiRicomponi();
                }
            }

            if (typeof callback === "function") {
                callback();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
};


function selezionaEntita(strEntita) {

    if (typeof strEntita !== "string") {
        return;
    }
    if (strEntita === "") {
        return;
    }

    //GABRIELE
    let arrEntita = strEntita.split("|");
    if (false) {

        for (e = 0; e < arrEntita.length; e++) {

            let entita_cod = arrEntita[e];
            let found = false;

            for (l = 0; l < mappa.Livelli.length && !found; l++) {

                let livello = mappa.Livelli[l];

                for (p = 0; p < livello.poligoni.length && !found; p++) {

                    let poligono = livello.poligoni[p];

                    if (poligono.Entita_Cod === entita_cod) {

                        found = true;

                        let debug = polylabel(poligono.getPath().getArray());
                        let lat = debug.lat();
                        let lng = debug.lng();

                        let latlng = new google.maps.LatLng(lat, lng);
                        let marker = new google.maps.Marker({ position: latlng });
                        marker.setMap(mappa.elemenotMappa);
                        if (e === 0) {
                            mappa.elemenotMappa.setCenter(latlng);
                            mappa.elemenotMappa.setZoom(14);
                        }
                    }
                }
            }
        }

    } else {

        var latlngBounds = new google.maps.LatLngBounds();
        var latlngCenter = null;

        for (l = 0; l < mappa.Livelli.length; l++) {

            let livello = mappa.Livelli[l];
            let p = 0;

            while (p < livello.poligoni.length) {

                let poligono = livello.poligoni[p];

                let found = false;
                for (e = 0; e < arrEntita.length && !found; e++) {

                    let entita_cod = arrEntita[e];
                    if (poligono.Entita_Cod === entita_cod) {

                        found = true;

                        let pl = new PolyLabel();
                        let p_arr = poligono.getPath().getArray();
                        for (v = 0; v < p_arr.length; v++) {
                            pl.add(p_arr[v]);
                            latlngBounds.extend(p_arr[v]);
                        }

                        if (latlngCenter === null || e === 0) {
                            let pos = pl.position();
                            latlngCenter = new google.maps.LatLng(pos.lat(), pos.lng());
                        }

                        //if (e == 0) {
                        //    poligono.fillOpacity = 1;
                        //} else {
                        //    poligono.fillOpacity = 0.3;
                        //}
                    }
                }

                if (!found) {

                    poligono.setMap(null);

                    livello.poligoni.splice(p, 1);

                } else {

                    poligono.cancellazione = "False";
                    poligono.inserimento = "False";
                    poligono.modifica = "False";

                    p++;

                }
            }
        }

        if (latlngCenter !== null) {

            mappa.elemenotMappa.setCenter(latlngCenter);
            mappa.elemenotMappa.setZoom(15);
            //let in_ne = mappa.elemenotMappa.getBounds().contains(latlngBounds.getNorthEast());
            //let in_sw = mappa.elemenotMappa.getBounds().contains(latlngBounds.getSouthWest());
            //if (!in_ne || !in_sw ) {
            //    mappa.elemenotMappa.fitBounds(latlngBounds);
            //}
            mappa.elemenotMappa.fitBounds(latlngBounds);
        }
    }
}

//#region "Operazioni Agenda"

function apriPreselezioneAgenda() {

    var Entita_Cod;
    Entita_Cod = getEntitaCod();

    if (Entita_Cod == "") {
        alert("Nessun elemento selezionato");
        return "true";
    }

    if (Entita_Cod.split(separatoreChiaveAlbero)[25] != "0") {
        preselezioneAgenda("0");
    } else {
        apriPreselezioneAgenda();
    }


}

function apriPreselezioneVisita() {

    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();

    if (ChiaveAlbero === "") {
        alert("Selezionare un'impianto.")
        return false;
    }


    if ($("#lat_cerca").val() == "") {
        CoordOk = mappa.CoordDaSelezione();
    } else {
        CoordOk = true;
    }

    if (!CoordOk) {
        $("#espandi_indirizzo").click();

        if (ChiaveAlbero == "") {
            alert("Occorre selezionare un elemento grafico.");
            return false;
        }
    }

    var lat = $("#lat_cerca").val();
    var lng = $("#long_cerca").val();

    ajaxAgronica(indirizzohttp + "/visite",
        "{ lat: '" + lat + "', lng: '" + lng + "', ChiaveAlbero: '" + ChiaveAlbero + "' }",
        function (risposta) {
            $("#framepop_up_visite").attr("src", "about:blank");
            $("#framepop_up_visite").attr("src", risposta.RispostaStringa);
            utility.log("redir to: " + risposta.RispostaStringa);
            $("#pop_up_visite").dialog("open");
        }, null);
}


function apriPreselezioneAgenda() {

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/ElencoOperazioniAgendaGraficabili",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {

                if (!stringhe.startsWith(msg.d, '<')) {
                    alert(msg.d);
                } else {
                    utility.log("#contentpop_up_opAgenda_Preselezione = " + msg.d);
                    $("#contentpop_up_opAgenda_Preselezione").html(msg.d);
                    $("#pop_up_opAgenda_Preselezione").dialog("open");
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
            alert(xhr.status);
            alert(thrownError);
        }
    });


}
function getEntitaCod() {
    //selezione multipla --> ok passo alla ricetta
    var listaImpiantiDaChiaveAlbero = $("#ChiaveAlberoMultiSelezione").val();
    if (listaImpiantiDaChiaveAlbero != "") {
        utility.log("ChiaveAlberoMultiSelezione: " + listaImpiantiDaChiaveAlbero);
        listaImpiantiDaChiaveAlbero = listaImpiantiDaChiaveAlbero.substring(0, listaImpiantiDaChiaveAlbero.length - 2);
        Entita_Cod = listaImpiantiDaChiaveAlbero;
    }
    else {
        Entita_Cod = $('#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica').val().toString();  //.replace(/\\/g, '\\\\'); //shape.selectedShape.chiavealbero.toString();       
    }

    return Entita_Cod;
}


function preselezioneAgenda(lav_cod) {

    var Entita_Cod = "";
    Entita_Cod = getEntitaCod();

    if (Entita_Cod == "") {
        alert("Nessun Elemento selezionato");
        return true;
    }

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/clickOpAgenda",
        data: "{ lav_cod: '" + lav_cod + "', ChiaveAlbero: '" + Entita_Cod + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {

                if (stringhe.startsWith(msg.d, 'errore')) {
                    alert(msg.d);
                } else {
                    $("#framepop_up_opAgenda").attr("src", "about:blank");
                    $("#framepop_up_opAgenda").attr("src", msg.d);
                    utility.log("redir to: " + msg.d);
                    $("#pop_up_opAgenda").dialog("open");
                    $("#pop_up_opAgenda_Preselezione").dialog("close");
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
            alert(xhr.status);
            alert(thrownError);
        }
    });

}

function chiudiFinestra(id) {
    $(id).dialog("close");
}


//#end region "Operazioni Agenda"



//#region "gestione multipoint"

var A_Multipoint = new Array();
var iLast = 0;

function addMarkerMultiPointModifica(codice, posizione) {
    $("#hiddenMultipointModifica").val($("#hiddenMultipointModifica").val() + codice + '^' + posizione + '§');
}


function addMarkerMultiPoint(obj, hiddenMultipoint, A, posizione) {

    if (obj.is(":checked")) {

        if (A == null) {
            utility.log("crea nuovo multipoint a:");
            A = new google.maps.Marker({
                position: posizione,
                map: mappa.elemenotMappa,
                draggable: true
            });
        }
        else {
            A.draggable = true;
        }
        A_Multipoint.push(A);
        hiddenMultipoint.val(hiddenMultipoint.val() + '(' + A_Multipoint[iLast].getPosition().lat() + ', ' + A_Multipoint[iLast].getPosition().lng() + ')|');

        utility.log("aggiunto: hiddenMultipoint.val" + hiddenMultipoint.val());

        iLast = iLast + 1;
    }
}

function ConfermaSalvataggioMultipoint() {
    interfaccia.loading(true);

    var hiddenPunti_M = $('#hiddenMultipoint').val();
    var txtDialogMultipointDes = $("#txtDialogMultipointDes").val();


    //    utility.log('M = ' + hiddenPunti_M);

    //se sono tutte valorizzate allora posso procedere
    var blocca = false;

    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString(); //.replace(/\\/g, '\\\\'); ;


    //utility.log(ChiaveAlbero);

    //se sono tutte valorizzate allora posso procedere
    if (ChiaveAlbero == "") {
        alert("nessun Elemento selezionato.");
        interfaccia.loading(false);
        return false;
    }

    if (blocca) {
        alert("completare i campi obbligatori");
        interfaccia.loading(false);
        return false;
    }

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/SalvaNuovoMultipoint",
        data: "{ chiaveAlbero: '" + ChiaveAlbero + "', hiddenPunti_M: '" + hiddenPunti_M + "', txtDialogMultipointDes: '" + txtDialogMultipointDes + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                utility.log("SalvaNuovoMultipoint, valore restituito: " + msg.d);
                if (msg.d.split(".")[0] == 'Operazione eseguita correttamente') {
                    $("#dialogMultipoint").dialog("close");
                    $('#hiddenMultipoint').val('');
                    for (var i = 0; i < A_Multipoint.length; i++) {
                        A_Multipoint[i].setMap(null);
                    }
                    AggiornaTutto();
                }
                else {
                    alert(msg.d);
                }
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

//# end region "gestione multipoint"




//da cambiare
function ricercaIndirizzo() {
    mappa.ricercaIndirizzo();
}


function CoordDaSelezione() {

    mappa.CoordDaSelezione();

}

function ApriGoogleMaps() {


    if ($("#lat_cerca").val() == "") {
        mappa.CoordDaSelezione();
    }


    var gDestinazione = "";
    if ($("#lat_cerca").val() != "" && $("#long_cerca").val() != "") {
        gDestinazione = $("#lat_cerca").val() + "+" + $("#long_cerca").val();
    } else {
        gDestinazione = $("#address").val();
    }


    if (gDestinazione != "") {
        var goURL = "https://www.google.it/maps/dir//" + gDestinazione + "/";
        window.open(goURL);
    } else {
        alert("Nessuna  coordinata, nessun indirizzo impostato.");
    }


}


function GiasPalmDettagli(piva, sa_cod, id) {
    utility.log("GiasPalmDettagli..");
    mappa.GiasPalmDettagli(piva, sa_cod, id);
}