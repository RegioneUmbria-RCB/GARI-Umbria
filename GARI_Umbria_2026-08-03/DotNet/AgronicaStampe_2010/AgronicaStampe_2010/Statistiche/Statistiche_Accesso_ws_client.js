var indirizzohttp = "./Statistiche_Accesso.aspx";
var pathNetCoreApi = null;

function LeggiPermessiBottoni() {

    let risp = null;
    ajaxAgronicaSync(indirizzohttp + "/GetPermessiBottoni",
        kendo.stringify({}), false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
        },
        function (risposta) {
            console.log("ERRORE Statistiche Utilizzo: impossibile ottenere i permessi dei bottoni: " + risposta.Errore);
        });
    return risp;
}

function LeggiPathNetCoreApi() {

    let risp = null;
    ajaxAgronicaSync(indirizzohttp + "/LeggiPathNetCoreApi",
        kendo.stringify({}), false,
        function (risposta) {
            risp = risposta.RispostaStringa;
        },
        function (risposta) {
            console.log("ERRORE Statistiche Utilizzo: impossibile ottenere indirizzo per chiamata API: " + risposta.Errore);
        });
    return risp;
}

function CaricaRicercaRapida() {
    return new Promise((resolve, reject) => {
        ajaxAgronica(indirizzohttp + "/caricaAziende",
            {},
            function (risposta) {
                let result = JSON.parse(risposta.RispostaStringa);
                resolve(result);
            }, function (risposta) { reject(); gestioneErrore(risposta.RispostaStringa) });
    })
}
function getDataGrafici() {
    let v_inizio = get_data("DataValiditaInizio");
    let v_fine = get_data("DataValiditaFine");
    let parametri = {
        "v_inizio": v_inizio == null ? '' : v_inizio,
        "v_fine": v_fine == null ? '' : v_fine,
        "piva": Get_KendoDDLValue("ddRicercaRapida"),
        "tipoFiltro": getTipoFiltro()
    }
    return new Promise((resolve, reject) => {
        ajaxAgronica(indirizzohttp + "/getDataGrafici",
            JSON.stringify(parametri),
            function (risposta) {
                let result = JSON.parse(risposta.RispostaStringa);
                resolve(result);
            }, function (risposta) { reject(); gestioneErrore(risposta.RispostaStringa) });
    })
}

function getDataTime(id, defaultValue) {
    let result = get_data(id);
    if (result == null)
        result = defaultValue;
    return result;
}

function setFiltroChiamata(conDettagli) {
    let v_inizio = getDataTime("DataValiditaInizio", "1900-01-01T00:00:00.000Z");
    let v_fine = getDataTime("DataValiditaFine", "2100-12-01T00:00:00.000Z");
    let piva = Get_KendoDDLValue("ddRicercaRapida");
    if (piva == '')
        piva = null;

    let parametri = {
        "FiltroPerDataCompetenzaOrDataRegistrazione": getTipoFiltro(),
        "IntervalloOperazioniDiCampagna": {
            "inizio": v_inizio,
            "fine": v_fine
        },
        "IntervalloPratiche": {
            "inizio": v_inizio,
            "fine": v_fine
        },
        "Username": null,
        "piva": piva,
        "DettagliQdC": conDettagli
    }
    return parametri;
}

function getElencoSinteticoMovimenti() {
    let v_inizio = get_data("DataValiditaInizio");
    let v_fine = get_data("DataValiditaFine");
    let parametri = {
        "v_inizio": v_inizio == null ? '' : v_inizio,
        "v_fine": v_fine == null ? '' : v_fine,
        "piva": Get_KendoDDLValue("ddRicercaRapida"),
        "tipoFiltro": getTipoFiltro()
    }

    return new Promise((resolve, reject) => {
        ajaxAgronica(indirizzohttp + "/getElencoSinteticoMovimenti",
            JSON.stringify(parametri),
            function (risposta) {
                if (risposta.RispostaStringa != '') {
                    let risp = JSON.parse(risposta.RispostaStringa);
                    let fileDati = risp.File;
                    let nomeFile = risp.NomeFile;
                    let estensione = risp.Estensione;

                    SaveAndOpenFileByteArray(nomeFile, fileDati, estensione);
                }
            }, function (risposta) {
                reject();
                gestioneErrore(risposta.RispostaStringa)
            });
    })

}

