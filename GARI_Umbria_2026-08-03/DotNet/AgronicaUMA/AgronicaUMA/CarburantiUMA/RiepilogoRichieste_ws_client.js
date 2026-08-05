var indirizzohttp = "./elencoRichieste.aspx";
var indirizzohttpRiepilogo = "./RiepilogoRichieste.aspx";
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

function CercaPivaRealeR(piva) {
    return new Promise(function (resolve, reject) {
        let parametri = {
            piva: piva
        };
        ajaxAgronica(indirizzohttpRiepilogo + "/CercaPivaReale",
            JSON.stringify(parametri),
            function (risposta) {
                resolve(JSON.parse(risposta.RispostaStringa));
            }, null);
    });
}

function RiepilogoRichieste(options) {

    var anno = isNaN(parseInt($('#anno')[0].value)) ? 0 : parseInt($('#anno')[0].value);

    if (parseInt(anno) > 1900 && parseInt(anno) < 2100) {
        setYear(parseInt(anno));
    }

    let parametri = {
        piva: KendoDDL("ddlAzienda").value(),
        statoCod: KendoDDL("statoPratica").value(),
        citta: KendoDDL("citta").value(),
        prov: KendoDDL("prov").value(),
        anno: anno,
        filtroConto: KendoDDL("conto").value(),
        nuovaVisibilita: usaNuovaVisibilita
    }

    ajaxAgronica(indirizzohttpRiepilogo + "/Trova_Riepiloghi",
        JSON.stringify(parametri), function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);

}

function RiempiStatiPratiche(options) {

    var parametri = {
    }

    ajaxAgronicaSync(indirizzohttp + "/RiempiStatiPratiche",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "cod": "-1", "stato": "TUTTI" };
            risp.unshift(objVuoto);
            options.success(risp);
        }, null);
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

function checkPerc(elem) {

    var parametri = {
        anno: $('#anno').val()
    }

    ajaxAgronica(indirizzohttp + "/CheckPerc",
        JSON.stringify(parametri),
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            

            if (risp != 0) {
                $("#legendaFisso").hide()

            }
            else {
                $("#legendaFisso").show()
                elem.html("Litri rendicontati superiori a rimanenze + acquistati")
            }
        }, null, null, false);
}
