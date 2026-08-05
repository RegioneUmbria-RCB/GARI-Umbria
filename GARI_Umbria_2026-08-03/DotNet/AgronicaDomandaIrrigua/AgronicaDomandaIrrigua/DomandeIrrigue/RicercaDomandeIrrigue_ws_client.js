const { start } = require("@popperjs/core");

function LeggiAziende(options) {
    let parametri = kendo.stringify({ "objP_server" : objP_server, "objP_utenti": objP_utenti });

    ajaxAgronicaSync(pathCoreWS + "/Anagrafica/Imprese.asmx/LeggiImpreseConFiltroUtente",
        parametri,
        false,
        function (risposta) {
            let data = JSON.parse(risposta.RispostaStringa);
            options.success(data);
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
        }
    );
}

function LeggiElencoDomanderrigue() {

    let parametri = kendo.stringify({ "data": kendo.stringify(getRicercaPars()) });

    ajaxAgronicaSync(indirizzohttp + "/LeggiElencoDomandeIrrigue",
        parametri,
        false,
        function (risposta) {
            $(cIdDatiElencoDomande).val(risposta.RispostaStringa);
            popolaGriglia("divKendoOut");
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
        }
    );
}

function ApriSchedaDomanda(id) {
    let parOri = kendo.stringify(getRicercaPars());
    let parametri = kendo.stringify({ "id": id , "dataOri" : parOri });

    ajaxAgronicaSync(indirizzohttp + "/ApriSchedaDomanda",
        parametri,
        false,
        function (risposta) {
            let risp = risposta.RispostaStringa;
            window.location.href = risp;
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
        }
    );
}

function getRicercaPars() {
    let startyear = $('#txtDaAnno').data("kendoNumericTextBox").value();
    if (startyear == null) {
        startyear = 0;
    }

    let endyear = $('#txtAAnno').data("kendoNumericTextBox").value();
    if (endyear == null) {
        endyear = 0;
    }
    let par = {
        "StartYear": startyear, "EndYear": endyear, "elencoPiva": KendoMultisel("cmbAziende").value()
    };
    return par;
}