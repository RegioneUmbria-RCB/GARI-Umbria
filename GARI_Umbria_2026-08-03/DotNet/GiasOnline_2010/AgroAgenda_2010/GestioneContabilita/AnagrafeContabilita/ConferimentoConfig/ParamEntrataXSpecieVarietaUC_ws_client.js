function Leggi_ParamEntrataXSpecieVarieta(options)
{
    var param = "{piva: '" + piva + "' }"
    ajaxAgronica(indirizzohttp + "/Leggi_ParamEntrataXSpecieVarieta",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

//Bottone Salva Modifiche
function SubmitParamEntrataXSpecieVarieta(options) { 
    var grid = $("#tab_parametri_specie_varieta").data("kendoGrid");
    var currentData = grid.dataSource.data();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var messageErr = "";
    messageErr += controllaRigheCompletePerSubmitParamEntrata(options.data.created);
    messageErr += controllaRigheCompletePerSubmitParamEntrata(options.data.updated);

    if (messageErr === "") {
        messageErr = controllaRigheUguali(currentData);
    }

    if (messageErr != null) {
        MessaggioErrore_Bootstrap(messageErr, "DIV_Messaggi");
        erroreSubmitParamEntrataXSpecieVarieta(grid);
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
        var param = "{piva: '" + $(cIdPiva).val() + "', righeInserite: '" + righeInserite + "', righeModificate: '" + righeModificate + "', righeCancellate: '" + righeCancellate + "',tutteleRighe:'" + Tuttelerighe+"' }"

        ajaxAgronicaSync(indirizzohttp + "/AggiornaDaGrigliaParamEntrataXSpecieVarieta",
            param, false,
            function (risposta) {
                allOk = true;
                grid.dataSource._destroyed = [];
                allOk=true
                grid.dataSource.read();
                grid.refresh();
                MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
            }, null);

        if (!allOk) {
            erroreSubmitParamEntrataXSpecieVarieta(grid);
        }
    }
}

function erroreSubmitParamEntrataXSpecieVarieta(grid) {

    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        var dsSort = [];
        dsSort.push({ field: "Veg_Des", dir: "asc" });
        dsSort.push({ field: "Cul_Des", dir: "asc" });
        dsSort.push({ field: "Validita_Inizio", dir: "asc" });
        dsSort.push({ field: "Validita_Fine", dir: "asc" });

        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }

}

function controllaRigheCompletePerSubmitParamEntrata(righe) {

    var messageErr = "";
    var grid = $("#tab_parametri_specie_varieta").data("kendoGrid");

    for (x = 0; x < righe.length; x++) {

        item = righe[x];
        //almeno specie deve essere scelta
        if (item.Veg_Cod > -1) {
            //almeno degrado o riferimento prezzo o formula liquidazione devono essere scelti
            if ((item.Riferimento_Prezzi_Des === " " || item.Riferimento_Prezzi_Des === "") && (item.Percentuale_Degrado <= 0 || item.Percentuale_Degrado >= 100) && (parseInt(item.FormulaFissaLiquidazione_Cod) === 0 || item.FormulaFissaLiquidazione_Cod === "")) {
                messageErr += "Inserire almeno la Formula di Liquidazione o il Riferimento Prezzo o il  % Degrado nella riga con specie " + item.Veg_Des + " e validità " + formattedDate(item.Validita_Inizio, "/") + " - " + formattedDate(item.Validita_Fine, "/") + "<br/>";
            }
            else {
                if (item.Riferimento_Prezzi_Des === "") {
                    if ((item.Percentuale_Degrado <= 0 || item.Percentuale_Degrado >= 100) && (parseInt(item.FormulaFissaLiquidazione_Cod) === 0 || item.FormulaFissaLiquidazione_Cod === "")) {
                        if (item.Cul_Cod == 0) {
                            messageErr += "Riferimento Prezzo non corretto alla riga con specie " + item.Veg_Des + " e validità " + formattedDate(item.Validita_Inizio, "/") + " - " + formattedDate(item.Validita_Fine, "/") + "<br/>";
                        }
                        else {
                            messageErr += "Riferimento Prezzo non corretto alla riga con specie " + item.Veg_Des + ",varietà " + item.Cul_Des + " e validità " + formattedDate(item.Validita_Inizio, "/ ") + " - " + formattedDate(item.Validita_Fine, " / ") + " <br/>";
                        }
                    }
                }
                if (item.Percentuale_Degrado <= 0 || item.Percentuale_Degrado >= 100) {
                    if (item.Riferimento_Prezzi_Des === "" && (parseInt(item.FormulaFissaLiquidazione_Cod) === 0 || item.FormulaFissaLiquidazione_Cod === "")) {
                        if (item.Cul_Cod == 0) {
                            messageErr += "Valore di % Degrado non corretto alla riga con specie " + item.Veg_Des + " e validità " + formattedDate(item.Validita_Inizio, "/") + " - " + formattedDate(item.Validita_Fine, "/") + "<br/>";
                        }
                        else {
                            messageErr += "Valore di % Degrado non corretto alla riga con specie " + item.Veg_Des + ",varietà " + item.Cul_Des + " e validità " + formattedDate(item.Validita_Inizio, "/ ") + " - " + formattedDate(item.Validita_Fine, " / ") + " <br/>";
                        }
                    }
                }
                if (parseInt(item.FormulaFissaLiquidazione_Cod) === 0 || item.FormulaFissaLiquidazione_Cod === "") {
                    if (item.Riferimento_Prezzi_Des === "" && (item.Percentuale_Degrado <= 0 || item.Percentuale_Degrado >= 100)) {
                        if (item.Cul_Cod == 0) {
                            messageErr += "Formula da applicare in Liquidazione non corretta alla riga con specie " + item.Veg_Des + " e validità " + formattedDate(item.Validita_Inizio, "/") + " - " + formattedDate(item.Validita_Fine, "/") + "<br/>";
                        }
                        else {
                            messageErr += "Formula da applicare in Liquidazione non corretta alla riga con specie " + item.Veg_Des + ",varietà " + item.Cul_Des + " e validità " + formattedDate(item.Validita_Inizio, "/ ") + " - " + formattedDate(item.Validita_Fine, " / ") + " <br/>";
                        }
                    }
                }
            }
            
        }
        else {
            messageErr += "Inserire almeno la specie" + "<br/>";
        }
    }
    return messageErr;
}

function controllaRigheUguali(currentData) { 
    var messageErr = "";
    let Veg_Cod_ = 0;
    let Veg_Cod_2 = 0;
    let Cul_Cod_ = 0;
    let Cul_Cod_2 = 0;
    let Regolamento_Cod_ = 0;
    let Regolamento_Cod_2 = 0;
        //Controllo che non ci siano due righe con la stessa specie , varieta ,regolamento , data inizio e fine
    //Da fare anche lato server  
    for (x = 0; x < currentData.length; x++) {
        item = currentData[x];
        for (y = 0; y < currentData.length; y++) {
            item2 = currentData[y];
            Veg_Cod_ = parseInt(item.Veg_Cod);
            Veg_Cod_2 = parseInt(item2.Veg_Cod);
            Cul_Cod_ = parseInt(item.Cul_Cod);
            Cul_Cod_2 = parseInt(item2.Cul_Cod);
            Regolamento_Cod_ = parseInt(item.Regolamento_Cod);
            Regolamento_Cod_2 = parseInt(item2.Regolamento_Cod);
            if ((x != y) && ((Veg_Cod_ === Veg_Cod_2) && (Cul_Cod_ === Cul_Cod_2) && (Regolamento_Cod_ === Regolamento_Cod_2)) &&
                (String(item.Validita_Inizio) === String(item2.Validita_Inizio)) && (String(item.Validita_Fine) === String(item2.Validita_Fine))) {
                foundErr = true;
                if (item.Cul_Des !== "" && item.Regolamento_Des !== "") {
                    return messageErr += "Esistono due righe uguali: Specie " + item.Veg_Des + ",Varietà " + item.Cul_Des + " ,Regolamento " + item.Regolamento_Des + ",Validità " + formattedDate(item.Validita_Inizio, "/") + " - " + formattedDate(item.Validita_Fine, "/") + "";
                }
                else {
                    if (item.Cul_Des === "" && item.Regolamento_Des === "") {
                        return messageErr += "Esistono due righe uguali: Specie " + item.Veg_Des + " e Validità " + formattedDate(item.Validita_Inizio, "/") + " - " + formattedDate(item.Validita_Fine, "/") + "";
                    }
                    else {
                        if (item.Cul_Des !== "") {
                            return messageErr += "Esistono due righe uguali: Specie " + item.Veg_Des + ",Varietà " + item.Cul_Des + " ,Validità " + formattedDate(item.Validita_Inizio, "/") + " - " + formattedDate(item.Validita_Fine, "/") + "";
                        }
                        else if (item.Regolamento_Des !== "") {
                            return messageErr += "Esistono due righe uguali: Specie " + item.Veg_Des + " ,Regolamento " + item.Regolamento_Des + " ,Validità " + formattedDate(item.Validita_Inizio, "/") + " - " + formattedDate(item.Validita_Fine, "/") + "";
                        }
                    }

                }

            }
        }
    }
}
 