function LeggiCentriDiCosto() {
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
            elencoCentroDiCosto = risp;
        },
        null);
}

function LeggiReparti() {  
    var param = "{ piva: '" + piva + "' }";
    ajaxAgronicaSync(indirizzohttp + "/LeggiReparti",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            objVuoto = {
                "Sa_Cod": 0,
                "Sa_Des": "",
                "Piano_Cod": 0,
                "Piano_Des": ""
            };
            risp.unshift(objVuoto);
            elencoReparti = risp;
        },
        null);
}

function Leggi_Celle(options) {
    var param = "{ piva: '" + piva + "' }";
    ajaxAgronicaSync(indirizzohttp + "/Leggi_Celle",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            for (var idx = 0; idx < risp.length; idx++) {
                var model = risp[idx];
                model.deleteAll = false;
            }
            options.success(risp);
        },
        null);
}

function SubmitCelle(options) {
    var grid = $("#" + IDControllo).data("kendoGrid");
    var currentData = grid.dataSource.data();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var messageErr = "";
    messageErr += controllaRigheCompletePerSubmitCelle(options.data.created);
    messageErr += controllaRigheCompletePerSubmitCelle(options.data.updated);
    messageErr += controllaRigheUgualiCelle(currentData);

    if (messageErr != null && messageErr !== "") {
        MessaggioErrore_Bootstrap(messageErr, "DIV_Messaggi");
        erroreSubmitCelle(grid);
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
            var record = setInsiemeCodRecord(currentData[i], currentData);
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

        ajaxAgronicaSync(indirizzohttp + "/AggiornaDaGrigliaCelle",
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
            erroreSubmitCelle(grid);
        }
    }
}

function controllaRigheCompletePerSubmitCelle(righe) {
    var messageErroreTotale = [];
    var grid = $("#" + IDControllo).data("kendoGrid");

    for (x = 0; x < righe.length; x++) {
        var messageErr = "";
        item = righe[x];

        if (!verificaRepartoCentroAziendale(item)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "L'associazione tra reparto e centro aziendale non è valido.";
        }

        if (isNullOrEmpty(item.Insieme_Des)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Il campo 'Cella' non può essere vuoto";
        }

        if (isNullOrEmpty(item.Identificativo)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Il campo 'Stiva' non può essere vuoto";
        }

        if (messageErr !== "") {
            var errore = messageErr + " <br />";
            messageErroreTotale.push(errore);
            messageErr = "";
        }
    }

    return messageErroreTotale.join('\n');
}

function controllaRigheUgualiCelle(righe) {
    var result = "";
    var celleDoppie = [];
    var stiveDoppie = [];
    var doppie = [];

    for (var i = 0; i < righe.length; i++) {
        var item = righe[i];

        var doppioneCella = righe.filter((e) => e.Insieme_Cod != item.Insieme_Cod &&
            e.Insieme_Des.toLowerCase() == item.Insieme_Des.toLowerCase());
        if (doppioneCella != undefined && doppioneCella != null && doppioneCella.length > 1) {
            var presente = celleDoppie.find((e) => e.Insieme_Des.toLowerCase() == item.Insieme_Des.toLowerCase());
            if (presente == undefined || presente == null || presente.length == 0)
                celleDoppie.push(item)
        }

        var doppioneStiva = righe.filter((e) => e.Identificativo.toLowerCase() == item.Identificativo.toLowerCase());
        if (doppioneStiva != undefined && doppioneStiva != null && doppioneStiva.length > 1) {
            var presente = stiveDoppie.find((e) => e.Identificativo.toLowerCase() == item.Identificativo.toLowerCase());
            if (presente == undefined || presente == null || presente.length == 0)
                stiveDoppie.push(item)
        }

        var doppione = righe.filter((e) => e.Insieme_Des.toLowerCase() == item.Insieme_Des.toLowerCase() &&
            e.Identificativo.toLowerCase() == item.Identificativo.toLowerCase());
        if (doppione != undefined && doppione != null && doppione.length > 1) {
            var presente = doppie.find((e) => e.Insieme_Des.toLowerCase() == item.Insieme_Des.toLowerCase() &&
                e.Identificativo.toLowerCase() == item.Identificativo.toLowerCase());
            if (presente == undefined || presente == null || presente.length == 0)
                doppie.push(item)
        }
    }

    if (celleDoppie != undefined && celleDoppie != null && celleDoppie.length > 0) {
        for (var i = 0; i < celleDoppie.length; i++) {
            var item = celleDoppie[i];
            if (result !== "")
                result += ";<br />";
            result += "La cella '" + item.Insieme_Des + "' risulta definito più volte per celle diverse";
        }
    }

    if (stiveDoppie != undefined && stiveDoppie != null && stiveDoppie.length > 0) {
        for (var i = 0; i < stiveDoppie.length; i++) {
            var item = stiveDoppie[i];
            if (result !== "")
                result += ";<br />";
            result += "La stiva '" + item.Identificativo + "' risulta definita su più celle";
        }
    }

    if (doppie != undefined && doppie != null && doppie.length > 0) {
        for (var i = 0; i < doppie.length; i++) {
            var item = doppie[i];
            if (result != "")
                result += ";<br />";
            result += "La cella '" + item.Insieme_Des + "' e la stiva '" + item.Identificativo + "', risulta definita più volte";
        }
    }

    return result;
}

function controllaCelleDoppie(item, righe) {
    var doppie = [];
    var doppioneCella = righe.filter((e) => e.Insieme_Cod != item.Insieme_Cod &&
        e.Insieme_Des.toLowerCase() == item.Insieme_Des.toLowerCase());

    if (doppioneCella != undefined && doppioneCella != null && doppioneCella.length > 1) {
        if (result !== "")
            result += ";<br />";
        result += "La cella '" + item.Insieme_Des + "' risulta definito più volte per celle diverse";
    }

    if (result !== "")
        result += ";<br />";
    result += "La stiva '" + item.Identificativo + "' risulta definita su più celle";

}

function verificaRepartoCentroAziendale(elem) {
    var valido = false;
    var repartoValido = elencoReparti.find((x => x.Sa_Cod == elem.Sa_Cod &&
        x.Piano_Cod == elem.Piano_Cod));
    if (repartoValido != undefined && repartoValido != null)
        valido = true;
    return valido;
}

function setInsiemeCodRecord(record, currentData) {
    var toInsert = currentData.find((elem) =>
        elem.Piva == record.Piva &&
        elem.Sa_Cod == record.Sa_Cod &&
        elem.Piano_Cod == record.Piano_Cod &&
        elem.Insieme_Des == record.Insieme_Des &&
        elem.Insieme_Cod > 0);

    if (toInsert != undefined && toInsert != null) {
        record.Insieme_Cod = toInsert.Insieme_Cod;
    }

    return record;
}

function isNullOrEmpty(value) {
    var result = (value == null);
    result = result || (value == "");
    return result;
}

function erroreSubmitCelle(grid) {

    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        var dsSort = [];
        dsSort.push({ field: "Sa_Cod", dir: "asc" });
        dsSort.push({ field: "Vas_Cod", dir: "asc" });
        dsSort.push({ field: "Piano_Cod", dir: "asc" });
        dsSort.push({ field: "Validita_Inizio", dir: "asc" });
        dsSort.push({ field: "Validita_Fine", dir: "asc" });

        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }

}