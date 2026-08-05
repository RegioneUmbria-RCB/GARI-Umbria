function ElencoIstitutiCredito() {
    var param = "{ piva: '" + piva + "' }";
    ajaxAgronicaSync(indirizzohttp + "/ElencoIstitutiCredito",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            elencoIstitutiCredito = risp;
        },
        null);
}

function Leggi_Liquidita(options) {
    var param = "{ piva: '" + piva + "' }";
    ajaxAgronicaSync(indirizzohttp + "/Leggi_Liquidita",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            for (var idx = 0; idx < risp.length; idx++) {
                var item = risp[idx];
                var risorsa = ImpostaRisorsaCausale(item.Risorsa_Cod);
                item.Risorsa_Cod = risorsa.risorsa_cod;
                item.Risorsa_Des = risorsa.risorsa_des;
                var istituto = ImpostaIstitutiCredito(item.Istituto_Cod);
                item.Istituto_Cod = istituto.Istituto_Cod;
                item.Istituto_Des = istituto.Istituto_Des;
                var chkDefault = ImpostaDefault(item.Default_Cod);
                item.Default_Cod = chkDefault.YesNo_Cod;
                item.Default_Des = chkDefault.YesNo_Des;
            }
            options.success(risp);
        },
        null);
}

function SubmitLiquidita(options) {
    var grid = $("#" + IDControllo).data("kendoGrid");
    var currentData = grid.dataSource.data();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var messageErr = "";
    messageErr += controllaRigheCompletePerSubmitLiquidita(options.data.created);
    messageErr += controllaRigheCompletePerSubmitLiquidita(options.data.updated);
    messageErr += controllaRigheUgualiLiquidita(currentData);

    if (messageErr != null && messageErr !== "") {
        MessaggioErrore_Bootstrap(messageErr, "DIV_Messaggi");
        erroreSubmitLiquidita(grid);
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var righeNonCancellate = [];

    for (var i = 0; i < currentData.length; i++) {
        currentData[i].Numero = NormalizzaNumeroConto(currentData[i].Numero);

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
            updatedRecords.push(currentData[i].toJSON());
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

        ajaxAgronicaSync(indirizzohttp + "/AggiornaDaGrigliaLiquidita",
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
            erroreSubmitLiquidita(grid);
        }
    }
}

function controllaRigheCompletePerSubmitLiquidita(righe) {
    var messageErroreTotale = [];
    var grid = $("#" + IDControllo).data("kendoGrid");

    for (x = 0; x < righe.length; x++) {
        var messageErr = "";
        item = righe[x];

        var istituto = ImpostaIstitutiCredito(item.Istituto_Cod);
        if (istituto == undefined || istituto == null) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Selezionare l'istituto di riferimento del conto corrente configurato";
        }

        if (isNullOrEmpty(item.Nazione)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "La nazione del conto corrente non può essere vuota";
        }
        else if (!(/[a-zA-Z]{2}/.test(item.Nazione))) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "La nazione del conto corrente deve avere essere formato da 2 lettere";
        }

        if (isNullOrEmpty(item.Cifre_Controllo)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Le cifre di controllo del conto corrente non possono essere vuota";
        }
        else if (!(/[0-9]{2}/.test(item.Cifre_Controllo))) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Le cifre di controllo del conto corrente deve avere essere formato da 2 numeri";
        }

        if (isNullOrEmpty(item.Cin)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Il Cin del conto corrente non può essere vuoto";
        }
        else if (!(/[a-zA-Z]{1}/.test(item.Cin))) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Il Cin del conto corrente deve essere formato da una lettera";
        }

        if (isNullOrEmpty(item.Abi)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "L'Abi del conto corrente non può essere vuoto";
        }
        else if (!(/[0-9]{5}/.test(item.Abi))) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "L'Abi del conto corrente deve avere essere formato da 5 numeri";
        }

        if (isNullOrEmpty(item.Cab)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Il Cab del conto corrente non può essere vuoto";
        }
        else if (!(/[0-9]{5}/.test(item.Cab))) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Il Cab del conto corrente deve avere essere formato da 5 numeri";
        }

        if (isNullOrEmpty(item.Numero)) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Il Numero del conto corrente non può essere vuoto";
        }
        else if (!(/[0-9]{1,12}/.test(item.Numero))) {
            if (messageErr !== "")
                messageErr += ";<br />";
            messageErr += "Il Numero del conto corrente deve avere essere formato da, almeno, 1 numero fino ad un massimo di 12";
        }

        if (messageErr !== "") {
            var errore = messageErr + " <br />";
            messageErroreTotale.push(errore);
            messageErr = "";
        }
    }

    return messageErroreTotale.join('\n');
}

function controllaRigheUgualiLiquidita(righe) {
    var result = "";
    var doppio = [];

    for (var i = 0; i < righe.length; i++) {
        var item = righe[i];

        var doppione = righe.filter((e) =>
            e.Cod_Liquidita != item.Cod_Liquidita &&
            e.Nazione.toLowerCase() == item.Nazione.toLowerCase() &&
            e.Cifre_Controllo == item.Cifre_Controllo &&
            e.Cin == item.Cin &&
            e.Abi == item.Abi &&
            e.Cab == item.Cab &&
            e.Numero == item.Numero
        );

        if (doppione != undefined && doppione != null && doppione.length > 1) {
            var presente = doppio.find((e) => e.Nazione.toLowerCase() == item.Nazione.toLowerCase() &&
                e.Cifre_Controllo == item.Cifre_Controllo &&
                e.Cin == item.Cin &&
                e.Abi == item.Abi &&
                e.Cab == item.Cab &&
                e.Numero == item.Numero);
            if (presente == undefined || presente == null || presente.length == 0)
                doppio.push(item)
        }
    }

    if (doppio != undefined && doppio != null && doppio.length > 0) {
        for (var i = 0; i < doppio.length; i++) {
            var item = doppio[i];
            if (result != "")
                result += ";<br />";
            result += "Il conto corrente con iban '" + item.Nazione.toLowerCase()
                + item.Cifre_Controllo + item.Cin + item.Abi + item.Cab + item.Numero
                + "', risulta definita più volte";
        }
    }

    return result;
}

function NormalizzaNumeroConto(numeroConto) {
    var result = numeroConto.padStart(12, "0")
    console.log(result);
    return result;
}

function isNullOrEmpty(value) {
    var result = (value == null);
    result = result || (value == "");
    return result;
}

function erroreSubmitLiquidita(grid) {

    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        var dsSort = [];
        dsSort.push({ field: "Piva", dir: "asc" });
        dsSort.push({ field: "Cod_Liquidita", dir: "asc" });
        dsSort.push({ field: "Validita_Inizio", dir: "asc" });
        dsSort.push({ field: "Validita_Fine", dir: "asc" });

        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }

}