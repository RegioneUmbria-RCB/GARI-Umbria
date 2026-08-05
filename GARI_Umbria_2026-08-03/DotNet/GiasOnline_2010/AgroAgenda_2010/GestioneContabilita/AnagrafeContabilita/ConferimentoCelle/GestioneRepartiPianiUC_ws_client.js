function LeggiCentriAziendali() {
    if (elencoCentriAziendali == null) {
        var param = "{ piva: '" + piva + "' }";
        ajaxAgronicaSync(indirizzohttp + "/LeggiCentriAziendali",
            param,
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                objVuoto = {
                    "Sa_Cod": 0,
                    "Sa_Des": ""
                };
                risp.unshift(objVuoto);
                elencoCentriAziendali = risp;
            },
            null);
    }
}

function Leggi_RepartiPiani(options) {
    var param = "{ piva: '" + piva + "' }";
    ajaxAgronicaSync(indirizzohttp + "/Leggi_RepartiPiani",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (risp != undefined && risp != null) {
                for (var i = 0; i < risp.length; i++) {
                    var item = risp[i];
                    item.Sa_Des = "";
                    if (item.Sa_Cod != undefined && item.Sa_Cod != null && item.Sa_Cod != '') {
                        var param = elencoCentriAziendali.find((e) => e.Sa_Cod == item.Sa_Cod);
                        if (param != null)
                            item.Sa_Des = param.Sa_Des;
                    }
                }
            }
            options.success(risp);
        },
        null);
}

function SubmitRepartiPiani(options) {
    var grid = $("#" + IDControllo).data("kendoGrid");
    var currentData = grid.dataSource.data();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var messageErr = "";
    messageErr += controllaRigheCompletePerSubmitReparti(options.data.created);
    messageErr += controllaRigheCompletePerSubmitReparti(options.data.updated);
    messageErr += controllaRigheUgualiReparti(currentData);

    if (messageErr != null && messageErr !== "") {
        MessaggioErrore_Bootstrap(messageErr, "DIV_Messaggi");
        erroreSubmitReparti(grid);
        return;
    }

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

        ajaxAgronicaSync(indirizzohttp + "/AggiornaDaGrigliaReparti",
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
            erroreSubmitReparti(grid);
        }
    }
}

function controllaRigheCompletePerSubmitReparti(righe) {
    var messageErroreTotale = [];
    var grid = $("#" + IDControllo).data("kendoGrid");

    for (x = 0; x < righe.length; x++) {
        var messageErr = "";
        item = righe[x];

        //if (!verificaChiaveReparti(item)) {
        //    if (messageErr !== "")
        //        messageErr += ";<br />";
        //    messageErr += "Non è possibile spostare un reparto da un centro di costo all'altro.";
        //}

        if (isNullOrEmpty(item.Piano_Des)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Il campo 'Piano_Des' non può essere vuoto";
        }

        if (messageErr !== "") {
            var errore = messageErr + " <br />";
            messageErroreTotale.push(errore);
            messageErr = "";
        }
    }

    return messageErroreTotale.join('\n');
}

function controllaRigheUgualiReparti(righe) {
    var result = "";
    var doppie = [];

    for (var i = 0; i < righe.length; i++) {
        var item = righe[i];

        var doppione = righe.filter((e) => e.Piano_Des.toLowerCase() == item.Piano_Des.toLowerCase());

        if (doppione != undefined && doppione != null && doppione.length > 1) {
            var presente = doppie.find((e) => e.Piano_Des.toLowerCase() == item.Piano_Des.toLowerCase());
            if (presente == undefined || presente == null || presente.length == 0)
                doppie.push(item)
        }
    }

    if (doppie != undefined && doppie != null && doppie.length > 0) {
        for (var i = 0; i < doppie.length; i++) {
            var item = doppie[i];
            if (result != "")
                result += ";<br />";
            result += "Il reparto '" + item.Piano_Des + "', risulta definito più volte";
        }
    }

    return result;
}

function verificaChiaveReparti(item) {
    var result = true;
    if (item.IdParam != null && item.IdParam != '') {
        var chiaveNuova = item.Piva + "_" + item.Sa_Cod + "_" + item.Piano_Cod;
        result = (item.IdParam == chiaveNuova);
    }
    return result;
}

function erroreSubmitReparti(grid) {

    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        var dsSort = [];
        dsSort.push({ field: "Sa_Cod", dir: "asc" });
        dsSort.push({ field: "Piano_Cod", dir: "asc" });

        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }
}

function isNullOrEmpty(value) {
    var result = (value == null);
    result = result || (value == "");
    return result;
}