

/*******************************************************************/
/********************* DOCUMENT READY ******************************/
/*******************************************************************/



//(function ($) {
//    var CoordMaskedTextBox = kendo.ui.MaskedTextBox.extend({
//        options: {
//            name: "CoordMaskedTextBox",
//            coord: "",
//            formato: ""
//        },
//        init: function (element, options) {

//            options.mask = "~";
//            options.rules = {
//                "~": /[+-]/
//            };

//            if (typeof options.formato !== "string") {
//                options.formato = "DEC"; //"GMS"
//            }
                                                                            
//            // The base call to the widget initialization.
//            kendo.ui.MaskedTextBox.fn.init.call(this, element, options);

//            this._gestisciFormato(this.options.formato);

//            $(element).keypress(function (e) {
//                let tb = $(this).data("kendoCoordMaskedTextBox");
//                let raw = tb.raw();
//                if (raw === "" || this.selectionStart === 0) {

//                    if ('0' <= e.key && e.key <= '9') {

//                        tb.value("+" + e.key);

//                        e.preventDefault();
//                    }
//                }
//            });

//        },
//        _gestisciFormato: function (formato) {

//            let mask = "~00°00'00\"";
//            let placeholder = "±GG°MM'SS\"";
//            if (formato === "DEC") {
//                mask = "~00.0000000°";
//                placeholder = "±GG.GGGGGGG°";
//            }

//            this.element.attr("placeholder", this.options.coord + " " + placeholder);
//            this.setOptions({ mask: mask, formato: formato });
//        },
//        _gestisciValue: function (decVal, formato) {

//            if (isNaN(decVal)) {
//                return "";
//            }

//            let val = ""
//            if (formato === "DEC") {

//                val = kendo.toString(decVal, "00.0000000000");

//            } else {

//                let max_deg = 90;
//                if (this.options.coord === "Lng") {
//                    max_deg = 180;
//                }

//                let sign = decVal < 0 ? -1 : 1;
//                let absVal = Math.abs(Math.round(decVal * 1000000));
//                if (absVal <= (max_deg * 1000000)) {

//                    let dec = absVal % 1000000 / 1000000;
//                    let deg = Math.floor(absVal / 1000000) * sign;
//                    let min = Math.floor(dec * 60);
//                    let sec = (dec - min / 60) * 3600;

//                    val = kendo.toString(deg, "00") + "" + kendo.toString(min, "00") + "" + kendo.toString(sec, "00");
//                }
//            }

//            if (val.length > 0 && val.charAt(0) !== "-") {
//                val = "+" + val;
//            }

//            return val;
//        },
//        switchFormato: function () {

//            let formato = "GMS";
//            if (formato === this.options.formato) {
//                formato = "DEC";
//            }

//            let val = this._gestisciValue(this.decimalValue(), formato);

//            this._gestisciFormato(formato);

//            this.value(val);
//        },
//        decimalValue: function () {

//            let decVal = Number.NaN;

//            if (this.options.formato === "DEC") {

//                let val = this.value();
//                if (val.length > 0) {
//                    decVal = parseFloat(val.replace("°", "").replace(this.options.promptChar, "").replace(",", "."));
//                }

//            } else {

//                let raw = this.raw();
//                if (raw.length === 7) { //+GGMMSS

//                    let deg = parseFloat(raw.slice(0, 3));
//                    let min = parseFloat(raw.slice(3, 5));
//                    let sec = parseFloat(raw.slice(5, 7));

//                    if (!isNaN(deg) && !isNaN(min) && !isNaN(sec)) {

//                        let sign = deg < 0 ? -1 : 1;
//                        let abs = Math.abs(deg);
//                        decVal = sign * (abs + (min / 60.0) + (sec / 3600));
//                    }
//                }
//            }

//            return decVal;
//        },
//        setDecimalValue: function (decVal) {

//            this.value(this._gestisciValue(decVal, this.options.formato));
//        }
//    });
//    kendo.ui.plugin(CoordMaskedTextBox);
//})(jQuery);