/**
 * funzione per ottenere l'elenco sintetico dei movimenti tramite NETCoreAPI. AL MOMENTO NON UTILIZZATA
 */
function getElencoSinteticoMovimenti_NetCoreAPI() {
    let parametri = setFiltroChiamata(false);
    return new Promise((resolve, reject) => {
        ajaxAgronicaApiCoreStdPostAsync(pathNetCoreApi + "/Statistiche/StatisticheUtilizzo",
            JSON.stringify(parametri),
            function (risposta) {
                if (risposta.RispostaStringa != '')
                    scaricaFileExcel(JSON.parse(risposta.RispostaStringa))
            },
            function (risposta) {
                reject();
                gestioneErrore(risposta.RispostaStringa);
            },
            true
        );
    })
}

function getReportDettagli() {
    let v_inizio = get_data("DataValiditaInizio");
    let v_fine = get_data("DataValiditaFine");
    let parametri = {
        "v_inizio": v_inizio == null ? '' : v_inizio,
        "v_fine": v_fine == null ? '' : v_fine,
        "piva": Get_KendoDDLValue("ddRicercaRapida"),
        "tipoFiltro": getTipoFiltro()
    }
    return new Promise((resolve, reject) => {
        ajaxAgronica(indirizzohttp + "/getReportDettagliServiziPerAzienda",
            JSON.stringify(parametri),
            function (risposta) {
                if (risposta.RispostaStringa != '') {
                    let risp = JSON.parse(risposta.RispostaStringa);
                    let fileDati = risp.File;
                    let nomeFile = risp.NomeFile;
                    let estensione = risp.Estensione;

                    SaveAndOpenFileByteArray(nomeFile, fileDati, estensione);
                }
                resolve();
            }, function (risposta) {
                reject();
                gestioneErrore(risposta.RispostaStringa)
            });
    })
}

function getElencoSinteticoDettagliOperazioni() {
    let v_inizio = get_data("DataValiditaInizio");
    let v_fine = get_data("DataValiditaFine");
    let parametri = {
        "v_inizio": v_inizio == null ? '' : v_inizio,
        "v_fine": v_fine == null ? '' : v_fine,
        "piva": Get_KendoDDLValue("ddRicercaRapida"),
        "tipoFiltro": getTipoFiltro()
    }
    return new Promise((resolve, reject) => {
        ajaxAgronica(indirizzohttp + "/getElencoSinteticoMovimentiConDettagli",
            JSON.stringify(parametri),
            function (risposta) {
                if (risposta.RispostaStringa != '') {
                    let risp = JSON.parse(risposta.RispostaStringa);
                    let fileDati = risp.File;
                    let nomeFile = risp.NomeFile;
                    let estensione = risp.Estensione;

                    SaveAndOpenFileByteArray(nomeFile, fileDati, estensione);
                }
            }, function (risposta) {
                reject();
                gestioneErrore(risposta.RispostaStringa)
            });
    })
}

/**
 * funzione per ottenere l'elenco sintetico dei dettegali delle operazioni tramite NETCoreAPI. AL MOMENTO NON UTILIZZATA
 */
function getElencoSinteticoDettagliOperazioni_NetCoreAPI() {
    let parametri = setFiltroChiamata(true);
    return new Promise((resolve, reject) => {
        ajaxAgronicaApiCoreStdPostAsync(pathNetCoreApi + "/Statistiche/StatisticheUtilizzo",
            JSON.stringify(parametri),
            function (risposta) {
                if (risposta.RispostaStringa != '')
                    scaricaFileExcel(JSON.parse(risposta.RispostaStringa))
            },
            function (risposta) {
                reject();
                gestioneErrore(risposta.RispostaStringa);
            },
            true
        );
    })
}

function gestioneErrore(risposta) {
    MessaggioErrore_Bootstrap('Errore: ' + risposta + ' <br/>', "DIV_Messaggi");
}

function scaricaFileExcel(risp) {
    let fileDati = risp.File;
    let nomeFile = risp.NomeFile + risp.Estensione;
    let estensione = risp.Estensione;

    SaveAndOpenFileByteArray(nomeFile, fileDati, estensione);
}