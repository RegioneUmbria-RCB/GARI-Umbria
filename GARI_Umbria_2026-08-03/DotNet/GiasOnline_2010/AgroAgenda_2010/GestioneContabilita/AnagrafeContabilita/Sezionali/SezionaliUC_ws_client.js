function Leggi_RegimiFiscale() {
    ajaxAgronicaSync(indirizzohttp + "/Leggi_RegimiFiscale",
        null,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            elencoRegimiFiscale = risp;
        },
        null);
}

function Leggi_Sezionali(options) {
    var param = "{ piva: '" + piva + "' }";
    ajaxAgronicaSync(indirizzohttp + "/Leggi_Sezionali",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (risp != undefined && risp != null) {
                for (var i = 0; i < risp.length; i++) {
                    var item = risp[i];
                    item.Regime_Fiscale_Cod = ImpostaValoreDefault(item.Regime_Fiscale_Cod, 0);
                    item.Regime_Fiscale_Des = ImpostaDescrizioneRegimeFiscale(item.Regime_Fiscale_Cod);
                    item.Esigibilita_Iva_Cod = ImpostaValoreDefault(item.Esigibilita_Iva_Cod, 0);
                    item.Esigibilita_Iva_Des = ImpostaDescrizioneEsigibilitaIva(item.Esigibilita_Iva_Cod);
                    item.Fatturazione_Elettronica_Cod = ImpostaValoreDefault(item.Fatturazione_Elettronica_Cod, 0);
                    item.Fatturazione_Elettronica_Des = ImpostaDescrizioneFatturazione(item.Fatturazione_Elettronica_Cod);
                }
            }
            options.success(risp);
        },
        null);
}

function SubmitSezionali(options) {
    var grid = $("#" + IDControllo).data("kendoGrid");
    var currentData = grid.dataSource.data();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var messageErr = "";
    messageErr += controllaRigheCompletePerSubmitSezionali(options.data.created);
    messageErr += controllaRigheCompletePerSubmitSezionali(options.data.updated);
    messageErr += controllaRigheUgualiSezionali(currentData);

    if (messageErr != null && messageErr !== "") {
        MessaggioErrore_Bootstrap(messageErr, "DIV_Messaggi");
        erroreSubmitSezionali(grid);
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

        ajaxAgronicaSync(indirizzohttp + "/AggiornaDaGrigliaSezionali",
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
            erroreSubmitSezionali(grid);
        }
    }
}

function controllaRigheCompletePerSubmitSezionali(righe) {
    var messageErroreTotale = [];
    var grid = $("#" + IDControllo).data("kendoGrid");

    for (x = 0; x < righe.length; x++) {
        var messageErr = "";
        item = righe[x];

        if (isNullOrEmpty(item.Sezionale_Des)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "La descrizione dell'impresa sezionale non può essere vuota";
        }

        if (item.Validita_Inizio > item.Validita_Fine) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "La 'Validità inizio' deve essere una data minore della 'Validità Fine'";
        }

        if (messageErr !== "") {
            var errore = messageErr + " <br />";
            messageErroreTotale.push(errore);
            messageErr = "";
        }
    }

    return messageErroreTotale.join('\n');
}

function controllaRigheUgualiSezionali(righe) {
    var result = "";
    var descrizioneDoppia = [];

    for (var i = 0; i < righe.length; i++) {
        var item = righe[i];

        var doppioneDes = righe.filter((e) =>
            e.Sezionale_Cod != item.Sezionale_Cod &&
            e.Sezionale_Des.toLowerCase() == item.Sezionale_Des.toLowerCase());

        if (doppioneDes != undefined && doppioneDes != null && doppioneDes.length > 0) {
            var presente = descrizioneDoppia.find((e) => e.Sezionale_Des.toLowerCase() == item.Sezionale_Des.toLowerCase());
            if (presente == undefined || presente == null)
                descrizioneDoppia.push(item)
        }
    }

    if (descrizioneDoppia != undefined && descrizioneDoppia != null && descrizioneDoppia.length > 0) {
        for (var i = 0; i < descrizioneDoppia.length; i++) {
            var item = descrizioneDoppia[i];
            if (result != "")
                result += ";<br />";
            result += "L'impresa sezionale '" + item.Sezionale_Des + "', risulta definita più volte";
        }
    }

    return result;
}

function isNullOrEmpty(value) {
    var result = (value == null);
    result = result || (value == "");
    return result;
}

function erroreSubmitSezionali(grid) {

    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        var dsSort = [];
        dsSort.push({ field: "Piva", dir: "asc" });
        dsSort.push({ field: "Cau_Pagamento_Cod", dir: "asc" });
        dsSort.push({ field: "Cau_Pagamento_Des", dir: "asc" });
        dsSort.push({ field: "Validita_Inizio", dir: "asc" });
        dsSort.push({ field: "Validita_Fine", dir: "asc" });

        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }

}