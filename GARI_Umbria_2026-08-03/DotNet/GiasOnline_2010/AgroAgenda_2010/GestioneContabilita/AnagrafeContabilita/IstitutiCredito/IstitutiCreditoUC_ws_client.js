
function Leggi_IstitutiCredito(options) {
    var param = "{ piva: '" + piva + "' }";
    ajaxAgronicaSync(indirizzohttp + "/Leggi_IstitutiCredito",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        },
        null);
}

function SubmitIstitutiCredito(options) {
    var grid = $("#" + IDControllo).data("kendoGrid");
    var currentData = grid.dataSource.data();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var messageErr = "";
    messageErr += controllaRigheCompletePerSubmitIstitutiCredito(options.data.created);
    messageErr += controllaRigheCompletePerSubmitIstitutiCredito(options.data.updated);
    messageErr += controllaRigheUgualiIstitutiCredito(currentData);

    if (messageErr != null && messageErr !== "") {
        MessaggioErrore_Bootstrap(messageErr, "DIV_Messaggi");
        erroreSubmitIstitutiCredito(grid);
        return;
    }

    console.log(currentData);

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var righeNonCancellate = [];

    for (var i = 0; i < currentData.length; i++) {
        //Formattazione data inizio in stringa
        var originalDateInizio = currentData[i].Validita_Inizio;
        var stringDateInizio = formattedReverseDate(sistemaDataInBaseAllaCulture(currentData[i].Validita_Inizio));
        currentData[i].Validita_Inizio = stringDateInizio;

        //Formattazione data fine in stringa
        var originalDateFine = currentData[i].Validita_Fine;
        var stringDateFine = formattedReverseDate(sistemaDataInBaseAllaCulture(currentData[i].Validita_Fine));
        currentData[i].Validita_Fine = stringDateFine;

        righeNonCancellate.push(currentData[i].toJSON());
        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        } else if (currentData[i].dirty) {
            //Formattazione data creazione in stringa
            var originalData_Creazione = currentData[i].Data_Creazione;
            var stringData_Creazione = formattedReverseDate(sistemaDataInBaseAllaCulture(currentData[i].Data_Creazione));
            currentData[i].Data_Creazione = stringData_Creazione;
            updatedRecords.push(currentData[i].toJSON());
            currentData[i].Data_Creazione = originalData_Creazione;
        }
        currentData[i].Validita_Inizio = originalDateInizio;
        currentData[i].Validita_Fine = originalDateFine;
    }

    for (var i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {
        var righeInserite = kendoEscapeOggetto(newRecords);
        var righeModificate = kendoEscapeOggetto(updatedRecords);
        var righeCancellate = kendoEscapeOggetto(deletedRecords);
        var Tuttelerighe = kendoEscapeOggetto(righeNonCancellate);

        var allOk = false;
        var param = "{piva: '" + piva + "', righeInserite: '" + righeInserite + "', righeModificate: '" + righeModificate + "', righeCancellate: '" + righeCancellate + "',tutteleRighe:'" + Tuttelerighe + "' }"

        ajaxAgronicaSync(indirizzohttp + "/AggiornaDaGrigliaIstitutiCredito",
            param, false,
            function (risposta) {
                allOk = true;
                grid.dataSource._destroyed = [];
                allOk = true
                grid.dataSource.read();
                grid.refresh();
                MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
            }, null);

        if (!allOk) {
            erroreSubmitIstitutiCredito(grid);
        }
    }
}

function controllaRigheCompletePerSubmitIstitutiCredito(righe) {
    var messageErroreTotale = [];
    var grid = $("#" + IDControllo).data("kendoGrid");

    for (x = 0; x < righe.length; x++) {
        var messageErr = "";
        item = righe[x];

        if (isNullOrEmpty(item.Istituto_Des)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "La descrizione dell'istituro di credito non può essere vuota";
        }

        if (messageErr !== "") {
            var errore = messageErr + " <br />";
            messageErroreTotale.push(errore);
            messageErr = "";
        }
    }

    return messageErroreTotale.join('\n');
}

function controllaRigheUgualiIstitutiCredito(righe) {
    var result = "";
    var descrizioneDoppia = [];
    var siglaDoppia = [];

    for (var i = 0; i < righe.length; i++) {
        var item = righe[i];

        var doppioneDes = righe.filter((e) =>
            e.Istituto_Cod != item.Istituto_Cod &&
            e.Istituto_Des.toLowerCase() == item.Istituto_Des.toLowerCase());

        if (doppioneDes != undefined && doppioneDes != null && doppioneDes.length > 1) {
            var presente = descrizioneDoppia.find((e) => e.Istituto_Des.toLowerCase() == item.Istituto_Des.toLowerCase());
            if (presente == undefined || presente == null || presente.length == 0)
                descrizioneDoppia.push(item)
        }
    }

    if (descrizioneDoppia != undefined && descrizioneDoppia != null && descrizioneDoppia.length > 0) {
        for (var i = 0; i < descrizioneDoppia.length; i++) {
            var item = descrizioneDoppia[i];
            if (result != "")
                result += ";<br />";
            result += "L'istituto di credito '" + item.Istituto_Des + "', risulta definito più volte";
        }
    }

    return result;
}

function isNullOrEmpty(value) {
    var result = (value == null);
    result = result || (value == "");
    return result;
}

function erroreSubmitIstitutiCredito(grid) {

    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        var dsSort = [];
        dsSort.push({ field: "Piva", dir: "asc" });
        dsSort.push({ field: "Istituto_Cod", dir: "asc" });
        dsSort.push({ field: "Istituto_Des", dir: "asc" });
        dsSort.push({ field: "Validita_Inizio", dir: "asc" });
        dsSort.push({ field: "Validita_Fine", dir: "asc" });

        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }

}