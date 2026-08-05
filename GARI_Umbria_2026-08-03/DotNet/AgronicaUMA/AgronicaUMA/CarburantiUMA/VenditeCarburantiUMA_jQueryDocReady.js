let venditaCarburantiResx = [{}];
let resxArrPath = [
    //"App_GlobalResources/AgronicaAgenda_2010.resx",
    //"Anagrafica/App_LocalResources/VenditeCarburantiUMA.aspx.resx"
];
let path_CoreWS = "";

let msgNoteObbligatoriePerDataDocumento = "E’ obbligatorio registrare il documento nello stesso giorno dell’emissione, questo per regolarizzare l’acquisto da parte dell’azienda. In casi di ritardo è obbligatorio indicarne il motivo nel campo Note e l’inadempienza sarà comunque segnalata all’Agenzia delle Dogane e Guardia di Finanza.";

function TraduciVenditeCarburanti(chiave, testoAlternativo) {
    return TraduzioneMultiResx(venditaCarburantiResx, chiave, testoAlternativo);
}

$(document).ready(function () {
    docR();
});

async function docR() {
    CaricaRisorseTraduzioni();

    await ddlAzienda_Load();

    //popolaGrigliaCarburantiVendita();

    $('#anno:text').val(new Date().getFullYear());


    $.logThis("DocReady: FINE");
}

function CaricaRisorseTraduzioni() {
    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        resxArrPath.forEach(function (resxSinglePath) {
            venditaCarburantiResx.push(readResxFile(resxSinglePath, "VenditeCarburantiUMA_jQueryDocReady.js"));
        });
    }
}