$(document).ready(function () {

    TraduzioniDaServer();

    //    $("#arrow").on("click", function () {
    $(document).on("click", "#arrow", function () {
        $('#pop_up_sup_app_modifica').val($("#pop_up_sup_google_modifica").val());
    });

    $(document).on("click", "#arrowBufferZone", function () {
        $('#txtSupBZ_Riduzione').val($("#txtSupBZ_Riduzione_Ricalcolata").html());
    });

    InizializzaMenuCosaDisegno();    

    var p = $("#map").offset();
    h = ($("#map").height() + p.top);
    w = ($("#map").width() + p.left);

    maxH = $("#map").height();

    InizializzaDatepicker();

    if (GisPurpose !== Enum_GisPurpose.SementiSportello) {
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


    $("#tipoGrado").kendoDropDownList({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: [
            { text: "Gradi Dec.", value: "DG" },
            { text: "Gradi M.S.", value: "TG" }
        ],
        index: 0,
        change: function () {
            let tipo = $("#tipoGrado").val();
            if (tipo == "TG") {
                $("#decimalDegree").hide();
                $("#TimeMinuteDegree").show();
            } else {
                $("#TimeMinuteDegree").hide();
                $("#decimalDegree").show();
            }
        }
    });

    $("#find_lat").kendoCoordMaskedTextBox({ coord: "Lat" });
    $("#find_lng").kendoCoordMaskedTextBox({ coord: "Lng" });

    $("#formatoCoord").on("click", function () {
        $("#find_lat").data("kendoCoordMaskedTextBox").switchFormato();
        $("#find_lng").data("kendoCoordMaskedTextBox").switchFormato();
    });

    $("#findCoord").on("click", function () {

        let lat = $("#find_lat").data("kendoCoordMaskedTextBox").decimalValue();
        let lng = $("#find_lng").data("kendoCoordMaskedTextBox").decimalValue();

        if (!Number.isNaN(lat) && !Number.isNaN(lng)) {

            mappa.CercaCoordinate(lat, lng);

        } else {
            //i18n__
            kendoDlgMessage("", "I campi Latitudine o Longitudine non contengono valori corretti");
        }
   });

    $("#indirizzoTB .findAddress").on("click", function (e) {
        let straddr = $("#address").val();
        if (straddr.length > 0) {
            mappa.ricercaIndirizzo(straddr, function (lat, lng) {

                $("#find_lat").data("kendoCoordMaskedTextBox").setDecimalValue(lat);
                $("#find_lng").data("kendoCoordMaskedTextBox").setDecimalValue(lng);

            });
        }
    });

    initialize();    

    VerificaLayerConfigurati();

    PosizionaRicercaIndirizzo();

    AgganciaDialogWindows();

    ConfiguraMenuXS();

    $("#menuHeaderSel").click(function () {

        ConfiguraMenuXS();
    });

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
                    AggiornaFiltro_Client_click();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                interfaccia.loading(false);
                alert(xhr.status);
                alert(thrownError);
            }
        });
    });


    let lat = Request_QueryString("Lat");
    let lng = Request_QueryString("Lng");
    if (lat !== null && lng !== null && lat !== "" && lng !== "") {
        let latlng = new google.maps.LatLng(parseFloat(lat.replace(",", ".")), parseFloat(lng.replace(",", ".")))
        mappa.elemenotMappa.setCenter(latlng);
        mappa.elemenotMappa.setZoom(18);

        let marker = new google.maps.Marker({
            position: latlng,
            map: mappa.elemenotMappa
        });

    }
});


/* Funzioni di appoggio su AgganciaDialogWindows */

//GABRIELE 25 03 2019
/*
function popUpImpiantoAvanti() {
    return InviaDatiNuovoImpianto();
}

function popUpImpiantoAvantiClose() {

    //elimino lo shape
    if (shape.selectedShape != undefined)
        shape.selectedShape.setMap(null);

}
*/

//GABRIELE 25 03 2019
/*
function popUpImpiantoAvantiOpen() {

    var pa = $("#panelBarDatiAzienda").data("kendoPanelBar");

    if (pa === undefined) {
        $("#panelBarDatiAzienda").kendoPanelBar({
            expandMode: "multiple"
        }).data("kendoPanelBar");

    }
    

}
*/

//GABRIELE 02 04 2019
/*
function dialogConfermaAssociazioneAvanti() {

    ModificaImpianto(Enum_TipoModifica.SalvataggioDiretto);

}
*/

