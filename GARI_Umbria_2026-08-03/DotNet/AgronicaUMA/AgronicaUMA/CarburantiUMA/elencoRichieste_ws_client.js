var indirizzohttp = "./elencoRichieste.aspx";
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

function ElencoRichieste(options) {
    
    var anno = isNaN(parseInt($('#anno')[0].value)) ? 0 : parseInt($('#anno')[0].value);

    if (parseInt(anno) > 1900 && parseInt(anno) < 2100) {
        setYear(parseInt(anno));
    }

    let parametri = {
        piva: KendoDDL("ddlAzienda").value(),
        statoCod: KendoDDL("statoPratica").value(),
        citta: KendoDDL("citta").value(),
        prov: KendoDDL("prov").value(),
        rendicontazioni: false,
        anno: anno,
        filtroConto: KendoDDL("conto").value(),
        nuovaVisibilita: usaNuovaVisibilita
    }

    ajaxAgronica(indirizzohttp + "/Trova_Richieste",
        JSON.stringify(parametri), function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);

}

function ElencoRendicontazioni(options) {
    
    var anno = isNaN(parseInt($('#anno')[0].value)) ? 0 : parseInt($('#anno')[0].value);

    let parametri = {
        piva: KendoDDL("ddlAzienda").value(),
        statoCod: KendoDDL("statoPratica").value(),
        citta: KendoDDL("citta").value(),
        prov: KendoDDL("prov").value(),
        rendicontazioni: true,
        anno: anno,
        filtroConto: KendoDDL("conto").value(),
        nuovaVisibilita: usaNuovaVisibilita
    }

    ajaxAgronica(indirizzohttp + "/Trova_Richieste",
        JSON.stringify(parametri), function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
    }, null);
}

function RicercaMacrousiLavorazioni(options) {
    
    var anno = parseInt($('#anno')[0].value);

    let parametri = {
        tipoPratica: KendoDDL("ddlTipoPratica").value(),
        statoCod: KendoDDL("statoPratica").value(),
        citta: KendoDDL("citta").value(),
        prov: KendoDDL("prov").value(),
        approvatore: KendoDDL("ddlApprovatore").value(),
        causali: KendoMultisel("DdlCausalidelnonutilizzo").value().join("|"),
        dettaglio: KendoDDL("ddlLivelloDettaglio").value(),
        colture: KendoDDL("ddlColtura").value(),
        lavorazione: KendoDDL("ddlLavorazione").value(),
        allevamento: KendoDDL("ddlAllevamento").value(),
        anno: anno,
        filtroConto: KendoDDL("conto").value(),
        nuovaVisibilita: true
    }

    ajaxAgronica(indirizzohttp + "/Ricerca_Macrousi_Lavorazioni",
        JSON.stringify(parametri), function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function Leggi_Causali_UMA(options) {

    return new Promise((resolve, reject) => {
        var parametri = {};

        ajaxAgronica(indirizzohttp + "/Leggi_Causali_UMA", JSON.stringify(parametri),
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}

function LeggiApprovatori(options) {
    var parametri = {
    }

    ajaxAgronicaSync(indirizzohttp + "/leggi_Approvatori",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            //objVuoto = { "cod": "-1", "Approvatore": "" };
            //risp.unshift(objVuoto);
            options.success(risp);
        }, null);
}

function LeggiAllevamenti(options) {
    var parametri = {
    }

    ajaxAgronicaSync(indirizzohttp + "/leggi_Allevamenti",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            //objVuoto = { "cod": "-1", "Approvatore": "" };
            //risp.unshift(objVuoto);
            options.success(risp);
        }, null);
}

function leggiColture(options) {
    var parametri = {
    }

    ajaxAgronicaSync(indirizzohttp + "/leggi_Colture",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            
            options.success(risp);
        }, null);
}

function leggiLavorazioni(options) {
    var parametri = {
    }

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Lavorazioni",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            
            options.success(risp);
        }, null);
}

function leggiAllevamenti(options) {
    var parametri = {
    }

    ajaxAgronicaSync(indirizzohttp + "/leggiAllevamenti",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            
            options.success(risp);
        }, null);
}

function ws_linkStampa(piva, richiesta_avanz, richiesta_cod) {
    return new Promise(function (resolve, reject) {

        var parametri = {
            piva: piva,
            richiestaAvanz: richiesta_avanz,
            richiestaCod: richiesta_cod
        }

        ajaxAgronica(indirizzohttp + "/StampaRichiesta", JSON.stringify(parametri), function (risposta) {
            resolve(risposta.RispostaStringa);
        }, null);
    });
}

function RiempiStatiPratiche(options) {

    var parametri = {
    }

    ajaxAgronicaSync(indirizzohttp + "/RiempiStatiPratiche",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            /*objVuoto = { "cod": "-1", "stato": "TUTTI" };
            risp.unshift(objVuoto);*/
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

function ContaRichieste(Piva, CUAA, anno, Avanzamento_Richiesta, isTerz, tipoAz) { //, btn) {
    
    let parametri = {
        piva: Piva,
        cuaa: CUAA,
        isTerzista: isTerz,
        anno: anno,
        Avanzamento_Richiesta: Avanzamento_Richiesta,
        Tipo_Azienda: tipoAz,
        bocciate: false
    };
    ajaxAgronicaSync(indirizzohttp + "/Conta_Richieste",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
        }, null);

    return risp;
}

function checkPerc(elem) {

    var parametri = {
        anno: $('#anno').val()
    }

    ajaxAgronica(indirizzohttp + "/CheckPerc",
        JSON.stringify(parametri),
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (risp == 0)
                elem.html("Litri rendicontati superiori a rimanenze + acquistati oppure esistono litri in recupero accise")
            else
                elem.html("Litri rendicontati (al netto del " + risp.toString() + "%) superiori a rimanenze + acquistati oppure esistono litri in recupero accise")
            if (risp != 0) {
                $("#legendaFisso").hide()
            }
            else {
                $("#legendaFisso").show()
            }
        }, null, null, false);
}

function checkPercAnno(elem, anno) {

    var parametri = {
        anno: anno
    }

    ajaxAgronica(indirizzohttp + "/CheckPerc",
        JSON.stringify(parametri),
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (risp == 0)
                elem.html("Litri rendicontati superiori a rimanenze + acquistati oppure esistono litri in recupero accise")
            else
                elem.html("Litri rendicontati (al netto del " + risp.toString() + "%) superiori a rimanenze + acquistati oppure esistono litri in recupero accise")
            if (risp != 0) {
                $("#legendaFisso").hide()
            }
            else {
                $("#legendaFisso").show()
            }
        }, null);
}