function SelectPreventivoColtivazioni(config) {

    var parametri = kendo.stringify({
        Sementieri_Sportello_Configurazione_cod: config,
        Tipologia_Report: "Preventivo",
        Regione: "Emilia Romagna"
    });

    ajaxAgronicaSync("GST_MenuEstrazioni.aspx/LeggiColtivazioni", parametri, true,
        function (risposta) {
            if (risposta.RispostaOK) {
                jSonParsed_Kendo_PreventivoColtivazione = JSON.parse(risposta.RispostaStringa);
            }
        }, null);
}

function SelectInterferenze(config) {

    var parametri = kendo.stringify({
        Sementieri_Sportello_Configurazione_cod: config,
        Regione: "Emilia Romagna"
    });

    ajaxAgronicaSync("GST_MenuEstrazioni.aspx/LeggiInterferenze", parametri, true,
        function (risposta) {
            if (risposta.RispostaOK) {
                jSonParsed_Kendo_Interferenze = JSON.parse(risposta.RispostaStringa);
            }
        }, null);
}

function SelectVariazioni(config) {

    var parametri = kendo.stringify({
        Sementieri_Sportello_Configurazione_cod: config,
        Regione: "Emilia Romagna"
    });

    ajaxAgronicaSync("GST_MenuEstrazioni.aspx/LeggiVariazioni", parametri, true,
        function (risposta) {
            if (risposta.RispostaOK) {
                jSonParsed_Kendo_Variazioni = JSON.parse(risposta.RispostaStringa);
            }
        }, null);
}

function SelectConsuntivo(config) {

    var parametri = kendo.stringify({
        Sementieri_Sportello_Configurazione_cod: config,
        Tipologia_Report: "Consuntivo",
        Regione: "Emilia Romagna"
    });

    ajaxAgronicaSync("GST_MenuEstrazioni.aspx/LeggiColtivazioni", parametri, true,
        function (risposta) {
            if (risposta.RispostaOK) {
                jSonParsed_Kendo_Consuntivo = JSON.parse(risposta.RispostaStringa);
            }
        }, null);
}

function caricaListaSportelli() {

    ajaxAgronicaSync("GST_MenuEstrazioni.aspx/LeggiListaSportelli", null, true,
        function (risposta) {
            if (risposta.RispostaOK) {
                jSonParsed_Kendo_Sportelli = JSON.parse(risposta.RispostaStringa);
            }
        }, null);
}

function VerificaPermessiEstrazione() {
    ajaxAgronicaSync("GST_MenuEstrazioni.aspx/VerificaPermessiEstrazione", null, true,
        function (risposta) {
            if (risposta.RispostaOK) {
                permessiRagSoc = (risposta.RispostaStringa.toLowerCase() === 'true');
            }
        }, null);
}