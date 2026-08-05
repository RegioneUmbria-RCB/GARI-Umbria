
var indirizzohttp = "./RiepilogoDatiUMA.aspx";

function LeggiDdlAnno(options) {
    var parametri = {
        piva: QS_Piva,
        tipo_richiesta: QS_Type
    };

    ajaxAgronicaSync(indirizzohttp + "/LeggiAnniRichiesteAzienda",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);            
            options.success(risp);
        }, null);

}

function LeggiRiepilogoDatiAzienda() {
    return new Promise(function (resolve, reject) {
        //WaitFrame.show();
        var parametri = {
            piva: QS_Piva
        };

        ajaxAgronica(indirizzohttp + "/LeggiRiepilogoDatiAzienda",
            JSON.stringify(parametri),
            function (risposta) {
                //WaitFrame.hide();
                dt = JSON.parse(risposta.RispostaStringa);

                resolve(dt);
            }, null);

    });
}

function LeggiRiepilogoDatiCarburanti() {
    return new Promise(function (resolve, reject) {
        //WaitFrame.show();
        var parametri = {
            piva: QS_Piva,
            anno: $("#ddlAnno").val(),
            tipo_richiesta: QS_Type
        };

        ajaxAgronica(indirizzohttp + "/LeggiRiepilogoDatiCarburanti",
            JSON.stringify(parametri),
            function (risposta) {
                //WaitFrame.hide();
                dt = JSON.parse(risposta.RispostaStringa);

                resolve(dt);
            }, null);

    });
}

function CercaPivaRealeD(piva) {
    return new Promise(function (resolve, reject) {
        let parametri = {
            piva: piva
        };
        ajaxAgronica(indirizzohttp + "/CercaPivaReale",
            JSON.stringify(parametri),
            function (risposta) {
                resolve(JSON.parse(risposta.RispostaStringa));
            }, null);
    });
}