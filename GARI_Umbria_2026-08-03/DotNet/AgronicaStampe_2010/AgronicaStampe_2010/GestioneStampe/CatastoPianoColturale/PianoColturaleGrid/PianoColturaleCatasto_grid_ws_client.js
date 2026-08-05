var indirizzohttp = "PianoColturaleCatasto_grid.aspx/";


async function leggiGrid() {
    WaitFrame.show();
    return new Promise((resolve, reject) => {
        ajaxAgronica(indirizzohttp + "Stampa_PianoColturale", {},
            function (risposta) {
                let idControllo = "#ppKendoPianoColturale";
                let hdPianoColturale = "#hdKendoPianoColturale";
                $("#hdKendoPianoColturale").val(risposta.RispostaStringa);
                WaitFrame.show();
                kendoGridPianoColturale(idControllo, hdPianoColturale);
                resolve();

            }, () => reject(), false);
    }) 
}

function CaricaComboCmb_Report(options) {
    let parametri = kendo.stringify({ "tabReport": tabReport });
    ajaxAgronica(indirizzohttp + "ListaReport", parametri, 
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null, null, true);

}

async function leggiReport(nomeReport, aggiorna) {
    return new Promise((resolve, reject) => {
        let parametri = kendo.stringify({ "tabReport": tabReport, "nomeReport": nomeReport });
        ajaxAgronica(indirizzohttp + "LeggiReport", parametri,
            function (risposta) {
                var params = JSON.parse(risposta.RispostaStringa);
                resolve(params);
            },
            function (risposta) {
                console.log("Errore nella lettura del report: " + risposta.Errore);
            }, null, false);
    });
}

function cancellaReport(nomeReport) {

    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({ "tabReport": tabReport, "nomeReport": nomeReport });
        ajaxAgronica(indirizzohttp + "CancellaReport", parametri,
            function (risposta) {
                var risp = risposta.RispostaStringa;
                Cmb_Report.dataSource.read();
                Cmb_Report.refresh();
                resolve(true);
            },
            function (risposta) {
                console.log("Errore nella cancellazione del report: " + risposta.Errore);
                reject(false);
            }, null, false);
    });
}

function salvaReport(parametriReport) {
    return new Promise((resolve, reject) => {
        let parametri = kendo.stringify({ "tabReport": tabReport, "report": kendo.stringify(parametriReport) });
        ajaxAgronica(indirizzohttp + "SalvaReport", parametri,
            function (risposta) {
                resolve(true);
            },
            function (risposta) {
                console.log("Errore nel salvataggio del report: " + risposta.Errore);
                reject(false);
            }, null, false);
    });
}

