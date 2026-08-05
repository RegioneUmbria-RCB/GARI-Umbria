
/* scriptMaps_DI_PARTENZA.js */

//Variabili Globali

//GABRIELE 2020-04-20
//hasTouchCapabilities = 'ontouchstart' in window && (navigator.maxTouchPoints || navigator.msMaxTouchPoints);
var GisMobileMode = Boolean(window.matchMedia('(max-width: 767px)').matches && ('ontouchstart' in window && (navigator.maxTouchPoints || navigator.msMaxTouchPoints)));

var Debug_Mode = Enum_debugMode.Off;

var gis_mappa_idle_wait_seconds_Trigger = null;

//secondi di attesa dall'utlimo evento di mappa IDLE
var gis_mappa_idle_wait_seconds_Trigger_TimeOut = 3500;

//distanza in metri dall'ultimo evento di mappa IDLE per far scattare l'evento
var gis_mappa_idle_DistanzaUpdate_SAT_MAP = 30000;

var Enum_StrumentoGPS_State = {
    Off: { value: 0, name: "Off", code: 0 },
    On: { value: 1, name: "On", code: 1 }
};

//indica il tipo di render, client side
var Enum_TipoRender = {
    Completa: { value: 0, name: "Completa", code: 0 },
    Parziale: { value: 1, name: "Parziale", code: 1 }
};

//indica il tipo di render, Server side
var Enum_TipoRender_ServerSide = {
    Completa: { value: 0, name: "Completa", code: 0 },
    Parziale: { value: 1, name: "Parziale", code: 1 }
};

