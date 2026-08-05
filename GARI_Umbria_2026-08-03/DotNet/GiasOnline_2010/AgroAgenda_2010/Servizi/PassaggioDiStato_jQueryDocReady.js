var objLista_DSS_Selezionati;

var resxObj = [];
var resxArrPath = [
    "App_GlobalResources/AgroProfilazione.resx",
    "AgronicaCoreDataProvider.dll/AgronicaCoreDataProvider.Gias"
];

jQuery(document).ready(function () {

    docReady();

});


async function docReady() {
    WaitFrame.show();

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            resxObj.push(readResxFile(resxSinglePath, "PassaggioDiStato_jQueryDocReady.js"));
        });
    }

    $("#btnConferma").click(function () { CambiaStatoOK() });

    UtenteAbilitato_Provisioning_R = await LeggiPermessoUtenteP(id_HD_Username, 362, 0);

    let praticaCodPerStatoDestinazione = 0;

    if (Number.isInteger(pratica_cod)) {
        praticaCodPerStatoDestinazione = pratica_cod;
    }
    else {
        if (pratica_cod != "") {
            praticaCodPerStatoDestinazione = pratica_cod.split("_")[0];
        }
    }

    Passaggio_di_Stato(praticaCodPerStatoDestinazione, passaggiodistato_cod);

    WaitFrame.hide();
}