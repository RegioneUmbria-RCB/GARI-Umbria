var indirizzohttp = "./bloccoParticelle.aspx";
var indirizzohttpWSGenerali = "./CdG_WS.aspx";

function EmptyRead(options) { }
function EmptySubmit(options) { }

function CercaDettagliAzienda(piva) {

    ajaxAgronicaSync(indirizzohttp + "/Cerca_Dettagli_Azienda",
        "{ piva: '" + piva + "' }",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
        }, null);

    return risp;
}

function LeggiBloccoParticelle(options) {
    
    var anno = isNaN(parseInt($('#anno')[0].value)) ? 0 : parseInt($('#anno')[0].value);

    if (parseInt(anno) > 1900 && parseInt(anno) < 2100) {
        setYear(parseInt(anno));
    }

    let parametri = {
        piva: KendoDDL("ddlAzienda").value(),
        citta: KendoDDL("citta").value(),
        prov: KendoDDL("prov").value(),
        anno: anno
    }

    ajaxAgronica(indirizzohttp + "/Leggi_Blocchi",
        JSON.stringify(parametri), function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);

}

function CercaPivaRealeB(piva) {
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

function RiempiCitta(options) {
    
    return new Promise(function (resolve, reject) {
    var parametri = {
        prov: KendoDDL("prov").value()
    }

    ajaxAgronica(indirizzohttp + "/RiempiCitta",
        JSON.stringify(parametri),
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            /*objVuoto = { "proCom": "-1", "citta": "TUTTE" };
            risp.unshift(objVuoto);*/
            WaitFrame.hide();
            options.success(risp);
        }, null);
    });
}

function RiempiProv(options) {

    var parametri = {
    }

    ajaxAgronica(indirizzohttp + "/RiempiProv",
        JSON.stringify(parametri),
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "PROV": "-1", "PROVINCIA": "TUTTE" };
            risp.unshift(objVuoto);
            options.success(risp);
        }, null);
}

function ws_Aggiorna_Blocco(updated) {
    return new Promise(function (resolve, reject) {
        let parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            righeModificate: JSON.stringify(updated)
        };
        ajaxAgronica(indirizzohttp + "/Aggiorna_Blocchi_UMA",
            JSON.stringify(parametri),
            function (risposta) {
                resolve();
            }, null);
    });
}