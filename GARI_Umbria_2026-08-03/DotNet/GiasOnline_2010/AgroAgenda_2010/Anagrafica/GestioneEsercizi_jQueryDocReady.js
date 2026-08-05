var gestioneEserciziResx = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "Anagrafica/App_LocalResources/GestioneEsercizi.aspx.resx"
];


//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady Dialog: INIZIO");

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            gestioneEserciziResx.push(readResxFile(resxSinglePath, "GestioneEsercizi_jQueryDocReady.js"));
        });
    }

    // value: new Date(new Date().getFullYear(), 11, 31)
    $("#txtDataChiusura").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",
        max: new Date(2100, 11, 31)
    }).data("kendoDatePicker");

    creaKendoDropDownList("ddlAzioneEsercizio", { read: popolaAzioneEsercizio }, "Azione_Des", "Azione_Cod");
    Set_KendoDDLValue("ddlAzioneEsercizio", 0);

    var parametriEsercizi = { poliennali: true, arboree: true };
    if ($.cookie("MenuBS_Anagrafica.parametriEserciziWindow") == undefined) {
        parametriEsercizi = { poliennali: true, arboree: true };
    } else {
        parametriEsercizi = JSON.parse($.cookie("MenuBS_Anagrafica.parametriEserciziWindow"))
        $('#checkPoliennali').prop('checked', parametriEsercizi.poliennali);
        $('#checkArboree').prop('checked', parametriEsercizi.arboree);
    }
    
    popolaGrigliaEsercizi("tab_esercizi");

    $('#checkPoliennali').change(function () {
        parametriEsercizi.poliennali = $('#checkPoliennali').is(":checked");
        $.cookie("MenuBS_Anagrafica.parametriEserciziWindow", JSON.stringify(parametriEsercizi), { path: "/", expires: 10000 });
        aggiornaGrigliaEsercizi("tab_esercizi");
    });

    $('#checkArboree').change(function () {
        parametriEsercizi.arboree = $('#checkArboree').is(":checked");
        $.cookie("MenuBS_Anagrafica.parametriEserciziWindow", JSON.stringify(parametriEsercizi), { path: "/", expires: 10000 });
        aggiornaGrigliaEsercizi("tab_esercizi");
    });

    //eventi di click pulsanti
    $("#btn_esegui").click(function () {
        eseguiAzioneEsercizi("#tab_esercizi");
    });

    // check apertura automatica finestra
    $('#checkApertura').prop('checked', $.cookie("MenuBS_Anagrafica.gestioneEserciziWindow") == "1");
    $('#checkApertura').change(function () {
        $.cookie("MenuBS_Anagrafica.gestioneEserciziWindow", $('#checkApertura').is(":checked") ? "1" : "0", { path: "/", expires: 10000 });
    });

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    $.logThis("DocReady dialog: FINE");

});

