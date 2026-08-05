var Txt_Data;
var GrigliaPrenotazionePianteRiepilogo;
var strPrenotazionePianteRiepilogo;
var riepilogoPrenPianteResx = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "AgronicaCoreDataProvider.dll/AgronicaCoreDataProvider.Gias",
    "DataAnalisiBI/PrenotazionePiante/App_LocalResources/PrenotazionePiante_Riepilogo.aspx.resx"
];

var objColors;

$(document).ready(function () {

    docReady();

});

async function docReady() {

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            riepilogoPrenPianteResx.push(readResxFile(resxSinglePath, "PrenotazionePiante_Riepilogo_jQueryDocReady.js"));
        });
    }

    $("#tabstrip").kendoTabStrip({
        animation: { open: { effects: "fadeIn" } }
    });

    $("#btn_AggiornaReport").click(function () { aggiornaReport(); });
    $("#btn_AggiornaReportSintetico").click(function () { aggiornaReportSintetico(); });

    Txt_Data = $("#Txt_Data").kendoDatePicker().data("kendoDatePicker");

    objColors = await Popola_objColors();

    //var date = new Date();
    //date.setHours(0, 0, 0, 0);
    //Txt_Data.value(date);
    //Txt_Data.trigger("change");
    aggiornaReportSintetico();
}