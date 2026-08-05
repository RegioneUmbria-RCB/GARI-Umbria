function RicercaValoriParametriQualitativiQualita() {

    var valoriParamQual = RicercaValoriParametriQualitativi(3, piva);
    var valoriParamQualArray = [];
    for (i = 0; i < valoriParamQual.length; i++) {
        datiQual = { "qualita_cod": valoriParamQual[i].val_cod, "qualita_des": valoriParamQual[i].val_des };
        valoriParamQualArray.push(datiQual);
    }
    elencoQualitaAssegnaLotto = valoriParamQualArray;

}

function RicercaValoriParametriQualitativiCertif() {

    var valoriParamQual = RicercaValoriParametriQualitativi(12, piva);
    var valoriParamQualArray = [];
    for (i = 0; i < valoriParamQual.length; i++) {
        datiCertif = { "certif_cod": valoriParamQual[i].val_cod, "certif_des": valoriParamQual[i].val_des };
        valoriParamQualArray.push(datiCertif);
    }
    elencoCertificAssegnaLotto = valoriParamQualArray;
}

function RicercaLottoAssegna(options) {
    var param = "{piva: '" + piva + "' }"
    ajaxAgronica(indirizzohttp + "/Carica_Lotto_AssegnaxRisumSpeVarQualCert",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function SubmitLottoAssegna(options) {

    var grid = $("#tab_lotto_assegna").data("kendoGrid");
    var currentData = grid.dataSource.data();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheLottoAssegna(options.data.created) +
        controllaRigheLottoAssegna(options.data.updated);
    console.log(nrErr);

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
        var param = "{piva: '" + piva + "', righeInserite: '" + righeInserite + "', righeModificate: '" + righeModificate + "', righeCancellate: '" + righeCancellate + "', tutteleRighe: '" + tutteLerighe + "'}"

        ajaxAgronicaSync(indirizzohttp + "/AggiornaLotto_AssegnaxRisumSpeVarQualCert",
            param, false,
            function (risposta) {
                allOk = true;
                grid.dataSource._destroyed = [];
                grid.dataSource.read();
                grid.refresh();
                MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
            }, function (risposta) {
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
            });
    }
}

// Controllo campi obbligatori
function controllaRigheLottoAssegna(righe) {

    var nrErr = 0;

    for (x = 0; x < righe.length; x++) {

        item = righe[x];
        if (item.Cod_RisUm === 0 || item.Veg_Cod === 0 || item.Cul_Cod === 0 || item.lotto === "") {
            nrErr++;
        }
    }

    return nrErr;
}