//indica il tipo di salvataggio in fase di modifica impianto
var Enum_TipoModifica = {
    ModificaImpianto: { value: 0, name: "ModificaImpianto", code: 0 },
    SalvataggioDiretto: { value: 1, name: "SalvataggioDiretto", code: 1 }
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

var Enum_PropagazioneSalvataggioPoligono = {
    NessunaPropagazione: { value: 0, name: "NessunaPropagazione", code: 0 },
    Appezzamento: { value: 1, name: "Appezzamento", code: 1 },
    Impianto: { value: 2, name: "Impianto", code: 2 },
    ImpiantoEdAppezzamento: { value: 3, name: "ImpiantoEdAppezzamento", code: 3 },
    ImpiantoSuAppezzamentoEsistente: { value: 4, name: "ImpiantoSuAppezzamentoEsistente", code: 4 }
};

var preparaXSalvataggioPoligoni_SavePlus = false;

var TipoRender = Enum_TipoRender.Completa;
var TipoRender_ServerSide = Enum_TipoRender_ServerSide.Completa;

var Render_ServerSideOk = false;

var miniWindowEditToolbar_width = 100;
var miniWindowEditToolbar_height = "auto";//230;
var miniWindowEditToolbar_offSet_Open = 645;
var miniWindowEditToolbar_offSet_Closed = 370;

//Posizionamento Finestra Importazione Dati GIS
var ImportazioneDatiGisPercWidth = 70;
var ImportazioneDatiGisMinWidth = 1022;
var ImportazioneDatiGisHeight = "98%";
var ImportazioneDatiGisTop = "0px";
var ImportazioneDatiGisRight = 2;
var nElementiNascosti = 0;

var response_ok;
//var geocoder;
//var Livelli;
//var mappa.MarkGps;
//var Testi = new Array();
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
//var colorButtons = {};

var GiasBase_Path = GiasBase_Domain + PATH_GIASBASE + "agronica/scripts/";
var GoogleUtility_Path = GiasBase_Path + "GoogleMaps/v3-utility-library/";
var MarkerClusterer_Path = GoogleUtility_Path + "markerclusterer";
var MarkerClusterer_ZoomLevel = 2000;

//var cosaStoDisegnando = google.maps.drawing.OverlayType.POLYGON;
//var shape.cosaPossoDisegnare = new Array();
//var gisTipoOggettoXLavCod = new Array();
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

var ModalitaBootstrap = false;
var AggiornaDatiGiasAlarm = false;
var CiSonoVecchiDatiNonImportati = false;
//var glayerDoveDisegno = "-1";
var gOperazioneDiAgendaDoveDisegno = "-1";
var gNuovo = false;
var isDebug = false;
var AbilitaPF = false;
var Codice_Fiscale_Tecnico = "";

//var overlay;

var CoordFromPoints;

var indirizzohttp = location.pathname;
indirizzohttp = indirizzohttp.split("/")[indirizzohttp.split("/").length - 1];

//mantenere allineato con codice VB
var Enum_GisPurpose = {
    Completo: { value: 0, name: "Completo", code: 0 },
    SementiSportello: { value: 1, name: "SementiSportello", code: 1 },
    SementiMappaturaLibera: { value: 2, name: "SementiMappaturaLibera", code: 2 }
};

var GisPurpose = Enum_GisPurpose.Completo;

switch (GisAjaxSync("LeggiGisPurpose", {})) {
    case Enum_GisPurpose.Completo.value:
        GisPurpose = Enum_GisPurpose.Completo;
        break;
    case Enum_GisPurpose.SementiSportello.value:
        GisPurpose = Enum_GisPurpose.SementiSportello;
        break;
    case Enum_GisPurpose.SementiMappaturaLibera.value:
        GisPurpose = Enum_GisPurpose.SementiMappaturaLibera;
        break;
}

var maxH;

var AperturaLayer = false;

var OffSet_contenitore_Tool = 20;
var mappa_zoom_mostraEtichette = 16;
var mappa_zoom_mostraPunti = 16;
var mappa_zoom_BloccoChiamateServer = 16;
var bAbilitaRenderPerEventoZoom = true;
var bAbilitaRenderPerEventoDrag = true;

//delay in millisecondi
var mappa_drag_delay = 2500;
var mappa_zoom_delay = 1000;

var ctrlPressed = false;

var PermessiGisServerSide = {
    bool_catasto: false,
    bool_precision: false,
    bool_esportazione: false,
    bool_bufferzone: false
}

//fine variabili globali

document.onkeydown = function (event) {
    ctrlPressed = event.ctrlKey && AttivaSelezioneMultipla;
};

document.onkeyup = function (event) {
    ctrlPressed = false;
};

//jQuery.logThis = function (text) {
//    if ((window['console'] != undefined)) {
//        console.log(text);
//    }
//}

// 

function SelectPoint(shape, flag) {
    if (flag) {
        shape.setIcon(shape.icon_sel);
    } else {
        shape.setIcon(shape.icon_norm);
    }
}

//GABRIELE 11 04 2019
function SelectShape(shape, flag) {
    if (flag) {

        shape.set('fillColor', shape.selectedColor);
        //shape.strokeWeight = 3;


    } else {

        shape.set('fillColor', shape.colore_precedente);
        shape.setEditable(false);

    }
}

function selezionaIndiceLayerDoveScrivoDaAlbero(livello, chiaveAlbero) {
    utility.warn('selezionaIndiceLayerDoveScrivoDaAlbero(livello:=$(' + livello + "), chiaveAlbero:=" + chiaveAlbero);

    var chiaviAlberoLayer = livello.TipoNodoAlberoAnagrafe.split(",");
    //var chiaveID = chiaveAlbero.split("\\")[0];
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
    mappa.ImpostaShape(false, false);
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

    if ($("#cmbViste option").length === 2) {
        $("#cmbViste").val(vDati[0].split("§")[0] + '§');

        setTimeout(function () {
            cmbViste_change();
        }, 1000);

    }

}

/********************************************************************/
/********************** INIT ****************************************/
/********************************************************************/

/* inizializzazione della mappa */

//#Region "Eventi google maps"

function mappa_bounds_changed() {
    utility.log("mappa_bounds_changed");

    //se viene richiesto un'aggiornamento lato server allora imposta il "!"
    if (TipoRender_ServerSide == Enum_TipoRender_ServerSide.Parziale) {
        AggiornamentoPendente(true);
    }
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

var AggiornaDate_Client_gestione_base_gl1;
var AggiornaDate_Client_gestione_base_gl2;
var AggiornaDate_Client_gestione_gl1;
var AggiornaDate_Client_gestione_gl2;
var AggiornaDate_Client_gestione_base_cmbVal;
var AggiornaDate_Client_gestione_cmbVal;

function AggiornaDate_Client_MemorizzaPrecedenti() {

    if (!AggiornaDate_Client_gestione_gl1) {
        AggiornaDate_Client_gestione_base_gl1 = $("#limita_data_da").val();
        AggiornaDate_Client_gestione_base_gl2 = $("#limita_data_a").val();
        AggiornaDate_Client_gestione_base_cmbVal = $("#ddlDateVisualizzaAvanzata_RicercheComuni").data("kendoDropDownList").value();
    }

    AggiornaDate_Client_gestione_gl1 = $("#limita_data_da").val();
    AggiornaDate_Client_gestione_gl2 = $("#limita_data_a").val();
    AggiornaDate_Client_gestione_cmbVal = $("#ddlDateVisualizzaAvanzata_RicercheComuni").data("kendoDropDownList").value();
}

function AggiornaDate_Client_ReimpostaPrecedenti() {
    $("#limita_data_da").val(AggiornaDate_Client_gestione_gl1);
    $("#limita_data_a").val(AggiornaDate_Client_gestione_gl2);
    $("#ddlDateVisualizzaAvanzata_RicercheComuni").data("kendoDropDownList").value(AggiornaDate_Client_gestione_cmbVal);
}

function AggiornaDate_Client_gestione() {

    let v1 = $("#limita_data_da").val();
    let v2 = $("#limita_data_a").val();


    if (v1 !== "" || v2 !== "") {
        AggiornaDate_Client_warning(true);
        if (AggiornaDate_Client_gestione_FiltroCambiato(v1, v2)) {
            AggiornamentoPendente(true);
        }
    } else {
        AggiornaDate_Client_warning(false);
    }


}

function AggiornaDate_Client_gestione_FiltroCambiato(v1, v2) {

    if (!v1)
        v1 = $("#limita_data_da").val();
    if (!v2)
        v2 = $("#limita_data_a").val();

    return ((AggiornaDate_Client_gestione_gl1 !== undefined) && (v1 !== AggiornaDate_Client_gestione_gl1 || v2 !== AggiornaDate_Client_gestione_gl2));
}

function AggiornaDate_Client_warning(mostra) {

    if (mostra) {
        $("#AggiornaDate_Client_warning").show();
        AggiornamentoPendente(true, "#AggiornaDate_Client");
    } else {
        $("#AggiornaDate_Client_warning").hide();
        AggiornamentoPendente(false, "#AggiornaDate_Client");
    }

}

function AggiornamentoPendente(mostra, divJQuery) {
    //$("#AggiornamentoPendente").hide();

    if (!divJQuery) {
        divJQuery = "#AggiornaFiltro_Client";
    }

    if (mostra) {
        $(divJQuery).addClass("aggiornamento-pendente");
        if ($(divJQuery).children("#rWn").length === 0) {
            $("<span id='rWn' class='fa fa-warning'></span>").appendTo(divJQuery);
        }
        //GABRIELE $('#ddl_Sa_Cod_html').blur();
    } else {
        $(divJQuery).removeClass("aggiornamento-pendente");
        if ($(divJQuery).children("#rWn").length === 1) {
            $(divJQuery).children("#rWn").remove();
        }
    }
}

var mappa_idle_resized = false;

function mappa_tilesloaded() {
    if (!mappa_idle_resized) {

        //GABRIELE 2020-08-27
        let piva = Request_QueryString("piva");
        let sa_cod = Request_QueryString("sa_cod");
        let entita_cod = Request_QueryString("entita_cod");

        if (piva === null || sa_cod === null || entita_cod === null) {

            // devo lasciare la tipologia layer a "Standard Entità"

            setTimeout(function () {

                ImpostaTipologiaLayerDaCookie();

            }, 1000)

        } else {

            setTimeout(function () {

                Global_Entita_Cod = entita_cod

                $("#ddl_azienda_html").data("kendoDropDownList").value(piva);

                ImpostaPivaSessione(piva);

                CaricaDDLCentro(true, sa_cod);

            }, 1000);
        }

    }

    mappa_idle_resized = true;
}

function mappa_idle() {

    utility.log("mappa_idle");

    bAbilitaRenderPerEventoZoom = true;

    //se viene richiesto un'aggiornamento lato server allora rimuove il "!"
    if (TipoRender_ServerSide == Enum_TipoRender_ServerSide.Parziale && Render_ServerSideOk) {
        AggiornamentoPendente(false);
        Render_ServerSideOk = false;
    }


    mappa_idle_disegnaEtichette();

    clearTimeout();
    gis_mappa_idle_wait_seconds_Trigger = setTimeout("mappa_idle_seconds()", gis_mappa_idle_wait_seconds_Trigger_TimeOut);
}

function mappa_idle_disegnaEtichette() {
    if (mappa.elemenotMappa) {
        mappa.checkVisibleMarkerLabels();
    }
}

function mappa_idle_seconds() {

    //gestione mappe satellitari...
    mappa_idle_satCal();

    //altre funzioni da chiamare dopo l'evento idle

}

var mappa_idle_satCal_lat = 0;
var mappa_idle_satCal_lng = 0;

function mappa_idle_satCal() {
    //se è attiva la gestione delle mappe satellitari avvio un'aggiornamento del calendario.
    if (DemoGisAttivo) {

        var bounds = mappa.elemenotMappa.getBounds();
        if (bounds !== undefined) {

            //lat, lng precedenti
            var latitude1 = mappa_idle_satCal_lat;
            var longitude1 = mappa_idle_satCal_lng;

            //lat, lgn correnti
            var latitude2 = bounds.getNorthEast().lat();
            var longitude2 = bounds.getNorthEast().lng();

            var distance = gis_mappa_idle_DistanzaUpdate_SAT_MAP + 1;
            if (mappa_idle_satCal_lat !== 0) {
                distance = google.maps.geometry.spherical.computeDistanceBetween(new google.maps.LatLng(latitude1, longitude1), new google.maps.LatLng(latitude2, longitude2));
            }

            console.log("mappa_idle_satCal, distanza da evento idle precedente ..: " + distance);
            if (distance > gis_mappa_idle_DistanzaUpdate_SAT_MAP) {
                //re-imposto distanza precedente su corrente..:
                mappa_idle_satCal_lat = latitude2;
                mappa_idle_satCal_lng = longitude2;

                //chiamo funzione
                customMapOverlayBaseInizializzaCalendario();
            }
        }
    }

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

function CircleSetRadiusSingolo(elemento, p, dParametroScala) {
    elemento.setRadius(p * dParametroScala); //p * 1128.497220 * 0.0027);
}

function CircleSetRadius(dParametroScala) {
    var zLevel = mappa.elemenotMappa.zoom;
    var p = Math.pow(2, (21 - zLevel));

    for (var iLivello = 0; iLivello < mappa.Livelli.length; iLivello++) {

        if (mappa.Livelli[iLivello].circle !== undefined) {
            for (var iCerchio = 0; iCerchio < mappa.Livelli[iLivello].circle.length; iCerchio++) {

                var colore = mappa.Livelli[iLivello].circle[iCerchio].fillColor;
                var dScalaColore = 1;
                if (colore !== "#000000") {
                    dScalaColore = 1.3;
                }
                CircleSetRadiusSingolo(mappa.Livelli[iLivello].circle[iCerchio], p, dParametroScala * dScalaColore); //p * 1128.497220 * 0.0027);
            }
        }

    }
}

/**
 * Evento di click sul poligono
 * @param {any} e
 */
function mappa_clickPoligono_event(e) {
    mappa_click_sat(e);
}

/**
 * Evento di click sulla mappa "libera" (senza poligono)
 * @param {any} e
 */
function mappa_click_event(e) {
    clearSelection();
    mappa_click_sat(e);
}

function mappa_click_sat(e) {

    if ($("#satChkAggiornaClickMappa").is(":checked")) {

        var dataSel = $("#pf_data").data("kendoCalendar").value();
        if (!dataSel) {
            dataSel = new Date();
        }
        var anno = dataSel.getUTCFullYear();
        var dataDa = "01/01/" + anno;
        var dataA = "31/12/" + anno;
        var latLng = e.latLng;

        var puntoWKT = "POINT(" + latLng.lng() + " " + latLng.lat() + ")";
        LetturaDatiElaboratiSuSensore(puntoWKT, mappa.elemenotMappa.zoom, $("#PfDdlSensoreElaborazione").data("kendoDropDownList").value(), dataDa, dataA);
    }

    if ($("#wmsChkAggiornaClickMappa").is(":checked")) {
        WmsGetFeatureInfo(e.latLng);
    }
}

function mappa_zoom_changed() {

    utility.log("mappa_zoom_changed (out)..." + mappa.elemenotMappa.zoom.toString());

    CircleSetRadius(1.5);

    ////se viene richiesto un'aggiornamento lato server allora imposta il "!"
    //if (TipoRender_ServerSide == Enum_TipoRender_ServerSide.Parziale) {
    //    AggiornamentoPendente(true);
    //}

    if (TipoRender == Enum_TipoRender.Parziale) {

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

function CreaPoligonoHeader() {
    $("#disenga_poligonoHeader").addClass("green");
    interfaccia.switchDrawingMode(google.maps.drawing.OverlayType.POLYGON);
    interfaccia.CreaPoligono();
}

function multipointHeader() {
    if ($("#multipointHeader").hasClass("green")) {
        ApriKendoDialog("#dialogMultipoint");
    } else {
        $("#multipointHeader").addClass("green");
        interfaccia.switchDrawingMode(google.maps.drawing.OverlayType.MARKER);
        interfaccia.CreaPoligono();
    }
}

function SelezionaPoligono() {
    mappa.SelezionaPoligono();
}

function Info_Poligono() {
    mappa.Info_Poligono();
}

var StrumentoGPS_State = Enum_StrumentoGPS_State.Off;

function StrumentoGPS() {

    if (StrumentoGPS_State === Enum_StrumentoGPS_State.Off) {
        $(".StrumentoGPS").addClass("gis-red");
        StrumentoGPS_State = Enum_StrumentoGPS_State.On;
    } else {
        $(".StrumentoGPS").removeClass("gis-red");
        StrumentoGPS_State = Enum_StrumentoGPS_State.Off;
        mappa.NascondiMarker();
    }

    CoordFromGPS();
}

var pressTimer;

function InizializzaMenuCosaDisegno() {
    //GABRIELE 03 04 2019
    /*
        utility.log("inizializza Long Press!");
        $("#disenga_poligono").mouseup(function () {
            clearTimeout(pressTimer);
            // Clear timeout
            utility.log("mouseup Long Press!");
            return false;
        }).mousedown(function () {
            // Set timeout
            pressTimer = window.setTimeout(function () { mostraMenuCosaDisegno() }, 2000)
            return false;
        });
    */
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

function GestioneEtichettaEstraiTesto(etichetta, tipoEtichetta) {

    var vEti = etichetta.split("|");
    var vRes = [];

    for (var i = 0; i < vEti.length; i++) {
        if (vEti[i][0] === "^") {
            vRes.push("§ " + vEti[i].replace("^", "").split("§")[1]);
        }
    }

    if (vRes.length == 0) {
        return "";
    }

    etichetta = vRes.join("|");

    if (tipoEtichetta === 1) {
        etichetta = etichetta.replace(/§/g, ":").replace(/\|/g, "<br />");
    } else {
        etichetta = etichetta.replace(/§/g, "");
    }

    return etichetta;
}

/**
 * Gestisce la generazione di un'etichetta con il plugin InfoBox, gestisce anche il newLine su §
 * @param {number} lat_centro latitudine
 * @param {number} lng_centro longitudine
 * @param {string} etichetta etichetta
 * @param {any} pEtichetta oggetto google point 
 */
function GestioneEtichetta(lat_centro, lng_centro, etichetta, pEtichetta) {

    //if (tipoEtichetta === undefined) {
    //    tipoEtichetta = 1;
    //}

    etichetta = GestioneEtichettaEstraiTesto(etichetta, 1);

    if (etichetta == '') {
        return;
    }

    var myLatlngCentro = new google.maps.LatLng(lat_centro, lng_centro);


    var boxText = document.createElement("div");

    boxText.style.cssText = "border: 1px solid black; margin-top: 8px; background: yellow; padding: 5px;";
    boxText.innerHTML = etichetta;

    var myOptions = {
        content: boxText
        , disableAutoPan: false
        , maxWidth: 0
        , pixelOffset: new google.maps.Size(0, 0)
        , zIndex: null
        , boxStyle: {
            background: "url('tipbox.gif') no-repeat"
            , opacity: 0.75
            , width: "auto"
        }
        , closeBoxMargin: "10px 2px 2px 2px"
        , closeBoxURL: "https://www.google.com/intl/en_us/mapfiles/close.gif"
        , infoBoxClearance: new google.maps.Size(1, 1)
        , isHidden: false
        , pane: "floatPane"
        , enableEventPropagation: false
    };

    var ibLabel = new InfoBox(myOptions);
    ibLabel.open(mappa.elemenotMappa, pEtichetta);


}

/**
 * Aggiunge gli oggetti ottenuti lato server
 * @param {any} p_place
 * @param {any} k_livelli
 * @param {any} colore_poligon
 * @param {any} polygonPaths
 * @param {any} lat_centro
 * @param {any} lng_centro
 * @param {any} circle_radius
 * @param {any} circle_opacity
 */
function ShapeAggiungiOggetti(p_place, k_livelli, colore_poligon, polygonPaths, lat_centro, lng_centro, circle_radius, circle_opacity) {


    //if (p_place.StandardEntita_layerDiAppartenenza === 1) {

    //    // Define the LatLng coordinates for the polygon's inner path.
    //    // Note that the points forming the inner path are wound in the
    //    // opposite direction to those in the outer path, to form the hole.
    //    let innerCoords = [
    //        { lat: 44.103653315892558, lng: 12.273610243822141 },
    //        { lat: 44.058767125460918, lng: 12.226918349290891 },
    //        { lat: 44.117950550679367, lng: 12.202199111009641 }
    //    ];
    //    let outerCoords = polygonPaths;

    //    let poligonoDebug = new google.maps.Polygon({
    //        paths: [outerCoords, innerCoords],
    //        strokeColor: "red",
    //        fillOpacity: 0.7,
    //        strokeWeight: 1,
    //        fillColor: "red",
    //    });

    //    poligonoDebug.setMap(mappa.elemenotMappa);


    //}
    //return;

    var xEtichetta = "";

    colore_poligon = aggiungiCancellettoSeNonEsiste(colore_poligon);

    //TODO: Rendere configurabile fillOpacity
    var trasparenza = k_livelli.trasparenza;
    if (trasparenza === undefined) {
        trasparenza = 0.6;
    }

    switch (p_place.TipologiaGML) {
        case 'Point':
            var isDraggable = false;//GABRIELE  !(p_place.layer == 55)

            //---------------------------------------------------------------
            //var icon = {
            //    path: "M-12,0a12,12 0 1,0 24,0a12,12 0 1,0 -24,0",
            //    fillColor: colore_poligon,
            //    fillOpacity: trasparenza,
            //    anchor: new google.maps.Point(0, 10),
            //    strokeColor: colore_poligon,
            //    strokeWeight: 1//,                scale: 0.5
            //};
            //---------------------------------------------------------------

            let strokeClr = "red";
            if (colore_poligon !== "") {
                strokeClr = aggiungiCancellettoSeNonEsiste(colore_poligon);
            }
            let path = "M-7,0 a 7,7 0 1, 0 14,0 a 7,7 0 1,0 -14,0";
            let circle = {
                path: path,
                fillColor: "transparent", //"yellow",
                strokeColor: strokeClr,
                //fillOpacity: .8,
                strokeWeight: 2,
                scale: 1,
                labelOrigin: new google.maps.Point(0, -14)
            };

            //GABRIELE prova disegno altro Marker
            //let width = 14;
            //let height = 14;
            //let radius = 3;

            //let canvas = document.createElement("canvas");
            //canvas.width = width;
            //canvas.height = height;
            //let context = canvas.getContext("2d");
            //context.clearRect(0, 0, width, height);
            //context.fillStyle = "rgba(255,255,0,1)";
            //context.strokeStyle = "rgba(0,0,0,1)";
            //context.beginPath();
            //context.moveTo(radius, 0);
            //context.lineTo(width - radius, 0);
            //context.quadraticCurveTo(width, 0, width, radius);
            //context.lineTo(width, height - radius);
            //context.quadraticCurveTo(width, height, width - radius, height);
            //context.lineTo(radius, height);
            //context.quadraticCurveTo(0, height, 0, height - radius);
            //context.lineTo(0, radius);
            //context.quadraticCurveTo(0, 0, radius, 0);
            //context.closePath();
            //context.fill();
            //context.stroke();

            //circle = {
            //    url: canvas.toDataURL(),
            //    labelOrigin: new google.maps.Point(7, -7)
            //};

            let circle_sel = {
                path: path,
                fillColor: strokeClr,//"#ffbf00",
                strokeColor: strokeClr,//"#ffbf00",
                fillOpacity: .8,
                strokeWeight: 2,
                scale: 1,
                labelOrigin: new google.maps.Point(0, -14)
            };

            let etichetta1 = p_place.etichetta;
            if (etichetta1 === "") {
                etichetta1 = GestioneEtichettaEstraiTesto(p_place.AppIdRate, 0);
            }

            var label = null;

            if (etichetta1 !== "") {
                label = new google.maps.Marker({
                    position: polygonPaths[0],
                    draggable: isDraggable,
                    label: { text: etichetta1, color: aggiungiCancellettoSeNonEsiste(colore_poligon) },
                    icon: {
                        path: google.maps.SymbolPath.CIRCLE,
                        scale: 1,
                        strokeOpacity: 0,
                        labelOrigin: new google.maps.Point(0, -14)
                    }
                });

                //vanni, 27/02/2018, se richiesto imposto un'etichetta ... 

                let visibile = ($("#chk_mostra_descrizioneImpianto").is(':checked') && k_livelli.MostraDescrizioneAssociata === "1");

                label.setVisible(visibile);
                
            }


            //' VAnni: 27/11/2020: ripristino della proprietà html precedentemente commentata .. 
            var punto = new google.maps.Marker({
                html: p_place.id,
                position: polygonPaths[0],
                draggable: isDraggable,
                icon: circle, //k_livelli.icona16,
                icon_norm: circle,
                icon_sel: circle_sel,
                selected: false,
                layerDiAppartenenza: p_place.layer,
                StandardEntita_layerDiAppartenenza: p_place.StandardEntita_layerDiAppartenenza,
                StandardEntita_layerDiAppartenenza_Des: p_place.StandardEntita_layerDiAppartenenza_Des,
                StandardEntita_layerDiAppartenenza_Icona32: p_place.StandardEntita_layerDiAppartenenza_Icona32,
                chiavealbero: utility.chiaveAlbero_ridotta_to_big(p_place.chiavealbero),
                etichetta: label
            });

            //if ($("#chk_mostra_descrizioneImpianto").is(':checked') && k_livelli.MostraDescrizioneAssociata === "1") {

            //    let etichetta = p_place.etichetta;
            //    if (etichetta === "") {
            //        etichetta = GestioneEtichettaEstraiTesto(p_place.AppIdRate, 0);
            //    }
                
            //    if (etichetta != "") {
            //        punto.setLabel({ text: etichetta, color: aggiungiCancellettoSeNonEsiste(colore_poligon) })
            //    }
            //    //if (p_place.Testo.indexOf("§") > 0) {
            //    //    GestioneEtichetta(lat_centro, lng_centro, p_place.Testo, punto);
            //    //} else {
            //    //    punto.setLabel({ text: p_place.Testo, color: "yellow" });
            //    //}
            //}

            //if (p_place.Testo.indexOf("§") > 0 && $("#chk_mostra_descrizioneImpianto").is(':checked') && k_livelli.MostraDescrizioneAssociata === "1") {
            //    GestioneEtichetta(lat_centro, lng_centro, p_place.Testo, punto);
            //}

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

            //vanni, 27/02/2018, se richiesto imposto un'etichetta ... 
            if ($("#chk_mostra_descrizioneImpianto").is(':checked') && k_livelli.MostraDescrizioneAssociata === "1") {

                if (p_place.Testo !== "") {

                    // ' VAnni: 7/3/2018: versione con etichette tradizionali
                    var etichetta = "";
                    var pEtichetta = undefined;

                    //if (InfoBox) {
                    if (false) {

                        // ' VAnni: 7/3/2018: versione con infobox
                        etichetta = p_place.Testo.replace(/§/g, ":").replace(/\|/g, "<br />");

                        var myLatlngCentro = new google.maps.LatLng(lat_centro, lng_centro);
                        var boxText = document.createElement("div");

                        boxText.style.cssText = "border: 1px solid black; margin-top: 8px; background: yellow; padding: 5px;";
                        boxText.innerHTML = etichetta;

                        var myOptions = {
                            content: boxText
                            , disableAutoPan: false
                            , maxWidth: 0
                            , pixelOffset: new google.maps.Size(-140, 0)
                            , zIndex: null
                            , boxStyle: {
                                background: "url('tipbox.gif') no-repeat"
                                , opacity: 0.75
                                , width: "230px"
                            }
                            , closeBoxMargin: "10px 2px 2px 2px"
                            , closeBoxURL: "https://www.google.com/intl/en_us/mapfiles/close.gif"
                            , infoBoxClearance: new google.maps.Size(1, 1)
                            , isHidden: false
                            , pane: "floatPane"
                            , enableEventPropagation: false
                        };

                        pEtichetta = new google.maps.Marker({
                            html: p_place.id,
                            position: myLatlngCentro,
                            draggable: isDraggable,
                            icon: k_livelli.icona16,
                            label: undefined,
                            layerDiAppartenenza: p_place.layer,
                            StandardEntita_layerDiAppartenenza: p_place.StandardEntita_layerDiAppartenenza,
                            StandardEntita_layerDiAppartenenza_Des: p_place.StandardEntita_layerDiAppartenenza_Des,
                            StandardEntita_layerDiAppartenenza_Icona32: p_place.StandardEntita_layerDiAppartenenza_Icona32,
                            chiavealbero: utility.chiaveAlbero_ridotta_to_big(p_place.chiavealbero)
                        });

                        var ibLabel = new InfoBox(myOptions);
                        ibLabel.open(mappa.elemenotMappa, pEtichetta);

                        k_livelli.punti.push(pEtichetta);

                    } else {

                        // ' VAnni: 7/3/2018: versione senza infobox
                        etichetta = p_place.Testo.replace(/§/g, "\n");

                        var myLatlngStart = new google.maps.LatLng(polygonPaths[0].lat(), polygonPaths[0].lng());
                        var startMarker = new google.maps.Marker({
                            html: p_place.id,
                            apriInfoAutomaticamente: true,
                            NonSelezionabile: true,
                            label: { text: "A", color: "yellow" },
                            position: myLatlngStart,
                            map: mappa.elemenotMappa,
                            layerDiAppartenenza: p_place.layer,
                            StandardEntita_layerDiAppartenenza: p_place.StandardEntita_layerDiAppartenenza,
                            StandardEntita_layerDiAppartenenza_Des: p_place.StandardEntita_layerDiAppartenenza_Des,
                            StandardEntita_layerDiAppartenenza_Icona32: p_place.StandardEntita_layerDiAppartenenza_Icona32,
                            chiavealbero: utility.chiaveAlbero_ridotta_to_big(p_place.chiavealbero)
                        });

                        var myLatlngEnd = new google.maps.LatLng(polygonPaths[polygonPaths.length - 1].lat(), polygonPaths[polygonPaths.length - 1].lng());
                        var etichettaMarker = new google.maps.Marker({
                            html: p_place.id,
                            apriInfoAutomaticamente: true,
                            NonSelezionabile: true,
                            label: { text: "B", color: "yellow" },
                            position: myLatlngEnd,
                            map: mappa.elemenotMappa,
                            layerDiAppartenenza: p_place.layer,
                            StandardEntita_layerDiAppartenenza: p_place.StandardEntita_layerDiAppartenenza,
                            StandardEntita_layerDiAppartenenza_Des: p_place.StandardEntita_layerDiAppartenenza_Des,
                            StandardEntita_layerDiAppartenenza_Icona32: p_place.StandardEntita_layerDiAppartenenza_Icona32,
                            chiavealbero: utility.chiaveAlbero_ridotta_to_big(p_place.chiavealbero)
                        });

                        //var myLatlngCentro = new google.maps.LatLng(lat_centro, lng_centro);
                        //pEtichetta = new google.maps.Marker({
                        //    html: p_place.id,                            
                        //    position: myLatlngCentro,
                        //    draggable: isDraggable,
                        //    icon: k_livelli.icona16,
                        //    label: { text: etichetta, color: "yellow" },
                        //    layerDiAppartenenza: p_place.layer,
                        //    chiavealbero: utility.chiaveAlbero_ridotta_to_big(p_place.chiavealbero)
                        //});
                        //k_livelli.punti.push(pEtichetta);


                        k_livelli.punti.push(startMarker);
                        k_livelli.punti.push(etichettaMarker);

                    }
                }
            }

            //if (p_place.vertici.length == 2) {
            //    k_livelli.polyline.push(new google.maps.Polyline({
            //        html: p_place.id,
            //        path: polygonPaths,
            //        zIndex: 100000,
            //        layerDiAppartenenza: p_place.layer,
            //        StandardEntita_layerDiAppartenenza: p_place.StandardEntita_layerDiAppartenenza,
            //        StandardEntita_layerDiAppartenenza_Des: p_place.StandardEntita_layerDiAppartenenza_Des,
            //        StandardEntita_layerDiAppartenenza_Icona32: p_place.StandardEntita_layerDiAppartenenza_Icona32,
            //        icona32: k_livelli.icona32,
            //        layerDiAppartenenza_nome: k_livelli.nome,
            //        flag_gps: p_place.flag_gps
            //    }));
            //}
            //else {
            //    //Linestring
            //    k_livelli.polyline.push(new google.maps.Polyline({
            //        html: p_place.id,
            //        path: polygonPaths,
            //        strokeColor: '#' + colore_poligon,
            //        fillOpacity: 1.0,
            //        strokeWeight: 1,
            //        zIndex: 100000,
            //        layerDiAppartenenza: p_place.layer,
            //        StandardEntita_layerDiAppartenenza: p_place.StandardEntita_layerDiAppartenenza,
            //        StandardEntita_layerDiAppartenenza_Des: p_place.StandardEntita_layerDiAppartenenza_Des,
            //        StandardEntita_layerDiAppartenenza_Icona32: p_place.StandardEntita_layerDiAppartenenza_Icona32,
            //        icona32: k_livelli.icona32,
            //        layerDiAppartenenza_nome: k_livelli.nome,
            //        flag_gps: p_place.flag_gps
            //    }));
            //}

            k_livelli.polyline.push(new google.maps.Polyline({
                html: p_place.id,
                path: polygonPaths,
                strokeColor: colore_poligon,
                fillOpacity: 1.0,
                strokeWeight: 1 + (3 * k_livelli.trasparenza),
                zIndex: 100000,
                layerDiAppartenenza: p_place.layer,
                StandardEntita_layerDiAppartenenza: p_place.StandardEntita_layerDiAppartenenza,
                StandardEntita_layerDiAppartenenza_Des: p_place.StandardEntita_layerDiAppartenenza_Des,
                StandardEntita_layerDiAppartenenza_Icona32: p_place.StandardEntita_layerDiAppartenenza_Icona32,
                icona32: k_livelli.icona32,
                layerDiAppartenenza_nome: k_livelli.nome,
                flag_gps: p_place.flag_gps
            }));

            break;
        case 'Circle':
            //circle
            console.info(circle_radius);

            //vanni, 27/02/2018, se richiesto imposto un'etichetta ... 
            if ($("#chk_mostra_descrizioneImpianto").is(':checked') && k_livelli.MostraDescrizioneAssociata === "1") {



                if (p_place.AppIdRate !== "") {

                    if (p_place.AppIdRate.indexOf("§") > 0 && $("#chk_mostra_descrizioneImpianto").is(':checked') && k_livelli.MostraDescrizioneAssociata === "1") {

                        var testoEtichetta = GestioneEtichettaEstraiTesto(p_place.AppIdRate, 0);

                        if (testoEtichetta !== '') {
                            var punto = new google.maps.Marker({
                                html: p_place.id,
                                position: polygonPaths[0],
                                draggable: isDraggable,
                                label: { text: testoEtichetta, color: "yellow" },
                                icon: k_livelli.icona16,
                                layerDiAppartenenza: p_place.layer,
                                StandardEntita_layerDiAppartenenza: p_place.StandardEntita_layerDiAppartenenza,
                                StandardEntita_layerDiAppartenenza_Des: p_place.StandardEntita_layerDiAppartenenza_Des,
                                StandardEntita_layerDiAppartenenza_Icona32: p_place.StandardEntita_layerDiAppartenenza_Icona32,
                                chiavealbero: utility.chiaveAlbero_ridotta_to_big(p_place.chiavealbero)
                            });


                            k_livelli.punti.push(punto);
                        }
                    }


                }
            }

            //radius: circle_radius,

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
                fillOpacity: circle_opacity,
                strokeWeight: 0,//1,
                fillColor: colore_poligon,
                selectedColor: '#7d7d7d',
                colore_precedente: colore_poligon,
                zIndex: 10000000000, //p_place.zindex
                layerDiAppartenenza: p_place.layer,
                StandardEntita_layerDiAppartenenza: p_place.StandardEntita_layerDiAppartenenza,
                StandardEntita_layerDiAppartenenza_Des: p_place.StandardEntita_layerDiAppartenenza_Des,
                StandardEntita_layerDiAppartenenza_Icona32: p_place.StandardEntita_layerDiAppartenenza_Icona32,
                icona32: k_livelli.icona32,
                layerDiAppartenenza_nome: k_livelli.nome,
                flag_gps: p_place.flag_gps
            }));
            break;
        case 'Polygon':
        case 'PolygonFill':
            //Poligoni

            let pLabel = null;

            let checkMostra = $("#chk_mostra_descrizioneImpianto").is(':checked');

            //vanni, 27/02/2018, se richiesto imposto un'etichetta ... 
            if (k_livelli.MostraDescrizioneAssociata === "1") {

                var etichetta = "";

                var idNoA = utility.chiaveAlbero_ridotta_to_big(p_place.chiavealbero).replace(' ', '');

                let layer_selezionato = TipologiaLayer();

                var treeview = $("#GIS_treeview").data("kendoTreeView");
                if (treeview !== undefined) {

                    var getitem = treeview.dataSource.get(idNoA);

                    if (getitem !== undefined) {
                        if (layer_selezionato === '11') {
                            etichetta = getitem.parent().parent().text;
                        } else {
                            etichetta = getitem.text;
                        }

                    }
                }

                if (p_place.etichetta !== undefined) {
                    etichetta = p_place.etichetta; //Gabriele
                }

                if (etichetta !== "") {

                    let NonSelezionabile = (layer_selezionato === '15');

                    var myLatlngCentro = new google.maps.LatLng(lat_centro, lng_centro);
                    pLabel = new google.maps.Marker({
                        html: p_place.id,
                        position: myLatlngCentro,
                        draggable: isDraggable,
                        NonSelezionabile: NonSelezionabile, //GABRIELE
                        //icon: k_livelli.icona16,
                        icon: {
                            path: google.maps.SymbolPath.CIRCLE,
                            scale: 0
                        },
                        label: { text: etichetta, color: "yellow" },
                        //title: etichetta, 
                        layerDiAppartenenza: p_place.layer,
                        StandardEntita_layerDiAppartenenza: p_place.StandardEntita_layerDiAppartenenza,
                        StandardEntita_layerDiAppartenenza_Des: p_place.StandardEntita_layerDiAppartenenza_Des,
                        StandardEntita_layerDiAppartenenza_Icona32: p_place.StandardEntita_layerDiAppartenenza_Icona32,
                        chiavealbero: utility.chiaveAlbero_ridotta_to_big(p_place.chiavealbero)
                    });

                    pLabel.setVisible(checkMostra);

                    //k_livelli.punti.push(pLabel_);
                }
            }
            if (p_place.TipologiaGML === 'Polygon') {

                let poligonoDaAggiungere = new google.maps.Polygon({
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
                    StandardEntita_layerDiAppartenenza: p_place.StandardEntita_layerDiAppartenenza,
                    StandardEntita_layerDiAppartenenza_Des: p_place.StandardEntita_layerDiAppartenenza_Des,
                    StandardEntita_layerDiAppartenenza_Icona32: p_place.StandardEntita_layerDiAppartenenza_Icona32,
                    icona32: k_livelli.icona32,
                    layerDiAppartenenza_nome: k_livelli.nome,
                    flag_gps: p_place.flag_gps,
                    etichetta: pLabel
                });

                google.maps.event.addListener(poligonoDaAggiungere, 'click', mappa_clickPoligono_event);

                google.maps.event.addListener(poligonoDaAggiungere.getPath(), 'set_at', function (index) {
                    riposizionaEtichetta(poligonoDaAggiungere, this);
                });

                google.maps.event.addListener(poligonoDaAggiungere.getPath(), 'insert_at', function (index) {
                    riposizionaEtichetta(poligonoDaAggiungere, this);
                });

                k_livelli.poligoni.push(poligonoDaAggiungere);

                debugDrag(poligonoDaAggiungere);

            } else {

                let colore_primario = ((typeof p_place.Colore_Primario === "string" && p_place.Colore_Primario !== '') ? p_place.Colore_Primario : colore_poligon);
                colore_primario = aggiungiCancellettoSeNonEsiste(colore_primario);
                let colore_retinatura = ((typeof p_place.Colore_Retinatura === "string" && p_place.Colore_Retinatura !== '') ? p_place.Colore_Retinatura : '#555');
                colore_retinatura = aggiungiCancellettoSeNonEsiste(colore_retinatura);
                let opacity = ((typeof p_place.Trasparenza === "number") ? (100.0 - p_place.Trasparenza) / 100.0 : 0.5)

                let poly = new google.maps.Polygon({
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
                    strokeColor: colore_primario,
                    fillOpacity: 0,
                    strokeWeight: 1,
                    //fillColor: colore_poligon,
                    //selectedColor: '#7d7d7d',
                    //colore_precedente: colore_poligon,
                    zIndex: p_place.zindex,
                    layerDiAppartenenza: p_place.layer,
                    StandardEntita_layerDiAppartenenza: p_place.StandardEntita_layerDiAppartenenza,
                    StandardEntita_layerDiAppartenenza_Des: p_place.StandardEntita_layerDiAppartenenza_Des,
                    StandardEntita_layerDiAppartenenza_Icona32: p_place.StandardEntita_layerDiAppartenenza_Icona32,
                    icona32: k_livelli.icona32,
                    layerDiAppartenenza_nome: k_livelli.nome,
                    flag_gps: p_place.flag_gps,
                    etichetta: pLabel
                });

                google.maps.event.addListener(poly, 'click', mappa_clickPoligono_event);

                k_livelli.poligoni.push(poly);

                let polyfill = new BW.PolyLineFill({
                    poly: polygonPaths,
                    fill: colore_primario,
                    stroke: colore_retinatura,
                    opacity: opacity,
                    zIndex: p_place.zindex
                });

                k_livelli.poligoni2.push(polyfill);

                poly.getPath().addListener('set_at', function (index, obj) {
                    polyfill.setPath(index, this.getAt(index), 'replace');
                });
                poly.getPath().addListener('insert_at', function (index) {
                    polyfill.setPath(index, this.getAt(index), 'insert');
                });
            }

            break;

        case 'MultiSurface':
            //Poligoni
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
                StandardEntita_layerDiAppartenenza: p_place.StandardEntita_layerDiAppartenenza,
                StandardEntita_layerDiAppartenenza_Des: p_place.StandardEntita_layerDiAppartenenza_Des,
                StandardEntita_layerDiAppartenenza_Icona32: p_place.StandardEntita_layerDiAppartenenza_Icona32,
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

//var dragStart_Path = null;
//var draggable_div = null;
//var polyDrag = null;

function debugDrag(poligono) {

    //if (poligono.layerDiAppartenenza !== "3")
    //    return;

    //poligono.setDraggable(true);
    //google.maps.event.addListener(poligono, "dragstart", function (e) {

    //    let map = this.getMap();
    //    let bounds = map.getBounds();
    //    map.setOptions({ restriction: { latLngBounds: bounds, strictBounds: true } });

    //    let path = this.getPath().getArray();

    //    if (polyDrag === null) {

    //        polyDrag = new DraggableOverlay(map, path);
    //    }


    //    dragStart_Path = new Array();
    //    path.forEach(function (elem, idx) {
    //        dragStart_Path.push(new google.maps.LatLng({ lat: elem.lat(), lng: elem.lng() }));
    //    });

    //    //let pt = map.getProjection().fromLatLngToPoint(e.latLng)
    //    //console.log("x:" + pt.x + " - y:" + pt.y);
    //    //draggable_div = document.createElement('div');
    //    //draggable_div.id = "map_shape";
    //    //draggable_div.style.cssText = "width: 50px; height: 50px; border-radius: 50%; background-color: blueviolet;";
    //    //draggable_div.style.position = "absolute";
    //    //draggable_div.style.top = pt.y;
    //    //draggable_div.style.left = pt.x;
    //    //document.body.appendChild(draggable_div);
    //    //$("#map_shape").kendoDraggable({
    //    //    hint: function () {
    //    //        return $("#map_shape").clone();
    //    //    }//,
    //    ////    //dragstart: draggableOnDragStart,
    //    ////    //dragend: draggableOnDragEnd
    //    //});
    //    ////kendo_draggable = $("#map_shape").data("kendoDraggable");
    //
    // //GABRIELE DRAG
    //        let draggable = $("#map_shape").data("kendoDraggable");
    //        draggable.trigger("dragstart");

    //         <%--GABRIELE DRAG--%>
    //        <div id="map_shape" style="width: 50px; height: 50px; border-radius: 50%; background-color: blueviolet"></div>
    //    //GABRIELE DRAG
    //    $("#map_shape").kendoDraggable({
    //        hint: function () {
    //            return $("#map_shape").clone();
    //        }//,
    //        //dragstart: draggableOnDragStart,
    //        //dragend: draggableOnDragEnd
    //    });
    //

    //    });
    //    google.maps.event.addListener(poligono, "dragend", function () {

    //        let map = this.getMap();
    //        map.setOptions({ restriction: undefined });

    //        if (dragStart_Path !== null) {
    //            this.setPath(dragStart_Path);
    //            dragStart_Path.length = 0;
    //            dragStart_Path = null;
    //        }
    //        //if (draggable_div !== null) {
    //        //    kendo_draggable = $("#map_shape").data("kendoDraggable");
    //        //    if (kendo_dragg)

    //        //}

    //        if (polyDrag !== null) {

    //            polyDrag.setMap(null);
    //            polyDrag = null;
    //        }

    //    });

}

function riposizionaEtichetta(poly, path) {

    if (typeof poly.etichetta !== "object") {
        return;
    }
    if (poly.etichetta === null) {
        return;
    }

    let pos = polylabel(path.getArray());
    poly.etichetta.setPosition(pos);

}

function aggiungiCancellettoSeNonEsiste(colore) {
    if (!stringhe.startsWith(colore, "#"))
        colore = "#" + colore;
    return colore
}

function ImpostaMappa(dati, setClick, setdblClick, functSelect, setMap_isInBound) {

    //Gabriele: gestire poly2

    for (var kk = 0; kk < dati.length; kk++) {

        if (setClick) {

            google.maps.event.addListener(dati[kk], 'mouseup', function () {
                if (functSelect != 'poly' && functSelect != 'circle') {
                    setSelectionPointMouseUp(this);
                }
            });
            google.maps.event.addListener(dati[kk], 'click', function (e) {
                if (functSelect == 'poly' || functSelect == 'circle') {
                    if (functSelect == 'poly') {
                        if (ctrlPressed) {
                            setMultiSelection(this);
                        } else {
                            if (e.vertex !== undefined) {

                                GlobalAgroDrawing.queryDeleteVertex(this, e.vertex);

                            } else {

                                setSelection(this);
                            }

                            //setSelection(this);
                        }
                    }
                    else {
                        setSelectionCircle(this);
                    }
                }
                else
                    setSelectionPoint(this);
            });
        }

        let flag = false;
        if (setMap_isInBound) {
            if (Bound_isMarkerIn(dati[kk])) {
                flag = true;
            }
        } else {
            flag = true;
        }

        if (flag) {

            dati[kk].setMap(mappa.elemenotMappa);

            if (typeof dati[kk].etichetta === "object") {
                if (dati[kk].etichetta !== null) {

                    dati[kk].etichetta.setMap(mappa.elemenotMappa);

                }
            }
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

    let w_w = window.outerWidth;
    let w_h = window.outerHeight;

    w_w = w_w * 0.68;
    w_h = w_h * 0.77;

    //ModalBootstrapApri(indirizzohttpCatasto, "Strumento di Riparto Catasto");
    let tt = Traduzione(AgronicaControlliGisResx, "jsLblStrumentodiRipartoCatasto");
    KendoWindowGenericApri(indirizzohttpCatasto, tt, undefined, parseInt(w_w), parseInt(w_h), 10, 10);

    $("#kendoWindowiFrameGeneric").data("kendoWindow").bind("close", onCloseRipartoCatasto);
}

function DialogSr() {


    var indirizzohttpSr = '';
    indirizzohttpSr = 'sr/sr.aspx?modal=true';


    //utility.log(indirizzohttp);

    let w_w = window.outerWidth;
    let w_h = window.outerHeight;

    w_w = w_w * 0.95;
    w_h = w_h * 0.8;

    //ModalBootstrapApri(indirizzohttpCatasto, "Strumento di Riparto Catasto");
    let tt = Traduzione(AgronicaControlliGisResx, "lbl_sr.Text");
    KendoWindowGenericApri(indirizzohttpSr, tt, undefined, parseInt(w_w), parseInt(w_h), 10, 10);

    $("#kendoWindowiFrameGeneric").data("kendoWindow");
}

function onCloseRipartoCatasto() {

    AggiornaLayer(false);

    AlberoAnagrafica2017lettura();

    $("#kendoWindowiFrameGeneric").data("kendoWindow").unbind("close", onCloseRipartoCatasto);
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

function copiaIncolla_Copia() {

    //gestione dei potenziali errori

    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString(); //.replace(/\\/g, '\\\\'); ;

    if (ChiaveAlbero == '') {

        //log
        copiaIncollaLogThis(ChiaveAlbero);
        let msg = Traduzione(AgronicaControlliGisResx, "jsMsgNessunElementoSelezionato");
        kendoDlgMessage("", msg);
        return 1;
    }

    if (shape.selectedShape_Copia == undefined) {
        if (ChiaveAlbero != '' && shape.selectedShape == undefined) {

            //log
            copiaIncollaLogThis(ChiaveAlbero);
            let msg1 = Traduzione(AgronicaControlliGisResx, "jsMsgNessunDisegnoElementoSelezionato");
            kendoDlgMessage("", msg1);
            return 1;
        }
    }

    //fine gestione potenziali errori


    
    //copia
    shape.selectedShape_Copia = shape.selectedShape;
    let msg = Traduzione(AgronicaControlliGisResx, "jsMsgCopiaIncollaGisAlbero");
    kendoDlgMessage("", msg);
    

}

function copiaIncolla_Incolla() {

    //incolla

    //nuovo elemento selezionato dall'albero per incollare (usato in questa funzione a scopo di log).
    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString(); //.replace(/\\/g, '\\\\'); ;

    //se non ho selezionato nulla per la copia lo dico all'utente
    if (shape.selectedShape_Copia === undefined) {
        let msg = Traduzione(AgronicaControlliGisResx, "jsMsgNessunElementoSelezionato");
        kendoDlgMessage("", msg);
        return 1;
    }

    
    var shapeDaChiaveAlbero = RicercaShapeDaChiaveAlbero(ChiaveAlbero);
    
    if (shapeDaChiaveAlbero === null) {

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
        let msg = Traduzione(AgronicaControlliGisResx, "jsMsgCopiaIncollaEsisteElemento");
        kendoDlgMessage("", msg);
    }


    //log
    copiaIncollaLogThis(ChiaveAlbero);
}

function RicercaShapeDaChiaveAlbero(ChiaveAlbero) {

    let shapeS = null;

    //reset del contatore e riparto .. scorro tutti i layer e tutti i path per identificare quello attivo
    let idxLivelli = 0;
    while (shapeS === null && idxLivelli < mappa.Livelli.length) {

        let livello = mappa.Livelli[idxLivelli];

        //al momento per un livello non esistono contemporaneamente punti e poligoni che posso selezionare attraverso l'albero.
        //istanzio quindi un unica variabile.
        let oggettiDisegnati = undefined;        
        if (livello.poligoni.length > 0) {
            oggettiDisegnati = livello.poligoni;            
        }

        if (livello.punti.length > 0) {
            oggettiDisegnati = livello.punti;            
        }

        if (oggettiDisegnati !== undefined) {
            let indShapes = 0;
            while (shapeS === null && indShapes < oggettiDisegnati.length) {

                if (oggettiDisegnati[indShapes].chiavealbero == ChiaveAlbero) {
                    shapeS = oggettiDisegnati[indShapes];
                }
                indShapes++;
            }
        }
        //esistono oggetti diseganti da impostare

        idxLivelli++;
    }

    return shapeS;
}

function editPunti() {

    //gestione dei potenziali errori

    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString(); //.replace(/\\/g, '\\\\'); ;
    if (ChiaveAlbero == '' && CoordFromPoints != '') {
        let msg1 = Traduzione(AgronicaControlliGisResx, "jsMsgNessunElementoSelezionato");
        kendoDlgMessage("", msg1);
        return 1;
    }

    if (ChiaveAlbero != '' && CoordFromPoints == '' && shape.selectedShape == undefined) {
        let msg2 = Traduzione(AgronicaControlliGisResx, "jsMsgNessunDisegnoElementoSelezionato");
        kendoDlgMessage("", msg2);
        return 1;
    }

    if (CoordFromPoints != '' && shape.selectedShape != undefined) {
        let msg3 = Traduzione(AgronicaControlliGisResx, "jsMsgEsisteGiaDisegnoPerElementoSelezionato.");
        kendoDlgMessage("", msg3);
        return 1;
    }
    //fine gestione dei potenziali errori


    if (CoordFromPoints != '') {
        CoordFromPoints = CoordFromPoints.substring(0, CoordFromPoints.length - 2);

        // (lat,long),(lat,long),(lat,long) --> length = 6
        var ll = CoordFromPoints.split(",").length;
        utility.log("--> length =" + ll);

        if (ll < 5) {
            let msg = Traduzione(AgronicaControlliGisResx, "jsMsgSelezionareTreVertici");
            kendoDlgMessage("", msg);
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
                    AggiornaTutto(false);
                    $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val("");
                }
                else {
                    let msg1 = Traduzione(AgronicaControlliGisResx, "jsMsgProblemaDuranteScomposizione");
                    alert(msg1 + msg.d);
                }
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
        piva: $('#pop_up_impianto_azienda').val(),
        ragione_sociale: $('#txt_sa_nome').val(),
        sLat: coord.sLat,
        sLong: coord.sLong,
        provincia: $('#ddl_Provincia_' + centro_ID_ind).val(),
        comune: $('#ddl_Comune_' + centro_ID_ind).val(),
        via: $('#txt_Via_' + centro_ID_ind).val(),
        frazione: $('#txt_Frazione_' + centro_ID_ind).val(),
        civico: $('#pop_up_finalita').val(),
        stato: $('#txt_ISO_Stato_' + centro_ID_ind).val(),
        note: $('#txt_Note_' + centro_ID_ind).val()
    };


    //se sono tutte valorizzate allora posso procedere
    var blocca = false;

    if (oggetto.piva == "" || oggetto.ragione_sociale == "") {
        blocca = true;
    }


    if (blocca) {
        let msgBlocca = Traduzione(AgronicaControlliGisResx, "jsMsgCompletareCampiObbligatori");
        alert(msgBlocca);
        return false;
    }

    interfaccia.loading(true);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/btn_SalvaNuoovoCentroAziendale_Click",
        //data: "{ piva: '" + oggetto.piva + "', ragione_sociale: '" + oggetto.ragione_sociale + "', sLat: '" + oggetto.sLat + "', sLong: '" + oggetto.sLong + "', provincia: '" + oggetto.provincia + "', comune: '" + oggetto.comune + "', via: '" + oggetto.via + "', frazione: '" + oggetto.frazione + "', civico: '" + oggetto.civico + "', stato: '" + oggetto.stato + "', note: '" + oggetto.note + "' }",
        data: JSON.stringify(oggetto),
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

                ChiudiKendoDialog('#popup_nuovo_centro');

                var descriz = "";
                descriz = oggetto.ragione_sociale;

                if (GisPurpose === Enum_GisPurpose.SementiSportello || GisPurpose === Enum_GisPurpose.SementiMappaturaLibera)
                    descriz += " (Lat.: " + oggetto.sLat + " - Long.: " + oggetto.sLong + ")";

                var ds = descriz + '|' + oggetto.via + ' ' + oggetto.civico + ' ' + oggetto.comune;
                AggiungiOptionInSelect('#pop_up_centro', sa_cod, ds);
                kendoDropDown_addNew("#ddl_Sa_Cod_html", sa_cod, descriz, "SaCod", "SaNome");

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
            ChiudiKendoDialog('#popup_nuova_azienda');

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

    GisAjax("CaricaAzienda", {}, function (risp) {

        $('#pop_up_impianto_azienda').html(risp);
        CaricaCentroAziendale();

    });
}

/********************* INDIRIZZI ***********************************/

function ricercaCoordinate() {

    //let tipo = $("#tipoGrado").val();

    //let lat = Number.NaN;
    //let lng = Number.NaN;

    //if (tipo == "TG") {

    //    let Lat_Degrees = $("#Lat_Degrees").val();
    //    let Lat_Minutes = $("#Lat_Minutes").val();
    //    let Lat_Seconds = $("#Lat_Seconds").val();

    //    let Lon_Degrees = $("#Lon_Degrees").val();
    //    let Lon_Minutes = $("#Lon_Minutes").val();
    //    let Lon_Seconds = $("#Lon_Seconds").val();

    //    if (Lat_Degrees != "" && Lat_Minutes != "" && Lat_Seconds != "" &&
    //        Lon_Degrees != "" && Lon_Minutes != "" && Lon_Seconds != "") {

    //        let tmp_point = new GeoPoint(Lon_Degrees + '° ' + Lon_Minutes + '\' ' + Lon_Seconds + '"', Lat_Degrees + '° ' + Lat_Minutes + '\' ' + Lat_Seconds + '"');

    //        lat = tmp_point.getLatDec();
    //        lng = tmp_point.getLonDec();

    //        $("#lat_cerca").val(lat);
    //        $("#long_cerca").val(lng);

    //    }
    //} else {

    //    let sLat = $("#lat_cerca").val();
    //    let sLon = $("#long_cerca").val();

    //    lat = parseFloat(sLat);
    //    lng = parseFloat(sLon);

    //    let point2 = new GeoPoint(lat, lng);

    //    let sDegMinSec_Long = point2.lonDeg;
    //    interpretaLatLong(sDegMinSec_Long, "Lat");

    //    let sDegMinSec_Lat = point2.latDeg;
    //    interpretaLatLong(sDegMinSec_Lat, "Lon");
    //}

    //if (!Number.isNaN(lat) && !Number.isNaN(lng)) {

    //    mappa.CercaCoordinate(lat, lng);

    //} else {

    //    kendoDlgMessage("", "I campi Lat e Long non contengono valori corretti");
    //}
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

        GisAjax("CaricaCentroAziendale", { Piva: azienda_selezionata }, function (risp) {

            $('#pop_up_centro').html(risp);

            CaricaComboOrganismoReferente("#pop_up_impianto_azienda", "#pop_up_app_OrganismoReferente");

            if ($(".ddl_Sa_Cod_sx").length > 0) {

                $('#pop_up_centro').val($('.ddl_Aziende').val() + "|" + $('.ddl_Sa_Cod_sx').val());
            }

        });
    }
}

function CaricaCampo() {

    var vAziendaSelezionata = $('#pop_up_centro').val().split("|");

    var aziendaSelezionata = vAziendaSelezionata[0];
    var centroSelezionato = vAziendaSelezionata[1];

    if (centroSelezionato === "") {
        centroSelezionato = "0";
    }

    ajaxAgronica(indirizzohttp + "/CaricaCampo", JSON.stringify({ piva: aziendaSelezionata, sa_cod: centroSelezionato }),
        function (risposta) {
            $('#pop_up_campo').html(risposta.RispostaStringa);
        }, null);

}

function CaricaComboOrganismoReferente(jQueryImpresa, jQueryOrganismoReferente) {
    var azienda_selezionata = $(jQueryImpresa).val();
    if (azienda_selezionata != "") {

        GisAjax("CaricaComboOrganismoReferente", { piva: azienda_selezionata }, function (risp) {

            $(jQueryOrganismoReferente).html(risp);

            if (jQueryImpresa === "#ddl_azienda_html") {
                mappa.CaricaCodiciAnagrafeRiportaCodice(jQueryOrganismoReferente, 1074);
            }
        });
    }
}
/******************* Carica Specie ************************/
/* funzione per il caricamento tramite ajax delle specie  */
function CaricaSpecie(combo_specie, veg_cod) {

    let risp = GisAjaxSync("CaricaSpecie", {});

    $(combo_specie).html(risp);

    if (veg_cod != '') {

        $(combo_specie).val(veg_cod)
        $(combo_specie).trigger("change");

    } else {

        let elem = combo_specie.slice(1);
        let opts = document.getElementById(elem).options;
        if (opts.length > 0) {
            $(combo_specie).val(opts[0].value);
            $(combo_specie).trigger("change");
        }
    }
}

function CaricaDestinazioneUso(combo, dest_cod) {

    let risp = GisAjaxSync("CaricaDestinazioneUso", {});

    $(combo).html(risp);

    if (dest_cod != '') {

        $(combo).val(dest_cod)
        $(combo).trigger("change");

    } else {

        let elem = combo.slice(1);
        let opts = document.getElementById(elem).options;
        if (opts.length > 0) {
            $(combo).val(opts[0].value);
            $(combo).trigger("change");
        }
    }
}

function CaricaSpecieConInfoAgenda(Cmb_Specie, Cmb_Cultivar, Cmb_Finalita, veg_cod, ChiaveAlbero) {
    ajaxAgronica(indirizzohttp + "/CaricaSpecieConInfoAgenda", JSON.stringify({ ChiaveAlbero: ChiaveAlbero, veg_cod: veg_cod }),
        function (risposta) {
            let obj_Impianto = risposta.RispostaStringa;

            //coltura_anno_1 è un campo riciclato
            Cmb_Specie.html(obj_Impianto.coltura_anno_1);

            if (obj_Impianto.movimentipresenti) {
                if (veg_cod !== "-1") {
                    Cmb_Specie.prop('disabled', 'disabled');
                    Cmb_Cultivar.prop('disabled', 'disabled');
                    if (obj_Impianto.trattamentipresenti) {
                        Cmb_Finalita.prop('disabled', 'disabled');
                    } else {
                        Cmb_Finalita.prop('disabled', false);
                    }
                } else {
                    Cmb_Specie.prop('disabled', false);
                    Cmb_Cultivar.prop('disabled', false);
                    Cmb_Finalita.prop('disabled', false);
                }

            }

            if (veg_cod !== "-1") {
                Cmb_Specie.val(veg_cod);
            }
            Cmb_Specie.trigger("change");
        }, null);
}

/******************* Carica Tipologia ************************/
/* funzione per il caricamento tramite ajax delle specie  */
function CaricaTipologia(Veg_Cod, combo_tipologia, valoreSelezionato) {

    let risp = GisAjaxSync("CaricaTipologia", { Veg_Cod: Veg_Cod });

    r_ok();

    $(combo_tipologia).html(risp);
    if (valoreSelezionato != "" && valoreSelezionato != "0") {

        $(combo_tipologia).val(valoreSelezionato);
    } else {

        let elem = combo_tipologia.slice(1);
        let opts = document.getElementById(elem).options;
        if (opts.length > 0) {
            $(combo_tipologia).val(opts[0].value);
            $(combo_tipologia).trigger("change");
        }
    }
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

    let risp = GisAjaxSync("CaricaVarieta", { Veg_Cod: Veg_Cod });

    r_ok();

    $(combo_varieta).html(risp);
    if (cul_cod != '' && cul_cod != '0') {
        $(combo_varieta).val(cul_cod)
    } else {

        let elem = combo_varieta.slice(1);
        let opts = document.getElementById(elem).options;
        if (opts.length > 0) {
            $(combo_varieta).val(opts[0].value);
            $(combo_varieta).trigger("change");
        }
    }
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

    let risp = GisAjaxSync("CaricaDisciplinare", { Veg_Cod: Veg_Cod });

    r_ok();
    $('#pop_up_disciplinare').html(risp);
}

/******************* Carica Finalita ************************/
/* funzione per il caricamento tramite ajax delle specie  */
function CaricaFinalita(combo_finalita, finalita) {
    var Veg_Cod = "";

    if (combo_finalita == '#pop_up_finalita') {
        Veg_Cod = $('#pop_up_specie').val();
    } else {
        //pop_up_varieta_modifica
        Veg_Cod = $('#pop_up_specie_modifica').val();
    }

    let risp = GisAjaxSync("CaricaFinalita", { Veg_Cod: Veg_Cod });

    r_ok();

    $(combo_finalita).html(risp);
    if (finalita != '' && finalita != "0") {
        $(combo_finalita).val(finalita)
    } else {

        let elem = combo_finalita.slice(1);
        let opts = document.getElementById(elem).options;
        if (opts.length > 0) {
            $(combo_finalita).val(opts[0].value);
            $(combo_finalita).trigger("change");
        }
    }
}

/******************* NUOVO IMPIANTO ************************/

/* verifica della correttezza di un poligono */
// GABRIELE 03 04 2019
/*
function VerificaPoligono(Poligono) {

    interfaccia.loading(true);
    var bTestPoligono = false;
    $.ajax({
        async: false,
        type: "POST",
        url: indirizzohttp + "/VerificaPoligono",
        data: "{ Poligono: '" + Poligono + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {

            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                if (msg.d == 'Ok') {
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
*/

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

/**
 * seleziona da tutte le caselle di testo i codici anagrafe e li mette in una stringa json
 * @param {string} jQuerySelettoreCodici selettore delle caselle
 */
function LeggiCodiciAnagrafeAggiuntivi(jQuerySelettoreCodici) {

    var CodiciArr = [];

    $(jQuerySelettoreCodici).each(function () {
        CodiciArr.push(LeggiCodiciAnagrafeGetStrEscaped($(this).attr("data-idCod"), $(this).val()));
    });

    var rval = "[ " + CodiciArr.join(",") + "]";

    return rval;

}

/**
 * Stringa con id_cod, val_cod
 * @param {any} id_cod
 * @param {any} val_Cod
 */
function LeggiCodiciAnagrafeGetStrEscaped(id_cod, val_Cod) {
    return " { \"id_cod\": " + id_cod + ",  \"val_cod\": \"" + val_Cod + "\" }";
}

function SementiMappaturaLiberaSpecieVegetalePermessa(veg_cod, finalita, data_inizio, data_fine, titolo) {

    let result = true;
    if (veg_cod !== undefined && veg_cod !== null && veg_cod !== "") {

        let objData = { veg_cod: veg_cod, finalita: finalita, data_inizio: data_inizio, data_fine: data_fine };
        let risp = GisAjaxSync("SementiMappaturaLiberaSpecieVegetalePermessa", objData);
        result = (risp === "true");
    }
    if (!result) {
        let msgNonCons = Traduzione(AgronicaControlliGisResx, "jsMsgSpecieSoggettaSportelloSalvaNonConsentito");
        kendoDlgMessage(titolo, msgNonCons);
    }
    return result;
}

function SementiAccettaInteferenze(nuovoImpianto) {

    if (GisPurpose !== Enum_GisPurpose.SementiSportello) {

        return true;
    }

    let veg_cod = ""
    let grva_cod = ""
    let data_inizio = ""
    let data_fine = ""
    let hiddenPunti = ""
    let entita_cod = ""

    if (nuovoImpianto) {

        veg_cod = $('#pop_up_specie').val();
        grva_cod = $('#pop_up_tipologia').val();
        data_inizio = $('#pop_up_data_inizio').val();
        data_fine = $('#pop_up_data_fine').val();
        hiddenPunti = $('#hiddenPunti_Nuovo').val();

    } else {

        veg_cod = $('#pop_up_specie_modifica').val();
        grva_cod = $("#pop_up_tipologia_modifica").val()
        data_inizio = $('#pop_up_m_data_inizio').val();
        data_fine = $('#pop_up_m_data_fine').val();
        hiddenPunti = $('#hiddenPunti_modifica').val();

        let hiddenID = $("#hiddenID").val().split("|");
        if (hiddenID.length > 1) {
            entita_cod = hiddenID[1];
        }
    }

    let risp = GisAjaxSync("InfoInterferenze", {
        veg_cod: veg_cod,
        grva_cod: grva_cod,
        data_inizio: data_inizio,
        data_fine: data_fine,
        hiddenPunti: hiddenPunti,
        entita_cod: entita_cod
    });

    if (risp !== "") {

        let jarr = JSON.parse(risp);

        if (jarr.length > 0) {

            let cont = "";
            cont += "<div style='padding:10px;'>";
            cont += "<div style='text-align:left; display:grid; grid-template-columns:repeat(6, auto); grid-gap:10px 20px;'>";
            for (e = 0; e < jarr.length; e++) {
                cont += "<div>&#x2022;</div>";
                cont += "<div>" + jarr[e].referente + "</div>";
                cont += "<div>" + jarr[e].indirizzo + "</div>";
                cont += "<div>" + jarr[e].veg_des + "</div>";
                cont += "<div>" + jarr[e].tipologia + "</div>";
                cont += "<div style='justify-self: end;'>" + jarr[e].distanza + "</div>";
            }
            cont += "</div>";
            cont += "</div>";

            kendoDlgMessage("Interferenze rilevate", cont);
        }

    }

    return true;
}

/* funzione per la creazione di un nuvo impianto  */
function InviaDatiNuovoImpianto() {

    let veg_cod = $('#pop_up_specie').val();
    let finalita = $('#pop_up_finalita').val();
    let data_inizio = $('#pop_up_data_inizio').val();
    let data_fine = $('#pop_up_data_fine').val();
    if (!SementiMappaturaLiberaSpecieVegetalePermessa(veg_cod, finalita, data_inizio, data_fine, "Nuovo impianto")) {
        return false;
    }

    PredisponiPerSalvataggio_Lotto(false);

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
        "hiddenPunti_Nuovo": hiddenPunti_Nuovo,
        "campo_cod": $("#pop_up_campo").val()
    };

    if (oggetto.campo_cod === null || oggetto.campo_cod === undefined) {
        oggetto.campo_cod = "0";
    }

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

        if (GisPurpose === Enum_GisPurpose.SementiSportello && (oggetto.data_inizio == "" || oggetto.data_fine == "" || oggetto.tipologia == "")) {
            blocca = true;
        }

        // controllo che nono siano nulle
        if (oggetto.piva == null || oggetto.sa_cod == null || oggetto.veg_cod == null || oggetto.sup_imp == null) {
            blocca = true;
        }

    }

    if (blocca) {
        kendoDlgMessage("Nuovo impianto", "Completare i campi obbligatori");
        return false;
    }

    interfaccia.loading(true);

    if (!SementiAccettaInteferenze(true)) {
        interfaccia.loading(false);
        return false;
    }

    var PropagazioneSalvataggioPoligono = $("#cmbElementoGrafico_GenerazionePoligoni").val();

    if (shape.glayerDoveDisegnoSuTipologiaStandard.toString() !== "19") {
        PropagazioneSalvataggioPoligono = "0";
    }

    //per chiamata Standard Ajax

    var codiciAnagrafeAggiuntivi = LeggiCodiciAnagrafeAggiuntivi(".CodiceAnagrafe");

    if ($("#terreno_nudo").prop("checked")) {

        let id_cod = parseInt($("#pop_up_destuso").val());
        let arr = JSON.parse(codiciAnagrafeAggiuntivi);
        arr.push({ id_cod: id_cod, val_cod: "" });
        codiciAnagrafeAggiuntivi = JSON.stringify(arr);

        oggetto.veg_cod = "0";
        oggetto.varieta = "0";
        oggetto.finalita = "0";
        oggetto.tipologia = "";
    }

    ajaxAgronica(indirizzohttp + "/SalvaNuovoImpianto2019",
        "{ piva: '" + oggetto.piva + "', sa_cod: '" + oggetto.sa_cod + "', veg_cod: '" + oggetto.veg_cod + "', sup_imp: '" + oggetto.sup_imp + "', piva: '" + oggetto.piva + "', data_inizio: '" + oggetto.data_inizio + "', data_fine: '" + oggetto.data_fine + "', varieta: '" + oggetto.varieta + "', finalita: '" + oggetto.finalita + "', disciplinare: '" + oggetto.disciplinare + "', tipologia: '" + oggetto.tipologia + "', nome: '" + oggetto.nome + "', lotto: '" + oggetto.lotto + "', data_semina: '" + oggetto.data_semina + "', data_raccolta: '" + oggetto.data_raccolta + "', via_stringa: '" + oggetto.via_stringa.replace("'", "`") + "', hiddenPunti_Nuovo: '" + oggetto.hiddenPunti_Nuovo + "', layer_cod: '" + shape.glayerDoveDisegnoSuTipologiaStandard + "', elementografico_des: '" + $("#txtElementoGrafico_Des").val() + "', CodiciAnagrafeAggiuntivi: '" + codiciAnagrafeAggiuntivi + "', PropagazioneSalvataggioPoligono: '" + PropagazioneSalvataggioPoligono + "', Campo_Cod: " + oggetto.campo_cod + " }",
        function (risposta) {

            $('#responseInterferenze').width('0px');
            $('#responseInterferenze').html('');

            utility.log('SalvaNuovoImpianto, esito = ' + risposta.RispostaStringa);

            //'  Vanni, 05/06/2014 09:42:52: verifico se e come ricaricare...
            //RicaricaAziende(oggetto.piva, oggetto.rag_soc);

            var app = risposta.RispostaStringa.split(".");
            if (app.length > 1) {
                RicaricaSituazioneMappa(app[1], "", oggetto);
            }

        }, null);

    return true;
}

function RicaricaSituazioneMappa(id, chiavealbero, oggetto) {
    var ricaricaNecessario = !stringhe.contains(layerNonAlbero, shape.glayerDoveDisegnoSuTipologiaStandard);

    if (ricaricaNecessario) {

        utility.log("Ricarica necessario");

        if (shape.selectedShape != undefined)
            shape.selectedShape.setMap(null);

        var saCod1 = oggetto.sa_cod.split("|")[1];

        if (oggetto.piva !== $("#ddl_azienda_html").data("kendoDropDownList").value()) {
            DDLCentroStartValue = saCod1;
            RicaricaAziende(oggetto.piva, oggetto.rag_soc, true);
        } else {


            if (saCod1 !== $("#ddl_Sa_Cod_html").data("kendoDropDownList").value()) {

                $("#ddl_Sa_Cod_html").data("kendoDropDownList").value(saCod1);

                //imposta il nuovo centro lato server, a questo punto manca simulo pure il click su "Aggiorna", passando "true"
                ImpostaSa_Cod(true);

            } else {
                AlberoAnagrafica2017lettura();
                AggiornaFiltro_Client_click();
            }
        }

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

        ShapeAggiungiOggetti(place[place.length - 1], placeLivelli[shape.glayerDoveDisegnoSuTipologiaStandard], col, shape.selectedShape.getPath(), lat_centro, lng_centro, false);
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
}

function ConfermaBufferZoneIntersection() {
    BufferZoneIntersection_salva();
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
                    ChiudiKendoDialog("#dialogAB");
                    AggiornaTutto(false);
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
        //i18n__
        alert("per procedere spuntare prima di confermare, altrimenti fare click su annulla.");
        return false;
    }

    var piva = DropDownValoreSelezione("#ddl_azienda_html");
    var sa_cod = DropDownValoreSelezione("#ddl_Sa_Cod_html");

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
    if (GisPurpose === Enum_GisPurpose.Completo && Entita_Cod != undefined) {
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
        //i18n__
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
            ChiudiKendoDialog("#dialogEliminaMultipoint");
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

            $('#AggiornaFiltro_Client').click();

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
        let msgConfEl = Traduzione(AgronicaControlliGisResx, "jsMsgSpuntaPerConfermareOppureAnnulla");
        alert(msgConfEl);
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

            ChiudiKendoDialog("#dialogEliminaImpianto");
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

            //dall'albero se ne va l'elemento cancellato, quindi ricarico
            AlberoAnagrafica2017lettura();

            //GABRIELE 2019 07 26 Evito unzoom in cancellazione
            //if ($('#AggiornaFiltro').length > 0) {
            //    $('#AggiornaFiltro').click();
            //} else {
            //    $("#AggiornaFiltro_Client").click();
            //}
            AggiornaLayer(false);

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

function GetPropagazioneSalvataggioPoligono() {

    let valSelezionato = $("#cmbAssocia_Modifica_GenerazionePoligoni").val();
    let optListaClassi = $("#cmbAssocia_Modifica_GenerazionePoligoni option[value=" + valSelezionato + "]").attr('class').split(/\s+/);;

    var rval = 0;

    for (var i = 0; i < optListaClassi.length; i++) {
        let a = optListaClassi[i];
        switch (a) {
            case "ModificaAppezzamento":
                rval = Enum_PropagazioneSalvataggioPoligono.Appezzamento.value;
                break;
            case "NuovoImpiantoModificaAppezzamento":
                rval = Enum_PropagazioneSalvataggioPoligono.ImpiantoSuAppezzamentoEsistente.value;
                break;
            case "NuovoImpianto":
                rval = Enum_PropagazioneSalvataggioPoligono.ImpiantoSuAppezzamentoEsistente.vlue;
                break;
            default:
                rval = parseInt(valSelezionato);
                break;
            //mi trovo sulla modifica di un impianto e non di un appezza.
        }
    }

    return rval;

}

/******************* MODIFICA IMPIANTO ************************/

function ModificaImpiantoVerificaDati(TipoSalvataggio) {

    var stop = false;
    var stopMsg = Traduzione(AgronicaControlliGisResx, "jsMsgCompletareCampiObbligatori");

    //caso di tipo Associazione a nodo albero
    if (TipoSalvataggio === Enum_TipoModifica.SalvataggioDiretto) {

    }

    //caso di tipo modifica
    if (TipoSalvataggio === Enum_TipoModifica.ModificaImpianto) {

        var pivasuperuser = $('#hiddenID').val().split("|")[0];
        var Entita_Cod = $('#hiddenID').val().split("|")[1];

        var hiddenPunti_modifica = $('#hiddenPunti_modifica').val();
        var sup_google = $('#pop_up_sup_google_modifica').val();
        var sup_impianto = $('#pop_up_sup_app_modifica').val();

        //GABRIELE 2019 07 30 Modifica Appezzamento...
        if (shape.glayerDoveDisegnoSuTipologiaStandard === "1") {

            if (GetPropagazioneSalvataggioPoligono() !== Enum_PropagazioneSalvataggioPoligono.ImpiantoSuAppezzamentoEsistente.value) {

                //se sono tutte valorizzate allora posso procedere
                if (pivasuperuser == "" || Entita_Cod == "" || sup_google == "" || sup_impianto == "" || hiddenPunti_modifica == "") {
                    stop = true;
                }
            } else {
                let oVerificaDatiNuovoImpianto = ModificaImpiantoVerificaDatiSuTipoImpianto();

                if (TxtValiditaInizioAppezzamentoGisModificaErroreDate()) {
                    oVerificaDatiNuovoImpianto.stop = true;
                    oVerificaDatiNuovoImpianto.stopMsg += Traduzione(AgronicaControlliGisResx, "jsMsgPeriodoValErroreRivedereDati");
                }

                return oVerificaDatiNuovoImpianto;

            }


        } else {

            return ModificaImpiantoVerificaDatiSuTipoImpianto();
        }

    }
    return { stop: stop, stopMsg: stopMsg };

}

function ModificaImpiantoVerificaDatiSuTipoImpianto() {

    var cmbAssocia_Modifica_GenerazionePoligoni = $("#cmbAssocia_Modifica_GenerazionePoligoni").val();

    var stop = false;
    var stopMsg = "Completare i dati obbligatori.";
    var specie = $('#pop_up_specie_modifica').val();
    var varieta = $('#pop_up_varieta_modifica').val();
    var finalita = $('#pop_up_finalita_modifica').val();
    var tipologia = $('#pop_up_tipologia_modifica').val();
    if (tipologia == null)
        tipologia = "0";
    var mData_Inizio = $('#pop_up_m_data_inizio').val();
    var mData_Fine = $('#pop_up_m_data_fine').val();
    var sup_google = $('#pop_up_sup_google_modifica').val();
    var hiddenPunti_modifica = $('#hiddenPunti_modifica').val();
    var sup_impianto = $('#pop_up_sup_app_modifica').val();
    var pivasuperuser = $('#hiddenID').val().split("|")[0];
    var Entita_Cod = $('#hiddenID').val().split("|")[1];

    //se sono tutte valorizzate allora posso procedere
    if (pivasuperuser == "" || Entita_Cod == "" || specie == "" || varieta == "" || finalita == "" || tipologia == "" || sup_google == "" || sup_impianto == "" || hiddenPunti_modifica == "") {

        var a = $("#pop_up_sup_app_modifica_chkModificaDatiDiAnagrafica");
        if (a) {
            let modificaAnagrafica = (a.is(":checked"));
            if (modificaAnagrafica && (specie == "" || varieta == "" || finalita == "" || tipologia == "")) {
                stop = true;
            }
        } else {
            stop = true;
        }

    }
    if (GisPurpose === Enum_GisPurpose.SementiSportello && (mData_Inizio == "" || mData_Fine == "")) {
        stop = true;
    }

    if (mData_Inizio == "" && cmbAssocia_Modifica_GenerazionePoligoni !== "1" && shape.glayerDoveDisegnoSuTipologiaStandard.toString() === "1" && !(specie === "" || specie === "null")) {
        stopMsg = stopMsg + Traduzione(AgronicaControlliGisResx, "jsMsgImpostareDataInizioImpianto");
        stop = true;
    }

    return { stop: stop, stopMsg: stopMsg };

}

function ModificaCatastoVerificaRiparto(TipoSalvataggio, area) {

    var sommaRiparti = 0.0;
    var sommaSuperficiCondotte = 0.0;

    var controlloBloccante = false;

    if ($(".SommaRiparto1").text() === "") {
        //elemento non renderizzato correttamente, impossibile proseguire.
        //i18n__
        kendo.alert("Non sono state lette correttamente le informazioni di riparto catastale, quindi non è possibile proseguire. Contattare l'assistenza.");
        return false;
    }

    if ($(".SommaTotaleSuperficieCondotta").text() === "") {
        //elemento non renderizzato correttamente, impossibile proseguire.
        //i18n__
        kendo.alert("Non sono state lette correttamente le informazioni sulle superfici condotte, quindi non è possibile proseguire. Contattare l'assistenza.");
        return false;
    }

    //$(".InfoRiparto1").each(function () {
    //    sommaRiparti += parseFloat($(this).text().replace(",", "."));
    //});


    sommaRiparti = parseFloat($(".SommaRiparto1").text().replace(",", "."));
    sommaSuperficiCondotte = parseFloat($(".SommaTotaleSuperficieCondotta").text().replace(",", "."));

    var SupNuova = parseFloat($("#pop_up_sup_app_modifica").val().replace(",", "."));
    var rval = true;
    var messaggio = "";
    if (sommaRiparti > SupNuova) {
        //i18n__
        messaggio = messaggio + "La superficie indicata per la particella (" + SupNuova.toString() + ") è minore della somma delle superfici dei riparti su Appezzamenti (" + sommaRiparti.toString() + ")<br/>";
        rval = false;
    }

    if (sommaSuperficiCondotte > SupNuova) {
        //i18n__
        messaggio = messaggio + "La superficie indicata per la particella (" + SupNuova.toString() + ") è minore della somma delle superfici condotte (" + sommaSuperficiCondotte.toString() + ")<br/>";
        rval = false;
    }

    if (messaggio !== "") {
        if (controlloBloccante) {
            kendo.alert(messaggio);
        } else {
            //i18n__
            messaggio += "<br /><br /><span style='font-weight: bold'>Procedendo sarà necessario verificare le superfici di riparto. Confermi l'operazione?</span>"
            kendoDlgMessage("Incongruenze riscontrate su modifica catasto", messaggio, function () {
                ModificaImpiantoProcedi(TipoSalvataggio, area, false);
            }, function () {

            })
        }

    }

    return rval;
}

/* funzione per la modifica di un impianto esistente */
function ModificaImpianto(TipoSalvataggio) {

    if (TipoSalvataggio === undefined) {
        TipoSalvataggio = Enum_TipoModifica.SalvataggioDiretto;
    }

    if (TipoSalvataggio !== Enum_TipoModifica.SalvataggioDiretto) {

        if (!document.getElementById("terreno_nudo_modifica").checked) {
            let veg_cod = $('#pop_up_specie_modifica').val();
            let finalita = $('#pop_up_finalita_modifica').val();
            let data_inizio = $('#pop_up_m_data_inizio').val();
            let data_fine = $('#pop_up_m_data_fine').val();
            if (!SementiMappaturaLiberaSpecieVegetalePermessa(veg_cod, finalita, data_inizio, data_fine, "Modifica impianto")) {
                return false;
            }
        }
    }

    interfaccia.loading(true);

    if (!SementiAccettaInteferenze(false)) {
        interfaccia.loading(false);
        return false;
    }

    if (TipoSalvataggio === Enum_TipoModifica.ModificaImpianto) {
        PredisponiPerSalvataggio_Lotto(true);
    }

    var pivasuperuser = $('#hiddenID').val().split("|")[0];
    var Entita_Cod = $('#hiddenID').val().split("|")[1];

    var stop = ModificaImpiantoVerificaDati(TipoSalvataggio);

    if (stop.stop) {
        interfaccia.loading(false);
        kendoDlgMessage("", stop.stopMsg);
        return false;
    }

    var comboJQuerySelector = "";
    var comboJQuerySelectorPropagazioneSalvataggioPoligono = "";

    if (KendoDialogStatoAperto("#pop_up_modificaImpianto")) {
        comboJQuerySelector = "#opt_associa_modifica";
        comboJQuerySelectorPropagazioneSalvataggioPoligono = "#cmbAssocia_Modifica_GenerazionePoligoni";
    }

    if (KendoDialogStatoAperto("#dialogConfermaAssociazione")) {
        comboJQuerySelector = "#option_associa";
        comboJQuerySelectorPropagazioneSalvataggioPoligono = "#cmbAssocia_GenerazionePoligoni";
    }

    var flagProsegui = true;

    if ($(comboJQuerySelector).val() != '1') {
        flagProsegui = false;
    }

    var verifica_EsistonoDatiGisSuElementiAnagraficiCollegati = false;
    var PropagazioneSalvataggioPoligono = $(comboJQuerySelectorPropagazioneSalvataggioPoligono).val();

    if (shape.glayerDoveDisegnoSuTipologiaStandard.toString() === "19" && PropagazioneSalvataggioPoligono !== "2") {
        flagProsegui = false;
        verifica_EsistonoDatiGisSuElementiAnagraficiCollegati = true;
    }

    var verifica_EsistenzaImpiantiDaEntita = false;
    if (shape.glayerDoveDisegnoSuTipologiaStandard.toString() === "1" && PropagazioneSalvataggioPoligono !== "1") {
        if ($("#cmbImpiantiFigli_Modifica").val() === "-1") {
            flagProsegui = false;
            verifica_EsistenzaImpiantiDaEntita = true;
        }
    }

    var area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);
    area = salvataggioDirettoConAppezzaAreaCalcola(area);

    // controllo se devo associare la superficie a impianto o appezzamento
    if (flagProsegui) {

        //posso fare il semplice salvataggio dei dati senza modificare la superficie
        ModificaImpiantoProcedi(TipoSalvataggio, area, true);

        //vanni, 26/9/2019: incapsulato in ModificaImpiantoProcedi
        //if (TipoSalvataggio === Enum_TipoModifica.ModificaImpianto) {
        //    ModificaDatiImpianto();
        //} else {
        //    salvataggioDiretto(area);
        //}

        //interfaccia.loading(false);
        return true;
    }

    var MVCArray = shape.selectedShape.getPath();

    if (TipoSalvataggio === Enum_TipoModifica.ModificaImpianto) {
        var sup_impianto = $('#pop_up_sup_app_modifica').val();
        area = sup_impianto;
    }

    //var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString().replace(/\\/g, '\\\\'); ;
    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();

    //verifico le condizioni di pre-salvataggio ed agisco di conseguenza...
    var verifica_EsistonoOperazioniSuImpianti = false;
    var verifica_EsisteCatastoAssociatoAdAppezzamento = false;

    if ($(comboJQuerySelector).val() == '2') {
        verifica_EsistonoOperazioniSuImpianti = true;
    }

    if ($(comboJQuerySelector).val() == '3') {
        verifica_EsistonoOperazioniSuImpianti = true;
        verifica_EsisteCatastoAssociatoAdAppezzamento = true;
    }

    var validita_inizio = "";
    var validita_fine = "";

    if (verifica_EsistenzaImpiantiDaEntita) {
        validita_inizio = $('#pop_up_m_data_inizio').val();
        validita_fine = $('#pop_up_m_data_fine').val();
    }


    if (shape.selectedShape.StandardEntita_layerDiAppartenenza == 3) {

        flagProsegui = false;

    } else {

        //altri casi
        var risp_ajax = "";
        ajaxAgronicaSync(indirizzohttp + "/VerifichePreSalvataggioConImpianti",
            "{ chiaveAlbero: '" + ChiaveAlbero + "' " +
            ", verifica_EsistonoOperazioniSuImpianti: " + verifica_EsistonoOperazioniSuImpianti +
            ", verifica_EsistonoDatiGisSuElementiAnagraficiCollegati: " + verifica_EsistonoDatiGisSuElementiAnagraficiCollegati +
            ", verifica_EsisteCatastoAssociatoAdAppezzamento: " + verifica_EsisteCatastoAssociatoAdAppezzamento +
            ", verifica_EsistenzaImpiantiDaEntita: " + verifica_EsistenzaImpiantiDaEntita +
            ", validita_inizio: '" + validita_inizio + "'" +
            ", validita_fine: '" + validita_fine + "'" +
            " }",
            true,
            function (risposta) {
                risp_ajax = risposta.RispostaStringa;
            },
            null
        );

        flagProsegui = (risp_ajax.EsistonoOperazioniSuImpianti &&
            risp_ajax.EsistonoDatiGisSuElementiAnagraficiCollegati &&
            risp_ajax.EsisteCatastoAssociatoAdAppezzamento &&
            risp_ajax.EsistonoImpiantiDaEntita
        );

    }

    if (flagProsegui) {

        //non ho alcun impedimento, posso procedere con la modifica     
        ModificaImpiantoProcedi(TipoSalvataggio, area, true);

        //vanni, 26/9/2019: incapsulato in ModificaImpiantoProcedi
        //if (TipoSalvataggio === Enum_TipoModifica.ModificaImpianto) {
        //    ModificaDatiImpianto();
        //} else {
        //    salvataggioDiretto(area);
        //}

        //vanni, 16/01/2015, test senza chiamata a salvataggio diretto                            
        //salvataggioDiretto(area);

        //GABRIELE 03 04 2019
        //ChiudiKendoDialog(popupDaChiudere);
        //interfaccia.loading(false);
        return true;
    }

    var messaggioFinale = "";

    if (shape.selectedShape.StandardEntita_layerDiAppartenenza !== 3) {

        if (verifica_EsisteCatastoAssociatoAdAppezzamento && !risp_ajax.EsisteCatastoAssociatoAdAppezzamento) {

            interfaccia.loading(false);

            let msgModificaManno = Traduzione(AgronicaControlliGisResx, "jsMsgAppezzamentoCollegatoCatastoModificaManuale");
            kendoDlgMessage("", msgModificaManno);
            $(comboJQuerySelector).val(2);
            //non posso proseguire in ogni caso...
            return false;
        }

        if (verifica_EsistenzaImpiantiDaEntita && !risp_ajax.EsistonoImpiantiDaEntita) {

            interfaccia.loading(false);

            kendoDlgMessage("", risp_ajax.MessaggioEsistonoImpiantiDaEntita);
            //non posso proseguire in ogni caso...
            return false;
        }



        flagProsegui = true;

        if (verifica_EsistonoOperazioniSuImpianti && !risp_ajax.EsistonoOperazioniSuImpianti) {
            messaggioFinale = messaggioFinale + risp_ajax.MessaggioOperazioniSuImpianti;
            flagProsegui = false;
        }

        if (verifica_EsistonoDatiGisSuElementiAnagraficiCollegati && !risp_ajax.EsistonoDatiGisSuElementiAnagraficiCollegati) {
            messaggioFinale = messaggioFinale + risp_ajax.MessaggioDatiGisSuElementiAnagraficiCollegati
            flagProsegui = false;
        }

        if (flagProsegui) {

            ModificaImpiantoProcedi(TipoSalvataggio, area);

            //ChiudiKendoDialog(popupDaChiudere);
            //interfaccia.loading(false);
            return true;
        }


        var lblAlert = "";
        var dialogAlert = "";
        var dialogAlertHidden = "";

        if (TipoSalvataggio === Enum_TipoModifica.ModificaImpianto) {
            lblAlert = '#lbl_alert_operazioni_Appezza';
            dialogAlert = "#dialogAllertOperazioniAppezza";
            $("#dialogAllertOperazioni_hidden").val(2);
        } else {
            lblAlert = '#lbl_alert_operazioni';
            dialogAlert = "#dialogAllertOperazioni";
            $("#dialogAllertOperazioni_hidden").val(1);
        }

        $(lblAlert).html(messaggioFinale);
        ApriKendoDialog(dialogAlert);

        //fine se non è catasto..
    } else {


        //caso catasto
        if ($(comboJQuerySelector).val() === '5') {
            flagProsegui = ModificaCatastoVerificaRiparto(TipoSalvataggio, area);
        } else {
            flagProsegui = true;
        }

        if (flagProsegui) {
            ModificaImpiantoProcedi(TipoSalvataggio, area);
        }
    }

    interfaccia.loading(false);

    return flagProsegui;

    // GABRIELE 03 04 2019    
    /*
    interfaccia.loading(false);

    kendoDlgConfirm("Operazioni", messaggioFinale,
        function () {
            if (TipoSalvataggio === Enum_TipoModifica.ModificaImpianto) {

                ModificaDatiImpianto();

            } else {

                //GABRIELE 03 04 2019
                //già calcolato precedentemente...
                //var area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);
                //area = salvataggioDirettoConAppezzaAreaCalcola(area);
                salvataggioDiretto(area);

            }
        },
        function () {
            AggiornaTutto(true);
        },
        "#dialogAllertOperazioni"
    );
     */
}

function ModificaImpiantoProcedi(TipoSalvataggio, area, SaltaVerificaNuovoElemento) {

    if (SaltaVerificaNuovoElemento) {
        if (TipoSalvataggio === Enum_TipoModifica.ModificaImpianto) {
            ModificaDatiImpianto();
        } else {
            salvataggioDiretto(area);
        }
    } else {

        if (TipoSalvataggio === Enum_TipoModifica.ModificaImpianto) {
            //vanni, 16/01/2015, test senza chiamata a salvataggio diretto, commentata chiamata a ModificaDatiImpianto
            if (isNuovoElemento()) {
                salvataggioDirettoConAppezza(area);
            }
            else {
                ModificaDatiImpianto();
            }
        }
        else {
            salvataggioDiretto(area);
        }
    }
}

function ModificaDatiImpianto() {
    interfaccia.loading(true);
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

    var sup_impianto;
    if ($('#opt_associa_modifica').val() == 1) {
        sup_impianto = 0;
    }
    else {
        sup_impianto = $('#pop_up_sup_app_modifica').val();
    }

    if (shape.glayerDoveDisegnoSuTipologiaStandard === "1") {
        sup_impianto = $('#pop_up_sup_app_modifica').val();
    }

    sup_impianto = salvataggioDirettoConAppezzaAreaCalcola(sup_impianto);

    var hiddenPunti_modifica = $('#hiddenPunti_modifica').val();

    var mData_Inizio = $('#pop_up_m_data_inizio').val();
    var mData_Fine = $('#pop_up_m_data_fine').val();
    var codice_socio = $('#txt_m_CodiceSocio').val();
    var stop = false;
    var stopMsg = "";

    var stop = ModificaImpiantoVerificaDati(Enum_TipoModifica.ModificaImpianto);

    if (stop.stop) {
        alert(stop.stopMsg);
        interfaccia.loading(false);
        return false;
    }

    var codiciAnagrafeAggiuntivi = LeggiCodiciAnagrafeAggiuntivi(".m_CodiceAnagrafe");

    if ($("#terreno_nudo_modifica").prop("checked")) {

        let id_cod = parseInt($("#pop_up_destuso_modifica").val());
        let arr = JSON.parse(codiciAnagrafeAggiuntivi);
        arr.push({ id_cod: id_cod, val_cod: "" });
        codiciAnagrafeAggiuntivi = JSON.stringify(arr);

        specie = "0";
        varieta = "0";
        finalita = "0";
        tipologia = "0";
    }

    var modificaAnagrafica = true;

    var o = {
        entita_cod: Entita_Cod,
        nome_appezza: nome_appezza,
        sup_imp: sup_impianto,
        data_inizio: mData_Inizio,
        data_fine: mData_Fine,
        specie: specie,
        varieta: varieta,
        finalita: finalita,
        tipologia: tipologia,
        hiddenPunti_modifica: hiddenPunti_modifica,
        via_stringa: via_stringa,
        lotto: lotto,
        codice_socio: codice_socio,
        CodiciAnagrafeAggiuntivi: codiciAnagrafeAggiuntivi
    }

    var a = $("#pop_up_sup_app_modifica_chkModificaDatiDiAnagrafica");
    var ApiSalvataggioImpianto = "/ModificaImpianto2018";
    if (a) {
        modificaAnagrafica = (a.is(":checked"));
        ApiSalvataggioImpianto = "/ModificaImpianto2019";
        $.extend(o, { ModificaAnagrafica: modificaAnagrafica });
    }
    //Vanni, 16/01/2017 16:09:24: ricondotto a chiamata standard ...
    ajaxAgronica(indirizzohttp + ApiSalvataggioImpianto,
        JSON.stringify(o),
        function (risposta) {

            interfaccia.loading(false);


            if (risposta.RispostaOK) {

                //alert(risposta.RispostaStringa);

                //16/01/2015, ripulisco la selezione.
                $('#hiddenPunti_modifica').val("");

                //pulisco la descrizione
                $(".smallDescrAlbero").html("")

                ChiudiKendoDialog("#pop_up_modificaImpianto");

                kendoDlgMessage("", "Operazione eseguita correttamente",
                    function () {
                        AlberoAnagrafica2017lettura();
                        AggiornaLayer(false);
                    });

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

function AggiornaTutto(autoFit) {

    CoordFromPoints = "";
    CoordFromPointsMVCArray = new google.maps.MVCArray();

    AggiornaLayer(autoFit);
    GestisciStrumentoScomponiRicomponi();
}

function GestisciStrumentoScomponiRicomponi() {

    var piva = DropDownValoreSelezione("#ddl_azienda_html");
    var sa_cod = DropDownValoreSelezione("#ddl_Sa_Cod_html");

    ajaxAgronica(
        indirizzohttp + "/GestisciStrumentoScomponiRicomponi",
        JSON.stringify({ piva: piva, sa_cod: sa_cod }),
        function (risposta) {

            if (risposta.RispostaStringa.Ricarica) {
                //i18n__
                kendoDlgMessage("", "Lo strumento di scomposizione ha attivato i layer necessari, sarà ricaricata la pagina.");
                window.location.href = indirizzohttp;

            } else {

                if (risposta.RispostaStringa.Mostra) {
                    $("#dialogScomponiPunti").dialog("open");
                }

            }

        }, null);
}

function Disenga_poligono_dropdown_show(show) {

    if (show) {
        $("#selimg_poligono").show();
        $("#selimg_multipoint").show();
    } else {
        $("#selimg_poligono").hide();
        $("#selimg_multipoint").hide();
    }
}

/*******************************************************************/
/********************* SHAPE ***************************************/
/*******************************************************************/

/* tolgo l'editing sullo shape */
function clearSelection() {

    utility.warn("clearSelection()");

    if (shape.selectedShape) {
        SelectShape(shape.selectedShape, false);
        shape.selectedShape = null;
        shape.LayerUiPulisciEvidenza();
    }
    if (shape.selectedShapeArray) {
        for (let i = 0; i < shape.selectedShapeArray.length; i++) {
            let shapeS = shape.selectedShapeArray[i];
            SelectShape(shapeS, false);
        }
        shape.LayerUiPulisciEvidenza();
    }

    AlberoAnagrafica().uncheckAll();

    $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val('');

    if (gNuovo)
        Disenga_poligono_dropdown_show(true);

    //nascondo se c'è la popup info
    mappa.NascondiInfo();

    /*GABRIELE
    
        if (shape.selectedShape) {
    
            $(".smallDescrAlbero").html("");
    
            bAbilitaRenderPerEventoDrag = true;
    
            utility.warn("clearSelection()");
    
            shape.selectedShape.set('fillColor', shape.selectedShape.colore_precedente);
            shape.selectedShape.setEditable(false);
    
            shape.selectedShape = null;
            $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val('');
            AlberoAnagrafico2017CheckAll(false);
    
            //$('.jstree-clicked').removeClass('jstree-clicked');
    
            if (gNuovo)
                Disenga_poligono_dropdown_show(true);
    
            //nascondo se c'è la popup info
            mappa.NascondiInfo();
        }
    */

}

function AlberoAnagrafico2017CheckAll(isAllChecked) {
    /*GABRIELE
        var treeview = $("#GIS_treeview").data("kendoTreeView");
        checkUncheckAllNodes(treeview.dataSource.view(), isAllChecked);
    
        if (!isAllChecked) {
            var multi = $("#GIS_multiselect").data("kendoMultiSelect");
            multi.value("");
            multi.input.blur();
        }
    */
}

function clearSelectionArray() {

    if (shape.selectedShapeArray) {

        utility.warn("clearSelectionArray()");

        for (i = 0; i < shape.selectedShapeArray.length; i++) {
            var ss = shape.selectedShapeArray[i];
            SelectShape(ss, false);
        }

        shape.selectedShapeArray = new Array();
        $("#ChiaveAlberoMultiSelezione").val("");
        ShapeAddedd = false;
    }

    if (shape.selectedPointsArray) {
        utility.warn("clearSelectionArray(), points");

        for (i = 0; i < shape.selectedPointsArray.length; i++) {
            var ss = shape.selectedPointsArray[i];
            SelectPoint(ss, false);
        }
        shape.selectedPointsArray = new Array();
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

    if (!ctrlPressed) {
        clearSelectionBoth();
    }

    shape.selectedPointsArray.push(shapeS);

    //in questo evento ancora non viene marcato selezionato, da qui il "not (!)" in chiamata
    addMarkerMultiPointModifica(shapeS.html, shapeS.getPosition(), !shapeS.selected);


    setSelectionAlberoBS(shapeS);
}

var PuntiSelezionati_Scomposti = new Array();
var UltimoPuntoSelezionato = undefined;
var UltimoPuntoSelezionato_Icona = undefined;

function setSelectionPoint(shapeS) {

    shapeS.selected = !shapeS.selected;
    //GABRIELE
    if (shapeS.selected) {
        shapeS.setIcon(shapeS.icon_sel);
        //UltimoPuntoSelezionato = shapeS;
        //PuntiSelezionati_Scomposti.push(shapeS);
        //CoordFromPoints = CoordFromPoints + shapeS.getPosition() + ",";
        //CoordFromPointsMVCArray.push(shapeS.getPosition());
    } else {
        shapeS.setIcon(shapeS.icon_norm);
    }

    //let treeview = AlberoAnagrafica();
    //let idNoA = shapeS.chiavealbero.toString().replace(' ', '');
    //treeview.checkFromId(idNoA);

    return;

    if (shapeS.getIcon() != flgSelezionato) {

        UltimoPuntoSelezionato = shapeS;
        UltimoPuntoSelezionato_Icona = shapeS.icon;
        PuntiSelezionati_Scomposti.push(shapeS);

        CoordFromPoints = CoordFromPoints + shapeS.getPosition() + ",";
        CoordFromPointsMVCArray.push(shapeS.getPosition());
        utility.log('test: ' + shapeS.html);
        utility.log('lat: ' + shapeS.getPosition());

        if (shapeS.apriInfoAutomaticamente) {
            $("#info_appezzamento").click();
        }

        if (!shapeS.NonSelezionabile) {
            shapeS.setIcon(flgSelezionato);
        }

        //'  Vanni, 17/02/2016 15:48:46: imposto la selezione dell'albero
        setSelectionAlberoBS(shapeS);
    }

}

var ShapeAddedd = false;
/* Evento di Selezione nultipla dello shape */
function setMultiSelection(shapeS) {

    var oldShape = shape.selectedShape;

    utility.log("setMultiSelection chiamata!");

    setSelection(shapeS);
    SelectShape(shapeS, true);
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

                //GABRIELE
                //shape.selectedShapeArray = jQuery.removeFromArray(shapeS, shape.selectedShapeArray);
                shape.selectedShapeArray = jQuery.grep(shape.selectedShapeArray, function (elem) {
                    return elem.Entita_Cod != shapeS.Entita_Cod;
                });

                clearSelection();

                //uncheckNode(shapeS);
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
    SelectShape(shape.selectedShape, true);

    //permessi
    if (shape.selectedShape.modifica == 'True') {
        shapeS.setEditable(true);
        $('#save-button').show();
        $('#save-buttonHeader').show();
        $("#save-plus").hide();
    }
    else {
        shapeS.setEditable(false);
        $('#save-button').hide();
        $('#save-buttonHeader').hide();
        $("#save-plus").hide();
    }

    if (shape.selectedShape.cancellazione == 'True') {
        shapeS.setEditable(true);
        $('#delete-button').show();
        $('#delete-buttonHeader').show();
    }
    else {
        shapeS.setEditable(false);
        $('#delete-button').hide();
        $('#delete-buttonHeader').hide();
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

    Disenga_poligono_dropdown_show(false);
    //fine permessi

    if (shapeS.chiavealbero != null) {
        setSelectionAlberoBS(shapeS);
    }


}

/* Evento di Selezione dello shape */
function setSelection(shapeS) {


    bAbilitaRenderPerEventoDrag = false;

    //GABRIELE
    //mappa.elemenotMappa.setCenter(getBounds(shapeS).getCenter(), mappa.elemenotMappa.fitBounds(getBounds(shapeS)));
    //var zoom = mappa.elemenotMappa.getZoom() - 1;
    //mappa.elemenotMappa.setZoom(zoom);

    if (!ctrlPressed)
        clearSelectionBoth();

    shape.selectedShape = shapeS;
    SelectShape(shape.selectedShape, true);

    //permessi
    if (shape.selectedShape.modifica == 'True') {

        shapeS.setEditable(true);

        $('#save-button').show();
        $('#save-buttonHeader').show();
        if (shape.selectedShape.StandardEntita_layerDiAppartenenza === 1) {
            $('#save-plus').show();
        } else {
            $('#save-plus').hide();
        }
    }
    else {

        shapeS.setEditable(false);
        $('#save-button').hide();
        $('#save-buttonHeader').hide();
        $('#save-plus').hide();
    }

    if (shape.selectedShape.cancellazione == 'True') {


        shapeS.setEditable(true);

        $('#delete-button').show();
        $('#delete-buttonHeader').show();
    }
    else {

        shapeS.setEditable(false);
        $('#delete-button').hide();
        $('#delete-buttonHeader').hide();
    }

    if (shape.selectedShape.informazioni == 'True') {

        shapeS.setEditable(true);

        //mostrerò tutte le informanzioni
        $('#info_appezzamento').show();
    }
    else {

        shapeS.setEditable(false);
        //mostro poche informazioni
        $('#info_appezzamento').show();
    }

    Disenga_poligono_dropdown_show(false);
    //fine permessi

    if (shapeS.chiavealbero != null) {
        setSelectionAlberoBS(shapeS, ctrlPressed);
    }
}

/**
 * selezione di un elemento cartografica e conseguente selezione in albero anagrafico
 * @param {string} shapeS identificativo selezionato
 */
function setSelectionAlberoBS(shapeS, multisel) {

    let treeview = AlberoAnagrafica();

    // se non devo aggiungere le selezione (per multiselezione)...
    if (!multisel) {
        treeview.uncheckAll();
    }

    let idNoA = shapeS.chiavealbero.toString().replace(' ', '');

    treeview.checkFromId(idNoA);

    if (Client_Flag_CatastoAppezzamento && idNoA.startsWith("10§")) {
        ImpostaFiltriCatastoDaChiaveAlbero(idNoA);
    }

    //if (!treeview.checkFromId(idNoA)) {
    //    return;
    //}

    //Parte GIS
    $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val(idNoA);
    shape.selezionaIndiceLayerDoveScrivoDaOggettoGrafico();

    /*GABRIELE
        generaAlberoAnagraficaCheck_EventoSimulato = true;
        
        //' VAnni: 14/11/2017: questo è il check ma va gestito
        $("#_" + getitem.uid).click();
        
        ////' VAnni: 14/11/2017: Questa è la selezione del singolo.        
        //var selectitem = treeview.findByUid(getitem.uid);
        //treeview.select(selectitem);
        
        //simulo click su "OK"
        var checkedNodes = [];
        
        getCheckedNodes(treeview.dataSource.view(), checkedNodes);
        agroDialogTreeViewFilter_populateMultiSelect("GIS", checkedNodes);
        
        //scroll dell'albero fino ad elemento selezionato
        myScrollTop(treeview);
    
        //aggiornamento Descrizione
        AnagraficaRiassunto(true);
    
        //Parte GIS
        $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val(idNoA);
        
        shape.selezionaIndiceLayerDoveScrivoDaOggettoGrafico();
        
        var selezionatoLayerDataChiaveAlbero = false;
        for (var i = 0; i < mappa.Livelli.length; i++) {
            if (!selezionatoLayerDataChiaveAlbero)
                selezionatoLayerDataChiaveAlbero = selezionaIndiceLayerDoveScrivoDaAlbero(mappa.Livelli[i], shapeS.chiavealbero.toString());
        }
    */
}

/* Eliminazione dello shape */
function deleteSelectedShape() {

    if (shape.selectedShapeArray.length === 0) {

        //NO multiselezione

        if (shape.selectedShape) {

            if (shape.glayerDoveDisegnoSuTipologiaStandard === "3" && !PermessiGisServerSide.bool_catasto) {
                //i18n__
                kendo.alert("Non si dispone dei permessi di gestione del catasto dal GIS.");
                return false;
            }

            //GABRIELE DIALOG ELIMINA IMPIANTO
            var Entita_Cod = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();
            utility.log("dialogEliminaImpiantoOnOpen = " + Entita_Cod);

            let entitaFlag = true;
            //verifica se posso eliminare i planning.. lo faccio se: non sono sementi, si tratta di un planning.
            if (GisPurpose === Enum_GisPurpose.Completo && Entita_Cod != undefined) {
                if (stringhe.startsWith(Entita_Cod, chiaveAlberoPlanning)) {

                    let risp = GisAjaxSync("VerificaCancella", { Entita_Cod: Entita_Cod });
                    entitaFlag = (risp === "true");
                    //altrimenti non deve comparire il check <entita_gias> in array opts
                    //$("#divEliminaEntitaGIAS").hide();
                }
            }

            let opts = [];
            if (entitaFlag) {
                opts.push({
                    id: "entita_grafiche", text: Traduzione(AgronicaControlliGisResx, "jsLblEliminaEntitaGrafiche")
                });
            }
            if (AbilitaPF) { // in V_M Dim permessi As New PermessiUtenteCartografia()  permessi.permessoPrecisionFarming
                opts.push({
                    id: "precision_farming", text: Traduzione(AgronicaControlliGisResx, "jsLblEliminaDatiRaccoltiPrecisionFarming")
                });
                opts.push({
                    id: "precision_farmingAB", text: Traduzione(AgronicaControlliGisResx, "jsLblEliminaLineeGuidaABPrecisionFarming")
                });
            }

            opts.push({ id: "dato_gias_palm", text: Traduzione(AgronicaControlliGisResx, "jsLblEliminaDatoMisuratoGiasPALM") });
            opts.push({ id: "entita_gias", text: Traduzione(AgronicaControlliGisResx, "jsLblEliminaEntitaGiasAPPIMP") });
            opts.push({ id: "conferma_elim", text: Traduzione(AgronicaControlliGisResx, "jsLblConfermaEliminazione"), style: "font-weight:bold;" });

            let content = "<div style='margin-top:20px;'>";
            content += "<ul style='list-style:none;'>";
            for (o = 0; o < opts.length; o++) {
                content += "<li style='padding-bottom:20px;'>"
                content += "<input type='checkbox' id='" + opts[o].id + "' class='k-checkbox'>";
                content += "<label class='k-checkbox-label' for='" + opts[o].id + "'"
                if (opts[o].style != undefined) {
                    content += " style='" + opts[o].style + "'"
                }
                content += ">" + opts[o].text + "</label>";
                content += "</li>"
            }
            content += "</ul>";
            content += "</div>";

            let win_el = document.createElement("div");
            win_el.id = "id_tmp_kendo_dlg";
            document.body.appendChild(win_el);
            let $win_el = $("#id_tmp_kendo_dlg");

            var msgEliminaimp1 = Traduzione(AgronicaControlliGisResx, "jsLblEliminaImpianto");

            var msgOk = Traduzione(AgronicaControlliGisResx, "jsLblok");
            var msgAnnulla = Traduzione(AgronicaControlliGisResx, "jsLblAnnulla");


            $win_el.kendoDialog({
                title: msgEliminaimp1,
                closable: false,
                modal: true,
                visible: false,
                content: content,
                actions: [
                    {
                        text: msgOk,
                        action: function () {

                            if ($("#conferma_elim").prop("checked") === false) {
                                //Non chiudo la dialog...
                                return false;
                            }

                            let av = shape.selectedShape.html.split("|");
                            let pivasuperuser = av[0];
                            let Entita_Cod = av[1];
                            //se sono tutte valorizzate allora posso procedere
                            if (pivasuperuser == "" || Entita_Cod == "") {
                                //nessun poligono selezionato. già controllato...
                                return false;
                            }

                            let isChecked = function (id) {
                                if ($("#" + id).length === 0) {
                                    return "false";
                                }
                                if ($("#" + id).prop("checked")) {
                                    return "true";
                                }
                                return "false";
                            }

                            let objdata = {
                                entita_cod: Entita_Cod,
                                EliminaGrafica: isChecked("entita_grafiche"),
                                EliminaImpianto: isChecked("entita_gias"),
                                EliminaPrecisionFarming: isChecked("precision_farming"),
                                EliminaPrecisionFarmingABLine: isChecked("precision_farmingAB"),
                                EliminaDatoGiasPalm: isChecked("dato_gias_palm")
                            };

                            interfaccia.loading(true);

                            $.ajax({
                                type: "POST",
                                url: indirizzohttp + "/Cancella",
                                data: JSON.stringify(objdata),
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (msg) {

                                    interfaccia.loading(false);

                                    if (msg.d.Esito.toLowerCase() === "false") {
                                        kendo.alert(msg.d.StringaRisposta);
                                    } else {
                                        var msgEliminaimp2 = Traduzione(AgronicaControlliGisResx, "jsLblEliminazioneImpianto");
                                        kendoDlgMessage(msgEliminaimp2, msg.d.StringaRisposta,
                                            function () {

                                                if (shape.selectedShape != undefined)
                                                    shape.selectedShape.setMap(null);

                                                //dall'albero se ne va l'elemento cancellato, quindi ricarico
                                                AlberoAnagrafica2017lettura();

                                                //GABRIELE 2019 07 26 Evito unzoom in cancellazione
                                                //if ($('#AggiornaFiltro').length > 0) {
                                                //    $('#AggiornaFiltro').click();
                                                //} else {
                                                //    $("#AggiornaFiltro_Client").click();
                                                //}
                                                AggiornaLayer(false);

                                            });
                                    }
                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    alert(xhr.status);
                                    alert(thrownError);
                                }
                            });

                        }
                    },
                    { text: msgAnnulla }
                ],
                close: function (e) {
                    this.destroy();
                }
            });

            $win_el.data("kendoDialog").open();

            return;
        }

    } else {

        // controllo che appartengano allo stesso layer
        let layer = shape.selectedShapeArray[0].StandardEntita_layerDiAppartenenza;
        let cnt = shape.selectedShapeArray.length;
        let idx = 1;
        let stessoLayer = true;
        while (stessoLayer && idx < cnt) {

            if (layer !== shape.selectedShapeArray[idx].StandardEntita_layerDiAppartenenza) {

                stessoLayer = false;
            }
            idx++;
        }

        if (!stessoLayer) {
            return;
        }

        if (layer === 3 && !PermessiGisServerSide.bool_catasto) {
            //i18n__
            kendo.alert("Non si dispone dei permessi di gestione del catasto dal GIS.");
            return;
        }

        let content = "<div style='padding: 10px 50px;'>";
        content += "<input type='checkbox' id='conferma_elim' class='k-checkbox'>";
        content += "<label class='k-checkbox-label' for='conferma_elim'>" + Traduzione(AgronicaControlliGisResx, "jsLblEliminaEntitaGrafiche") + "</label>";
        content += "</div>";

        let win_el = document.createElement("div");
        win_el.id = "id_tmp_kendo_dlg";
        document.body.appendChild(win_el);
        let $win_el = $("#id_tmp_kendo_dlg");

        var msgEliminaimp1 = Traduzione(AgronicaControlliGisResx, "jsLblEliminaImpianto");

        var msgOk = Traduzione(AgronicaControlliGisResx, "jsLblok");
        var msgAnnulla = Traduzione(AgronicaControlliGisResx, "jsLblAnnulla");

        $win_el.kendoDialog({
            title: msgEliminaimp1,
            closable: false,
            modal: true,
            visible: false,
            content: content,
            actions: [
                {
                    text: msgOk,
                    action: function () {

                        if ($("#conferma_elim").prop("checked") === false) {
                            //Non chiudo la dialog...
                            return false;
                        }

                        let entita = [];
                        $.each(shape.selectedShapeArray, function (i, e) {
                            let av = e.html.split("|");
                            entita.push({
                                pivasuperuser: av[0],
                                entita_cod: av[1]
                            });
                        });

                        interfaccia.loading(true);

                        $.ajax({
                            type: "POST",
                            url: indirizzohttp + "/CancellaEntitaGrafiche",
                            data: JSON.stringify({ entita: JSON.stringify(entita) }),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (msg) {

                                interfaccia.loading(false);

                                if (msg.d.Esito.toLowerCase() === "false") {

                                    kendo.alert(msg.d.StringaRisposta);

                                } else {

                                    let msgEliminaimp2 = Traduzione(AgronicaControlliGisResx, "jsLblEliminazioneImpianto");
                                    kendoDlgMessage(msgEliminaimp2, msg.d.StringaRisposta,
                                        function () {

                                            $.each(shape.selectedShapeArray, function (i, e) {
                                                e.setMap(null);
                                            });

                                            //non cancello entità anagrafica quindi non necessario... 
                                            //AlberoAnagrafica2017lettura();

                                            AggiornaLayer(false);
                                        }
                                    );
                                }
                            },
                            error: function (xhr, ajaxOptions, thrownError) {

                                interfaccia.loading(false);

                                alert(xhr.status);
                                alert(thrownError);
                            }
                        });
                    }
                },
                { text: msgAnnulla }
            ],
            close: function (e) {
                this.destroy();
            }
        });

        $win_el.data("kendoDialog").open();

        return;
    }

    if ($("#hiddenMultipointModifica")) {
        ApriKendoDialog("#dialogEliminaMultipoint");
        return;
    }
}

function dialogAB() {
    ApriKendoDialog("#dialogAB");
}

function dialogAnalisiMeteo() {

    let CoordOk = CoordDaSelezione();

    if (!CoordOk) {
        kendoDlgMessage("", "Nessun elemento grafico selezionato.");
        return false;
    }

    let ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();
    let lat = $("#find_lat").data("kendoCoordMaskedTextBox").decimalValue();
    let lng = $("#find_lng").data("kendoCoordMaskedTextBox").decimalValue();

    ajaxAgronicaSync(indirizzohttp + "/AnalisiMeteoBs",
        "{ lat: '" + lat + "', lng: '" + lng + "', ChiaveAlbero: '" + ChiaveAlbero + "' }",
        true,
        function (risposta) {

            //Lavez - 24/05/2022 - cerco di rendere parametrizzabile la chiamata in modale
            var resp = risposta.RispostaStringa.split('&ViewModal=');
            var viewmodal = false;
            if (resp.length > 1) {
                if (resp[1] == "True") {
                    viewmodal = true;
                }
            }

            if (viewmodal===true){
                ModalKendoApri(risposta.RispostaStringa + dialogAnalisiDecidiSeFiltro(), "Analisi Meteo", null, true, "90%", "90%");
            } else {
                window.location = risposta.RispostaStringa + dialogAnalisiDecidiSeFiltro()
            }

            //ModalBootstrapApri(risposta.RispostaStringa + dialogAnalisiDecidiSeFiltro(), "Analisi Meteo e DSS");
            
            //

        }, null);

}

function dialogAnalisiDatiReteAcqua() {

    let CoordOk = CoordDaSelezione();

    if (!CoordOk) {
        kendoDlgMessage("", "Nessun elemento grafico selezionato.");
        return false;
    }

    let ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();
    let lat = $("#find_lat").data("kendoCoordMaskedTextBox").decimalValue();
    let lng = $("#find_lng").data("kendoCoordMaskedTextBox").decimalValue();

    ajaxAgronicaSync(indirizzohttp + "/AnalisiDatiReteAcqua",
        "{ lat: '" + lat + "', lng: '" + lng + "', ChiaveAlbero: '" + ChiaveAlbero + "' }",
        true,
        function (risposta) {

            //Lavez - 24/05/2022 - cerco di rendere parametrizzabile la chiamata in modale
            var resp = risposta.RispostaStringa.split('&ViewModal=');
            var viewmodal = false;
            if (resp.length > 1) {
                if (resp[1] == "True") {
                    viewmodal = true;
                }
            }

            if (viewmodal === true) {
                ModalKendoApri(risposta.RispostaStringa + dialogAnalisiDecidiSeFiltro(), "Analisi Dati Rete Acqua", null, true, "90%", "90%");
            } else {
                window.location = risposta.RispostaStringa + dialogAnalisiDecidiSeFiltro()
            }

            //ModalBootstrapApri(risposta.RispostaStringa + dialogAnalisiDecidiSeFiltro(), "Analisi Meteo e DSS");

            //

        }, null);

}

function dialogAnalisiModelli() {

    let CoordOk = CoordDaSelezione();

    if (!CoordOk) {
        let msgNoSel = Traduzione(AgronicaControlliGisResx, "jslblNessunElementoGraficoSelezionato");
        kendoDlgMessage("", msgNoSel);
        return false;
    }

    let ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();
    let lat = $("#find_lat").data("kendoCoordMaskedTextBox").decimalValue();
    let lng = $("#find_lng").data("kendoCoordMaskedTextBox").decimalValue();

    ajaxAgronicaSync(indirizzohttp + "/AnalisiModelliBs",
        "{ lat: '" + lat + "', lng: '" + lng + "', ChiaveAlbero: '" + ChiaveAlbero + "' }",
        true,
        function (risposta) {

            //Lavez - 24/05/2022 - cerco di rendere parametrizzabile la chiamata in modale
            var resp = risposta.RispostaStringa.split('&ViewModal=');
            var viewmodal = false;
            if (resp.length > 1) {
                if (resp[1] == "True") {
                    viewmodal = true;
                }
            }

            if (viewmodal === true) {
                ModalKendoApri(risposta.RispostaStringa + dialogAnalisiDecidiSeFiltro(), "Modelli DSS", null, true, "90%", "90%");
            } else {
                window.location = risposta.RispostaStringa + dialogAnalisiDecidiSeFiltro()
            }

            //ModalBootstrapApri(risposta.RispostaStringa + dialogAnalisiDecidiSeFiltro(), "Analisi Meteo e DSS");
            

        }, null);
}

function dialogAnalisiRilievi() {

    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();

    ajaxAgronicaSync(indirizzohttp + "/AnalisiRilieviBs",
        "{ lat: '', lng: '', ChiaveAlbero: '" + ChiaveAlbero + "' }",
        true,
        function (risposta) {

            //ModalBootstrapApri(risposta.RispostaStringa + dialogAnalisiDecidiSeFiltro(), "Analisi schede rilievi");
            window.location = risposta.RispostaStringa + dialogAnalisiDecidiSeFiltro()

        }, null);

}

function dialogAnalisiDecidiSeFiltro() {
    var u = "";

    var cfgAlbero = JSON.parse($("#" + hdAlberoAnagrafica2017cfg_ClientID).val());

    if (cfgAlbero.FiltroImpiantiIdTestataTemp != 0) {
        u = "&f=true";
    }

    return u;
}

function dialogApriAnalisi() {

    var xEntitaCod = $("#hiddenMultipointModifica").val().toString();

    if (xEntitaCod === "") {
        //i18n__
        kendoDlgMessage("", "Occorre selezionare un elemento grafico di tipo analisi.");
        return false;
    }


    var vEntita_Cod = xEntitaCod.split("§");
    var Entita_Cod = vEntita_Cod[vEntita_Cod.length - 1].split("|")[1].split("^")[0];

    ajaxAgronica(indirizzohttp + "/ApriSitoAnalisixVisualizzazione",
        "{ Entita_Cod: '" + Entita_Cod + "' }",
        function (risposta) {

            if (risposta.RispostaStringa !== "false") {
                KendoWindowGenericApri(risposta.RispostaStringa, "Analisi", undefined, "517px", "90%", "60%", "0px");
                //ModalBootstrapApri(risposta.RispostaStringa, "Dati di Analisi",undefined, true, "50%", undefined, "500px", undefined);
            } else {
                //i18n__
                alert("Occorre selezionare un elemento grafico di tipo analisi.");
                return false;
            }


        }, null);

}

function dialogBufferZoneIntersection() {


    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();

    //se sono tutte valorizzate allora posso procedere
    if (ChiaveAlbero == "") {
        kendoDlgMessage("", "Nessun elemento selezionato.");
        interfaccia.loading(false);
        return false;
    }

    if (ChiaveAlbero != '' && CoordFromPoints == '' && shape.selectedShape == undefined) {
        //i18n__
        kendoDlgMessage("", "Nessun disegno per l'elemento selezionato.");
        return false;
    }


    //verificare se selezionato appezzamento.
    $("#tool_bar").hide();
    ApriKendoDialog("#dialogBufferZoneIntersection");
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
                //i18n__
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
    ApriKendoDialog("#dialogMultipoint");
}

function dialogRateo() {
    ApriKendoDialog("#dialogRateo");
}

function dialogGeneraPlanning() {
    $("#dialogGeneraPlanning").dialog("open");
}

function ClearSelect(obj) {

    //todo
    return;

    $('.toolSelezionato').removeClass('toolSelezionato');
    obj.addClass('toolSelezionato');
    obj.effect("bounce", { direction: 'down', times: 5 }, 300);
}

function BlinkFast(obj) {
    //todo
    return;
    $('.toolSelezionato').removeClass('toolSelezionato');
    obj.addClass('toolSelezionato toolSelezionatoEvidenziato');
    //    obj.effect("bounce", { direction: 'down', times: 30, distance: 50, mode: 'effect' }, 300);
    obj.effect("bounce", { direction: 'down', times: 5 }, 300);
}

function predisponiLayoutDialogImpianto() {

    if (GisPurpose === Enum_GisPurpose.SementiSportello) {

        $("#div_pop_up_campo").hide();
        $("#div_pop_up_impianto_azienda").attr("class", "col-lg-6 col-md-6 col-sm-6 col-xs-12");
        $("#div_pop_up_centro").attr("class", "col-lg-6 col-md-6 col-sm-6 col-xs-12");
        $("#lblpop_up_centro").html("Campo");
        $("#lblpop_up_data_inizio").html("Data Inserimento");
        $("#lblpop_up_data_fine").html("Data Raccolto");
        $("#lblpop_up_m_data_inizio").html("Data Inserimento");
        $("#lblpop_up_m_data_fine").html("Data Raccolto");

        //$("#pop_up_data_inizio").attr("disabled", "disabled");
        //$("#pop_up_m_data_inizio").attr("disabled", "disabled");
        //$("#pop_up_data_fine").attr("disabled", "disabled");
        //$("#pop_up_m_data_fine").attr("disabled", "disabled");

        //$("#...").data("kendoDatePicker").enable(false);

        $("#pop_up_data_inizio").data("kendoDatePicker").readonly(true);
        $("#pop_up_m_data_inizio").data("kendoDatePicker").readonly(true);
        $("#pop_up_data_fine").data("kendoDatePicker").readonly(true);
        $("#pop_up_m_data_fine").data("kendoDatePicker").readonly(true);
    }
    else {

        $("#righelloCampoVicino").hide();
    }
}

function pop_up_sup_app_modifica_chkModificaDatiDiAnagrafica_click() {
    if ($("#pop_up_sup_app_modifica_chkModificaDatiDiAnagrafica").is(":checked")) {
        mostraNascondiPanelBar("#panelBarDatiModifica_Specie", true);
        mostraNascondiPanelBar("#panelBarDatiModifica_Altro", true);
    } else {
        mostraNascondiPanelBar("#panelBarDatiModifica_Specie", false);
        mostraNascondiPanelBar("#panelBarDatiModifica_Altro", false);
    }
}

function predisponiLayoutDialogImpiantoCkAnagrafica() {
    var a = $("#pop_up_sup_app_modifica_chkModificaDatiDiAnagrafica");
    if (a) {
        if (shape.selectedShape.StandardEntita_layerDiAppartenenza == "19") {
            $("#pop_up_sup_app_modifica_divModificaDatiDiAnagrafica").show();
        } else {
            $("#pop_up_sup_app_modifica_divModificaDatiDiAnagrafica").hide();
        }
    }
}

function cmbImpiantiFigli_Modifica_Change() {

    var selezione = $("#cmbImpiantiFigli_Modifica").val();

    if (selezione !== "-1") {
        //seleziona un impianto
        $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val(selezione);

        //ne carico i dati...
        mappa.getProprieta("", selezione, 1, true);

    } else {
        //ripristino valore iniziale
        $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val($("#HiddenSelezioneAlberoAnagraficaAlberoAnagraficaModificaImpianto").val());
    }

}

function kendoModificaImpianto_Open() {

    var pa = $("#panelBarDatiModifica").data("kendoPanelBar");
    if (pa === undefined) {
        $("#panelBarDatiModifica").kendoPanelBar({
            expandMode: "multiple"
        }).data("kendoPanelBar");
    }

    let optValueNessuno = Traduzione(AgronicaControlliGisResx, "jsLblNessuno");
    let optValueParticella = Traduzione(AgronicaControlliGisResx, "jsLblParticellaCatastale");
    let optValueImpianto = Traduzione(AgronicaControlliGisResx, "jsLblImpianto");
    let optValueImpiantoAppezzamento = Traduzione(AgronicaControlliGisResx, "jsLblImpiantoAppezzamento");

    if (shape.selectedShape.StandardEntita_layerDiAppartenenza == "3") {

        $("#opt_associa_modifica").html('<option value="0">' + optValueNessuno + '</option><option value="5" selected="selected">' + optValueParticella + '</option>')
        $("#divDatiAppezza").hide();
        $("#lbl_smallDescrAzienda_modifica").hide();
        $("#lbl_smallDescrCentro_modifica").hide();
        $("#smallDescrAzienda_modifica").hide();
        $("#smallDescrCentro_modifica").hide();
        mappa.getProprieta("", shape.selectedShape.Entita_Cod, 2, false, "#smallDescrAlbero_modifica");
    } else {
        $("#opt_associa_modifica").html('<select class="form-control" id="opt_associa_modifica"><option value="1">' + optValueNessuno + '</option><option value="2">' + optValueImpianto + '</option><option value="3">' + optValueImpiantoAppezzamento + '</option></select>')
        $("#divDatiAppezza").show();
        $("#lbl_smallDescrAzienda_modifica").show();
        $("#lbl_smallDescrCentro_modifica").show();
        $("#smallDescrAzienda_modifica").show();
        $("#smallDescrCentro_modifica").show();
        $("#smallDescrAzienda_modifica").html($("#ddl_azienda_html").data("kendoDropDownList").text());
        $("#smallDescrCentro_modifica").html($("#ddl_Sa_Cod_html").data("kendoDropDownList").text());
    }
}

function preparaXSalvataggioPoligoniImpostaTitoloWin(titolo) {
    $($("#stileAnagraficaRiassuntoModifica").parent().parent().parent().parent().parent().parent()).find("span.k-window-title.k-dialog-title").text(titolo);
}

/* Salvataggio dello Shape */
function preparaXSalvataggioPoligoni(SavePlus) {

    if (shape.glayerDoveDisegnoSuTipologiaStandard === "3" && !PermessiGisServerSide.bool_catasto) {
        //i18n__
        kendo.alert("Non si dispone dei permessi di gestione del catasto dal GIS.");
        return false;
    }

    $("#disenga_poligonoHeader").removeClass("green");
    $("#multipointHeader").removeClass("green");
    $("#selimg_multipoint").removeClass("green");

    let anagRiassunto = "";
    /*GABRIELE
        var selAlbero = $("#GIS_multiselect").data("kendoMultiSelect").dataSource.data();
    
        for (var i = 0; i < selAlbero.length; i++) {
            anagRiassunto = anagRiassunto + selAlbero[i].text + ", ";
        }
    */
    $("#smallDescrAlbero_modifica").html(anagRiassunto);

    $("#HiddenSelezioneAlberoAnagraficaAlberoAnagraficaModificaImpianto").val($("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val());

    $("#divImpiantiFigli_Modifica").hide();

    /*GABRIELE
        if (shape.selectedShape.StandardEntita_layerDiAppartenenza === "1") {
            var impiantiFigli = $("#cmbImpiantiFigli_Modifica");
    
            impiantiFigli.html("");
    
            impiantiFigli.append($('<option>', {
                value: "-1",
                text: 'Seleziona un impianto ...'
            }));
    
    
            var selAlbero = $("#GIS_multiselect").data("kendoMultiSelect").dataSource.data();
    
            for (var i = 0; i < selAlbero.length; i++) {
    
                if (selAlbero[i].id.split(separatoreChiaveAlbero)[0] !== "5") {
    
                    impiantiFigli.append($('<option>', {
                        value: selAlbero[i].id,
                        text: selAlbero[i].text
                    }));
                }
            }
        }
    */

    if ($("#hiddenMultipointModifica").val() != "" && shape.selectedShape) {
        //i18n__
        kendoDlgMessage("Modifica", "Modifica di elementi multipli non consentita.");
        return false;
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
                        let msgOk = Traduzione(AgronicaControlliGisResx, "jsMsgSalvataggioAvvenutoConSuccesso");
                        kendoDlgMessage(msgOk);
                    }
                    else {
                        kendoDlgMessage(msg.d);
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
        return true;
    }

    if (shape.selectedShape === null) {

        let lblModi = Traduzione(AgronicaControlliGisResx, "jsLblModifica");
        let lblModTi = Traduzione(AgronicaControlliGisResx, "jsMsgNessunElementoSelezionato");

        kendoDlgMessage(lblModi, lblModTi);
        return false;
    }

    //i punti Salvati
    var MVCArray = shape.selectedShape.getPath();

    var test = MVCArray.getArray().toString();
    utility.log("Prepara per salvataggio poligoni: ");

    //controllo se è un nuovo shape o la modifica di uno vecchio
    let nuovoPoligono = (shape.selectedShape.html == undefined);

    let rAA = GisAjaxSync("VerificaPoligono", { Poligono: test });
    if (rAA !== "Ok") {

        let lblDisegna = Traduzione(AgronicaControlliGisResx, "jsLblDisegnaPoligono");
        kendoDlgMessage(lblDisegna, rAA);

        if (nuovoPoligono) {
            shape.selectedShape.setMap(null);
        }

        return false;
    }

    var dlgAssConferma = Traduzione(AgronicaControlliGisResx, "jsLblConferma");
    var dlgAssAnnulla = Traduzione(AgronicaControlliGisResx, "jsLblAnnulla");

    if (nuovoPoligono) {

        $('#hiddenPunti_Nuovo').val(test);

        var area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);

        //aggiungo l'infobox
        $('#pop_up_sup_app').val(area);
        $('#pop_up_sup_google').val(area);

        predisponiLayoutDialogImpianto();

        var salvataggio_diretto = false;
        let chiave_albero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val();
        if (chiave_albero != '') {
            let arr = chiave_albero.split(separatoreChiaveAlbero);
            salvataggio_diretto = (arr[0] !== "51");
        } else {
            //se ho il valore
        }

        utility.log("salvataggio_diretto:" + salvataggio_diretto);

        if (salvataggio_diretto == true) {
            $('#lblDialogSup').html(area);

            //GABRIELE 02 04 2019
            //ApriKendoDialog("#dialogConfermaAssociazione");

            let kendoDlg = $("#dialogConfermaAssociazione").data("kendoDialog");
            if (kendoDlg === undefined) {


                var dlgAssTitolo = Traduzione(AgronicaControlliGisResx, "jsLblAssociazione");


                $("#dialogConfermaAssociazione").kendoDialog({
                    title: dlgAssTitolo,
                    closable: false,
                    modal: true,
                    visible: false,
                    actions: [
                        {
                            text: dlgAssConferma,
                            action: function () {
                                return ModificaImpianto(Enum_TipoModifica.SalvataggioDiretto);
                            }
                        },
                        {
                            text: dlgAssAnnulla,
                            action: function () {
                                if (shape.selectedShape != undefined) {
                                    shape.selectedShape.setMap(null);
                                }
                                return true;
                            }
                        }
                    ]
                });

                kendoDlg = $("#dialogConfermaAssociazione").data("kendoDialog");
                $("#dialogConfermaAssociazione").removeClass("load-hidden");
            }

            kendoDlg.open();

        } else {

            // GABRIELE 08 04 2019
            if (shape.glayerDoveDisegnoSuTipologiaStandard === "33") {

                //Planning...

                //controllo trasferito a monte
                if (chiave_albero === "") {
                    //i18n__
                    kendoDlgMessage("Nuovo impianto pianificato", "Selezionare un planning dall'anagrafica...");
                    shape.selectedShape.setMap(null);
                    return false;
                }

                let arr = chiave_albero.split(separatoreChiaveAlbero);
                let prog_cod = arr[23];

                let area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);
                let vertici = MVCArray.getArray().toString()
                //$('#hiddenPunti_modifica').val(vertici);

                let url = GisAjaxSync("NuovoPlanningDaGis", { Prog_Cod: prog_cod, Vertici: vertici, Area: area });
                GestisciEditPlanning("Nuovo planning", url);

            } else {

                let kendoDlg = $("#pop_up_impianto").data("kendoDialog");
                if (kendoDlg === undefined) {

                    var popUpImpiantoDlgTitolo = Traduzione(AgronicaControlliGisResx, "jsLblNuovoImpiantoColturale");

                    let h90 = $(window).height() * 0.9;
                    $("#pop_up_impianto").kendoDialog({
                        width: "90%",
                        height: h90 + "px",
                        title: popUpImpiantoDlgTitolo,
                        closable: true,
                        modal: true,
                        visible: false,
                        open: function () {

                            let pa = $("#panelBarDatiAzienda").data("kendoPanelBar");
                            if (pa === undefined) {
                                $("#panelBarDatiAzienda").kendoPanelBar({
                                    expandMode: "multiple"
                                });
                            }
                        },
                        close: function () {
                            if (shape.selectedShape != undefined)
                                shape.selectedShape.setMap(null);
                        },
                        actions: [
                            {
                                text: dlgAssConferma,
                                action: function () {

                                    return InviaDatiNuovoImpianto();
                                }
                            }
                        ]
                    });

                    kendoDlg = $("#pop_up_impianto").data("kendoDialog");

                    $("#pop_up_impianto").removeClass("load-hidden");


                    if (GisPurpose === Enum_GisPurpose.Completo) {
                        $("#btnVerificaVicini").css("display", "none");
                    } else {
                        $("#btnVerificaVicini").click(function () {
                            //verificaInterferenze("MostraCampoPiuVicino", "Verifica vicini");
                            verificaVicini();
                        });
                    }
                }

                kendoDlg.open();

                ImpostaVisibilita_pop_up_impianto();

                interfaccia.initDatePiker();

                interfaccia.loading(true);

                CaricaSpecie('#pop_up_specie', '');

                CaricaDestinazioneUso('#pop_up_destuso', '');

                CaricaAzienda();

                if (GisPurpose !== Enum_GisPurpose.SementiSportello) {
                    CaricaDateDefault('#pop_up_data_inizio', '#pop_up_data_fine');
                }

                ImpostaPulsantiNuovoImpianto();
                interfaccia.loading(false);
                mappa.identificaIndirizzi(MVCArray);
                //'  Vanni, 03/12/2015 09:51:53: vavava
                inizializzaDDLProvincie('ddl_Provincia_', imprese_ID_ind);
                inizializzaDDLProvincie('ddl_Provincia_', centro_ID_ind);

                if (GisPurpose !== Enum_GisPurpose.Completo) {
                    $("#terreno_nudo").prop("disabled", "disabled");
                } else {

                    let chk = document.getElementById("terreno_nudo");
                    chk.checked = false;
                    TerrenoNudoClick(chk);
                }
            }
        }

        if (shape.selectedShape !== undefined && shape.selectedShape !== null) {
            shape.selectedShape.setEditable(false);
        }

    } else {

        //GABRIELE 02 04 2019
        if (shape.selectedShape.StandardEntita_layerDiAppartenenza == "33") {

            var area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);
            $('#hiddenPunti_modifica').val(MVCArray.getArray().toString());

            modificaDiretto(shape.selectedShape.Entita_Cod, area, false);

            shape.selectedShape.setEditable(false);

            //Planning... 
            let arr = shape.selectedShape.chiavealbero.split(separatoreChiaveAlbero);
            let prog_ent_cod = arr[24];
            let prog_cod = arr[23];

            let url = GisAjaxSync("EditPlanningDaGisArea", { Prog_Entita_Cod: prog_ent_cod, Prog_Cod: prog_cod, Area: area });
            //i18n__
            GestisciEditPlanning("Modifica planning", url);

        } else {

            //if (shape.selectedShape.StandardEntita_layerDiAppartenenza == "3") {
            //    //CATASTO

            //    var area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);
            //    $('#hiddenPunti_modifica').val(MVCArray.getArray().toString());

            //    modificaDiretto(shape.selectedShape.Entita_Cod, area, true);

            //    shape.selectedShape.setEditable(false);

            //} else {

            //utility.log("shape.selectedShape.html:" + shape.selectedShape.html);

            //i18n__
            preparaXSalvataggioPoligoniImpostaTitoloWin("Modifica APPEZZAMENTO");
            if (shape.selectedShape.StandardEntita_layerDiAppartenenza === 1 && SavePlus) {
                preparaXSalvataggioPoligoni_SavePlus = true;
            }

            predisponiLayoutDialogImpianto();

            predisponiLayoutDialogImpiantoCkAnagrafica();

            if (shape.selectedShape.StandardEntita_layerDiAppartenenza == "3") {

            }

            $('#hiddenPunti_modifica').val(MVCArray.getArray().toString());
            $('#hiddenID').val(shape.selectedShape.html);

            var area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);

            $('#pop_up_sup_google_modifica').val(area);

            let kendoDlg = $("#pop_up_modificaImpianto").data("kendoDialog");

            if (kendoDlg === undefined) {

                let h90 = $(window).height() * 0.9;

                var pop_up_modificaImpianto_titolo = Traduzione(AgronicaControlliGisResx, "jsLblModificaImpianto");


                $("#pop_up_modificaImpianto").kendoDialog({
                    width: "90%",
                    height: h90 + "px",
                    title: pop_up_modificaImpianto_titolo,
                    closable: true,
                    modal: true,
                    visible: false,
                    open: function () {
                        kendoModificaImpianto_Open();
                    },
                    close: function () {
                        //if (shape.selectedShape != undefined)
                        //    shape.selectedShape.setMap(null);
                    },
                    actions: [
                        {
                            text: "Conferma",
                            action: function () {
                                return ModificaImpianto(Enum_TipoModifica.ModificaImpianto);
                            }
                        }
                    ]
                });

                kendoDlg = $("#pop_up_modificaImpianto").data("kendoDialog");

                $("#pop_up_modificaImpianto").removeClass("load-hidden");

                if (GisPurpose === Enum_GisPurpose.Completo) {
                    $("#btnVerificaVicini_2").css("display", "none");
                } else {
                    $("#btnVerificaVicini_2").click(function () {
                        //verificaInterferenze("MostraCampoPiuVicino", "Verifica vicini");
                        verificaVicini_2();
                    });
                }

                if (GisPurpose !== Enum_GisPurpose.Completo) {
                    $("#terreno_nudo_modifica").prop("disabled", "disabled");
                }

            }

            kendoDlg.open();

            //$('#pop_up_sup_app_modifica').val(area);

            ImpostaVisibilita_pop_up_modificaImpianto();

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
                var ChiaveAlbero = shape.selectedShape.chiavealbero;

                //vanni, 04/10/2019, funzione con verifiche..:
                //CaricaSpecie("#pop_up_specie_modifica", Veg_Cod);
                if (shape.selectedShape.StandardEntita_layerDiAppartenenza == "19") {

                    CaricaDestinazioneUso("#pop_up_destuso_modifica", "")
                    if (Veg_Cod == "-1") {

                        CaricaSpecie("#pop_up_specie_modifica", "");

                        let chk = document.getElementById("terreno_nudo_modifica");
                        chk.checked = true;
                        TerrenoNudoClick(chk);

                        //GABRIELE CODICE ANAGRAFE DESTINAZIONE USO CHI LO LEGGE???
                        //$("#pop_up_destuso_modifica").val(3262);
                    } else {

                        CaricaSpecieConInfoAgenda($("#pop_up_specie_modifica"), $("#pop_up_varieta_modifica"), $("#pop_up_finalita_modifica"), Veg_Cod, ChiaveAlbero);
                    }
                } else {

                    CaricaSpecie("#pop_up_specie_modifica", Veg_Cod);
                    CaricaDestinazioneUso("#pop_up_destuso_modifica", "")
                }

                var Entita_Cod = shape.selectedShape.Entita_Cod;
                mappa.getProprieta(Veg_Cod, Entita_Cod, 1);
                if (GisPurpose !== Enum_GisPurpose.SementiSportello) {
                    CaricaDateDefault('#pop_up_m_data_inizio', '#pop_up_m_data_fine');
                }
                shape.selectedShape.setEditable(false);
                interfaccia.loading(false);
            }, 100);

            //} 
            //else particelle
        }
    }
}

function GestisciEditPlanning(titolo, url) {

    if (url === "") {

        if (shape.selectedShape != undefined && shape.selectedShape != null) {
            shape.selectedShape.setMap(null)
            shape.selectedShape = null;
        }
        return;
    }

    let winElem_id = "Planning_KendoWindow";
    let winElem = document.createElement("div");
    winElem.style.cssText = "padding:0px;";
    winElem.id = winElem_id;
    document.body.appendChild(winElem);
    let $win_el = $("#" + winElem_id);

    let styleElem = document.createElement('style');
    styleElem.type = "text/css";
    styleElem.innerHTML = ".body_overflow_hidden { overflow: hidden; }";
    winElem.appendChild(styleElem);

    let frameH = window.innerHeight * 0.9;

    let frameElem = document.createElement("iframe");
    frameElem.id = "GST_IFrame";
    frameElem.style.cssText = "width:-webkit-fill-available; height:" + frameH + "px; opacity:0;";
    frameElem.setAttribute('frameborder', '0');
    //frameElem.setAttribute('src', risp);
    winElem.appendChild(frameElem);

    $win_el.kendoWindow({
        title: titolo,
        width: "90%",
        draggable: false,
        visible: false,
        modal: true,
        resizable: false,
        actions: [
            "Close"
        ],
        open: function (e) {
            e.sender.element.css("opacity", "0");
            //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
            $("body").addClass("body_overflow_hidden");
        },
        activate: function (e) {
            e.sender.element.css("opacity", "1");
        },
        close: function (e) {
            $("body").removeClass("body_overflow_hidden");
            this.destroy();
        }//,
        //height: frameH + "px",
        //iframe: true,
        //content: risp
    });

    //let parent = $win_el.parent();
    //parent.find('.k-window-title').css('text-align', 'center');
    //parent.css('padding-top', '48px');
    //let titlebar = parent.find('.k-window-titlebar');
    //titlebar.css({
    //    "margin-top": "-48px",
    //    "font-size": "large"
    //});

    frameElem.setAttribute('src', url);

    $win_el.data("kendoWindow").center().open();

    WaitFrame.show();
}

//GABRIELE 04 04 2019
function OpacityReset() {
    $("#GST_IFrame").css("opacity", "1");
    WaitFrame.hide();
}

//GABRIELE 04 04 2019
function PianoColturaleRientraIframeGIS(oConferma) {

    /*
     * let oConferma = {
     * SalvataggioConfermato: false,
     * Descrizione: "",
     * Programmazione_Entita_Cod: "",
     * TipoOperazione_DB: 2
     * };
     */

    let kWin = $("#Planning_KendoWindow").data("kendoWindow");
    if (kWin !== undefined) {
        kWin.close();
    }

    if (oConferma.TipoOperazione_DB === 1) {
        //Nuovo elemento
        if (shape.selectedShape != undefined && shape.selectedShape != null) {
            shape.selectedShape.setMap(null)
            shape.selectedShape = null;
        }

        if (oConferma.SalvataggioConfermato) {

            AggiornaTutto(false);

            //Devo cercare il poligono con Programmazione_Cod e Programmazione_Entita_Cod...

            //ricarico albero???
            let AnagTree = AlberoAnagrafica();
            let hds = AnagTree.dataSource;
            if (hds.data().length > 0) {
                //Rileggo solo se già caricato...
                hds.read();
                //scroll dell'albero fino al nodo che rappresenta il poligono trovato in precedenza...
            }
        }
        return;
    }

    // Modifica elemento
    if (!oConferma.SalvataggioConfermato) {
        return;
    }

    //Confermata modifica 
    if (typeof shape.selectedShape.etichetta === "object") {
        if (shape.selectedShape.etichetta !== null) {

            let pos = polylabel(shape.selectedShape.getPath().getArray());
            shape.selectedShape.etichetta.setPosition(pos);
            let label = shape.selectedShape.etichetta.getLabel();
            label.text = oConferma.Descrizione;
            shape.selectedShape.etichetta.setLabel(label);

        }
    }

    let chiaveAlbero = shape.selectedShape.chiavealbero;
    let AnagTree = AlberoAnagrafica();
    AnagTree.changeLabel(chiaveAlbero, oConferma.DescrizioneAlbero);

}

function Abilita_Disabilita_Select_GIS(jQuerySelect, abilita) {

    //nonostante il disabled, l'evento click si attiva lo stesso.
    //per questo motivo uso il pointer-events (http://stackoverflow.com/questions/6657545/setting-attribute-disabled-on-a-span-element-does-not-prevent-click-events)

    //if (!abilita) {        
    //    $(jQuerySelect).css('pointer-events', 'none');
    //} else {        
    //    $(jQuerySelect).css('pointer-events', '');

    //}

    if (abilita) {
        $(jQuerySelect).removeAttr("disabled");
    } else {
        $(jQuerySelect).attr("disabled", true);
    }

}

function cmbAssocia_Modifica_NuovoImpiantoClearFormSingle(jQuerySelector) {
    $($(jQuerySelector)).each(function () {
        $(this).removeAttr("disabled");
        $(this).val("");
    });
}

function cmbAssocia_Modifica_NuovoImpiantoClearForm() {
    cmbAssocia_Modifica_NuovoImpiantoClearFormSingle("#panelBarDatiModifica_Specie select");
    cmbAssocia_Modifica_NuovoImpiantoClearFormSingle("#panelBarDatiModifica_Specie input");
    cmbAssocia_Modifica_NuovoImpiantoClearFormSingle("#panelBarDatiModifica_Altro select");
    cmbAssocia_Modifica_NuovoImpiantoClearFormSingle("#panelBarDatiModifica_Altro input");


}

function cmbAssocia_Modifica_GenerazionePoligoni_Date() {

    var TxtValiditaInizioAppezzamentoGisModifica = $("#pop_up_m_data_inizio").data("kendoDatePicker");
    var TxtValiditaFineAppezzamentoGisModifica = $("#pop_up_m_data_fine").data("kendoDatePicker");

    $(".divAppezzaDate").removeClass("divAppezzaDateNascondi");

    mappa.getProprieta("", shape.selectedShape.Entita_Cod, 3, false, "#smallDescrAlbero_modifica");

    TxtValiditaInizioAppezzamentoGisModifica.unbind("change");
    TxtValiditaInizioAppezzamentoGisModifica.bind("change", function () {
        TxtValiditaInizioAppezzamentoGisModificaTest(true, this.value());
    });


    TxtValiditaFineAppezzamentoGisModifica.unbind("change");
    TxtValiditaFineAppezzamentoGisModifica.bind("change", function () {
        TxtValiditaInizioAppezzamentoGisModificaTest(false, this.value());
    });

}

/**
 * True se la data (iniziale o finale) è corretta
 * @param {any} Data1
 * @param {any} Data2
 * @param {any} TestDataIniziale
 */
function TxtValiditaInizioAppezzamentoGisTestData(Data1, Data2, TestDataIniziale) {
    if (TestDataIniziale) {
        return (Data1 >= Data2);
    } else {
        return (Data1 <= Data2);
    }
}

function TxtValiditaInizioAppezzamentoGisModificaErroreDate() {
    return ($("#i_appezza_data_inizio").is(":visible") || $("#i_appezza_data_fine").is(":visible"));
}

function TxtValiditaInizioAppezzamentoGisModificaTest(DataIniziale, DataAppezzamentoTest) {

    let descrizioneInfSup = "inferiore";
    let descrizioneInizio = "inizio";
    let jQueryimpianto_data = "#pop_up_m_data_inizio";
    let jQueryLblAppezza = "#lbl_appezza_data_inizio";
    let warningJQuerySel = "#i_appezza_data_inizio";
    if (!DataIniziale) {
        jQueryimpianto_data = "#pop_up_m_data_fine";
        descrizioneInfSup = "superiore";
        descrizioneInizio = "fine";
        warningJQuerySel = "#i_appezza_data_fine";
        jQueryLblAppezza = "#lbl_appezza_data_fine";

    }

    $(warningJQuerySel).hide();

    let val_ini_impianto = kendo.parseDate(DataAppezzamentoTest);
    let val_ini_appezzamento = kendo.parseDate($(jQueryLblAppezza).text());

    if (!TxtValiditaInizioAppezzamentoGisTestData(val_ini_impianto, val_ini_appezzamento, DataIniziale)) {
        MessaggioDataNonValida(descrizioneInizio, descrizioneInfSup, val_ini_appezzamento);
        $(warningJQuerySel).show();
        $(jQueryimpianto_data).data("kendoDatePicker").value("");
    }

}

function MessaggioDataNonValida(descrizioneInizio, descrizioneInfSup, val_ini_appezzamento) {
    //i18n__
    kendo.alert("La data di " + descrizioneInizio + " dell'impianto non può essere " + descrizioneInfSup + " alla data di " + descrizioneInizio + " dell'appezzamento:" + val_ini_appezzamento.toLocaleDateString());
}

/**
 * La gestione attraverso classi è necessaria perchè la combo si può comportare in maniera diversa in base al layer selezionato (vedi "default" su switch)
 * */
function cmbAssocia_Modifica_GenerazionePoligoni_Change() {

    let valSelezionato = $("#cmbAssocia_Modifica_GenerazionePoligoni").val();
    let optCls = $("#cmbAssocia_Modifica_GenerazionePoligoni option[value=" + valSelezionato + "]").attr('class');
    let optListaClassi = "";

    if (optCls !== undefined) {
        optListaClassi = optCls.split(/\s+/);
    }

    for (var i = 0; i < optListaClassi.length; i++) {
        let a = optListaClassi[i];
        switch (a) {
            case "ModificaAppezzamento":
                cmbAssocia_Modifica_GenerazionePoligoni_MostraImpiantiForm(false);
                break;
            case "NuovoImpiantoModificaAppezzamento":
                cmbAssocia_Modifica_GenerazionePoligoni_MostraImpiantiForm(true);
                cmbAssocia_Modifica_NuovoImpiantoClearForm();
                cmbAssocia_Modifica_GenerazionePoligoni_Date();
                break;
            case "NuovoImpianto":
                cmbAssocia_Modifica_GenerazionePoligoni_MostraImpiantiForm(true);
                cmbAssocia_Modifica_NuovoImpiantoClearForm();
                cmbAssocia_Modifica_GenerazionePoligoni_Date();
                break;
            default:
                break;
            //mi trovo sulla modifica di un impianto e non di un appezza.
        }
    }

}

function cmbAssocia_Modifica_GenerazionePoligoni_MostraImpiantiForm(MostraOrNascondi) {

    if (MostraOrNascondi) {
        mostraNascondiPanelBar("#panelBarDatiModifica_Specie", true);
        mostraNascondiPanelBar("#panelBarDatiModifica_Altro", true);
    } else {
        mostraNascondiPanelBar("#panelBarDatiModifica_Specie", false);
        mostraNascondiPanelBar("#panelBarDatiModifica_Altro", false);
    }

}

function riempiSelectGenerazioneLayer(id, opts) {

    let selem = document.getElementById(id);
    while (selem.options.length > 0) {
        selem.remove(0);
    }

    let impostaValore = false;
    for (o = 0; o < opts.length; o++) {
        let optToAdd = new Option(opts[o].text, opts[o].val);
        $(optToAdd).attr("id", id + "_" + (o + 1).toString());
        selem.options.add(optToAdd);
        if (opts[o].val === "3") {
            impostaValore = true;
        }
    }

    if (impostaValore) {
        $("#" + id).val("3");
    }


}

function ImpostaVisibilita_pop_up_modificaImpianto() {
    utility.log("ImpostaVisibilita_pop_up_modificaImpianto, glayerDoveDisegnoSuTipologiaStandard = " + shape.glayerDoveDisegnoSuTipologiaStandard);

    //'  Vanni, 28/09/2015 10:03:43: personalizzazione KWS, un po' Hardcoded ma appena c'è tempo configuriamo la cosa.
    settaVisibilitaKWS("#kws_m_appezza", "#div_m_lotto", "#pop_up_impiantoxDescr");
    $("#pop_up_modificaImpianto").data("kendoDialog").title("Modifica " + $("#navBarLayerAttivoDescrizione").html());

    if (shape.glayerDoveDisegnoSuTipologiaStandard != "19" && shape.glayerDoveDisegnoSuTipologiaStandard != "1") {
        $("#pop_up_modificaImpianto_DatiImp").hide();
        $("#Dati_data_inizio_fine_modifica").hide();

    } else {
        $("#pop_up_modificaImpianto_DatiImp").show();
        $("#Dati_data_inizio_fine_modifica").show();
    }


    //GABRIELE 16 04 2019
    let arr = [];
    if (GisPurpose === Enum_GisPurpose.SementiSportello || GisPurpose === Enum_GisPurpose.SementiMappaturaLibera) {
        arr.push({ text: "Impianto", val: "2" });
    } else {
        arr.push({ text: "Appezzamento", val: "1" });
        arr.push({ text: "Impianto", val: "2" });
        arr.push({ text: "Impianto e Appezzamento", val: "3" });
    }
    riempiSelectGenerazioneLayer("cmbAssocia_Modifica_GenerazionePoligoni", arr);


    if (shape.glayerDoveDisegnoSuTipologiaStandard !== "1" && shape.glayerDoveDisegnoSuTipologiaStandard !== "19") {
        if (shape.glayerDoveDisegnoSuTipologiaStandard === "3") {
            $("#pop_up_sup_app_modifica_divAssocia").show();
        }
        mostraNascondiPanelBar("#panelBarDatiModifica_Specie", false);
        mostraNascondiPanelBar("#panelBarDatiModifica_Altro", false);
    } else {

        if (shape.glayerDoveDisegnoSuTipologiaStandard === "1") {
            $("#cmbAssocia_Modifica_GenerazionePoligoni").val("1");
            $("#pop_up_sup_app_modifica_divAssocia").hide();

            //GABRIELE 2019 07 30
            mostraNascondiPanelBar("#panelBarDatiModifica_Specie", false);
            mostraNascondiPanelBar("#panelBarDatiModifica_Altro", false);
        } else {

            if (shape.glayerDoveDisegnoSuTipologiaStandard === "19") {
                $("#pop_up_sup_app_modifica_divAssocia").show();
            }
            //saranno mostrati su richiesta...:
            var a = $("#pop_up_sup_app_modifica_chkModificaDatiDiAnagrafica");
            let modificaAnagrafica = true;
            if (a) {
                modificaAnagrafica = (a.is(":checked"));
            }
            mostraNascondiPanelBar("#panelBarDatiModifica_Specie", modificaAnagrafica);
            mostraNascondiPanelBar("#panelBarDatiModifica_Altro", modificaAnagrafica);
        }
    }
}

function isKws() {
    getCodiceFiscaleTecnico();

    return (Codice_Fiscale_Tecnico == "13171470159")

}

function settaVisibilitaKWS(kws, lotto, pop_up_impiantoxDescr) {

    ajaxAgronicaSync(indirizzohttp + "/getCodiceFiscaleTecnico", null, false,
        function (risposta) {

            Codice_Fiscale_Tecnico = risposta.RispostaStringa;

            //se KWS...
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

        }, null);


}

function ImpostaVisibilita_pop_up_impianto() {

    utility.log("ImpostaVisibilita_pop_up_impianto, glayerDoveDisegnoSuTipologiaStandard = " + shape.glayerDoveDisegnoSuTipologiaStandard);

    //'  Vanni, 28/09/2015 10:03:43: personalizzazione KWS, un po' Hardcoded ma appena c'è tempo configuriamo la cosa.
    settaVisibilitaKWS("#kws_appezza", "#div_lotto", "#pop_up_impiantoxDescr");

    $("#pop_up_impianto").data("kendoDialog").title("Nuovo " + $("#navBarLayerAttivoDescrizione").html());

    if (shape.glayerDoveDisegnoSuTipologiaStandard != "19") {
        $("#PopupNuovaImpresa-Button").hide();
        $("#PopupNuovoSa-Button").hide();
        $("#divpop_up_specie").hide();
        $("#placeTipologia").hide();
        $("#divOpzioni_pop_up_impianto").hide();
    } else {

        ImpostaVisibilita_pop_up_impianto_AziendaCentro();

        $("#divpop_up_specie").show();
        $("#placeTipologia").show();
        $("#divOpzioni_pop_up_impianto").show();

        mostraNascondiPanelBar("#panelBarDatiAzienda_Specie", true);
        mostraNascondiPanelBar("#panelBarDatiAzienda_Altro", true);
    }

    //GABRIELE 16 04 2019
    let arr = [];
    if (GisPurpose === Enum_GisPurpose.SementiSportello || GisPurpose === Enum_GisPurpose.SementiMappaturaLibera) {
        arr.push({ text: "Impianto", val: "2" });
    } else {
        arr.push({ text: "Appezzamento", val: "1" });
        arr.push({ text: "Impianto", val: "2" });
        arr.push({ text: "Impianto e Appezzamento", val: "3" });
    }
    riempiSelectGenerazioneLayer("cmbElementoGrafico_GenerazionePoligoni", arr);

    if (shape.glayerDoveDisegnoSuTipologiaStandard.toString() === "1") {
        $("#cmbElementoGrafico_GenerazionePoligoni").val("1");

        mostraNascondiPanelBar("#panelBarDatiAzienda_Specie", false);
        mostraNascondiPanelBar("#panelBarDatiAzienda_Altro", false);

    }

}

function ImpostaVisibilita_pop_up_impianto_AziendaCentro() {

    var urlPermessiImpresa = pathCoreWS + 'AgronicaCoreUtentiBIZ/Utenti_Permessi_R.asmx/Controlla_Permessi_Utente'

    var d = new Date();
    var paramImpresa = { Id_Servizio: 5, Id_Attivita: 170, Id_Operazione: 2, DataOraControllo: d, xFiltroAggiuntivo: "", objParametri_Utenti: objP_utenti };
    var paramCentro = { Id_Servizio: 5, Id_Attivita: 171, Id_Operazione: 2, DataOraControllo: d, xFiltroAggiuntivo: "", objParametri_Utenti: objP_utenti };

    ajaxAgronica(urlPermessiImpresa,
        JSON.stringify(paramImpresa),
        function (risposta) {

            if (risposta.RispostaStringa === "true") {
                $("#PopupNuovaImpresa-Button").show();
            } else {
                $("#PopupNuovaImpresa-Button").hide();
            }

        }, null);


    ajaxAgronica(urlPermessiImpresa,
        JSON.stringify(paramCentro),
        function (risposta) {

            if (risposta.RispostaStringa === "true") {
                $("#PopupNuovoSa-Button").show();
            } else {
                $("#PopupNuovoSa-Button").hide();
            }

        }, null);

    //$("#PopupNuovaImpresa-Button").hide();
    //$("#PopupNuovoSa-Button").hide();
}

/**
 * assegna l'area ad un oggetto json in base a cosa selezionato nella combo indicata nel parametro.
 * @param {number} area Area da assegnare
 */
function salvataggioDirettoConAppezzaAreaCalcola(area) {

    var rval = "";
    var comboJQuerySelector = "";
    var comboJQuerySelectorPropagazioneSalvataggioPoligono = "";

    var flgModificaImpianto = false;

    if (KendoDialogStatoAperto("#pop_up_modificaImpianto") || KendoDialogStatoAperto("#dialogAllertOperazioniAppezza")) {
        comboJQuerySelector = "#opt_associa_modifica";
        comboJQuerySelectorPropagazioneSalvataggioPoligono = "#cmbAssocia_Modifica_GenerazionePoligoni";
        flgModificaImpianto = true;
    }

    if (KendoDialogStatoAperto("#dialogConfermaAssociazione")) {
        comboJQuerySelector = "#option_associa";
        comboJQuerySelectorPropagazioneSalvataggioPoligono = "#cmbAssocia_GenerazionePoligoni";
    }

    if (KendoDialogStatoAperto("#dialogAllertOperazioni") && !flgModificaImpianto) {
        comboJQuerySelector = "#option_associa";
        comboJQuerySelectorPropagazioneSalvataggioPoligono = "#cmbAssocia_GenerazionePoligoni";
    }

    var PropagazioneSalvataggioPoligono = $(comboJQuerySelectorPropagazioneSalvataggioPoligono).val();

    if (shape.glayerDoveDisegnoSuTipologiaStandard.toString() !== "19" && shape.glayerDoveDisegnoSuTipologiaStandard !== "1") {
        PropagazioneSalvataggioPoligono = "0";
    }

    var supAppezzamento = area.toString().replace(",", ".");
    var supImpianto = area.toString().replace(",", ".");

    if (shape.glayerDoveDisegnoSuTipologiaStandard === "1" && (PropagazioneSalvataggioPoligono === "2" || PropagazioneSalvataggioPoligono === "3")) {
        return "{ \"AssociazioneArea\": 1, \"AreaAppezzamento\": 0, \"AreaImpianto\": " + supImpianto + ", \"PropagazioneSalvataggioPoligono\": " + PropagazioneSalvataggioPoligono + " }";
    }

    if (comboJQuerySelector == "" && comboJQuerySelectorPropagazioneSalvataggioPoligono == "") {
        return "{ \"AssociazioneArea\": 1, \"AreaAppezzamento\": 0, \"AreaImpianto\": 0, \"PropagazioneSalvataggioPoligono\": " + PropagazioneSalvataggioPoligono + " }";
    }

    if ($(comboJQuerySelector).val() == '1') {
        rval = "{ \"AssociazioneArea\": 1, \"AreaAppezzamento\": 0, \"AreaImpianto\": 0, \"PropagazioneSalvataggioPoligono\": " + PropagazioneSalvataggioPoligono + " }";
    }

    if ($(comboJQuerySelector).val() == '2') {
        rval = "{ \"AssociazioneArea\": 2, \"AreaAppezzamento\": 0, \"AreaImpianto\": " + supImpianto + ", \"PropagazioneSalvataggioPoligono\": " + PropagazioneSalvataggioPoligono + " }";
    }

    if ($(comboJQuerySelector).val() == '3') {
        rval = "{ \"AssociazioneArea\": 3, \"AreaAppezzamento\": " + supAppezzamento + ", \"AreaImpianto\": " + supImpianto + ",  \"PropagazioneSalvataggioPoligono\": " + PropagazioneSalvataggioPoligono + " }";
    }

    if ($(comboJQuerySelector).val() == '5') {
        rval = "{ \"AssociazioneArea\": 5, \"AreaAppezzamento\": " + supAppezzamento + ", \"AreaImpianto\": " + supImpianto + ",  \"PropagazioneSalvataggioPoligono\": " + PropagazioneSalvataggioPoligono + " }";
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
                    //i18n__
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
                                    area = salvataggioDirettoConAppezzaAreaCalcola(area);
                                    salvataggioDirettoConAppezza(area);
                                } else {
                                    //attenzione
                                    $('#lbl_alert_operazioni_Appezza').html(msg.d);
                                    ApriKendoDialog('#dialogAllertOperazioniAppezza');
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
                    ApriKendoDialog('#dialogAllertOperazioni');

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

            interfaccia.loading(false);

            $('#responseInterferenze').width('0px');
            $('#responseInterferenze').html('');

            //GABRIELE errore in Copia/Incolla
            //ChiudiKendoDialog("#dialogConfermaAssociazione");

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {

                //16/01/2015, azzero oggetti.
                if (shape.selectedShape != undefined)
                    shape.selectedShape.setMap(null);
                $('#hiddenPunti_modifica').val("");

                AlberoAnagrafica2017lettura();
                AggiornaTutto(false);
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function modificaDiretto(entita_cod, area, show_dlg) {

    //    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString().replace(/\\/g, '\\\\');  
    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();
    var hiddenPunti_Nuovo = $('#hiddenPunti_modifica').val();

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/ModificaElementoGraficoDaChiaveAlbero",
        data: "{ Entita_Cod: " + entita_cod + ", ChiaveAlbero: '" + ChiaveAlbero + "', hiddenPunti_Nuovo: '" + hiddenPunti_Nuovo + "', Area: '" + area + "', ElementoGrafico_Des: '' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            if (show_dlg) {
                let opOk = Traduzione(AgronicaControlliGisResx, "jsMsgOperazioneEseguitaCorrettamente");
                kendoDlgMessage("", opOk);
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

            interfaccia.loading(false);

            $('#responseInterferenze').width('0px');
            $('#responseInterferenze').html('');

            ChiudiKendoDialog("#dialogConfermaAssociazione");

            if (shape.selectedShape != undefined)
                shape.selectedShape.setMap(null);

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {

                if (area !== "") {
                    AlberoAnagrafica2017lettura();
                }
                AggiornaTutto(false);
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
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

    $(".chkLayerHidden").prop("checked", "checked");

    var tipologia_layer = TipologiaLayer();

    for (var i = 0; i < mappa.Livelli.length; i++) {

        //Analisi Cronologia Azienda -> Nessuna operazione
        if (tipologia_layer != "11" || mappa.Livelli[i].id != "0") {

            clearSingleOverlay(null, mappa.Livelli[i].poligoni, !selezionaSeTrue);
            clearSingleOverlay(null, mappa.Livelli[i].poligoni2, !selezionaSeTrue);
            clearSingleOverlay(null, mappa.Livelli[i].punti, !selezionaSeTrue);

            clearSingleOverlay(null, mappa.Livelli[i].circle, !selezionaSeTrue);

            clearSingleOverlay(null, mappa.Livelli[i].polyline, !selezionaSeTrue);
            clearSingleOverlay(null, mappa.Livelli[i].ABLabel, !selezionaSeTrue);

        }
    }

    mappa.GestioneCluster();
}

/* se il layer è checked = true lo visualizzo altimenti non lo presento nella mappa */
function clearOverlaysByID(id) {

    for (var i = 0; i < mappa.Livelli.length; i++) {
        if (mappa.Livelli[i].id == id) {

            clearSingleOverlay(null, mappa.Livelli[i].poligoni, true);
            clearSingleOverlay(null, mappa.Livelli[i].poligoni2, true);
            clearSingleOverlay(null, mappa.Livelli[i].punti, true);
            clearSingleOverlay(null, mappa.Livelli[i].circle, true);
            clearSingleOverlay(null, mappa.Livelli[i].polyline, true);
            clearSingleOverlay(null, mappa.Livelli[i].ABLabel, true);

        }
    }
}

/* se il layer è checked = true lo visualizzo altimenti non lo presento nella mappa */
function clearOverlays(obj) {

    let liv = 0;
    while (liv < mappa.Livelli.length) {
        let livello = mappa.Livelli[liv];
        if (livello.id == obj.attr("id")) {

            clearSingleOverlay(obj, livello.poligoni, false);
            clearSingleOverlay(obj, livello.poligoni2, false);
            clearSingleOverlay(obj, livello.punti, false);
            clearSingleOverlay(obj, livello.circle, false);
            clearSingleOverlay(obj, livello.polyline, false);
            clearSingleOverlay(obj, livello.ABLabel, false);

            liv = mappa.Livelli.length;
        }
        liv++;
    }
    //for (var i = 0; i < mappa.Livelli.length; i++) {
    //    if (mappa.Livelli[i].id == obj.attr("id")) {

    //        clearSingleOverlay(obj, mappa.Livelli[i].poligoni, false);
    //        clearSingleOverlay(obj, mappa.Livelli[i].poligoni2, false);
    //        clearSingleOverlay(obj, mappa.Livelli[i].punti, false);
    //        clearSingleOverlay(obj, mappa.Livelli[i].circle, false);
    //        clearSingleOverlay(obj, mappa.Livelli[i].polyline, false);
    //        clearSingleOverlay(obj, mappa.Livelli[i].ABLabel, false);

    //    }
    //}


    mappa.GestioneCluster();

    interfaccia.chekLayers(false);

    SalvaTipologiaLayerInCookie();
}

function clearSingleOverlay(obj, oggetti, forzaClear) {

    for (var kk = 0; kk < oggetti.length; kk++) {

        let oggetto = oggetti[kk];
        let mostraSeTrue = true;

        if (obj == null) {
            if (forzaClear) {
                mostraSeTrue = false;
            }
        }
        else {
            if (obj.is(':checked') == false || forzaClear == true) {
                mostraSeTrue = false;
            }
        }

        if (mostraSeTrue) {
            oggetto.setMap(mappa.elemenotMappa);
        } else {
            oggetto.setMap(null);
        }
        if (typeof oggetto.etichetta === "object") {
            if (oggetto.etichetta !== null) {
                if (mostraSeTrue) {
                    oggetto.etichetta.setMap(mappa.elemenotMappa);
                } else {
                    oggetto.etichetta.setMap(null);
                }
            }
        }
    }

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

    //Situazione check/unchek precedente...
    let prevLayers = {};
    $("#check-layer").find("input[type=checkbox]").each(function (idx, elem) {
        prevLayers[elem.id] = elem.checked;
    });

    let wrapper = document.getElementById("check-layer");
    while (wrapper.firstChild) {
        wrapper.removeChild(wrapper.firstChild);
    }

    let layerGrid = document.createElement("div");
    layerGrid.style.cssText = "display: grid; grid-template-columns: auto; grid-gap: 2px; margin-top: 10px; overflow-x: hidden;";

    wrapper.appendChild(layerGrid);

    let tipologia_layer = TipologiaLayer();

    for (let idx = 0; idx < layers.length; idx++) {

        let rowWrapper = document.createElement("div");
        rowWrapper.className = "chkLayerRow";
        rowWrapper.id = "chkLayerRow_" + layers[idx].id;

        let chkHidden = "";
        //Analisi Cronologia Azienda -> Nessuna operazione
        if (tipologia_layer == "11" && layers[idx].id == "0") {
            rowWrapper.style.cssText = "display: none; ";
            chkHidden = " chkLayerHidden";
        }
        rowWrapper.style.cssText += "padding: 3px; border-radius: 4px;";

        wrapper.appendChild(rowWrapper);

        let row = document.createElement("div");
        row.style.cssText = "display: grid; grid-template-columns: auto auto 1fr; grid-gap: 5px; align-items: center;"

        let col1 = document.createElement("div");
        col1.style.cssText = "width: 32px; height: 32px; background: url(" + layers[idx].icona32 + ") 0 0 / contain no-repeat white; cursor: pointer;";
        col1.onclick = function () {
            shape.settaLayerDoveDisegnare(layers[idx].id, layers[idx].icona32, layers[idx].nome);
        }
        let col2 = document.createElement("div");
        col2.style.cssText = "width: 32px; height: 32px; border-radius: 3px; background-color: #" + layers[idx].colore_1;
        let col3 = document.createElement("div");
        col3.style.cssText = "height: 100%; display: flex; align-items: center;"

        if (layers[idx].tiles != undefined) {
            if (layers[idx].tiles.length > 0) {
                let bkgrnd = "linear-gradient(to right, #" + layers[idx].tiles[0].colore_primario + ", #" + layers[idx].tiles[0].colore_secondario;
                let tiles = document.createElement("div");
                tiles.style.cssText = "margin-top: 15px; height: 16px; border-bottom-left-radius: 3px; border-bottom-right-radius: 3px; border-top: 1px solid white; background-image: " + bkgrnd;
                col2.appendChild(tiles);
            }
        }

        let chk = document.createElement("input");
        chk.id = layers[idx].id;
        chk.type = "checkbox";
        chk.className = "k-checkbox chkLayer" + chkHidden;
        let checked = true;
        if (prevLayers[chk.id] !== undefined) {
            checked = prevLayers[chk.id];
        }

        if (checked) {
            chk.setAttribute("checked", "checked");
        }

        chk.onclick = function () {
            clearOverlays($(this));
        }
        let lbl = document.createElement("label");
        lbl.className = "k-checkbox-label label-nowrap-ellipsis";
        lbl.setAttribute("for", layers[idx].id);
        lbl.innerHTML = layers[idx].nome;
        col3.appendChild(chk);
        col3.appendChild(lbl);

        row.appendChild(col1);
        row.appendChild(col2);
        row.appendChild(col3);

        rowWrapper.appendChild(row);
    }

    //console.warn("makeToolBar");

    //var checklayer = $('#check-layer');
    //checklayer.html('');

    //var tipologia_layer = TipologiaLayer();

    //var hidden = "";
    //var chkHidden = "";

    //for (var i = 0; i < layers.length; ++i) {

    //    //Analisi Cronologia Azienda -> Nessuna operazione
    //    if (tipologia_layer == "11" && layers[i].id == "0") {
    //        hidden = "style = 'display: none;'";
    //        chkHidden = " chkLayerHidden";
    //    } else {
    //        hidden = "";
    //        chkHidden = "";
    //    }


    //    var chiamataFunzione = 'shape.settaLayerDoveDisegnare(' + layers[i].id + ', \'' + layers[i].icona32 + '\', \'' + layers[i].nome + '\')';

    //    //utility.log("chiamataFunzione:" + chiamataFunzione);

    //    //var box_img = '<div class="col-xs-1" style="margin-right: 22px;"><img src="' + layers[i].icona32 + '" onclick="' + chiamataFunzione + '");" /></div>';
    //    //var box = '<div class="col-xs-1 color-button" style="background-color:#' + layers[i].colore_1 + '" alt="' + layers[i].colore_1 + '">&nbsp</div>';
    //    //var box_1 = '<div class="col-xs-1 color-button" style="background-color:#' + layers[i].colore_2 + '" alt="' + layers[i].colore_2 + '">&nbsp</div>';
    //    //var html = '<div class="col-xs-1"><input type="checkbox" class="k-checkbox chkLayer" onclick="clearOverlays($(this));" checked="checked" id="' + layers[i].id + '"/><label class="k-checkbox-label" for="' + layers[i].id + '">' + layers[i].nome + '</label></div>';

    //    //var box_img = '<img src="' + layers[i].icona32 + '" onclick="' + chiamataFunzione + '");" style="margin-right: 5px;"/>';
    //    var box_img = '<img src="' + layers[i].icona32 + '" onclick="' + chiamataFunzione + '" style="margin-right: 5px;"/>';
    //    var box = '<span class="color-button" style="border-radius:3px; background-color:#' + layers[i].colore_1 + '" alt="' + layers[i].colore_1 + '">';
    //    if (layers[i].tiles != undefined) {
    //        if (layers[i].tiles.length > 0) {
    //            box += '<div style="margin-top: 15px; height: 16px; border-bottom-left-radius: 3px; border-bottom-right-radius: 3px; border-top: 1px solid white; background-image: linear-gradient(to right, #' + layers[i].tiles[0].colore_primario + ', #' + layers[i].tiles[0].colore_secondario + ');"></div>';
    //            /*
    //            box += '<div style="margin-top: 15px; height: 16px; display: flex; border-top: 1px solid white;">';
    //            box += '<div style="height: 100%; width: 50%; background-color:#' + layers[i].tiles[0].colore_primario + ';"></div>';
    //            box += '<div style="height: 100%; width: 50%; background-color:#' + layers[i].tiles[0].colore_secondario + ';"></div>';
    //            box += '</div>';
    //            */
    //        }
    //    }
    //    box += '</span>';
    //    var box_1 = ''//'<span class="color-button" style="background-color:#' + layers[i].colore_2 + '" alt="' + layers[i].colore_2 + '"></span>';
    //    var html = '<input type="checkbox" class="k-checkbox chkLayer ' + chkHidden + '" onclick="clearOverlays($(this));" checked="checked" id="' + layers[i].id + '"/><label class="k-checkbox-label label-nowrap-ellipsis" style="margin-top: auto; margin-bottom: auto;" for="' + layers[i].id + '">' + layers[i].nome + '</label>';

    //    checklayer.append("<div id='chkLayerRow_" + layers[i].id + "' class='chkLayerRow' " + hidden + "><div style='display: flex;'> " + box_img + " " + box + " " + box_1 + html + "</div></div>");
    //}

}

function isLayerVisualizzato(idL) {
    var trovato = false;
    for (var i = 0; i < layers.length; ++i) {
        if (layers[i].id == idL)
            trovato = true;
    }

    return trovato;
}

/**
 * Seleziona oggetto cartografico da albero.
 * @param {string} id id dell'albero
 * @param {boolean} multi si tratta di multiselezione
 */
/*GABRIELE
function selezioneDaIdAlbero(id, multi, SelezionaInAlbero) {

    utility.log('Da modificare per selezione ');

    $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val(id);

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
                        setSelection(mappa.Livelli[i].poligoni[kk], SelezionaInAlbero);
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
            selezioneDaIdAlberoNoMappaTrovata(id);
            Disenga_poligono_dropdown_show(true);
        }
    }
}

function selezioneDaIdAlberoNoMappaTrovata(id) {

    $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val(id);
    setSelectionAlberoBScheck(id);

    // vanni, 09/04/2018: ora viene chiamata sul click OK
    ////aggiornamento Descrizione
    //AnagraficaRiassunto(true);

}
*/
$(window).resize(function () {

    for (var j = 0; j < mappa.Livelli.length; j++) {
        interfaccia.CreaPannelloColore('abc', mappa.Livelli[j].datiViste);
    }

    PosizionaRicercaIndirizzo();

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

function verificaInterferenze(webService, titolo) {

    let Veg_Cod = $('#pop_up_specie').val()

    if (Veg_Cod == "") {

        kendoDlgMessage(titolo, "Selezionare una specie");
        return;
    }

    interfaccia.loading(true);

    let hiddenPunti_Nuovo = $('#hiddenPunti_Nuovo').val();
    let myData_Inizio = $('#pop_up_data_inizio').val();
    let myData_Fine = $('#pop_up_data_fine').val();
    var pop_up_tipologia = $('#pop_up_tipologia').val();

    let risp = GisAjaxSync(webService, {
        veg_cod: Veg_Cod,
        grva_cod: pop_up_tipologia,
        data_inizio: myData_Inizio,
        data_fine: myData_Fine,
        hiddenPunti_Nuovo: hiddenPunti_Nuovo
    });

    interfaccia.loading(false);

    kendoDlgMessage(titolo, risp);
}

function verificaVicini() {

    let Veg_Cod = $('#pop_up_specie').val()

    if (Veg_Cod == "") {

        kendoDlgMessage("Verifica prossimità", "Selezionare una specie");
        return;
    }

    interfaccia.loading(true);

    let hiddenPunti_Nuovo = $('#hiddenPunti_Nuovo').val();
    let myData_Inizio = $('#pop_up_data_inizio').val();
    let myData_Fine = $('#pop_up_data_fine').val();
    var pop_up_tipologia = $('#pop_up_tipologia').val();

    let risp = GisAjaxSync("VerificaVicini", {
        veg_cod: Veg_Cod,
        grva_cod: pop_up_tipologia,
        data_inizio: myData_Inizio,
        data_fine: myData_Fine,
        hiddenPunti_Nuovo: hiddenPunti_Nuovo
    });

    interfaccia.loading(false);

    let obj = JSON.parse(risp);

    let cont = obj.msg;
    if (obj.arr.length > 0) {
        cont += "<div style='padding:10px;'>";
        cont += "<div style='text-align:left; display:grid; grid-template-columns:auto auto auto auto auto; grid-gap:10px 20px;'>";
        for (e = 0; e < obj.arr.length; e++) {
            cont += "<div>&#x2022;</div>";
            cont += "<div>" + obj.arr[e].referente + "</div>";
            cont += "<div>" + obj.arr[e].indirizzo + "</div>";
            cont += "<div>" + obj.arr[e].veg_des + "</div>";
            cont += "<div style='justify-self: end;'>" + obj.arr[e].distanza + "</div>";
        }
        cont += "</div>";
        cont += "</div>";
    }

    kendoDlgMessage("Verifica prossimità", cont);

}

function verificaVicini_2() {

    let hiddenID = $("#hiddenID").val().split("|");
    let hiddenPunti_Modifica = $('#hiddenPunti_modifica').val();

    if (hiddenID.length < 2) {
        return;
    }
    let pivasuperuser = hiddenID[0];
    let entita_cod = hiddenID[1];
    //se sono tutte valorizzate allora posso procedere
    if (pivasuperuser == "" || entita_cod == "") {
        //alert("nessun poligono selezionato.");
        return;
    }

    interfaccia.loading(true);

    let risp = GisAjaxSync("VerificaVicini_2", {
        entita_cod: entita_cod,
        hiddenPunti_Modifica: hiddenPunti_Modifica
    });

    interfaccia.loading(false);

    let obj = JSON.parse(risp);

    let cont = obj.msg;
    if (obj.arr.length > 0) {
        cont += "<div style='padding:10px;'>";
        cont += "<div style='text-align:left; display:grid; grid-template-columns:auto auto auto auto auto; grid-gap:10px 20px;'>";
        for (e = 0; e < obj.arr.length; e++) {
            cont += "<div>&#x2022;</div>";
            cont += "<div>" + obj.arr[e].referente + "</div>";
            cont += "<div>" + obj.arr[e].indirizzo + "</div>";
            cont += "<div>" + obj.arr[e].veg_des + "</div>";
            cont += "<div style='justify-self: end;'>" + obj.arr[e].distanza + "</div>";
        }
        cont += "</div>";
        cont += "</div>";
    }

    kendoDlgMessage("Verifica prossimità", cont);
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
    ChiudiKendoDialog("#pop_up_import_shape");
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

    var appData_Inizio_Fine;


    appData_Inizio_Fine = $('#Dati_data_inizio_fine').html();


    $('#Dati_data_inizio_fine').html('')
    $('#placeData_Inizio_Fine').append(appData_Inizio_Fine);

    //se sono in modalità sementi

    //if (GisPurpose === Enum_GisPurpose.SementiSportello) {

    //    $('#pop_up_m_data_inizio').val(data_inizio_sportello);
    //    $('#pop_up_m_data_fine').val(data_fine_sportello);

    //    $('#pop_up_data_fine').val(data_fine_sportello);
    //    var m = new Date();
    //    var month = m.getMonth() + 1
    //    var day = m.getDate()
    //    var year = m.getFullYear()
    //    $('#pop_up_data_inizio').val(day + "/" + month + "/" + year);

    //}

    if (!ModalitaBootstrap)
        InizializzaDatepickerUI();
}

//Log di quanto Inizializzato
function InizializzaProprietajsExtracted_LOG() {
    utility.warn("InizializzaProprietajs (inizializzazione da server) ");
    utility.log("AggiornaDatiGiasAlarm = " + AggiornaDatiGiasAlarm.toString());
    utility.log("CiSonoVecchiDatiNonImportati = " + CiSonoVecchiDatiNonImportati.toString());
    utility.log("AbilitaPF = " + AbilitaPF.toString());
    utility.log("Codice_Fiscale_Tecnico = " + Codice_Fiscale_Tecnico.toString());
}

//inizializzazione da server
function InizializzaProprietajs(serverLeggiDatiGIASAlarm, serverCiSonoVecchiDatiNonImportati, serverAbilitaPF, lCodice_Fiscale_Tecnico, sFinestraTemporale_GIS_Inizio, sFinestraTemporale_GIS_Fine, bModalitaBootstrap) {
    AggiornaDatiGiasAlarm = serverLeggiDatiGIASAlarm;
    ModalitaBootstrap = bModalitaBootstrap;
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
        $('#li_nuova_ricetta').hide();
        $('#li_PianoRateoVariabile').hide();
        $('#li_ab-button').hide();
        $('#img_tool').hide();
        $('#scala_colori_contenitore').hide();

    }

    let flgDescr = false;
    if (sFinestraTemporale_GIS_Inizio != '') {
        $("#limita_data_da").val(sFinestraTemporale_GIS_Inizio);
        flgDescr = true;
    }

    if (sFinestraTemporale_GIS_Fine != '') {
        $("#limita_data_a").val(sFinestraTemporale_GIS_Fine);
        flgDescr = true;
    }

    if (flgDescr) {
        //i18n__
        $("#" + lbl_FinestraTemporale_ClientID).html("Visualizzazione limitata tra il " + sFinestraTemporale_GIS_Inizio + " e il " + sFinestraTemporale_GIS_Fine);
    }

    if (GisPurpose === Enum_GisPurpose.SementiSportello) {

        $('#pop_up_m_data_inizio').val(sFinestraTemporale_GIS_Inizio);
        $('#pop_up_m_data_fine').val(sFinestraTemporale_GIS_Fine);

        $('#pop_up_data_inizio').val(sFinestraTemporale_GIS_Inizio);
        $('#pop_up_data_fine').val(sFinestraTemporale_GIS_Fine);
    }
}

function MostraNascondiPulsanti(bool_catasto, bool_precision, bool_esportazione, bool_bufferzone) {

    if (bool_bufferzone == 'False') {
        $("#li_BufferZone").hide();
    } else {
        $("#li_BufferZone").show();
        PermessiGisServerSide.bool_bufferzone = true;
    }

    if (bool_catasto == 'False') {
        $('#li_ripartoCatasto-button').hide();
    }
    else {
        $('#li_ripartoCatasto-button').show();
        PermessiGisServerSide.bool_catasto = true;
    }

    if (bool_precision == 'False') {
        $('#li_ab-button').hide();
        $('#li_nuova_ricetta').hide();
        $('#img_tool').hide();
        $('#li_PianoRateoVariabile').hide();
        $('#li_GeneraPlanning').hide();

        $('#divEliminaPrecision').hide();
        $('#divEliminaPrecisionAB').hide();
    }
    else {
        $('#li_ab-button').show();
        $('#li_nuova_ricetta').show();
        $('#img_tool').show();
        $('#li_PianoRateoVariabile').show();
        $('#li_GeneraPlanning').show();

        $('#divEliminaPrecision').show();
        $('#divEliminaPrecisionAB').show();
        PermessiGisServerSide.bool_precision = true;
    }

    if (bool_esportazione == 'False') {
        $('#li_export-button').hide();
    }
    else {
        $('#li_export-button').show();
        PermessiGisServerSide.bool_esportazione = true;
    }

}

function ImpostaVisibilitaPulsanti(nuovo, salva, elimina, esporta, importa, ab, meteo, visite,rete_acqua) {

    gNuovo = nuovo;

    if (nuovo) {
        Disenga_poligono_dropdown_show(true);
    }
    else {
        Disenga_poligono_dropdown_show(false);
    }

    if (GisPurpose === Enum_GisPurpose.SementiSportello || GisPurpose === Enum_GisPurpose.SementiMappaturaLibera) {
        $('#save-plus').hide();

    } else {
        if (salva)
            $('#save-plus').show();
        else
            $('#save-plus').hide();
    }

    if (salva) {
        $('#save-button').show();
        $('#save-buttonHeader').show();
    } else {
        $('#save-button').hide();
        $('#save-buttonHeader').hide();
    }

    if (elimina) {
        $('#delete-button').show();
        $('#delete-buttonHeader').show();
    } else {

        $('#delete-button').hide();
        $('#delete-buttonHeader').hide();
    }

    if (importa)
        $('#li_import-button').show();
    else
        $('#li_import-button').hide();

    if (esporta)
        $('#li_export-button').show();
    else
        $('#li_export-button').hide();

    if (ab)
        $('#li_ab-button').show();
    else
        $('#li_ab-button').hide();

    if (meteo) {
        $("#li_AnalisiMeteo").show();
        $("#li_AnalisiRilievi").show();

    } else {
        $("#li_AnalisiMeteo").hide();
        $("#li_AnalisiRilievi").hide();
    }

    if (visite) {
        $("#li_nuova_visita").show();
        $("#nuova_visitaHeader").show();

    } else {
        $("#li_nuova_visita").hide();
        $("#nuova_visitaHeader").hide();
    }

    if (rete_acqua) {
        $("#li_AnalisiDatiReteAcqua").show();
    } else {
        $("#li_AnalisiDatiReteAcqua").hide();
    }
}

function GestisciLayerDettagli() {
    interfaccia.loading(true);

    $('#hiddenPrincipale_1_Dettagli_2').val(2);

    var tipologia_layer = TipologiaLayer();

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

                //GABRIELE 2019 07 26
                //ApriKendoDialog("#dialogGestioneColoriLayer");

                //if (msg.d.toString() != '') {
                //    $("#ddlTipologiaLayer").html(msg.d);

                //    //' VAnni: 3/8/2018: gestita inizializzazione della sezione tematizzazione, che veniva sempre caricata e mai ripulita
                //    $("#ddlTipologiaLayer").prepend("<option value='-999999' selected='selected'>Seleziona un tema</option>");
                //    $("#place_tabella").html("");
                //    //Caricaplace_tabella_Dettagli($("#ddlTipologiaLayer").val());

                //    SliderKendo();
                //    ColorPickerKendo();

                //}

                GestioneColoriLayersDettagli(msg.d);
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


                    SliderKendo();
                    ColorPickerKendo();

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

function Caricaplace_tabella_Dettagli2(tipologiaLayerId, layerId, id_table) {
    interfaccia.loading(true);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/Caricaplace_tabella_Dettagli2",
        data: "{ tipologiaLayerId: '" + tipologiaLayerId + "', layerId: '" + layerId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            interfaccia.loading(false);

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {

                let kendodata = null;
                if (msg.d !== '') {
                    kendodata = JSON.parse(msg.d);
                }

                $(id_table).css("opacity", "0");

                let grid = $(id_table).data("kendoGrid");
                if (grid != undefined) {
                    grid.destroy();
                    $(id_table).empty();
                }

                if (kendodata !== null) {

                    let col = 0;
                    while (col < kendodata.kendo_columns.length) {
                        let column = kendodata.kendo_columns[col];
                        if (column.field === "Colore_Primario" || column.field === "Colore_Secondario") {
                            column.editor = function (container, options) {
                                // create an input element
                                let input = $("<input/>");
                                // set its name to the field to which the column is bound ('name' in this case)
                                input.attr("name", options.field);
                                let color = options.model[options.field];
                                if (color != '') {
                                    color = "#" + color;
                                }
                                input.attr("value", color);
                                // append it to the container
                                input.appendTo(container);
                                // initialize a Kendo UI AutoComplete

                                var msgOk = Traduzione(AgronicaControlliGisResx, "jsLblok");
                                var msgAnnulla = Traduzione(AgronicaControlliGisResx, "jsLblAnnulla");

                                input.kendoColorPicker({
                                    //buttons: false,
                                    //clearButton: true,
                                    //preview: false,
                                    messages: {
                                        apply: msgOk,
                                        cancel: msgAnnulla
                                    },
                                    change: function (e) {
                                        // Elimino il # all'inizio
                                        options.model[options.field] = e.value.substr(1);
                                    }
                                });
                            }
                        }
                        col++;
                    }

                    // per evitaer errore "Cannot parse color" in ColorPicker
                    $.each(kendodata.kendo_rows, function (idx, row) {
                        if (row.Colore_Primario == '') {
                            row.Colore_Primario = null;
                        }
                        if (row.Colore_Secondario == '') {
                            row.Colore_Secondario = null;
                        }
                    });

                    $(id_table).kendoGrid({
                        dataSource: {
                            data: kendodata.kendo_rows,
                            schema: {
                                model: {
                                    fields: kendodata.kendo_model
                                }
                            },
                            batch: true
                        },
                        columns: kendodata.kendo_columns,
                        resizable: false,
                        pageable: false,
                        editable: true
                    });

                    let h = $(id_table).parent().height() - $(id_table + " .k-grid-header").outerHeight() - 2;
                    $(id_table + " .k-grid-content").css("height", h + "px");

                    $(id_table).css("opacity", "1");
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

function GestioneColoriLayersDettagli(strOptions) {

    if (strOptions.toString() === '') {
        return;
    }

    let h60 = $(window).height() * 0.60;

    let content = "<div>";
    content += "<select id='id_ddlLayers' style='width: 100%;'>";
    content += strOptions;
    content += "</select>";
    content += "<div style='padding-top: 15px; height: " + h60 + "px; overflow-y: hidden;'>";
    content += "<style type='text/css'>";
    content += " .allineadestra { text-align: right !important; }";
    content += " .allineacentro { text-align: center !important; }";
    content += " .color-border { border: 1px solid #ccc; }";
    content += " .k-dirty { display: none; }";
    content += " .k-colorpicker .k-selected-color { width: 100%; }";
    content += "</style>";
    content += "<div id='id_tableLayers'></div>";
    content += "</div>";
    content += "</div>";

    let win_el = document.createElement("div");
    win_el.id = "id_tmp_kendo_dlg";
    document.body.appendChild(win_el);
    let $win_el = $("#id_tmp_kendo_dlg");

    var msgOk = Traduzione(AgronicaControlliGisResx, "jsLblok");
    var msgAnnulla = Traduzione(AgronicaControlliGisResx, "jsLblAnnulla");

    $win_el.kendoDialog({
        title: "Gestione colori temi",
        width: "50%",
        closable: true,
        modal: true,
        visible: false,
        content: content,
        actions: [
            {
                text: msgOk,
                action: function () {
                    let layerId = $("#id_ddlLayers").data("kendoDropDownList").value();
                    let tipologiaLayerId = $('#tipologia_layer').data("kendoDropDownList").value();
                    return SalvaImpostazioniLayer(tipologiaLayerId, $("#id_tableLayers").data("kendoGrid"), layerId);
                }
            },
            { text: msgAnnulla }
        ],
        open: function (e) {

            $("#id_ddlLayers").kendoDropDownList({
                change: function (e) {
                    let layerId = this.value();
                    let tipologiaLayerId = $('#tipologia_layer').data("kendoDropDownList").value();
                    Caricaplace_tabella_Dettagli2(tipologiaLayerId, layerId, "#id_tableLayers");
                }
            });

            let ddl = $("#id_ddlLayers").data("kendoDropDownList");

            ddl.select(0);
            ddl.trigger("change");
        },
        close: function (e) {
            this.destroy();
        }
    });

    $win_el.data("kendoDialog").open();
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

function GestioneColoriLayers(strOptions) {

    if (strOptions.toString() === '') {
        return;
    }

    let h60 = $(window).height() * 0.60;

    let content = "<div>";
    content += "<select id='id_ddlLayers' style='width: 100%;'>";
    content += strOptions;
    content += "</select>";
    content += "<div style='padding-top: 15px; height: " + h60 + "px; overflow-y: hidden;'>";
    content += "<style type='text/css'>";
    content += " .allineadestra { text-align: right !important; }";
    content += " .allineacentro { text-align: center !important; }";
    content += " .color-border { border: 1px solid #ccc; }";
    content += " .k-dirty { display: none; }";
    content += " .k-colorpicker .k-selected-color { width: 100%; }";
    content += "</style>";
    content += "<div id='id_tableLayers'></div>";
    content += "</div>";
    content += "</div>";

    let win_el = document.createElement("div");
    win_el.id = "id_tmp_kendo_dlg";
    document.body.appendChild(win_el);
    let $win_el = $("#id_tmp_kendo_dlg");

    var msgOk = Traduzione(AgronicaControlliGisResx, "jsLblok");
    var msgAnnulla = Traduzione(AgronicaControlliGisResx, "jsLblAnnulla");

    $win_el.kendoDialog({
        title: "Impostazioni layers",
        width: "75%",
        closable: true,
        modal: true,
        visible: false,
        content: content,
        actions: [
            {
                text: msgOk,
                action: function () {
                    let tipologiaLayer = $("#id_ddlLayers").data("kendoDropDownList").value();
                    return SalvaImpostazioniLayer(tipologiaLayer, $("#id_tableLayers").data("kendoGrid"));
                }
            },
            { text: msgAnnulla }
        ],
        open: function (e) {

            $("#id_ddlLayers").kendoDropDownList({
                change: function (e) {
                    let value = this.value();
                    Caricaplace_tabella2(value, "#id_tableLayers");
                }
            });

            let ddl = $("#id_ddlLayers").data("kendoDropDownList");

            ddl.value(TipologiaLayer());
            ddl.trigger("change");

        },
        close: function (e) {
            this.destroy();
        }
    });

    $win_el.data("kendoDialog").open();
}

function Caricaplace_tabella2(value, id_table) {
    interfaccia.loading(true);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/Caricaplace_tabella2",
        data: "{ valoreSelezionato: '" + value + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            interfaccia.loading(false);

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {

                let kendodata = null;
                if (msg.d !== '') {
                    kendodata = JSON.parse(msg.d);
                }

                $(id_table).css("opacity", "0");

                let grid = $(id_table).data("kendoGrid");
                if (grid != undefined) {
                    grid.destroy();
                    $(id_table).empty();
                }

                if (kendodata !== null) {

                    var msgOk = Traduzione(AgronicaControlliGisResx, "jsLblok");
                    var msgAnnulla = Traduzione(AgronicaControlliGisResx, "jsLblAnnulla");

                    let col = 0;
                    while (col < kendodata.kendo_columns.length) {
                        let column = kendodata.kendo_columns[col];
                        if (column.field === "Colore_Primario" || column.field === "Colore_Secondario") {
                            column.editor = function (container, options) {
                                // create an input element
                                let input = $("<input/>");
                                // set its name to the field to which the column is bound ('name' in this case)
                                input.attr("name", options.field);
                                let color = options.model[options.field];
                                if (color != '') {
                                    color = "#" + color;
                                }
                                input.attr("value", color);
                                // append it to the container
                                input.appendTo(container);
                                // initialize a Kendo UI AutoComplete
                                input.kendoColorPicker({
                                    //buttons: false,
                                    //clearButton: true,
                                    //preview: false,
                                    messages: {
                                        apply: msgOk,
                                        cancel: msgAnnulla
                                    },
                                    change: function (e) {
                                        // Elimino il # all'inizio
                                        options.model[options.field] = e.value.substr(1);
                                    }
                                });
                            }
                        }

                        //GABRIELE 2020-09-03
                        if (value != 1 && column.field === "flag_visibile") {
                            column.hidden = true;
                        }
                        col++;
                    }

                    // per evitaer errore "Cannot parse color" in ColorPicker
                    $.each(kendodata.kendo_rows, function (idx, row) {
                        if (row.Colore_Primario == '') {
                            row.Colore_Primario = null;
                        }
                        if (row.Colore_Secondario == '') {
                            row.Colore_Secondario = null;
                        }
                    });

                    $(id_table).kendoGrid({
                        dataSource: {
                            data: kendodata.kendo_rows,
                            schema: {
                                model: {
                                    fields: kendodata.kendo_model
                                }
                            },
                            batch: true
                        },
                        columns: kendodata.kendo_columns,
                        resizable: false,
                        pageable: false,
                        editable: true
                    });

                    let h = $(id_table).parent().height() - $(id_table + " .k-grid-header").outerHeight() - 2;
                    $(id_table + " .k-grid-content").css("height", h + "px");

                    $(id_table + " .k-grid-content").on("change", "input.grid-chk", function (e) {
                        let grid = $(id_table).data("kendoGrid");
                        let dataItem = grid.dataItem($(e.target).closest("tr"));
                        let field = "MostraDescrizioneAssociata";
                        if (e.currentTarget.id.includes("vis")) {
                            field = "flag_visibile";
                        }
                        dataItem[field] = (this.checked ? 1 : 0);
                    });

                    $(id_table + " .gridSlider").each(function () {

                        $(this).kendoSlider({
                            increaseButtonTitle: "+",
                            decreaseButtonTitle: "-",
                            min: 0,
                            max: 100,
                            smallStep: 10,
                            largeStep: 10,
                            tickPlacement: "none",
                            showButtons: false,
                            change: function (e) {
                                let grid = $(id_table).data("kendoGrid");
                                let dataItem = grid.dataItem($(this.wrapper).closest("tr"));
                                dataItem.trasparenza = e.value / 100.0;
                            }
                        });

                        let slider = $(this).data("kendoSlider");
                        let val = parseFloat($(this).attr("value").replace(",", ".")) * 100;
                        slider.value(val);
                    });

                    $(id_table).css("opacity", "1");
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

function SalvaImpostazioniLayer(tipologiaLayer, grid, layer) {

    if (grid === undefined) {
        return false;
    }

    let hiddenPrincipale_1_Dettagli_2 = $('#hiddenPrincipale_1_Dettagli_2').val();

    let data = grid.dataSource.data();

    let stringa_dati = "";

    if (hiddenPrincipale_1_Dettagli_2 === "1") {

        $.each(data, function (idx, item) {

            let toJoin = new Array();

            //Gabriele 2019 09 03

            //toJoin.push(item.LayerElementiGrafici_Cod.toString());

            let LayerElementiGrafici_Cod = item.LayerElementiGrafici_Cod.toString();
            if (tipologiaLayer == "100") {
                LayerElementiGrafici_Cod = ("00000000000" + LayerElementiGrafici_Cod).slice(-11);
            }
            toJoin.push(LayerElementiGrafici_Cod);

            if (item.Colore_Primario !== null) {
                toJoin.push(item.Colore_Primario.toString());
            } else {
                toJoin.push("");
            }
            if (item.hasOwnProperty("Colore_Secondario") && item.Colore_Secondario !== null) {
                toJoin.push(item.Colore_Secondario.toString());
            } else {
                toJoin.push("");
            }
            if (item.hasOwnProperty("Varianza")) {
                toJoin.push(item.Varianza.toString());
            } else {
                toJoin.push("");
            }
            toJoin.push(item.trasparenza.toString());
            toJoin.push(item.zindex.toString());

            //GABRIELE 2020-09-03
            let flag_visibile = item.flag_visibile;
            if (tipologiaLayer != 1) {
                flag_visibile = 1;
            }
            toJoin.push(flag_visibile.toString());

            toJoin.push(item.MostraDescrizioneAssociata.toString());
            toJoin.push(tipologiaLayer.toString());

            if (idx > 0) {
                stringa_dati += "$";
            }
            stringa_dati += toJoin.join("|");
        });

    } else if (hiddenPrincipale_1_Dettagli_2 === "2") {

        if (layer === undefined) {
            return false;
        }

        $.each(data, function (idx, item) {

            let toJoin = new Array();
            toJoin.push(item.LayerTiles_Cod.toString());
            if (item.Colore_Primario !== null) {
                toJoin.push(item.Colore_Primario.toString());
            } else {
                toJoin.push("");
            }
            if (item.Colore_Secondario !== null) {
                toJoin.push(item.Colore_Secondario.toString());
            } else {
                toJoin.push("");
            }
            if (item.hasOwnProperty("Varianza")) {
                toJoin.push(item.Varianza.toString());
            } else {
                toJoin.push("");
            }
            toJoin.push(tipologiaLayer.toString());
            toJoin.push(layer.toString());

            if (idx > 0) {
                stringa_dati += "$";
            }
            stringa_dati += toJoin.join("|");
        });

    }

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/SalvaColoriLayer2",
        data: "{ Tipologia:'" + hiddenPrincipale_1_Dettagli_2 + "', Dati: '" + stringa_dati + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            interfaccia.loading(false);

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {

                //kendoDlgMessage("Impostazioni layers", "<div>Impostatazioni aggiornate...</div><div style='margin-top: 25px;'>Verrà ricaricata la pagina.</div>",
                //    function () {
                //        var hrefReload = window.location.href.split("?");
                //        var hrefReloadQueryString = "";
                //        if (hrefReload.length == 2) {
                //            hrefReloadQueryString = "?" + hrefReload[1];
                //        }
                //        location.href = indirizzohttp + hrefReloadQueryString;
                //    });

                AggiornaElencoTipologie(TipologiaLayer(), false);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });

    return true;
}

function TipologiaLayer() {
    let value = $("#tipologia_layer").data("kendoDropDownList").value();
    return value;
}

function SalvaTipologiaLayerInCookie() {

    let tipologiaSelezionata = TipologiaLayer();

    let layers = {};
    $("#check-layer").find("input[type=checkbox]").each(function (idx, elem) {
        layers[elem.id] = elem.checked;
    });

    let obj = { tipologia: tipologiaSelezionata, layers: layers };
    let date = new Date();
    date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
    $.cookie("GIS.TipologiaLayer." + usernameLoggato, JSON.stringify(obj), { expires: date });

}

function ImpostaTipologiaLayerDaCookie() {

    let json_obj = $.cookie("GIS.TipologiaLayer." + usernameLoggato);

    let ddl = $("#tipologia_layer").data("kendoDropDownList");

    if (json_obj !== null) {

        let obj = JSON.parse(json_obj);

        let old_val = ddl.value();
        ddl.value(obj.tipologia);
        if (ddl.value() != "") {
            ddl.trigger("change");
        } else {
            ddl.value(old_val);
        }

        if (typeof obj.layers === "object") {

            $("#check-layer").find("input[type=checkbox]").each(function (idx, elem) {
                if (obj.layers[elem.id] !== undefined) {
                    elem.checked = obj.layers[elem.id];
                }
            });
        }

    } else {

        if (GisPurpose === Enum_GisPurpose.SementiSportello || GisPurpose === Enum_GisPurpose.SementiMappaturaLibera) {
            ddl.value("100");
            ddl.trigger("change");
        }
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

                //GABRIELE 2019 07 24
                //ApriKendoDialog("#dialogGestioneColoriLayer");

                //if (msg.d.toString() != '') {

                //    $("#ddlTipologiaLayer").html(msg.d);
                //    $('#ddlTipologiaLayer').find('option[value="' + $("#tipologia_layer").val() + '"]').attr("selected", true);
                //    Caricaplace_tabella($("#ddlTipologiaLayer").val());
                //}

                GestioneColoriLayers(msg.d);
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

                    SliderKendo();
                    ColorPickerKendo();

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

function AggiornaElencoTipologie(Layer_Selezionato, autoFit) {

    console.log('AggiornaElencoTipologie');
    if (autoFit === undefined) {
        autoFit = true;
    }

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/AggiornaElencoTipologie2",
        data: "{ Layer_Selezionato: '" + Layer_Selezionato + "'}",
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

                AggiornaLayer(autoFit);

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

    mappa.markerClusterer_clear();

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
                for (var kk = 0; kk < mappa.Livelli[i].poligoni2.length; kk++) {
                    mappa.Livelli[i].poligoni2[kk].setMap(null);
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
                mappa.Livelli[i].poligoni2 = new Array();
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
        for (var kk = 0; kk < mappa.Livelli[i].poligoni2.length; kk++) {
            mappa.Livelli[i].poligoni2[kk].setMap(null);
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

function caricaDatiPFXML() {
    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString(); //.replace(/\\/g, '\\\\'); ;

    if (ChiaveAlbero === "") {
        kendo.alert("Nessun Elemento Selezionato");
    } else {
        var vChiaveAlbero = ChiaveAlbero.split("§");
        if (vChiaveAlbero[0] === "60") {
            CaricaDatiPrecisionXmlDaAllegati(vChiaveAlbero[vChiaveAlbero.length - 1], 1);
        } else {
            kendo.alert("Selezionare un elemento di tipo Operazione/Ricetta");
        }
    }
}

/**
 * Istanzia gli oggetti necessari in sessione e richiama ri-esegue l'aggiornamento
 * @param {any} Codici codici di op. ricette (o allegati) separati da virgola
 * @param {any} TipoCodici 1 per ricette, 2 per allegati
 */

function CaricaDatiPrecisionXmlDaAllegati(Codici, TipoCodici) {
    ajaxAgronica(indirizzohttp + "/CaricaDatiPrecisionXmlDaAllegati", JSON.stringify({ Codici: Codici, TipoCodici: TipoCodici }),
        function (risposta) {
            $("#AggiornaFiltro_Client").click();
        }, null);
}

function AggiornaLayer(autoFit, callback) {
    if (mappa.Livelli !== null)
        svuotaTutto_nuoviVettori();

    var Layer_Selezionato = TipologiaLayer();

    interfaccia.loading(true);

    var wktBoundaySTIntersects = "";

    if (TipoRender_ServerSide == Enum_TipoRender_ServerSide.Parziale) {
        wktBoundaySTIntersects = mappa.GoogleBoundsWKT();
    }

    var cfgAlbero = $("#" + hdAlberoAnagrafica2017cfg_ClientID).val();

    var metodAggiorna = "/AggiornaLayerConIntersezioneWKT_2";

    if ($("#cBtn_FiltraImpianti").html() !== undefined) {
        metodAggiorna = "/AggiornaLayerConIntersezioneWKTCfgAlbero_2";
    }

    $.ajax({
        type: "POST",
        url: indirizzohttp + metodAggiorna,
        data: "{ Layer_Selezionato: '" + Layer_Selezionato + "', AggiornaDatiGiasAlarm: '" + AggiornaDatiGiasAlarm.toString() + "', wktBoundaySTIntersects: '" + wktBoundaySTIntersects + "', cfgAlbero: " + cfgAlbero + " }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            Render_ServerSideOk = true;

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
                    utility.log("spacchetta= " + diff);

                    var l_place = place.length;
                    var l_livelli = mappa.Livelli.length;
                    start = (new Date).getTime();

                    var autoFitDaCheck = $("#ckbCentraSuAggiorna").is(":checked");
                    var autoFitSuEntita = (Request_QueryString("Entita") !== null);

                    mappa.ImpostaShape(true, (autoFitDaCheck || autoFitSuEntita));
                    diff = (new Date).getTime() - start;
                    utility.log("tutto impostashape= " + diff);

                    GestisciStrumentoScomponiRicomponi();

                }

            }

            if (typeof callback === "function") {
                callback();
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
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

    let latlngBounds = new google.maps.LatLngBounds();
    let latlngCenter = null;

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

//#region "Operazioni Agenda"

function apriPreselezioneAgendaVerifica() {

    var Entita_Cod;
    Entita_Cod = getChiaveAlberoCodes();

    if (Entita_Cod == "") {
        let msgVer = Traduzione(AgronicaControlliGisResx, "jsMsgNessunElementoSelezionato");
        kendoDlgMessage("", msgVer);
        return true;
    }

    return false
}

function getChiaviAlberoConIDAgendaDaArray(ArrayOggettiShape, rval) {

    var idAgendaImpostato = false;

    if (ArrayOggettiShape.length > 0) {

        var cChiaveAlbero = ArrayOggettiShape[0].chiavealbero;

        rval.idAgenda = cChiaveAlbero.split(separatoreChiaveAlbero)[25];
        rval.chiaveAlberoCodes.push(cChiaveAlbero);

        for (var i = 1; i < ArrayOggettiShape.length; i++) {

            cChiaveAlbero = ArrayOggettiShape[i].chiavealbero;
            var curIdAgenda = cChiaveAlbero.split(separatoreChiaveAlbero)[25];

            rval.chiaveAlberoCodes.push(cChiaveAlbero);

            if (rval.idAgenda !== curIdAgenda && !idAgendaImpostato) {
                rval.idAgenda = "0";
                idAgendaImpostato = true;
            }

        }

        return true;
    }

    return false;
}

function getChiaviAlberoConIDAgenda() {



    var rval = {
        idAgenda: "-1",
        chiaveAlberoCodes: []
    };


    if (getChiaviAlberoConIDAgendaDaArray(shape.selectedShapeArray, rval)) {
        return rval
    }


    if (getChiaviAlberoConIDAgendaDaArray(PuntiSelezionati_Scomposti, rval)) {
        return rval
    }

    if (shape.selectedShape !== undefined && shape.selectedShape !== null) {
        rval.idAgenda = shape.selectedShape.chiavealbero.split(separatoreChiaveAlbero)[25];
        rval.chiaveAlberoCodes.push(shape.selectedShape.chiavealbero);
    }

    return rval;

}

function apriPreselezioneVisita() {

    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();

    if (ChiaveAlbero === "") {
        kendoDlgMessage("", "Selezionare un'impianto.");
        return false;
    }

    CoordDaSelezione();
    let lat = $("#find_lat").data("kendoCoordMaskedTextBox").decimalValue();
    let lng = $("#find_lng").data("kendoCoordMaskedTextBox").decimalValue();

    ajaxAgronica(indirizzohttp + "/visite",
        "{ lat: '" + lat + "', lng: '" + lng + "', ChiaveAlbero: '" + ChiaveAlbero + "' }",
        function (risposta) {

            ModalBootstrapApri(risposta.RispostaStringa, "Info Visita");

        }, null);
}

function apriPreselezioneAgenda() {

    var DevoUscire = apriPreselezioneAgendaVerifica();
    if (DevoUscire) {
        return;
    }

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
                    //$("#pop_up_opAgenda_Preselezione").dialog("open");
                    ApriKendoDialog("#pop_up_opAgenda_Preselezione")
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

function getChiaveAlberoCodes() {


    var chiaviAlbero = getChiaviAlberoConIDAgenda();
    return chiaviAlbero.chiaveAlberoCodes.join("|");

}

/**
 * Nuova operazione di agenda o modifica di operazione esistente (lav_cod = -1)
 * @param {any} lav_cod
 */

function preselezioneAgenda(lav_cod, btn, idAgendaParametro, TipoOperazioneDB) {

    var xCompatibilita = false;
    if (TipoOperazioneDB === undefined) {
        xCompatibilita = true;
    }

    CoordDaSelezione();

    let ChiaveAlberoCodes = getChiaviAlberoConIDAgenda();

    let lat = $("#find_lat").data("kendoCoordMaskedTextBox").decimalValue();
    let lng = $("#find_lng").data("kendoCoordMaskedTextBox").decimalValue();

    var idAgenda = "0";
    if (lav_cod < 0) {

        if (idAgendaParametro) {
            idAgenda = idAgendaParametro;
        } else {
            idAgenda = ChiaveAlberoCodes.idAgenda;
        }

        if (parseInt(idAgenda) <= 0) {
            //i18n__
            kendoDlgMessage("", "Nessuna operazione selezionata");
            return;
        }
        lav_cod = 0;
    }

    var urlAgnd;
    var payloadAgnd;

    if (xCompatibilita) {
        urlAgnd = "/clickOpAgendaBS_idAgenda";
        payloadAgnd = "{ lav_cod: '" + lav_cod + "', ChiaveAlbero: '" + ChiaveAlberoCodes.chiaveAlberoCodes.join("|") + "', lat: '" + lat + "', lng: '" + lng + "', idAgenda: '" + idAgenda + "' }";
    } else {
        urlAgnd = "/clickOpAgendaBS_idAgenda_TipoOperazioneDB";
        payloadAgnd = "{ lav_cod: '" + lav_cod + "', ChiaveAlbero: '" + ChiaveAlberoCodes.chiaveAlberoCodes.join("|") + "', lat: '" + lat + "', lng: '" + lng + "', idAgenda: '" + idAgenda + "', TipoOperazioneDB: '" + TipoOperazioneDB.toString() + "'}";
    }

    ajaxAgronica(indirizzohttp + urlAgnd,
        payloadAgnd,
        function (risposta) {
            ChiudiKendoDialog("#pop_up_opAgenda_Preselezione");
            //ModalBootstrapApri(risposta.RispostaStringa, "Op. Agenda");

            let lav_des = "Op. Agenda";
            if (btn !== undefined) {
                lav_des += " - " + btn.value;
            }

            ModalKendoApri(risposta.RispostaStringa, lav_des, PreselezioneAgendaAnnulla);
        }, null);

}

function PreselezioneAgendaAnnulla() {

    if ($("#slide-in-handle-agenda").length > 0) {
        ajaxAgronica(indirizzohttp + "/PreselezioneAgendaAnnulla", null,
            function (risposta) {
            }, null);
    }

}

function chiudiFinestra(id) {
    $(id).dialog("close");
}

//#end region "Operazioni Agenda"

//#region "gestione multipoint"

var A_Multipoint = new Array();
var iLast = 0;

/**
 * Aggiunge o rimuove un punto dal ctrl hidden
 * @param {string} codice
 * @param {string} posizione
 * @param {boolean} selezionato
 */

function addMarkerMultiPointModifica(codice, posizione, selezionato) {

    let hpm = $("#hiddenMultipointModifica").val();
    let arrayAppoggio = [];
    if (hpm !== "") {
        arrayAppoggio = hpm.split("§");
    }

    let elementoArray = codice + '^' + posizione;

    //rimuovo se già presente
    let index = arrayAppoggio.indexOf(elementoArray);
    if (index > -1) {
        arrayAppoggio.splice(index, 1);
    }

    if (selezionato) {
        arrayAppoggio.push(elementoArray);
    }

    $("#hiddenMultipointModifica").val(arrayAppoggio.join("§"));
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

                $("#multipointHeader").removeClass("green");
                $("#selimg_multipoint").removeClass("green");

                //vanni, 20/02/2020: TODO: ORRORE DI CODIFICA !!!!!!!! si testa una stringa ????
                utility.log("SalvaNuovoMultipoint, valore restituito: " + msg.d);
                if (msg.d.split(".")[0] == 'Operazione eseguita correttamente') {
                    ChiudiKendoDialog("#dialogMultipoint");
                    $('#hiddenMultipoint').val('');
                    for (var i = 0; i < A_Multipoint.length; i++) {
                        A_Multipoint[i].setMap(null);
                    }
                    AggiornaTutto(false);
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
//function ricercaIndirizzo() {
//    mappa.ricercaIndirizzo(document.getElementById("address").value);
//}

function CoordDaSelezione() {

    return mappa.CoordDaSelezione(function (lat, lng) {
        $("#find_lat").data("kendoCoordMaskedTextBox").setDecimalValue(lat);
        $("#find_lng").data("kendoCoordMaskedTextBox").setDecimalValue(lng);
    });
}

function ApriGoogleMaps() {

    let coordOK = CoordDaSelezione();

    let gDestinazione = "";
    if (coordOK) {
        let lat = $("#find_lat").data("kendoCoordMaskedTextBox").decimalValue();
        let lng = $("#find_lng").data("kendoCoordMaskedTextBox").decimalValue();
        gDestinazione = lat + "+" + lng;
    } else {
        gDestinazione = $("#address").val();
    }

    if (gDestinazione != "") {
        var goURL = "https://www.google.it/maps/dir//" + gDestinazione + "/";
        window.open(goURL);
    } else {
        //i18n__
        kendoDlgMessage("", "Nessuna coordinata e nessun indirizzo impostato.");
    }
}

function GiasPalmDettagli(piva, sa_cod, id) {
    utility.log("GiasPalmDettagli..");
    mappa.GiasPalmDettagli(piva, sa_cod, id);
}

/////// vavavav

function AnnataAgrariaImpianto() {
    $("#limita_data_da").val(AnnataAgrariaInizio);
    $("#limita_data_a").val(AnnataAgrariaFine);
    $("#limite_inferiore_precedente").click();
    $("#limite_superiore_sucessivos").click();
}

function OggiImpianto() {
    let oggi1 = kendo.toString(kendo.parseDate(new Date()), 'dd/MM/yyyy');
    $("#limita_data_da").val(oggi1);
    $("#limita_data_a").val(oggi1);
    $("#limite_inferiore_precedente").click();
    $("#limite_superiore_sucessivo").click();

}

function FiltroTemporaleReimpostaPartenza() {
    $("#limita_data_da").val(AggiornaDate_Client_gestione_base_gl1);
    $("#limita_data_a").val(AggiornaDate_Client_gestione_base_gl2);
    $("#ddlDateVisualizzaAvanzata_RicercheComuni").data("kendoDropDownList").value(AggiornaDate_Client_gestione_base_cmbVal);
    $("#limite_inferiore_precedente").click();
    $("#limite_superiore_sucessivos").click();
}

function AgganciaEventiPulsanti() {

    $(document).on("click", "#chk_mostra_markerClusters", function () {
        let mostraFlag = $("#chk_mostra_markerClusters").is(':checked');
        mappa.ShowHideMarkerClusters(mostraFlag);
    });

    $(document).on("click", "#chk_mostra_descrizioneImpianto", function () {
        let mostraFlag = $("#chk_mostra_descrizioneImpianto").is(':checked');
        mappa.ShowHideEtichette(mostraFlag);
    });

    $(document).on("click", "#pop_up_sup_app_modifica_chkModificaDatiDiAnagrafica", function () {
        pop_up_sup_app_modifica_chkModificaDatiDiAnagrafica_click();
    });

    $(document).on("click", "#LayerSelezionaTutto", function () {
        clickLayerSelezionaTutto(true);
    });

    $(document).on("click", "#LayerDeSelezionaTutto", function () {
        clickLayerSelezionaTutto(false);
    });

    //$(document).on("click", "#selimg_multipoint", function () {
    //    //vanni, qui mp
    //    if ($("#selimg_multipoint").hasClass("green")) {
    //        ApriKendoDialog("#dialogMultipoint");
    //    } else {
    //        $("#selimg_multipoint").addClass("green");
    //        interfaccia.switchDrawingMode(google.maps.drawing.OverlayType.MARKER);
    //        interfaccia.CreaPoligono();
    //    }
    //});

    $(document).on("click", "#selimg_poligono", function () {
        scegliLayerDoveDisegnare();
    });

    $("#print").click(function () {
        $('#gisMenuStrumenti-menu').hide();
        window.print();
    });

    $('#selimg_EraseSelection').click(function () {
        ClearSelect($(this));
    });
    $('#select_poligono').click(function () {
        ClearSelect($(this));
    });
    $('#disenga_poligono').click(function () {
        ClearSelect($(this));
    });
    $('#delete-button').click(function () {
        ClearSelect($(this));
    });
    $('#delete-buttonHead').click(function () {
        ClearSelect($(this));
    });
    $('#save-button').click(function () {
        ClearSelect($(this));
    });
    $('#save-buttonHead').click(function () {
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
    $('#info_appezzamentoHead').click(function () {
        ClearSelect($(this));
    });

    $("#gestisci_layer_principale").click(function () {
        GestisciLayerPrincipale();
    });
    $("#gestisci_dettagli_layer").click(function () {
        GestisciLayerDettagli();
    });

    $("#ddlDateVisualizzaAvanzata_RicercheComuni").kendoDropDownList({
        change: function (e) {
            let tipologiaSelezionata = this.value();

            switch (tipologiaSelezionata) {
                case "0":
                    $("#visualizza_Tutto").click();
                    break;
                case "1":
                    OggiImpianto();
                    break;
                case "2":
                    AnnataAgrariaImpianto();
                    break;
                case "3":
                    FiltroTemporaleReimpostaPartenza();
                    break;
                default:
            }

        }
    });

    //$("#tipologia_layer").change(function () {

    //    //change della combo sul pannello dei layers
    //    var tipologiaSelezionata = $('#tipologia_layer').val();

    //    if (tipologiaSelezionata === '11' || tipologiaSelezionata === '7' || tipologiaSelezionata === '9') {
    //        TematizzazioneAttivaDisattiva();
    //        TematizzazionePosizionaAuto();
    //    }

    //    var cfgAlbero = JSON.parse($("#" + hdAlberoAnagrafica2017cfg_ClientID).val());
    //    cfgAlbero.TipologiaLayer_Cod = tipologiaSelezionata;

    //    $("#" + hdAlberoAnagrafica2017cfg_ClientID).val(JSON.stringify(cfgAlbero));


    //    ColorazioneAutomatica(tipologiaSelezionata);
    //    AggiornaElencoTipologie();

    //    let date = new Date();
    //    date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
    //    $.cookie("GIS.TipologiaLayer", tipologiaSelezionata, { expires: date });

    //});

    $("#tipologia_layer").css("width", "100%");
    $("#tipologia_layer").kendoDropDownList({
        change: function (e) {

            //change della combo sul pannello dei layers
            let tipologiaSelezionata = this.value();

            if (tipologiaSelezionata === '11' || tipologiaSelezionata === '7' || tipologiaSelezionata === '9') {
                TematizzazioneAttivaDisattiva();
                TematizzazionePosizionaAuto();
            }

            let cfgAlbero = JSON.parse($("#" + hdAlberoAnagrafica2017cfg_ClientID).val());
            cfgAlbero.TipologiaLayer_Cod = tipologiaSelezionata;

            $("#" + hdAlberoAnagrafica2017cfg_ClientID).val(JSON.stringify(cfgAlbero));

            ColorazioneAutomatica(tipologiaSelezionata);
            AggiornaElencoTipologie(tipologiaSelezionata);

            SalvaTipologiaLayerInCookie();
        }
    });

    $("#tipologia_layer").siblings(".k-dropdown-wrap").css("border-top-right-radius", "0px")
    $("#tipologia_layer").siblings(".k-dropdown-wrap").css("border-bottom-right-radius", "0px")

    $("#nuova_agenda").click(function () {
        apriPreselezioneAgenda();
    });

    $("#nuova_agendaHeader").click(function () {
        apriPreselezioneAgenda();
    });

    $("#nuova_agendaHeader_lg").click(function () {
        apriPreselezioneAgenda();
    });

    $("#modifica_agendaHeader_lg").click(function () {
        preselezioneAgenda(-1);
    });

    $("#nuova_visita").click(function () {
        apriPreselezioneVisita();
    });

    $("#nuova_visitaHeader").click(function () {
        apriPreselezioneVisita();
    });

    $("#nuova_visitaHeader_lg").click(function () {
        apriPreselezioneVisita();
    });

    $("#ImpostaImpresaDaSelezione").click(function () {
        ImpostaImpresaDaSelezione();
    });

    $("#ImpostaImpresaDaSelezioneHeader").click(function () {
        ImpostaImpresaDaSelezione();
    });

    $("#ritaglio-sfondo").click(function () {
        RitagliaSfondo();
    });

    $("#import-button").click(function () {

        var url = "CaricaShape.aspx";
        if (CiSonoVecchiDatiNonImportati) {
            url = url + "?isFromAlert=1";
        }

        //ModalKendoApri(url, "Importazione Dati GIS");
        var winWidth = Math.round(window.outerWidth * (ImportazioneDatiGisPercWidth / 100));
        if (winWidth < ImportazioneDatiGisMinWidth) {
            winWidth = ImportazioneDatiGisMinWidth;
        } 
        var leftPos = window.outerWidth - winWidth - ImportazioneDatiGisRight;
        KendoWindowGenericApri(url, "Importazione Dati GIS", ImportazioneDatiGISClose, winWidth.toString() + "px", ImportazioneDatiGisHeight, leftPos.toString() + "px", ImportazioneDatiGisTop);
    });

    function ImportazioneDatiGISClose() {

    }

    $("#export-button").click(function () {

        var Entita_Cod = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString();
        var ePiva = "";
        var eSa_cod = "-1";
        var eApp = Entita_Cod.split("§");

        if (eApp[0] === "3") {

            ePiva = eApp[1];
            eSa_cod = eApp[2];

        }

        //i18n__
        var urlToOpen = "EsportaPF.aspx?piva=" + ePiva + "&sa_cod=" + eSa_cod;
        ModalKendoApri(urlToOpen, "Esportazione Dati GIS");

    });

    $("#ripartoCatasto-button").click(function () {
        catasto();
    });

    $("#PopupNuovaImpresa-Button").click(function () {
        $('#popup_nuova_azienda').data("kendoDialog").open();
    });

    $("#PopupNuovoSa-Button").click(function () {
        $('#popup_nuovo_centro').data("kendoDialog").open();
    });

    $("#nuovapiva-button").click(function () {
        nuovaPivaButtonClick()
    });

    $("#AnalisiMeteo").click(function () {
        dialogAnalisiMeteo();
    });

    $("#AnalisiModelli").click(function () {
        dialogAnalisiModelli();
    });

    $("#AnalisiRilievi").click(function () {
        dialogAnalisiRilievi();
    });

    $("#AnalisiDatiReteAcqua").click(function () {
        dialogAnalisiDatiReteAcqua();
    });

    $("#ApriAnalisi").click(function () {
        dialogApriAnalisi();
    });

    $("#BufferZone").click(function () {
        dialogBufferZoneIntersection();
    });


    $("#CopiaOggettoCopia").click(function () {
        copiaIncolla_Copia();
    });

    $("#CopiaOggettoIncolla").click(function () {
        copiaIncolla_Incolla();
    });

    $("#nuova_ricetta").click(function () {
        precision.clickRicetta(true);
    });

    $("#DialogSr").click(function () {
        DialogSr();
    });
}

//GABRIELE 11 04 2019
function scegliLayerDoveDisegnare() {

    interfaccia.switchDrawingMode(google.maps.drawing.OverlayType.POLYGON);
    controllaSePossoDisegnare();
    return;

    let opts_name = "scelta_layer";
    let opts = [];
    opts.push({ id: "1", text: "Impianto/Appezzamento" });
    opts.push({ id: "33", text: "Impianto pianificato" });

    let content = "<div style='margin-top:20px;'>";
    content += "<ul style='list-style:none;'>";
    for (o = 0; o < opts.length; o++) {
        content += "<li style='padding-bottom:20px;'>"
        content += "<input type='radio' name='" + opts_name + "' id='layer_" + opts[o].id + "' value = '" + opts[o].id + "' class='k-radio'";
        if (o === 0) {
            content += " checked='checked'";
        }
        content += ">";
        content += "<label class='k-radio-label' for='layer_" + opts[o].id + "'>" + opts[o].text + "</label>";
        content += "</li>"
    }
    content += "</ul>";
    content += "</div>";

    let win_el = document.createElement("div");
    win_el.id = "id_tmp_kendo_dlg";
    document.body.appendChild(win_el);
    let $win_el = $("#id_tmp_kendo_dlg");

    var msgOk = Traduzione(AgronicaControlliGisResx, "jsLblok");
    var msgAnnulla = Traduzione(AgronicaControlliGisResx, "jsLblAnnulla");
    var lblTitoloDisegnaP = Traduzione(AgronicaControlliGisResx, "jsLblDisegnaNuovoPoligono");

    $win_el.kendoDialog({
        title: lblTitoloDisegnaP,
        closable: false,
        modal: true,
        visible: false,
        content: content,
        actions: [
            {
                text: msgOk,
                action: function () {

                    let sel_layer = $("input[name='" + opts_name + "']:checked").val();

                    if (sel_layer === "33") {

                        let chiave_albero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val();
                        if (chiave_albero === "") {
                            //i18n__
                            kendoDlgMessage("Nuovo impianto pianificato", "Selezionare un planning dall'anagrafica...");

                            return false;
                        }
                    }

                    shape.glayerDoveDisegno = sel_layer;
                    mappa.drawingManager.setDrawingMode(google.maps.drawing.OverlayType.POLYGON);

                    return true;
                }
            },
            { text: msgAnnulla }
        ],
        close: function (e) {
            this.destroy();
        }
    });

    $win_el.data("kendoDialog").open();
}

//GABRIELE 08 04 2019
function controllaSePossoDisegnare() {

    if (GisPurpose === Enum_GisPurpose.SementiSportello) {

        //Se lo sportello è in sola lettura???
        let risp = GisAjaxSync("SementiSportelloPossoDisegnare", {});
        if (risp !== "") {
            //i18n__
            kendoDlgMessage("Nuovo poligono", risp);
            mappa.drawingManager.setDrawingMode(null);

            return;
        }

    }
    if (shape.glayerDoveDisegnoSuTipologiaStandard === "3") {
        //Catasto...
        if (!PermessiGisServerSide.bool_catasto) {
            //i18n__
            kendoDlgMessage("Nuova particella catastale", "Non si dispone dei permessi di gestione del catasto GIS.");

            mappa.drawingManager.setDrawingMode(null);

            return;
        }
        let chiave_albero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val();
        if (chiave_albero === "") {
            kendoDlgMessage("Nuova particella catastale", "Selezionare una particella catastale dall'anagrafica.");

            mappa.drawingManager.setDrawingMode(null);

            return;
        }


    }
    if (shape.glayerDoveDisegnoSuTipologiaStandard === "33") {

        //Planning...
        let chiave_albero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val();
        if (chiave_albero === "") {
            //i18n__
            kendoDlgMessage("Nuovo impianto pianificato", "Selezionare un planning dall'anagrafica.");

            mappa.drawingManager.setDrawingMode(null);

        }
    }
}

function TematizzazionePosizionaAuto() {
    //$("#scala_colori_contenitore").data("kendoWindow").setOptions({
    //    width: width
    //});

    var height = 90;//118;
    $("#scala_colori_contenitore").data("kendoWindow").setOptions({
        height: height
    });

    //var top = window.innerHeight - parseInt($("#scala_colori_contenitore").data("kendoWindow").options.height.replace("px", "")) + 50;

    //var mytop = $("#mapBS").position().top + $("#mapBS").height();
    var mytop = $("#gisMappa").position().top + $("#gisMappa").height() - height;
    var left = 3;

    $("#scala_colori_contenitore").data("kendoWindow").setOptions({
        position: {
            top: mytop, // or "100px"
            left: left
        }
    });
}

function nuovaPivaButtonClick() {
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
}

function AgganciaEventiControlli() {

    //i18n__

    $("#PfDdlSensoreElaborazione").kendoDropDownList({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: [],
        optionLabel: {
            text: "Seleziona un sensore...",
            value: "Sel"
        },
        change: PfDdlSensoreElaborazione_Change
    });

    $("#WmsDdlSensoreElaborazione").kendoDropDownList({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: [{ text: "Catasto - Agenzia Entrate (inspire)", value: "Wms - Catasto" }],
        optionLabel: {
            text: "Seleziona un livello...",
            value: "Sel"
        },
        change: wmsDdlSensoreElaborazione_Change
    });

    $("#cmbAssocia_Modifica_GenerazionePoligoni").on("change", function () {
        cmbAssocia_Modifica_GenerazionePoligoni_Change();
    });

    $("#ElaboraMappaDettagliataSuSelezione").click(function () {
        ElaboraMappaDettagliataSuSelezione_Click();
    });

    $('#pop_up_impianto_azienda').change(function () {
        CaricaCentroAziendale();
        CaricaComboOrganismoReferente("#pop_up_impianto_azienda", "#pop_up_app_OrganismoReferente");
    });

    $('#pop_up_centro').change(function () {
        CaricaCampo();
    });

    $('#pop_up_specie').change(function () {
        response_ok = 4;
        if (GisPurpose === Enum_GisPurpose.Completo) {
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


    $("#cmbImpiantiFigli_Modifica").change(function () {

        cmbImpiantiFigli_Modifica_Change();

    });

}

/**
 * Aggancio eventi e gestione del completamento del poligono
 */

/*
var cancelDrawingShape = false;

google.maps.event.addListener(drawingManager, 'overlaycomplete', function (e) {
    var lastDrawnShape = e.overlay;
    if (cancelDrawingShape) {
        cancelDrawingShape = false;
        lastDrawnShape.setMap(null); // Remove drawn but unwanted shape
        return;
    }

    // Else, do other stuff with lastDrawnShape
});

$(document).keydown(function (event) {
    if (event.keyCode === 27) { // Escape key pressed
        cancelDrawingShape = true;
        drawingManager.setDrawingMode(null); // To terminate the drawing, will result in autoclosing of the shape being drawn.
    }
});
*/

function AgganciaEventiGoogle() {

    //google.maps.event.addDomListener(document, 'keyup', function (e) {

    //    let code = (e.keyCode ? e.keyCode : e.which);

    //    if (code === 27) { // Escape key pressed

    //        console.log("keyup event");
    //        //drawingManager.setDrawingMode(null);
    //    }
    //});


    //*********************************************************************************************
    //GABRIELE 20 11 2020 DrawingManager
    //*********************************************************************************************
    //google.maps.event.addListener(mappa.drawingManager, 'overlaycomplete', function (e) {

    //    utility.log("overlaycomplete");

    //    var newShape = e.overlay;
    //    newShape.type = e.type;

    //    if (e.type != google.maps.drawing.OverlayType.MARKER) {

    //        mappa.drawingManager.setDrawingMode(null);
    //        google.maps.event.addListener(newShape, 'click', function () {
    //            setSelection(newShape);
    //        });

    //        //GABRIELE 02 04 2019
    //        //setSelection(newShape);
    //        if (shape.glayerDoveDisegnoSuTipologiaStandard !== "-1" && shape.glayerDoveDisegnoSuTipologiaStandard !== undefined) {

    //            //GABRIELE 02 04 2019
    //            shape.selectedShape = newShape;

    //            preparaXSalvataggioPoligoni();

    //        } else {

    //            let msgWarningTitolo = Traduzione(AgronicaControlliGisResx, "jsLblAttenzione");
    //            let msgWarningTesto = Traduzione(AgronicaControlliGisResx, "jsMsgNonEStatoSelezionatoAlcunLayer");
    //            kendoDlgMessage(msgWarningTitolo, msgWarningTesto);

    //            //kendoDlgMessage("Attenzione...", "<p>Non è stato selezionato alcun layer dove disegnare.</p><p>Selezionare un layer facendo click sulla relativa icona nell elenco dei layer.</p>");

    //            if (newShape != undefined) {
    //                newShape.setMap(null);
    //            }
    //            clearSelectionBoth();

    //        }

    //    } else {

    //        utility.log("addMarkerMultiPoint");
    //        addMarkerMultiPoint($("#chkMP"), $("#hiddenMultipoint"), null, newShape.getPosition());

    //    }


    //});

    let layers = [
        { text: "Impianti", value: 19 }
    ];

    if (GisPurpose !== Enum_GisPurpose.SementiSportello && GisPurpose !== Enum_GisPurpose.SementiMappaturaLibera) {

        layers.push({ text: "Appezzamenti", value: 1 });
        layers.push({ text: "Impianti pianificati", value: 33 });
        layers.push({ text: "Aree omogenee", value: 8 });
        layers.push({ text: "Ettari equivalenti", value: 12 });
        layers.push({ text: "Fasce di rispetto", value: 86 });
    }

    GlobalAgroDrawing = new AgroDrawing(mappa, layers,
        function (newShape, layer) {
            google.maps.event.addListener(newShape, 'click', function () {
                setSelection(newShape);
            });

            shape.glayerDoveDisegnoSuTipologiaStandard = "" + layer + "";

            shape.selectedShape = newShape;
            preparaXSalvataggioPoligoni();
        },
        mappa_click_event
    );
    //*********************************************************************************************
    //*********************************************************************************************
    //*********************************************************************************************


    //Eventi Google Maps
    // Clear the current selection when the drawing mode is changed, or when the
    // map is clicked.

    //GABRIELE 02 04 2019
    //google.maps.event.addListener(mappa.drawingManager, 'drawingmode_changed', clearSelectionBoth);

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
    google.maps.event.addListener(mappa.elemenotMappa, 'click', mappa_click_event);

    google.maps.event.addDomListener(document.getElementById('delete-button'), 'click', deleteSelectedShape);
    google.maps.event.addDomListener(document.getElementById('delete-buttonHeader'), 'click', deleteSelectedShape);

    google.maps.event.addDomListener(document.getElementById('save-button'), 'click', function () {
        preparaXSalvataggioPoligoni(false);
    });

    google.maps.event.addDomListener(document.getElementById('save-plus'), 'click', function () {
        preparaXSalvataggioPoligoni(true);
    });

    google.maps.event.addDomListener(document.getElementById('save-buttonHeader'), 'click', function () {
        preparaXSalvataggioPoligoni(false);
    });

    var impi = Traduzione(AgronicaControlliGisResx, "jsLblImpianti");

    //*********************************************************************************************
    // GABRIELE 20 11 2020 DrawingManager
    //*********************************************************************************************
    ////GABRIELE 03 04 2019
    ////google.maps.event.addDomListener(document.getElementById('disenga_poligono'), 'click', CreaPoligono);
    ////google.maps.event.addDomListener(document.getElementById('disenga_poligono'), 'click', function () {
    //google.maps.event.addDomListener(document.getElementById('selimg_poligono'), 'click', function () {
    //    if (shape.glayerDoveDisegnoSuTipologiaStandard == "-1") {            
    //        shape.settaLayerDoveDisegnare(undefined, undefined, undefined, "19", "Impianto32.png", impi);
    //    }
    //});
    //*********************************************************************************************

    //google.maps.event.addDomListener(document.getElementById('selimg_multipoint'), 'click', function () {
    //    if (shape.glayerDoveDisegnoSuTipologiaStandard == "-1") {
    //        shape.settaLayerDoveDisegnare(undefined, undefined, undefined, "19", "Impianto32.png", impi);
    //    }
    //});
    google.maps.event.addDomListener(document.getElementById('selimg_multipoint'), 'click', function () {
        mappaturaSmart();
    });

    google.maps.event.addDomListener(document.getElementById('copia-oggettoCopia'), 'click', function () {
        copiaIncolla_Copia();
    });

    google.maps.event.addDomListener(document.getElementById('copia-oggettoIncolla'), 'click', function () {
        copiaIncolla_Incolla();
    });

    google.maps.event.addDomListener(document.getElementById('caricaDatiPFXML'), 'click', function () {
        caricaDatiPFXML();
    });

    google.maps.event.addDomListener(document.getElementById('disenga_poligonoHeader'), 'click', CreaPoligonoHeader);

    //GABRIELE
    //google.maps.event.addDomListener(document.getElementById('multipointHeader'), 'click', multipointHeader);
    google.maps.event.addDomListener(document.getElementById('multipointHeader'), 'click', function () {
        mappaturaSmart();
    });

    //seleziono un poligono
    google.maps.event.addDomListener(document.getElementById('select_poligono'), 'click', SelezionaPoligono);
    google.maps.event.addDomListener(document.getElementById('selimg_EraseSelection'), 'click', clearSelectionBoth);

    google.maps.event.addDomListener(document.getElementById('info_appezzamento'), 'click', Info_Poligono);
    google.maps.event.addDomListener(document.getElementById('info_appezzamentoHeader'), 'click', Info_Poligono);
    google.maps.event.addDomListener(document.getElementById('StrumentoGPS'), 'click', StrumentoGPS);
    google.maps.event.addDomListener(document.getElementById('StrumentoGPSHeader'), 'click', StrumentoGPS);

    google.maps.event.addDomListener(document.getElementById('PianoRateoVariabile'), 'click', dialogRateo);
    google.maps.event.addDomListener(document.getElementById('GeneraPlanning'), 'click', dialogGeneraPlanning);

}

function ImpostaImpresaDaSelezione() {

    if (shape !== undefined)
        if (shape.selectedShape !== undefined)
            if (shape.selectedShape.chiavealbero !== undefined) {

                var piva = shape.selectedShape.chiavealbero.split("§")[1];

                if (piva !== "") {
                    $("#ddl_azienda_html").data("kendoDropDownList").value(piva);
                    CambiaSelezione("ddl_azienda_html", piva);
                    AggiornaFiltro_Client_click();
                }

            }


}

function GisAjaxSync(url, objData) {

    let retVal = "";

    $.ajax({
        async: false,
        type: "POST",
        url: indirizzohttp + "/" + url,
        data: JSON.stringify(objData),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                retVal = msg.d;
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });

    return retVal;
}

function GisAjax(url, objData, callback) {

    $.ajax({
        async: false,
        type: "POST",
        url: indirizzohttp + "/" + url,
        data: JSON.stringify(objData),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                if (typeof callback === "function") {
                    callback(msg.d);
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}