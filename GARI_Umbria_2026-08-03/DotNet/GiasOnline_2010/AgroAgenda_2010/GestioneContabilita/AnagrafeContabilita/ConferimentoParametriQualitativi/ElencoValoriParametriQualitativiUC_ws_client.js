function Carica_ModuloGenerazione_ElencoValoriParametriQualitativi() {
    var param = "{ piva: '" + piva + "' }";
    ajaxAgronicaSync(indirizzohttp + "/LeggiModuloGenerazione",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            elencoModuloGenerazione = risp;
        }, null);
}

function Carica_ElencoValoriParametriQualitativi() {
    var param = "{ piva: '" + piva + "' }";
    ajaxAgronicaSync(indirizzohttp + "/LeggiElencoValoriParametriQualitativi",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            elencoElencoValoriParametriQualitativi = risp.filter(x =>
                elencoTipoParametriDisabilitati.findIndex(i => i == x.Tabella_Cod) < 0);
        }, null);
}

function Leggi_ElencoValoriParametriQualitativi(options) {
    var param = "{piva: '" + piva + "' }"
    ajaxAgronica(indirizzohttp + "/Leggi_ValoriParametriQualitativi",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            if (risp !== undefined && risp !== null && risp.length > 0) {
                for (var x = 0; x < risp.length; x++) {
                    var model = risp[x];
                    var specieOrigin = model.OFiltro_Veg_Cod;
                    var varietaOrigin = model.OFiltro_Cul_Cod;

                    if (specieOrigin != undefined && specieOrigin != null && specieOrigin != '') {
                        var specie = specieOrigin.split('|').filter((e) => e != undefined && e != null && e != "");
                        var descrizioni = ImpostaSpecie(specie);

                        model.OFiltro_Veg_Cod = specie;
                        model.OFiltro_Veg_Cod_Orig = specie;
                        model.Specie_Des_String = descrizioni;
                    }
                    else {
                        model.OFiltro_Veg_Cod = [];
                        model.OFiltro_Veg_Cod_Orig = [];
                        model.Specie_Des_String = "";
                    }

                    if (varietaOrigin != undefined && varietaOrigin != null && varietaOrigin != '' && model.OFiltro_Veg_Cod.length == 1) {
                        var varieta = varietaOrigin.split('|').filter((e) => e != undefined && e != null && e != "");
                        var descrizioni = ImpostaVarieta(model.OFiltro_Veg_Cod[0], varieta);

                        model.OFiltro_Cul_Cod = varieta;
                        model.OFiltro_Cul_Cod_Orig = varieta;
                        model.Varieta_Des_String = descrizioni;
                    }
                    else {

                        model.OFiltro_Cul_Cod = [];
                        model.OFiltro_Cul_Cod_Orig = [];
                        model.Varieta_Des_String = "";
                    }
                }
            }

            options.success(risp);
        }, null);
}

function Salva_ElencoValoriParametriQualitativi(options) {
    var grid = $("#" + IDControllo).data("kendoGrid");
    var currentData = grid.dataSource.data();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var messageErr = "";
    messageErr += controllaRigheCompletePerSubmitElencoValoriParametriQualitativi(options.data.created);
    messageErr += controllaRigheCompletePerSubmitElencoValoriParametriQualitativi(options.data.updated);
    messageErr += controllaRigheUgualiElencoValoriParametriQualitativi(currentData);

    if (messageErr != null && messageErr !== "") {
        MessaggioErrore_Bootstrap(messageErr, "DIV_Messaggi");
        erroreSubmitElencoValoriParametriQualitativi(grid);
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

        ajaxAgronicaSync(indirizzohttp + "/AggiornaDaGrigliaElencoValoriParametriQualitativi",
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
            erroreSubmitElencoValoriParametriQualitativi(grid);
        }
    }
}

