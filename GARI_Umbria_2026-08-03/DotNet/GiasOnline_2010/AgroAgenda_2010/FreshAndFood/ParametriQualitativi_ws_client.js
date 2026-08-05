var indirizzohttp = "./ParametriQualitativi.aspx";

function Leggi_ModuliAnagrafeAttivi(options) {

    var param = kendo.stringify({
        piva: $(cIdPiva).val()
    });

    ajaxAgronica(indirizzohttp + "/Leggi_ModuliAnagrafeAttivi",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function CaricaGrigliaTestata_ParametriQualitativi(options, parametriDiLettura) {

    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        modulo_generazione: parametriDiLettura[0]
    });

    ajaxAgronica(indirizzohttp + "/CaricaGrigliaTestata_ParametriQualitativi",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);

}

function CaricaGrigliaDettagli_ParametriQualitativi(options, parametriDiLettura) {

    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        modulo_generazione: parametriDiLettura[0],
        id_testata: parametriDiLettura[1]
    });

    ajaxAgronica(indirizzohttp + "/CaricaGrigliaDettagli_ParametriQualitativi",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            for (var x = 0; x < risp.length; x++) {

                risp[x].Tipo_Des = "";

                let obj_Tipo_Des = ElencoTipoTabella.find(obj => obj.Tipo === risp[x].Tipo);

                if (obj_Tipo_Des !== undefined && obj_Tipo_Des !== null && obj_Tipo_Des !== "" && obj_Tipo_Des.Tipo_Des !== undefined) {
                    risp[x].Tipo_Des = obj_Tipo_Des.Tipo_Des;
                }
            }

            options.success(risp);
        }, null);

}

function CaricaddlTabellaGrigliaDettagli_ParametriQualitativi(modulo_generazione) {

    var ElencoTabella = [];

    var param = kendo.stringify({
        modulo_generazione: modulo_generazione
    });

    ajaxAgronica(indirizzohttp + "/CaricaddlTabella",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            ElencoTabella = risp;
        }, null);

    return ElencoTabella;

}

