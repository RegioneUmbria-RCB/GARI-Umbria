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
            resxObj.push(readResxFile(resxSinglePath, "Conferimento_Config_jQueryDocReady.js"));
        });
    }

    ImpostaIndirizzoHttpConferimento();
    piva = $(cIdPiva).val();

    kendo.ui.DatePicker.fn.options.max = new Date(2100, 11, 31);

    OnTabShow("a_tabAttivazioneModuli");

    $('.nav-tabs a').on('shown.bs.tab', function (event) {
        var tabId = event.target.id;
        OnTabShow(tabId);
    });

    $.logThis("DocReady: FINE");

});

function OnTabShow(tabId) {
    $("#tabAttivazioneModuli").hide();
    $("#tabConfigurazioneConferimenti").hide();
    $("#tabAssegna_Lotti").hide();
    $("#tabCriteri_Aggregazione").hide();
    $("#tabGestione_Reparti_Piani").hide();
    $("#tabGestione_Celle").hide();

    switch (tabId) {
        case "a_tabAttivazioneModuli":
            Conferimento_Attivazione_ModuliUC_DocReady();
            $("#tabAttivazioneModuli").show();
            break;

        case "a_tabConfigurazioneConferimenti":
            Carica_ParamEntrataXSpecieVarieta()
            $("#tabConfigurazioneConferimenti").show();
            break;

        case "a_tabAssegna_Lotti":
            Conferimento_Assegna_LottiUC_DocReady()
            $("#tabAssegna_Lotti").show();
            break;

        case "a_tabCriteri_Aggregazione":
            Carica_Lotto_AssegnaxRisumSpeVarQualCert();
            $("#tabCriteri_Aggregazione").show();
            break;

        case "a_tabGestione_Reparti_Piani":
            Conferimento_GestionePianiReparti_DocReady() 
            $("#tabGestione_Reparti_Piani").show();
            break;

        case "a_tabGestione_Celle":
            Conferimento_GestioneCelle_DocReady()
            $("#tabGestione_Celle").show();
            break;
            
        default:
            console.log("Tab inesistente da definire in pagina");
            break;

    }
}