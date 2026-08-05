function Carica_Elenco_GruppiReferenze() {
    var param = "{ piva: '" + piva + "' }";
    ajaxAgronicaSync(indirizzohttp + "/Leggi_Elenco_GruppiReferenze",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            elencoTestate = risp;
        }, null);
}

function Carica_Elenco_ParametriQualitativi() {
    var param = "{ piva: '" + piva + "' }";
    ajaxAgronicaSync(indirizzohttp + "/Leggi_Elenco_ParametriQualitativi",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            elencoParametriQualitativi = risp;
        }, null);
}


function Leggi_ParametriQualitativiXReferenza(options) {
    var param = "{piva: '" + piva + "' }"
    ajaxAgronica(indirizzohttp + "/Leggi_ParametriQualitativiXReferenza",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            for (var i = 0; i < risp.length; i++) {
                var item = risp[i];

                item.Tabella_Key_Rif_Des = "";
                if (item.Tabella_Key_Rif != undefined && item.Tabella_Key_Rif != null && item.Tabella_Key_Rif != '') {
                    var param = risp.find((e) => e.Tabella_Key == item.Tabella_Key_Rif);
                    item.Tabella_Key_Rif_Des = param.Tabella_Key;
                }

                var yesNo = { Cod: item.ChkReferenza, Des: item.ChkReferenzaDes };
                ImpostaYesNo(yesNo);
                item.ChkReferenzaDes = yesNo.Des;

                var yesNo = { Cod: item.ChkObbligatorio, Des: item.ChkObbligatorioDes };
                ImpostaYesNo(yesNo);
                item.ChkObbligatorioDes = yesNo.Des;
            }
            options.success(risp);
        }, null);
}

function Salva_ParametriQualitativiXReferenza(options) {
    var grid = $("#" + IDControllo).data("kendoGrid");
    var currentData = grid.dataSource.data();


    var groupByTestata = raggruppaPerTestata(currentData);

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var messageErr = controllaRigheCompletePerSubmitParametriQualitativiXReferenza(groupByTestata);

    if (messageErr != null && messageErr !== "") {
        MessaggioErrore_Bootstrap(messageErr, "DIV_Messaggi");
        erroreSubmitPParametriQualitativiXReferenza(grid);
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

        ajaxAgronicaSync(indirizzohttp + "/AggiornaDaGrigliaParametriQualitativiXReferenza",
            param, false,
            function (risposta) {
                allOk = true;
                grid.dataSource._destroyed = [];
                grid.dataSource.read();
                grid.refresh();
                MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
            }, null);

        if (!allOk) {
            erroreSubmitPParametriQualitativiXReferenza(grid);
        }
    }
}

function erroreSubmitPParametriQualitativiXReferenza(grid) {
    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        var dsSort = [];
        dsSort.push({ field: "Id_Testata", dir: "asc" });
        dsSort.push({ field: "Ordine", dir: "asc" });

        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }

}

function raggruppaPerTestata(righe) {
    var result = [];
    for (var i = 0; i < righe.length; i++) {
        var item = righe[i];
        var presente = result.find((e) => e.Id_Testata == item.Id_Testata);
        if (presente == undefined) {
            presente = { Id_Testata: item.Id_Testata, Id_Testata_Des: item.Id_Testata_Des, Parametri: [] };
            result.push(presente);
        }
        presente.Parametri.push(item);
    }
    return result;
}

function controllaRigheCompletePerSubmitParametriQualitativiXReferenza(groupByTestata) {
    var messageErr = "";

    for (var i = 0; i < groupByTestata.length; i++) {
        var idTestata = groupByTestata[i].Id_Testata;
        var descTestata = groupByTestata[i].Id_Testata_Des;
        var parametri = groupByTestata[i].Parametri;

        messageErr += verificaChiaveParametroQualitativoXReferenza(parametri);
        messageErr += controllaParametriRiferimento(idTestata, descTestata, parametri);
        messageErr += controllaRigheUgualiXReferenza(idTestata, descTestata, parametri);
        messageErr += controllaOrdineParametri(idTestata, descTestata, parametri);

    }

    return messageErr;
}

