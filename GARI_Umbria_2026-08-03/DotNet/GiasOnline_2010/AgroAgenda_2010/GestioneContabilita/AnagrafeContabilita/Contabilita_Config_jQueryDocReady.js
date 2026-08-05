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
            resxObj.push(readResxFile(resxSinglePath, "Contabilita_jQueryDocReady.js"));
        });
    }

    ImpostaIndirizzoHttpContabilita();
    piva = $(cIdPiva).val();

    kendo.ui.DatePicker.fn.options.max = new Date(2100, 11, 31);

    OnTabShow("a_tabModalitaPagamento");

    $('.nav-tabs a').on('shown.bs.tab', function (event) {
        var tabId = event.target.id;
        OnTabShow(tabId);
    });

    $.logThis("DocReady: FINE");

});

function OnTabShow(tabId) {
    $("#tabModalitaPagamento").hide();
    $("#tabCausaleTrasporto").hide();
    $("#tabIstitutiCredito").hide();
    $("#tabSezionali").hide();
    $("#tabLiquidita").hide();

    switch (tabId) {
        case "a_tabModalitaPagamento":
            ModalitaPagamento_DocReady();
            $("#tabModalitaPagamento").show();
            break;

        case "a_tabCausaleTrasporto":
            CausaleTrasporto_DocReady();
            $("#tabCausaleTrasporto").show();
            break;

        case "a_tabIstitutiCredito":
            IstitutiCredito_DocReady();
            $("#tabIstitutiCredito").show();
            break;

        case "a_tabSezionali":
            Sezionali_DocReady();
            $("#tabSezionali").show();
            break;

        case "a_tabLiquidita":
            Liquidita_DocReady();
            $("#tabLiquidita").show();
            break;
            
        default:
            console.log("Tab inesistente da definire in pagina");
            break;

    }
}