

//////////////////////////////////////////////////////////
// Testata griglie di campionamento
//////////////////////////////////////////////////////////

var indirizzohttp = "./ConfigurazioneImballiProdotto.aspx";


function RicercaImballiProdotto(options) {
    ajaxAgronica(indirizzohttp + "/CaricaGrigliaImballiProdotto",
                "{ piva: '" + $(cIdPiva).val() + "'}",
			     function (risposta) {
			        risp = JSON.parse(risposta.RispostaStringa);
			        options.success(risp);
			    }, null);
}

function erroreSubmit(grid) {

    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }

}

function controllaRigheCompletePerSubmit(righe) {

    var nrErr = 0;

    for (x = 0; x < righe.length; x++) {

        item = righe[x];

        if (item.Tabella_Cod == 0 ||
            item.Mat_Cod == 0 ||
            item.Veg_Cod == 0) {

            nrErr++;

        }
    }

    return nrErr;
}

//function controllaRigheValidePerSubmit(righe, precMess) {

//    var errMess = precMess;

//    for (x = 0; x < righe.length; x++) {
//        item = righe[x];
//        if (item.Validita_Inizio > item.Validita_Fine) {

//            if (errMess != "")
//                errMess += " <br/>";
//            errMess += "La data inizio " + item.Validita_Inizio.format("dd/MM/yyyy") + " è minore delle data fine " + item.Validita_Fine.format("dd/MM/yyyy");

//        }
//    }

//    return errMess;
//}

function SubmitImballiProdotto(options) {

    var errMess = "";

    var grid = $("#tab_griglia_configurazioneimballiprodotto").data("kendoGrid");

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheCompletePerSubmit(options.data.created) +
        controllaRigheCompletePerSubmit(options.data.updated); 

    if (nrErr > 0) {
        if (nrErr == 1)
            MessaggioErrore_Bootstrap("Esiste una riga con dati non completi", "DIV_Messaggi");
        else
            MessaggioErrore_Bootstrap("Esistono " + nrErr + " righe con dati non completi", "DIV_Messaggi");

        erroreSubmit(grid);
        return;
    }

//    errMess = controllaRigheValidePerSubmit(options.data.created, "");
//    errMess = controllaRigheValidePerSubmit(options.data.updated, errMess);

    if (errMess != "") {
        MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi")

        erroreSubmit(grid);
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti

    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    var currentData = grid.dataSource.data();
    for (var i = 0; i < currentData.length; i++) {

//        var originalDateInizio = currentData[i].Validita_Inizio;
//        var stringDateInizio = formattedReverseDate(sistemaDataInBaseAllaCulture(currentData[i].Validita_Inizio));
//        //var utcDateInizio = currentData[i].Validita_Inizio.getFullYear().toString() + currentData[i].Validita_Inizio.getMonth().toString() + currentData[i].Validita_Inizio.getDay().toString();
//        currentData[i].Validita_Inizio = stringDateInizio;

//        var originalDateFine = currentData[i].Validita_Fine;
//        var stringDateFine = formattedReverseDate(sistemaDataInBaseAllaCulture(currentData[i].Validita_Fine));
//        //var utcDateFine = currentData[i].Validita_Fine.getFullYear().toString() + currentData[i].Validita_Fine.getMonth().toString() + currentData[i].Validita_Fine.getDay().toString();
//        currentData[i].Validita_Fine = stringDateFine;

        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }

//        currentData[i].Validita_Inizio = originalDateInizio;
//        currentData[i].Validita_Fine = originalDateFine;
    }

    for (var i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        var righeInserite = kendoEscapeOggetto(newRecords);
        var righeModificate = kendoEscapeOggetto(updatedRecords);
        var righeCancellate = kendoEscapeOggetto(deletedRecords);

        var allOk = false;

        var param = "{piva: '" + $(cIdPiva).val() + "', Modulo_Generazione: 2, Tipo_Config: 1, righeInserite: '" + righeInserite + "', righeModificate: '" + righeModificate + "', righeCancellate: '" + righeCancellate + "' }"
        ajaxAgronicaSync(indirizzohttp + "/AggiornaImballiProdotto",
            param, false,
            function (risposta) {
                allOk = true;
                grid.dataSource._destroyed = [];
                grid.dataSource.read();
                grid.refresh();
                MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
            }, null);

        if (!allOk) {
            erroreSubmit(grid);
        }

    }

}

 