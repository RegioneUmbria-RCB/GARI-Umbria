function SalvaVariazioniWS(config, elencoVariazioni) {

    if (elencoVariazioni == undefined) elencoVariazioni = "";

    var parametri = kendo.stringify({
        Sportello: config,
        ElencoVariazioni: elencoVariazioni
    });

    ajaxAgronicaSync("GST_Menu.aspx/SalvaVariazioni", parametri, true,
        function (risposta) {
            if (risposta.RispostaOK) {
                kendo.alert(risposta.RispostaStringa);
            }
        }, null);
}

function VerificaPermessiEstrazione() {
    ajaxAgronicaSync("GST_Menu.aspx/VerificaPermessiEstrazione", null, true,
        function (risposta) {
            if (risposta.RispostaOK) {
                if (risposta.RispostaStringa.toLowerCase() === 'true') {
                    $("#btnSalvaVariazioni").css("display", "block")
                }
            }
        }, null);
}