function dialogAllertOperazioniAvantiCommon() {

    var area = gMapsUtility.getArea(CoordFromPoints, CoordFromPointsMVCArray, shape.selectedShape);

    if ($('#dialogAllertOperazioni_hidden').val() == 1) {
        area = salvataggioDirettoConAppezzaAreaCalcola(area);
        salvataggioDiretto(area);
    }
    else {
        ModificaDatiImpianto();        
        ChiudiKendoDialog("#pop_up_modificaImpianto");        
    }
}

function dialogAllertOperazioniAvanti() {
    
    dialogAllertOperazioniAvantiCommon();   

    ChiudiKendoDialog("#dialogAllertOperazioni");

}

function dialogAllertOperazioniClose() {    

    //GABRIELE 2019 07 26 per evitare unzoom da testare...
    //AggiornaTutto(true);
    AggiornaTutto(false);
    
}

function dialogAllertOperazioniAppezzaClose() {

    //GABRIELE 2019 07 26 per evitare unzoom dopo la modifica impianto
    //AggiornaTutto(true);
    AggiornaTutto(false);

}

function dialogAllertOperazioniAppezzaAvanti() {
   
    dialogAllertOperazioniAvantiCommon();

    ChiudiKendoDialog("#dialogAllertOperazioniAppezza");

}

/* GABRIELE 25 03 2019
function ModificaImpiantoAvanti() {
    ModificaImpianto(Enum_TipoModifica.ModificaImpianto);
    return false;
}
*/

function DialogEliminaImpiantoAvanti() {
    EliminaImpianto();
}

function DialogEliminaImpiantoOpen() {
    dialogEliminaImpiantoOnOpen();
}

function dialogEliminaMultipointAvanti() {
    EliminaMultipointSelezionati();
}

function GoogleBoundsWKT() {

}

function RitagliaSfondo() {    
    
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
        //i18n__
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
 
}

/* Nuova Azienda */
function popupNuovaAziendaAvanti() {
    InviaDatiNuovaAzienda();
}

function popupNuovaAziendaClose() {

}

function popupNuovaAziendaOpen() {
    ImpostaAziendaPadreDefault();
}


/* Nuovo Centro */
function popupNuovoCentroAvanti() {
    InviaDatiNuovoCentro();
}

function popupNuovoCentroClose() {

}

function popupNuovoCentroOpen() {    
}

function popUpModificaImpiantoOpen() {
    var pa = $("#panelBarDatiModifica").data("kendoPanelBar");

    if (pa === undefined) {
        $("#panelBarDatiModifica").kendoPanelBar({
            expandMode: "multiple"
        }).data("kendoPanelBar");

    }
}

function ScalaColoriContenitoreDisponi() {

    //al momento non ho a disposizone un evento adeguato .. quindi surrogo col solito metodo del timeOut...    
    setTimeout(ScalaColoriContenitoreDisponiAttiva, 1000);
    
}

function ScalaColoriContenitoreDisponiAttiva() {
    interfaccia.CreaPannelloColore_Disponi(0);
}

function DialogGestioneColoriLayerAvanti() {
    ConfermaGestioneColoriLayer();
}

function dialogMultipointAvanti() {
    ConfermaSalvataggioMultipoint();
}


function RateoVariabile_Open() {

    if ($("#wmsToolsNavBar").is(":visible")) {
        $("#divlblDialogRateo_dataSentinel").show();
        let kendoCal = $("#pf_data").data("kendoCalendar").value();
        if ($("#txtDialogRateo_dataSentinel").val() === "") {
            $("#txtDialogRateo_dataSentinel").data("kendoDatePicker").value(kendoCal);
        }
        
    } else {
        $("#divlblDialogRateo_dataSentinel").hide();
    }
}
function RateoVariabileConferma() {
    precision.pfRateo(true);
}


function satPfTools_close() {
    WinGestioneAperturaPosizionamento("#miniWindowEditToolbar", miniWindowEditToolbar_width, miniWindowEditToolbar_offSet_Closed, miniWindowEditToolbar_height);
}

function satPfTools_open() {
    WinGestioneAperturaPosizionamento("#miniWindowEditToolbar", miniWindowEditToolbar_width, miniWindowEditToolbar_offSet_Open, miniWindowEditToolbar_height);
}

