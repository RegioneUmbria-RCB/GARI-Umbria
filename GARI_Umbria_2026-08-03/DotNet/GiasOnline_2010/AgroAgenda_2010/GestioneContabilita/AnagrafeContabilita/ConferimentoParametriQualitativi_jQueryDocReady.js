var resxObj = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "AgronicaCoreDataProvider.dll/AgronicaCoreDataProvider.Gias"
];
var piva = null;


//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            resxObj.push(readResxFile(resxSinglePath, "ConferimentoParametriQualitativi_jQueryDocReady.js"));
        });
    }

    ImpostaIndirizzoHttpConferimento();
    piva = $(cIdPiva).val();

    kendo.ui.DatePicker.fn.options.max = new Date(2100, 11, 31);

    OnTabShow("a_tabGruppiReferenze");

    $('.nav-tabs a').on('shown.bs.tab', function (event) {
        var tabId = event.target.id;
        OnTabShow(tabId);
    });

    $.logThis("DocReady: FINE");

});

function OnTabShow(tabId) {
    $("#tabGruppiReferenze").hide();
    $("#tabParametriQualitativi").hide();
    $("#tabParametriQualitativiXReferenza").hide();
    $("#tabElencoValoriParametriQualitativi").hide();
    
    switch (tabId) {
        case "a_tabGruppiReferenze":
            $("#tabGruppiReferenze").show();
            GruppiReferenzeUC_DocReady();
            break;

        case "a_tabParametriQualitativi":
			$("#tabParametriQualitativi").show();
            ParametriQualitativiUC_DocReady();
            break;

        case "a_tabParametriQualitativiXReferenza":
            $("#tabParametriQualitativiXReferenza").show();
            ParametriQualitativiXReferenzaUC_DocReady()
            break;

        case "a_tabElencoValoriParametriQualitativi":
            $("#tabElencoValoriParametriQualitativi").show();
            ElencoValoriParametriQualitativiUC_DocReady();
            break;

        default:
            console.log("Tab inesistente da definire in pagina");
            break;

    }
}

function AggiungiSegnalazione(messaggioIniziale, messaggioAggiunta) {
    var result = messaggioIniziale;
    if (result != "")
        result += "<br />";
    result += messaggioAggiunta;
    return result;
}
function ImpostaSpecie(codiciSpecie) {
    var result = "";
    if (codiciSpecie != undefined && codiciSpecie != null && codiciSpecie.length > 0) {
        for (var s = 0; s < codiciSpecie.length; s++) {
            var desc = elencoSpecie.find(e => e.Veg_Cod == codiciSpecie[s]);
            if (desc !== undefined && desc !== null) {
                if (result != "") {
                    result += ",";
                }
                result += desc.Veg_Des;
            }
        }
    }
    return result;
}
function ImpostaVarieta(codiceSpecie, codiciVarieta) {
    var result = "";
    if (codiciVarieta != undefined && codiciVarieta != null && codiciVarieta.length > 0) {
        var varietaSpecie = RicercaVarieta(piva, codiceSpecie);
        for (var s = 0; s < codiciVarieta.length; s++) {
            var desc = varietaSpecie.find(e => e.Cul_Cod == codiciVarieta[s])
            if (desc !== undefined && desc !== null) {
                if (result != "") {
                    result += ",";
                }
                result += desc.Cul_Des;
            }
        }
    }
    return result;
}
function Carica_MultiSelect_SpecieVarieta(risp) {
    if (risp !== undefined && risp !== null && risp.length > 0) {
        for (var x = 0; x < risp.length; x++) {
            var model = risp[x];
            var specieOrigin = model.OFiltro_Veg_Cod;
            var varietaOrigin = model.OFiltro_Cul_Cod;

            if (specieOrigin != undefined && specieOrigin != null && specieOrigin != '') {
                var specie = specieOrigin.split('|').filter((e) => e != undefined && e != null && e != "");
                var descrizioni = ImpostaSpecie(specie);

                model.OFiltro_Veg_Cod = specie;
                model.OFiltro_Veg_Cod_Orig = specie;
                model.Specie_Des_String = descrizioni;
            }
            else {
                model.OFiltro_Veg_Cod = [];
                model.OFiltro_Veg_Cod_Orig = [];
                model.Specie_Des_String = "";
            }

            if (varietaOrigin != undefined && varietaOrigin != null && varietaOrigin != '' && model.OFiltro_Veg_Cod.length == 1) {
                var varieta = varietaOrigin.split('|').filter((e) => e != undefined && e != null && e != "");
                var descrizioni = ImpostaVarieta(model.OFiltro_Veg_Cod[0], varieta);

                model.OFiltro_Cul_Cod = varieta;
                model.OFiltro_Cul_Cod_Orig = varieta;
                model.Varieta_Des_String = descrizioni;
            }
            else {

                model.OFiltro_Cul_Cod = [];
                model.OFiltro_Cul_Cod_Orig = [];
                model.Varieta_Des_String = "";
            }
        }
    }
}