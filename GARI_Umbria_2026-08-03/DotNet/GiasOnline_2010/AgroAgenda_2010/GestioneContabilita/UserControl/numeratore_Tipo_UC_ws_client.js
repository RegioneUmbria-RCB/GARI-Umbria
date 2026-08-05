var indirizzohttp = "./Numeratore_Tipo.aspx";

function CaricaNumeratoriTipo(options) {

    var param = kendo.stringify({ piva: $(cIdPiva).val() });
    ajaxAgronica(indirizzohttp + "/CaricaNumeratoriTipo",
        param,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}


function SubmitNumeratoreTipo(options) {


    var grid = $("#tab_numeratore_tipo").data("kendoGrid");
    var currentData = grid.dataSource.data();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheNumeratoreTipo(options.data.created) +
        controllaRigheNumeratoreTipo(options.data.updated);

    if (nrErr > 0) {
        if (nrErr === 1)
            MessaggioErrore_Bootstrap("Esiste una riga con dati non completi", "DIV_Messaggi");
        else
            MessaggioErrore_Bootstrap("Esistono " + nrErr + " righe con dati non completi", "DIV_Messaggi");

        // erroreSubmitFattoriVariazioneParametriQualitativi(grid);
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    // modificate / inserite
    for (let i = 0; i < currentData.length; i++) {

        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
    }

    // cancellate
    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {

        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        var righeInserite = kendoEscapeOggetto(newRecords);
        var righeModificate = kendoEscapeOggetto(updatedRecords);
        var righeCancellate = kendoEscapeOggetto(deletedRecords);

        var allOk = false;

        var objParametri = new Object();
        objParametri.Piva = $(cIdPiva).val();
        objParametri.RigheInserite = righeInserite;
        objParametri.RigheModificate = righeModificate;
        objParametri.RigheCancellate = righeCancellate;

        var paramEscaped = kendoEscapeOggetto(objParametri);
        var param = "{paramString: '" + paramEscaped + "'}";

        //var param = "{piva: '" + $(cIdPiva).val() + "', righeInserite: '" + righeInserite + "', righeModificate: '" + righeModificate + "', righeCancellate: '" + righeCancellate + "' }"

        ajaxAgronicaSync(indirizzohttp + "/AggiornaNumeratoriTipo",
            param, false,
            function (risposta) {
                allOk = true;
                grid.dataSource._destroyed = [];
                grid.dataSource.read();
                grid.refresh();
                MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
            }, function (risposta) {
                var errori = risposta.RispostaStringa + ' ' + risposta.Errore;
                MessaggioErrore_Bootstrap(errori, "DIV_Messaggi");
            });

        if (!allOk) {
            erroreSubmitGriglia(grid);
        }
    }
}

function erroreSubmitGriglia(grid) {
    var dsSort = [];
    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }
}


// Controllo campi obbligatori
function controllaRigheNumeratoreTipo(righe) {

    var nrErr = 0;

    for (let x = 0; x < righe.length; x++) {

        let item = righe[x];

        if (item.Cod_RisUm === 0 || item.Veg_Cod === 0 || item.Cul_Cod === 0 || item.lotto === "") {
            nrErr++;
        }
    }

    return nrErr;
}