function FinestraTemporale_Tool_close() {
    AggiornaDate_Client_ReimpostaPrecedenti();
}

function FinestraTemporale_Tool_open() {
    AggiornaDate_Client_MemorizzaPrecedenti();
}

function FinestraTemporale_Tool_conferma() {
    AggiornaDate_Client_gestione();
    $("#AggiornaFiltro_Client").click();
}
/* Fine Funzioni di appoggio su AgganciaDialogWindows */



function AgganciaDialogWindows() {

    //GABRIELE 25 03 2019 gestito display: none nel foglio di stile
    //GeneraKendoDialogMaps("#pop_up_impianto", "Nuovo impianto colturale", popUpImpiantoAvanti, popUpImpiantoAvantiClose, popUpImpiantoAvantiOpen, undefined);
    //GeneraKendoDialogMaps("#pop_up_modificaImpianto", "Modifica Impianto", ModificaImpiantoAvanti, undefined,popUpModificaImpiantoOpen, undefined);
    //GABRIELE 02 04 2019
    //GeneraKendoDialogMaps("#dialogConfermaAssociazione", "Associazione", dialogConfermaAssociazioneAvanti, undefined, undefined, undefined);    

    GeneraKendoDialogMaps("#dialogAllertOperazioni", Traduzione(AgronicaControlliGisResx, "jsLblOperazioni"), dialogAllertOperazioniAvanti, dialogAllertOperazioniClose, undefined, undefined);
    GeneraKendoDialogMaps("#dialogAllertOperazioniAppezza", Traduzione(AgronicaControlliGisResx, "jsLblOperazioni"), dialogAllertOperazioniAppezzaAvanti, dialogAllertOperazioniAppezzaClose, undefined, undefined );

    GeneraKendoWindowMaps("#contenitore_tool", "Layer", "60%", "95%");

    //GeneraKendoWindowMaps("#FinestraTemporale_Tool", "Filtro temporale", "60%", "95%", undefined, FinestraTemporale_Tool_close, FinestraTemporale_Tool_open, undefined, ["close"]);
    let actionsFinestraTemporaleTool = [
        { text: "Applica Filtro", action: FinestraTemporale_Tool_conferma },
        { text: "Annulla", action: FinestraTemporale_Tool_close }
    ];
    GeneraKendoDialogMaps("#FinestraTemporale_Tool", Traduzione(AgronicaControlliGisResx, "jsLblFiltroTemporale"), undefined, undefined, FinestraTemporale_Tool_open, "60%", "nessuno",actionsFinestraTemporaleTool);

    GeneraKendoWindowMaps("#satPfTools", Traduzione(AgronicaControlliGisResx, "jsLblAnalisiDatiSatellitari"), "60%", "95%", undefined, satPfTools_close, satPfTools_open);

    GeneraKendoWindowMaps("#miniWindowEditToolbar", Traduzione(AgronicaControlliGisResx, "jsLblDisegna"), "60%", "95%", undefined, undefined, undefined, undefined, ["Close"]);

    GeneraKendoWindowMaps("#wmsTools", Traduzione(AgronicaControlliGisResx, "jsLblGestioneWMS"), "60%", "45%");

    GeneraKendoWindowMaps("#kendoWindowiFrameGeneric", Traduzione(AgronicaControlliGisResx, "jsLblGeneric"), "60%", "95%");

    GeneraKendoWindowMaps("#scala_colori_contenitore", Traduzione(AgronicaControlliGisResx, "jsLblTemi"), "60%", "20%", undefined, undefined, ScalaColoriContenitoreDisponi, ScalaColoriContenitoreDisponi);

    GeneraKendoDialogMaps("#dialogEliminaImpianto", Traduzione(AgronicaControlliGisResx, "jsLblEliminaImpianto"), DialogEliminaImpiantoAvanti, undefined, DialogEliminaImpiantoOpen, undefined);
    GeneraKendoDialogMaps("#dialogEliminaMultipoint", Traduzione(AgronicaControlliGisResx, "jsLblElimina"), dialogEliminaMultipointAvanti, undefined, undefined, undefined);

    GeneraKendoDialogMaps("#popup_nuova_azienda", Traduzione(AgronicaControlliGisResx, "jsLblNuovaAzienda"), popupNuovaAziendaAvanti, popupNuovaAziendaClose, popupNuovaAziendaOpen, undefined );
    GeneraKendoDialogMaps("#popup_nuovo_centro", Traduzione(AgronicaControlliGisResx, "jsLblNuovoCentroAziendale"), popupNuovoCentroAvanti, popupNuovoCentroClose, popupNuovoCentroOpen, undefined  );

    GeneraKendoDialogMaps("#dialogGestioneColoriLayer", Traduzione(AgronicaControlliGisResx, "jsLblGestioneColoriLayer"), DialogGestioneColoriLayerAvanti);

    GeneraKendoDialogMaps("#dialogMultipoint", Traduzione(AgronicaControlliGisResx, "jsLblMultipoint"), dialogMultipointAvanti, undefined, undefined, undefined);        

    GeneraKendoDialogMaps("#dialogBufferZoneIntersection", Traduzione(AgronicaControlliGisResx, "jsLblBufferZone"), ConfermaBufferZoneIntersection, undefined, dialogBufferZoneIntersection_Open, undefined, "Salva Su Appezzamento");

    GeneraKendoDialogMaps("#dialogAB", Traduzione(AgronicaControlliGisResx, "jsLblNuovaLineaGuidaAB"), ConfermaSalvataggioAB);
    GeneraKendoDialogMaps("#dialogRateo", Traduzione(AgronicaControlliGisResx, "jsLblGeneraUnaMappaPrescrizioneRateoVariabile"), RateoVariabileConferma, undefined, RateoVariabile_Open);

    GeneraKendoDialogMaps("#pop_up_opAgenda_Preselezione", Traduzione(AgronicaControlliGisResx, "jsLblSelezionaOperazioneAgenda"), undefined, undefined, undefined, undefined, "nessuno");

    //gestiti da modale generica...
    //GeneraKendoDialogMaps("#pop_up_import_shape", "Importazione Shape", function () { });
    //GeneraKendoDialogMaps("#pop_up_export_shape", "Esportazione Shape", function () { });

    //testato fino a qui ...




    GeneraKendoDialogMaps("#dialogCultivarRicette", Traduzione(AgronicaControlliGisResx, "jsLblSelezionaVarieta"), function () { });
    GeneraKendoDialogMaps("#dialogGeneraPlanning", Traduzione(AgronicaControlliGisResx, "jsLblGeneraNuovaPianificazioneSelezione"), function () { });
    
    GeneraKendoDialogMaps("#dialogEliminaImpiantoPuntiScomposti", Traduzione(AgronicaControlliGisResx, "jsLblEliminaPuntiScomposti"), function () { });
    
    GeneraKendoDialogMaps("#dialogSessioneScaduta", Traduzione(AgronicaControlliGisResx, "jsLblSessioneScaduta"), function () { });
        
    GeneraKendoDialogMaps("#dialogScomponiPunti", Traduzione(AgronicaControlliGisResx, "jsLblStrumentoScomposizionePunti"), function () { });
    
    
    GeneraKendoDialogMaps("#pop_up_Ricette", Traduzione(AgronicaControlliGisResx, "jsLblRicette"), function () { });
    GeneraKendoDialogMaps("#pop_up_Ritaglia", Traduzione(AgronicaControlliGisResx, "jsLblRitaglia"), function () { });
    GeneraKendoDialogMaps("#pop_up_opAgenda", Traduzione(AgronicaControlliGisResx, "jsLblOperazioneAgenda"), function () { });
    



}

var ConfiguraMenuXS_Selezionato = 0;
function ConfiguraMenuXS() {

    if (ConfiguraMenuXS_Selezionato == 3) {
        ConfiguraMenuXS_Selezionato = 0
    }

    ConfiguraMenuXS_Selezionato++;


    $(".A-Gis-Tool-xs").hide();

    $(".menuFix").show();
    $(".menuSel").show();

    $(".menu" + ConfiguraMenuXS_Selezionato.toString()).show();

    $(".menuSelTxt").removeClass("menuSelSelezionato");
    $("#menuSel" + ConfiguraMenuXS_Selezionato.toString()).addClass("menuSelSelezionato");

}
