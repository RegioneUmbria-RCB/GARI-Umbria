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

function Leggi_GruppiReferenze(options) {
    var param = "{piva: '" + piva + "' }"
    ajaxAgronica(indirizzohttp + "/Leggi_GruppiReferenze",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            Carica_MultiSelect_SpecieVarieta(risp);
            options.success(risp);
        }, null);
}

function Salva_GruppiReferenze(options) {
    var grid = $("#" + IDControllo).data("kendoGrid");
    var currentData = grid.dataSource.data();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var messageErr = "";
    messageErr += controllaRigheCompletePerSubmitGruppoReferenze(options.data.created);
    messageErr += controllaRigheCompletePerSubmitGruppoReferenze(options.data.updated);
    messageErr += controllaRigheDoppie(currentData);

    if (messageErr != "") {
        MessaggioErrore_Bootstrap(messageErr, "DIV_Messaggi");
        erroreSubmitGruppiReferenze(grid);
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

        ajaxAgronicaSync(indirizzohttp + "/AggiornaDaGrigliaGruppiReferenze",
            param, false,
            function (risposta) {
                allOk = true;
                grid.dataSource._destroyed = [];
                grid.dataSource.read();
                grid.refresh();
                MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
            }, null);

        if (!allOk) {
            erroreSubmitGruppiReferenze(grid);
        }
    }
}
function controllaRigheCompletePerSubmitGruppoReferenze(righe) {
    var result = "";

    for (var i = 0; i < righe.length; i++) {
        var item = righe[i];

        if ((item.OFiltro_Veg_Cod.length == 0) && (item.OFiltro_Cul_Cod.length > 0))
            result = AggiungiSegnalazione(result, "Per il gruppo referenze '" + item.Descrizione + "' son state configurate le varietà senza aver impostato la specie");
        else if ((item.OFiltro_Veg_Cod.length > 1) && (item.OFiltro_Cul_Cod.length > 0))
            result = AggiungiSegnalazione(result, "Per il gruppo referenze '" + item.Descrizione + "' son state configurate le varietà nonostante siano impostate più di una specie");
    }

    return result;
}

function controllaRigheDoppie(righe) {
    var result = "";
    var doppie = [];

    for (var i = 0; i < righe.length; i++) {
        var item = righe[i];

        var doppione = righe.filter((e) => e.Modulo_Cod == item.Modulo_Cod &&
            e.Descrizione.toLowerCase() == item.Descrizione.toLowerCase());

        if (doppione != undefined && doppione != null && doppione.length > 1) {
            var presente = doppie.find((e) => e.Modulo_Cod == item.Modulo_Cod &&
                e.Descrizione.toLowerCase() == item.Descrizione.toLowerCase());
            if (presente == undefined || presente == null || presente.length == 0)
                doppie.push(item)
        }
    }

    if (doppie != undefined && doppie != null && doppie.length > 0) {
        for (var i = 0; i < doppie.length; i++) {
            var item = doppie[i];
            result = AggiungiSegnalazione(result, "Il gruppo referenze '" + item.Descrizione + "' risulta definito più volte per il medesimo periodo di tempo");
        }
    }

    return result;
}

function erroreSubmitGruppiReferenze(grid) {

    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        var dsSort = [];
        dsSort.push({ field: "Modulo_Cod", dir: "asc" });
        dsSort.push({ field: "Id_Testata", dir: "asc" });
        dsSort.push({ field: "Validita_Inizio", dir: "asc" });
        dsSort.push({ field: "Validita_Fine", dir: "asc" });

        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }

}