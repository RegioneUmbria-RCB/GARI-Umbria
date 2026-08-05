
function Leggi_CausaleTrasporto(options) {
    var param = "{ piva: '" + piva + "' }";
    ajaxAgronicaSync(indirizzohttp + "/Leggi_CausaleTrasporto",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            for (var idx = 0; idx < risp.length; idx++) {
                var item = risp[idx];
                var tipoCausale = ImpostaTipoCausaleTrasporto(item.Tipo_Causale_Trasporto_Cod);
                item.Tipo_Causale_Trasporto_Cod = tipoCausale.Tipo_Cod;
                item.Tipo_Causale_Trasporto_Des = tipoCausale.Tipo_Des;
            }
            options.success(risp);
        },
        null);
}

function SubmitCausaleTrasporto(options) {
    var grid = $("#" + IDControllo).data("kendoGrid");
    var currentData = grid.dataSource.data();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var messageErr = "";
    messageErr += controllaRigheCompletePerSubmitCausaleTrasporto(options.data.created);
    messageErr += controllaRigheCompletePerSubmitCausaleTrasporto(options.data.updated);
    messageErr += controllaRigheUgualiCausaleTrasporto(currentData);

    if (messageErr != null && messageErr !== "") {
        MessaggioErrore_Bootstrap(messageErr, "DIV_Messaggi");
        erroreSubmitCausaleTrasporto(grid);
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

        ajaxAgronicaSync(indirizzohttp + "/AggiornaDaGrigliaCausaleTrasporto",
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
            erroreSubmitCausaleTrasporto(grid);
        }
    }
}

function controllaRigheCompletePerSubmitCausaleTrasporto(righe) {
    var messageErroreTotale = [];
    var grid = $("#" + IDControllo).data("kendoGrid");

    for (x = 0; x < righe.length; x++) {
        var messageErr = "";
        item = righe[x];

        if (isNullOrEmpty(item.Causale_Trasporto_Des)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "La descrizione della causale di trasporto non può essere vuota";
        }

        if (isNullOrEmpty(item.Causale_Trasporto_Sigla)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "La sigla della della causale di trasporto non può essere vuota";
        }

        if (messageErr !== "") {
            var errore = messageErr + " <br />";
            messageErroreTotale.push(errore);
            messageErr = "";
        }
    }

    return messageErroreTotale.join('\n');
}

function controllaRigheUgualiCausaleTrasporto(righe) {
    var result = "";
    var descrizioneDoppia = [];
    var siglaDoppia = [];

    for (var i = 0; i < righe.length; i++) {
        var item = righe[i];

        var doppioneDes = righe.filter((e) =>
            e.Causale_Trasporto_Cod != item.Causale_Trasporto_Cod &&
            e.Causale_Trasporto_Des.toLowerCase() == item.Causale_Trasporto_Des.toLowerCase());

        if (doppioneDes != undefined && doppioneDes != null && doppioneDes.length > 1) {
            var presente = descrizioneDoppia.find((e) => e.Causale_Trasporto_Des.toLowerCase() == item.Causale_Trasporto_Des.toLowerCase());
            if (presente == undefined || presente == null || presente.length == 0)
                descrizioneDoppia.push(item)
        }

        var doppioneSigla = righe.filter((e) =>
            e.Causale_Trasporto_Cod != item.Causale_Trasporto_Cod &&
            e.Causale_Trasporto_Sigla.toLowerCase() == item.Causale_Trasporto_Sigla.toLowerCase());

        if (doppioneSigla != undefined && doppioneSigla != null && doppioneSigla.length > 1) {
            var presente = siglaDoppia.find((e) => e.Causale_Trasporto_Sigla.toLowerCase() == item.Causale_Trasporto_Sigla.toLowerCase());
            if (presente == undefined || presente == null || presente.length == 0)
                siglaDoppia.push(item)
        }
    }

    if (descrizioneDoppia != undefined && descrizioneDoppia != null && descrizioneDoppia.length > 0) {
        for (var i = 0; i < descrizioneDoppia.length; i++) {
            var item = descrizioneDoppia[i];
            if (result != "")
                result += ";<br />";
            result += "La causale di trasporto '" + item.Causale_Trasporto_Des + "', risulta definita più volte";
        }
    }

    if (siglaDoppia != undefined && siglaDoppia != null && siglaDoppia.length > 0) {
        for (var i = 0; i < siglaDoppia.length; i++) {
            var item = siglaDoppia[i];
            if (result != "")
                result += ";<br />";
            result += "La sigla '" + item.Causale_Trasporto_Sigla + "' della causale di trasporto '" + item.Causale_Trasporto_Des + "', risulta definita più volte";
        }
    }

    return result;
}

function isNullOrEmpty(value) {
    var result = (value == null);
    result = result || (value == "");
    return result;
}

function erroreSubmitCausaleTrasporto(grid) {

    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        var dsSort = [];
        dsSort.push({ field: "Piva", dir: "asc" });
        dsSort.push({ field: "Causale_Trasporto_Cod", dir: "asc" });
        dsSort.push({ field: "Validita_Inizio", dir: "asc" });
        dsSort.push({ field: "Validita_Fine", dir: "asc" });

        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }

}