var grdSchemaDocumenti = "grdSchemaDocumenti";

function TraduciLavorazioni(chiave, testoAlternativo) {
    return TraduzioneMultiResx(confSchemaDocumentiResx, chiave, testoAlternativo);
}

$(document).ready(function () {
    docR();
});


async function docR() {
    WaitFrame.show();

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            SchemaDocumenti.push(readResxFile(resxSinglePath, "SchemaDocumenti_UC_jQueryDocReady.js"));
        });
    }

    //let docRDataPrecaricata = await ws_leggiConfigurazione();

    let tabstrip = $("#tabstrip_elenco").kendoTabStrip({
        select: onActivate,
        animation: {
            open: {
                effects: "fadeIn"
            }
        }
    }).data("kendoTabStrip");
    popolaGrigliaSchemaDocumenti("grdSchemaDocumenti");
    init_tabSchemaDocumenti = true;

    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    document.getElementById("panelArea").style.opacity = "1";

    //Nascondo Tab Linee Produzioni
    $("#tabstrip_elenco").show();

    WaitFrame.hide();
}


function onActivate(e) {
    //var tabStrip = $("#tabstrip_elenco").kendoTabStrip().data("kendoTabStrip");
    //tabStrip.select(e.item);
    //var elemento = tabStrip.select().index();

    if (!init_tabSchemaDocumenti) {
        WaitFrame.show();
        popolaGrigliaSchemaDocumenti("grdSchemaDocumenti");
        WaitFrame.hide();
    }
}