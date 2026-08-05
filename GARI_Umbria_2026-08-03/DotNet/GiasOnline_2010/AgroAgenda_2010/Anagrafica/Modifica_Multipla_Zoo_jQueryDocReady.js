var permesso_modificaMultipla;
var KendoModifica_Multipla_Zoo;

var resxObj = [];
var resxArrPath = [
    "Zoo/App_LocalResources/Zoo_Animali_Edit.aspx.resx",
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "AgronicaCoreDataProvider.dll/AgronicaCoreDataProvider.Gias"
];

$(document).ready(function () {

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            resxObj.push(readResxFile(resxSinglePath, "Modifica_Multipla_Zoo_jQueryDocReady.js"));
        });
    }

    if (modificaMultipla != "" && modificaMultipla != "0") {
        permesso_modificaMultipla = true;
    } else {
        permesso_modificaMultipla = false;
    }

    popolaGriglia_ModificaMultipla("divKendoModifica_Multipla_Zoo");
});