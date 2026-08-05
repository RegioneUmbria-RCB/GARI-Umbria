function Carica_ModuloGenerazione() {
    var param = "{ piva: '" + piva + "' }";
    ajaxAgronicaSync(indirizzohttp + "/LeggiModuloGenerazione",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            elencoModuloGenerazione = risp;
        }, null);
}

function Leggi_ParametriQualitativi(options) {
    var param = "{piva: '" + piva + "' }"
    ajaxAgronica(indirizzohttp + "/Leggi_ParametriQualitativi",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (risp !== undefined && risp !== null && risp.length > 0) {
                for (var i = 0; i < risp.length; i++) {
                    var tipo = elencoTipoParametro.find(e => e.Tipo_Cod == risp[i].Tipo_Cod);
                    if (tipo !== undefined && tipo !== null) {
                        risp[i].Tipo_Des = tipo.Tipo_Des;
                    }
                }
            }
            options.success(risp);
        }, null);
}

function Salva_ParametriQualitativi(options) {
    var grid = $("#" + IDControllo).data("kendoGrid");
    var currentData = grid.dataSource.data();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var messageErr = "";
    messageErr += controllaRigheCompletePerSubmitParametriQualitativi(options.data.created);
    messageErr += controllaRigheCompletePerSubmitParametriQualitativi(options.data.updated);
    messageErr += controllaRigheUgualiParametriQualitativi(currentData);

    if (messageErr != "") {
        MessaggioErrore_Bootstrap(messageErr, "DIV_Messaggi");
        erroreSubmitParametriQualitativi(grid);
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

        ajaxAgronicaSync(indirizzohttp + "/AggiornaDaGrigliaParametriQualitativi",
            param, false,
            function (risposta) {
                allOk = true;
                grid.dataSource._destroyed = [];
                grid.dataSource.read();
                grid.refresh();
                MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
            }, null);

        if (!allOk) {
            console.log("Errore");
            erroreSubmitParametriQualitativi(grid);
        }
    }
}

function controllaRigheCompletePerSubmitParametriQualitativi(righe) {
    var messageErroreTotale = [];
    var grid = $("#" + IDControllo).data("kendoGrid");

    for (x = 0; x < righe.length; x++) {
        var messageErr = "";
        item = righe[x];

        if (!verificaChiaveParametroQualitativo(item)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Non è possibile spostare un parametro da una referenza all'altra.";
        }

        if (contieneSpazi(item.Tabella_Cod_Des)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "nel campo 'Descrizione sintetica' non è consentiro usare gli spazi";
        }

        var rangeError = rangeMassimoMinimoValido(item);
        if (rangeError != "") {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += rangeError;
        }

        if (item.Tipo_Cod == 3) {
            if (!isNumeric(item.NumDecimali_Maximo.toString()) ||
                parseFloat(item.NumDecimali_Maximo) < 0) {
                if (messageErr !== "")
                    messageErr += ";<br />";
                messageErr += "Il 'Numero massimo di decimali' deve essere un valore numerico maggiore, oppure uguale a 0(Zero)";
            }
        }

        if (messageErr !== "") {
            var errore = "Segnalazioni per il parametro '" + item.Tabella_Cod_Des + "': <br />" + messageErr + " <br />";
            messageErroreTotale.push(errore);
            messageErr = "";
        }
    }

    return messageErroreTotale.join('\n');
}

function verificaChiaveParametroQualitativo(item) {
    var result = true;
    if (item.IdParam != null && item.IdParam != '') {
        var chiave = item.Piva + '_' + item.Tabella_Cod + '_' + item.Modulo_Cod;
        result = (item.IdParam == chiave);
    }
    return result;
}

function controllaRigheUgualiParametriQualitativi(righe) {
    var result = "";
    var doppie = [];

    for (var i = 0; i < righe.length; i++) {
        var item = righe[i];

        var doppione = righe.filter((e) => e.Modulo_Cod == item.Modulo_Cod &&
            e.Tabella_Cod_Des.toLowerCase() == item.Tabella_Cod_Des.toLowerCase());

        if (doppione != undefined && doppione != null && doppione.length > 1) {
            var presente = doppie.find((e) => e.Modulo_Cod == item.Modulo_Cod &&
                e.Tabella_Cod_Des.toLowerCase() == item.Tabella_Cod_Des.toLowerCase());
            if (presente == undefined || presente == null || presente.length == 0)
                doppie.push(item)
        }
    }

    if (doppie != undefined && doppie != null && doppie.length > 0) {
        for (var i = 0; i < doppie.length; i++) {
            var item = doppie[i];
            if (result != "")
                result += ";<br />";
            result += "Il parametro qualitativo '" + item.Tabella_Cod_Des + "', con descrizione '" + item.Tabella_Des +"' risulta definito più volte";
        }
    }

    return result;
}

function erroreSubmitParametriQualitativi(grid) {
    
    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        var dsSort = [];
        dsSort.push({ field: "Piva", dir: "asc" });
        dsSort.push({ field: "Modulo_Cod", dir: "asc" });
        dsSort.push({ field: "Tabella_Cod", dir: "asc" });
        dsSort.push({ field: "Modulo_Cod", dir: "asc" });
        dsSort.push({ field: "Validita_Inizio", dir: "asc" });
        dsSort.push({ field: "Validita_Fine", dir: "asc" });

        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }

}

function contieneSpazi(s) {
    return /\s/g.test(s);
}

function rangeMassimoMinimoValido(item) {
    var errore = "";
    switch (item.Tipo_Cod) {
        case 3: // Numero
            if (!isNumberOrEmpty(item.Valore_Minimo)) {
                if (errore !== "")
                    errore += ";<br />";
                errore += "Il 'Valore Minimo' deve essere un valore numerico";
            }

            if (!isNumberOrEmpty(item.Valore_Maximo)) {
                if (errore !== "")
                    errore += ";<br />";
                errore += "Il 'Valore Massimo' deve essere un valore numerico";
            }

            if (errore == "") {
                if ((item.Valore_Minimo != "") && (item.Valore_Maximo != "")) {
                    var minimo = parseFloat(item.Valore_Minimo);
                    var massimo = parseFloat(item.Valore_Maximo);

                    if (minimo > massimo) {
                        if (errore !== "")
                            errore += ";<br />";
                        errore += "Il 'Valore Minimo' deve essere inferiore al 'Valore Massimo'";
                    }
                }
            }
            break;
        case 4: // Caratteri
        default: // Altre tipologie
            break;
    }

    return errore;
}

function isNumberOrEmpty(value) {
    var result = (value == null);
    result = result || (value == "");
    result = result || (isNumeric(value + ""));
    return result;
}