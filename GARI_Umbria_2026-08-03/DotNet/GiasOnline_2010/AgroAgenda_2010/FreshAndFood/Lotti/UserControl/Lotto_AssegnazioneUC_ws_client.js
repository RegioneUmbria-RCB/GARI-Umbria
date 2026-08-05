function RicercaValoriParametriQualitativiQualita() {

    var valoriParamQual = RicercaValoriParametriQualitativi(3, cIdPiva);
    var valoriParamQualArray = [];
    for (i = 0; i < valoriParamQual.length; i++) {
        datiQual = { "qualita_cod": valoriParamQual[i].val_cod, "qualita_des": valoriParamQual[i].val_des };
        valoriParamQualArray.push(datiQual);
    }
    elencoQualitaAssegnaLotto = valoriParamQualArray;

}

function RicercaValoriParametriQualitativiCertif() {

    var valoriParamQual = RicercaValoriParametriQualitativi(12, cIdPiva);
    var valoriParamQualArray = [];
    for (i = 0; i < valoriParamQual.length; i++) {
        datiCertif = { "certif_cod": valoriParamQual[i].val_cod, "certif_des": valoriParamQual[i].val_des };
        valoriParamQualArray.push(datiCertif);
    }
    elencoCertificAssegnaLotto = valoriParamQualArray;
}

function RicercaLottoAssegna(options) {

    var Tipo_Lotto = $(hfTipo_Lotto).val().toUpperCase();

    var param = "{piva: '" + cIdPiva + "', tipo_lotto: '" + Tipo_Lotto + "' }"
    ajaxAgronica(indirizzohttp_Lotto_AssegnazioneUC + "/Carica_Lotto_AssegnaxRisumSpeVarQualCert",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            if (Tipo_Lotto === "L") {
                var Algoritmo_Config_Letti = [];
                for (var x = 0; x < risp.length; x++) {

                    //Creo l'Array per la gestione degli Algoritmo_Config nella Multiselect della griglia
                    Algoritmo_Config_Letti = risp[x].Algoritmo_Config_String.split("|");
                    risp[x].Algoritmo_Config = [];
                    for (var y = 0; y < Algoritmo_Config_Letti.length; y++) {
                        if (Algoritmo_Config_Letti[y] !== "") {
                            risp[x].Algoritmo_Config.push(Algoritmo_Config_Letti[y].toString());
                        }
                    }

                    //Creo l'Array per la gestione degli Algoritmo_Config_Des nella Multiselect della griglia
                    risp[x].Algoritmo_Config_Des = [];
                    risp[x].Algoritmo_Config_Des_String = "";
                    for (var y = 0; y < Algoritmo_Config_Letti.length; y++) {
                        for (var b = 0; b < elencoParametriAssegnaLotto.length; b++) {
                            if (parseInt(Algoritmo_Config_Letti[y]) === elencoParametriAssegnaLotto[b].Algoritmo_Config) {
                                risp[x].Algoritmo_Config_Des.push(elencoParametriAssegnaLotto[b].Algoritmo_Config_Des);
                            }
                        }
                    }

                    if (risp[x].Algoritmo_Config_Des.length > 0)
                        risp[x].Algoritmo_Config_Des_String = risp[x].Algoritmo_Config_Des.join(",");
                }
            }
           

            options.success(risp);
        }, null);
}

function RicercaTipologieLavorazioni() {

    var param = "{piva: 'AAAAAAAAAAA'}";
    var risultato_lettura = [];

    ajaxAgronicaSync(indirizzohttp_Lotto_AssegnazioneUC + "/LeggiPreparazioni",
        param,
        false,
        function (risposta) {
            risultato_lettura = JSON.parse(risposta.RispostaStringa);

            var objVuoto = {
                "Preparazione_Des": "",
                "Preparazione_Cod": 0
            };
            risultato_lettura.unshift(objVuoto);
        },
        null);

    elencoTipologieLavorazioniAssegnaLotto = risultato_lettura;

}

