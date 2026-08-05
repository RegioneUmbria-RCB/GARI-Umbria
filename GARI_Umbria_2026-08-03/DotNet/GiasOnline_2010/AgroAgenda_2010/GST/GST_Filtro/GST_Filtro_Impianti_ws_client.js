function SalvaEstrazioneWS(which) {

    var parametri = kendo.stringify({
        tipoEstrazione: which
    });

    ajaxAgronicaSync("GST_Filtro_Impianti.aspx/SalvaEstrazione", parametri, true,
        function (risposta) {
            if (risposta.RispostaOK) {
                kendo.alert(risposta.RispostaStringa);
            }
        }, null);
}

function VerificaPermessiEstrazione() {
    ajaxAgronicaSync("GST_Filtro_Impianti.aspx/VerificaPermessiEstrazione", null, true,
        function (risposta) {
            if (risposta.RispostaOK) {
                if (!(risposta.RispostaStringa.toLowerCase() === 'true')) {
                    $("#rowBtnEstrazioni").css("display", "none")
                }
            }
        }, null);
}

function mostraSportello() {
    ajaxAgronicaSync("GST_Filtro_Impianti.aspx/InfoSportello", null, true,
        function (risposta) {
            if (risposta.RispostaOK) {
                var infoSportello = JSON.parse(risposta.RispostaStringa);
                $("#spanSportello").append("<b>SPORTELLO:</b> " + infoSportello.sportello_des);
                $("input[id$='Txt_DataInizioImpianto']").datepicker("setValue", infoSportello.data_inizio);
                $("input[id$='Txt_DataFineImpianto']").datepicker("setValue", infoSportello.data_fine);
            }
        }, null);
}