function verificaChiaveParametroQualitativoXReferenza(parametri) {
    var messageErr = "";
    var parametriErrati = [];

    for (var p = 0; p < parametri.length; p++) {
        var item = parametri[p];
        if (item.IdParam != null && item.IdParam != '') {
            var chiave = item.Piva + '_' + item.Id_Testata + '_1_' + item.Tabella_ID;
            if (item.IdParam != chiave) {
                parametriErrati.push("Non è possibile spostare il parametro '" + item.Tabella_Key + "' sulla referenza '" + item.Id_Testata_Des + "'.");
            }
        }
    }

    for (var m = 0; m < parametriErrati.length; m++)
        messageErr = AggiungiSegnalazioneParametroQualitativoXReferenza(parametriErrati[m]);

    return messageErr;
}

function controllaParametriRiferimento(idTestata, descTestata, parametri) {
    var messageErr = "";

    var parametriReferenziati = parametri.filter((e) => e.Tabella_Key_Rif != "");
    var parametriDoppi = [];
    for (var i = 0; i < parametriReferenziati.length; i++) {
        var item = parametriReferenziati[i];
        var presente = (parametriDoppi.indexOf(item.Tabella_Key_Rif_Des) > -1);
        if (!presente) {
            var doppio = parametriReferenziati.filter((e) =>
                idTestata == item.Id_Testata &&
                e.Tabella_Key_Rif == item.Tabella_Key_Rif).length;
            if (doppio > 1) {
                parametriDoppi.push(item.Tabella_Key_Rif_Des);
            }
        }
    }
    if (parametriDoppi.length > 0) {
        messageErr += "Per la testata '" + descTestata + "', i seguenti riferiemnti ad altri paramentri risultano essere impostati più volte:<br />";
        messageErr += parametriDoppi.join(", ");
    }

    return AggiungiSegnalazioneParametroQualitativoXReferenza(messageErr);
}

function controllaRigheUgualiXReferenza(idTestata, descTestata, parametri) {
    var messageErr = "";

    var parametriDoppi = [];
    for (var i = 0; i < parametri.length; i++) {
        var item = parametri[i];
        var presente = (parametriDoppi.indexOf(item.Tabella_Key) > -1);
        if (!presente) {
            var doppio = parametri.filter((e) =>
                idTestata == item.Id_Testata &&
                e.Tabella_ID == item.Tabella_ID).length;
            if (doppio > 1) {
                parametriDoppi.push(item.Tabella_Key);
            }
        }
    }
    if (parametriDoppi.length > 0) {
        messageErr += "Per la testata '" + descTestata + "', i seguenti paramentri risultano essere impostati più volte:<br />";
        messageErr += parametriDoppi.join(", ");
    }

    return AggiungiSegnalazioneParametroQualitativoXReferenza(messageErr);
}

function controllaOrdineParametri(idTestata, descTestata, parametri) {
    var messageErr = "";

    var nonValidi = parametri.filter((e) =>
        idTestata == e.Id_Testata &&
        (e.Ordine <= 0 || e.Ordine > 100));

    if (nonValidi.length > 0) {
        messageErr += "Per la testata '" + descTestata + "', i seguenti paramentri risultano avere un ordine non valido:<br />";
        messageErr += nonValidi
            .map((e) => "Parametro: '" + e.Tabella_Key + "' - Ordine: " + e.Ordine)
            .join(",<br />");
    }
    else {
        var ordiniDoppi = [];
        for (var i = 0; i < parametri.length; i++) {
            var item = parametri[i];
            var doppi = parametri.filter((e) =>
                idTestata == item.Id_Testata &&
                e.Ordine == item.Ordine);

            if (doppi.length > 1) {
                for (var d = 0; d < doppi.length; d++) {
                    var presente = ordiniDoppi.find((e) =>
                        e.Id_Testata == item.Id_Testata &&
                        e.Tabella_Key == item.Tabella_Key);
                    if (presente == undefined) {
                        ordiniDoppi.push(item);
                    }
                }
            }
        }
        if (ordiniDoppi.length > 0) {
            messageErr += "Per la testata '" + descTestata + "', i seguenti paramentri risultano avere lo stesso ordine:<br />";
            messageErr += ordiniDoppi
                .map((e) => "Parametro: '" + e.Tabella_Key + "' - Ordine: " + e.Ordine)
                .join(",<br />");
        }
    }
    
    return AggiungiSegnalazioneParametroQualitativoXReferenza(messageErr);
}

function AggiungiSegnalazioneParametroQualitativoXReferenza(messageErr) {
    if (messageErr != undefined && messageErr != null && messageErr != "")
        return messageErr + '<br />';
    else
        return messageErr;
}