function SubmitLottoAssegna(options) {

    var grid = $("#tab_lotto_assegna").data("kendoGrid");
    var currentData = grid.dataSource.data();

    var Tipo_Lotto = $(hfTipo_Lotto).val().toUpperCase();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheLottoAssegna(options.data.created, Tipo_Lotto) +
        controllaRigheLottoAssegna(options.data.updated, Tipo_Lotto);

    if (nrErr > 0) {
        if (nrErr == 1)
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
    var righeNonCancellate = [];

    for (var i = 0; i < currentData.length; i++) {

        var originalDateInizio = currentData[i].validita_inizio;
        var stringDateInizio = formattedReverseDate(sistemaDataInBaseAllaCulture(currentData[i].validita_inizio));
        currentData[i].validita_inizio = stringDateInizio;

        var originalDateFine = currentData[i].validita_fine;
        var stringDateFine = formattedReverseDate(sistemaDataInBaseAllaCulture(currentData[i].validita_fine));
        currentData[i].validita_fine = stringDateFine;

        righeNonCancellate.push(currentData[i].toJSON());

        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }

        currentData[i].validita_inizio = originalDateInizio;
        currentData[i].validita_fine = originalDateFine;

    }

    for (var i = 0; i < grid.dataSource._destroyed.length; i++) {

        var originalDateInizio = grid.dataSource._destroyed[i].validita_inizio;
        var stringDateInizio = formattedReverseDate(sistemaDataInBaseAllaCulture(grid.dataSource._destroyed[i].validita_inizio));
        grid.dataSource._destroyed[i].validita_inizio = stringDateInizio;

        var originalDateFine = grid.dataSource._destroyed[i].validita_fine;
        var stringDateFine = formattedReverseDate(sistemaDataInBaseAllaCulture(grid.dataSource._destroyed[i].validita_fine));
        grid.dataSource._destroyed[i].validita_fine = stringDateFine;

        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        var righeInserite = kendoEscapeOggetto(newRecords);
        var righeModificate = kendoEscapeOggetto(updatedRecords);
        var righeCancellate = kendoEscapeOggetto(deletedRecords);
        var tutteLerighe = kendoEscapeOggetto(righeNonCancellate);

        var allOk = false;
        var param = "{piva: '" + cIdPiva + "', righeInserite: '" + righeInserite + "', righeModificate: '" + righeModificate + "', righeCancellate: '" + righeCancellate + "', tutteleRighe: '" + tutteLerighe+"'}"

        ajaxAgronicaSync(indirizzohttp_Lotto_AssegnazioneUC + "/AggiornaLotto_AssegnaxRisumSpeVarQualCert",
            param, false,
            function (risposta) {
                allOk = true;
                grid.dataSource._destroyed = [];
                grid.dataSource.read();
                grid.refresh();
                MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
            }, function (risposta) {
                if (Tipo_Lotto === "L") {
                    grid.dataSource._destroyed = [];
                    grid.dataSource.read();
                    grid.refresh();
                }
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
            });

        //if (!allOk) {
        //    erroreSubmitFattoriVariazioneParametriQualitativi(grid);
        //}
    }
}

// Controllo campi obbligatori
function controllaRigheLottoAssegna(righe,Tipo_Lotto) {

    var nrErr = 0;

    for (x = 0; x < righe.length; x++) {

        item = righe[x];

        if (Tipo_Lotto === "E") {

            if (item.Cod_RisUm === 0 || item.Veg_Cod === 0 || item.Cul_Cod === 0 || item.lotto === "") {
                nrErr++;
            }

        }
        else if (Tipo_Lotto === "L") {

            if (item.Preparazione_Cod === 0) {
                nrErr++;
            }
        }

    }

    return nrErr;
}

/*
function erroreSubmitFattoriVariazioneParametriQualitativi(grid) {

    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        var dsSort = [];
        dsSort.push({ field: "Tabella_Des", dir: "asc" });
        dsSort.push({ field: "val_des", dir: "asc" });

        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }

}
*/
