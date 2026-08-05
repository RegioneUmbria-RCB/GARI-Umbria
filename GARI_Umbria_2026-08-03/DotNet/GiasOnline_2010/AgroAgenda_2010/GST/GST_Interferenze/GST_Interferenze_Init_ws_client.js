function SalvaInterferenzeWS() {

    ajaxAgronicaSync("GST_Interferenze_Init.aspx/SalvaInterferenze", null, true,
        function (risposta) {
            if (risposta.RispostaOK) {
                kendo.alert(risposta.RispostaStringa);
            }
        }, null);
}

function VerificaPermessiEstrazione() {
    ajaxAgronicaSync("GST_Interferenze_Init.aspx/VerificaPermessiEstrazione", null, true,
        function (risposta) {
            if (risposta.RispostaOK) {
                if (risposta.RispostaStringa.toLowerCase() === 'true') {
                    $("#rowBtnEstrazioni").css("display", "block")
                }
            }
        }, null);
}