function controllaRigheCompletePerSubmitElencoValoriParametriQualitativi(righe) {
    var verificaNumerica = false;
    var verificaAlfaNumerica = false;
    var minimo = null;
    var massimo = null;
    var decimali = null;
    var messageErroreTotale = [];
    var grid = $("#" + IDControllo).data("kendoGrid");

    for (x = 0; x < righe.length; x++) {
        var messageErr = "";
        item = righe[x];

        if (!verificaChiaveElencoValoriParametriQualitativi(item)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Non è possibile spostare un parametro da una referenza all'altra.";
        }

        if (!verificaCongruenzaElencoValoriParametriQualitativi(item)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Il parametro non è selezionabile dalla referenza.";
        }

        if (isNullOrEmpty(item.Descrizione)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Il campo 'Descrizione' non può essere vuoto";
        }

        if (isNullOrEmpty(item.Sigla)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Il campo 'Sigla' non può essere vuoto";
        }

        var rangeError = rangeMassimoMinimoValido(item);
        if (rangeError != "") {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += rangeError;
        }

        if (messageErr !== "") {
            var errore = "Segnalazioni per il parametro '" + item.Tabella_Cod_Des + "': <br />" + messageErr + " <br />";
            messageErroreTotale.push(errore);
            messageErr = "";
        }
    }

    return messageErroreTotale.join('\n');
}

function verificaChiaveElencoValoriParametriQualitativi(item) {
    var result = true;
    if (item.IdParam != null && item.IdParam != '') {
        var chiaveNuova = item.Piva + "_" + item.Tabella_Cod + "_" + item.Tabella_Par_Cod + "_" + item.Modulo_Cod;
        result = (item.IdParam == chiaveNuova);
    }
    return result;
}

function verificaCongruenzaElencoValoriParametriQualitativi(item) {
    var elencoValori = elencoElencoValoriParametriQualitativi.filter(x =>
        x.Tabella_Cod == item.Tabella_Cod &&
        x.Modulo_Generazione == item.Modulo_Cod);
    var valido = (elencoValori != undefined && elencoValori != null && elencoValori.length > 0);
    return valido;
}

function controllaRigheUgualiElencoValoriParametriQualitativi(righe) {
    var result = "";
    var doppie = [];

    for (var i = 0; i < righe.length; i++) {
        var item = righe[i];

        var doppione = righe.filter((e) => e.Modulo_Cod == item.Modulo_Cod &&
            e.Tabella_Cod_Des.toLowerCase() == item.Tabella_Cod_Des.toLowerCase() &&
            e.Descrizione.toLowerCase() == item.Descrizione.toLowerCase() &&
            e.Sigla.toLowerCase() == item.Sigla.toLowerCase());

        if (doppione != undefined && doppione != null && doppione.length > 1) {
            var presente = doppie.find((e) => e.Modulo_Cod == item.Modulo_Cod &&
                e.Tabella_Cod_Des.toLowerCase() == item.Tabella_Cod_Des.toLowerCase() &&
                e.Descrizione.toLowerCase() == item.Descrizione.toLowerCase() &&
                e.Sigla.toLowerCase() == item.Sigla.toLowerCase());
            if (presente == undefined || presente == null || presente.length == 0)
                doppie.push(item)
        }
    }

    if (doppie != undefined && doppie != null && doppie.length > 0) {
        for (var i = 0; i < doppie.length; i++) {
            var item = doppie[i];
            if (result != "")
                result += ";<br />";
            result += "Il parametro qualitativo '" + item.Tabella_Cod_Des + "', con descrizione '" + item.Descrizione + "' risulta definito più volte";
        }
    }

    return result;
}

function erroreSubmitElencoValoriParametriQualitativi(grid) {

    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        var dsSort = [];
        dsSort.push({ field: "Tabella_Cod", dir: "asc" });
        dsSort.push({ field: "Tabella_Par_Cod", dir: "asc" });
        dsSort.push({ field: "Validita_Inizio", dir: "asc" });
        dsSort.push({ field: "Validita_Fine", dir: "asc" });

        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }

}

function rangeMassimoMinimoValido(item) {
    var errore = "";
    switch (item.Tipo_Cod) {
        case 3: // Numero
            if (!isNumberOrEmpty(item.Valore_Min)) {
                if (errore !== "")
                    errore += ";<br />";
                errore += "Il 'Valore Minimo' deve essere un valore numerico";
            }

            if (!isNumberOrEmpty(item.Valore_Max)) {
                if (errore !== "")
                    errore += ";<br />";
                errore += "Il 'Valore Massimo' deve essere un valore numerico";
            }

            if (errore == "") {
                if ((item.Valore_Min != "") && (item.Valore_Max != "")) {
                    var minimo = parseFloat(item.Valore_Min);
                    var massimo = parseFloat(item.Valore_Max);

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

function isNullOrEmpty(value) {
    var result = (value == null);
    result = result || (value == "");
    return result;
}