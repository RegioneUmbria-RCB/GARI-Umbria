var indirizzohttp = "./GestoreCache.aspx";


function RicercaElementiCache(options) {

    var param = kendo.stringify({
    });

    ajaxAgronica(indirizzohttp + "/RicercaElementiCache",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function DammiUrlPerScaching()
{
    var urls = [];

    var param = kendo.stringify({
    });

    ajaxAgronicaSync(indirizzohttp + "/DammiUrlPerScaching",
        param, true,
        function (risposta) {
            urls = JSON.parse(risposta.RispostaStringa);
            urls.push(indirizzohttp + "/PulisciCache");
        }, null);

    return urls
}

function pulisciCache()
{
    var param = kendo.stringify({});
    var rispostaOk = '';
    var rispostaKo = '';
    WaitFrame.show();
    setTimeout(function ()
    {
        ajaxAgronicaSync(indirizzohttp + "/PulisciCache",
            param,
            true,
            function (risposta)
            {
                rispostaOk = risposta.RispostaStringaCustom;
                var errore = "";
                if (risposta !== undefined && risposta !== null) {
                    if (risposta.Errore !== null && risposta.Errore != undefined)
                        errore = risposta.Errore;
                    else
                        errore = "Errore nella chiamata di ClearCache"

                    if (risposta.ErroriGias !== null && risposta.ErroriGias != undefined && risposta.ErroriGias.length > 0) {
                        errore = risposta.ErroriGias.map(e => e.messaggio).join("");
                    }
                }
                rispostaOk = rispostaOk + errore;
            },
            function (risposta) {
                var errore = "";
                if (risposta !== undefined && risposta !== null) {
                    if (risposta.Errore !== null && risposta.Errore != undefined)
                        errore = risposta.Errore;
                    else
                        errore = "Errore nella chiamata di ClearCache"

                    if (risposta.ErroriGias !== null && risposta.ErroriGias != undefined && risposta.ErroriGias.length > 0) {
                        errore = risposta.ErroriGias.map(e => e.messaggio).join("");
                    }                    
                }
                else
                    errore = "Errore nella chiamata di ClearCache"
                rispostaKo = errore;
                rispostaOk = risposta.RispostaStringaCustom;
            }
        );

        popolaGrigliaCache();
        WaitFrame.hide();  
        if (rispostaOk.length > 0)
            MessaggioOK_Kendo(rispostaOk);
        if (rispostaKo.length > 0)
            MessaggioErrore_Kendo(rispostaKo);
    }, 150);    
}

function pulisciCachePermessi() {
    var param = kendo.stringify({});
    var rispostaOk = '';
    var rispostaKo = '';
    WaitFrame.show();
    setTimeout(function () {
        ajaxAgronicaSync(indirizzohttp + "/PulisciCachePermessi",
            param,
            true,
            function (risposta) {
                rispostaOk = risposta.RispostaStringaCustom;
                var errore = "";
                if (risposta !== undefined && risposta !== null) {
                    if (risposta.Errore !== null && risposta.Errore != undefined)
                        errore = risposta.Errore;
                    else
                        errore = "Errore nella chiamata di ClearCache"

                    if (risposta.ErroriGias !== null && risposta.ErroriGias != undefined && risposta.ErroriGias.length > 0) {
                        errore = risposta.ErroriGias.map(e => e.messaggio).join("");
                    }
                }
                rispostaOk = rispostaOk + errore;
            },
            function (risposta) {
                var errore = "";
                if (risposta !== undefined && risposta !== null) {
                    if (risposta.Errore !== null && risposta.Errore != undefined)
                        errore = risposta.Errore;
                    else
                        errore = "Errore nella chiamata di ClearCache"

                    if (risposta.ErroriGias !== null && risposta.ErroriGias != undefined && risposta.ErroriGias.length > 0) {
                        errore = risposta.ErroriGias.map(e => e.messaggio).join("");
                    }
                }
                else
                    errore = "Errore nella chiamata di ClearCache"
                rispostaKo = errore;
                rispostaOk = risposta.RispostaStringaCustom;
            }
        );

        popolaGrigliaCache();
        WaitFrame.hide();
        if (rispostaOk.length > 0)
            MessaggioOK_Kendo(rispostaOk);
        if (rispostaKo.length > 0)
            MessaggioErrore_Kendo(rispostaKo);
    }, 150);
}

function pulisciCacheImpostazioni() {
    var param = kendo.stringify({});
    var rispostaOk = '';
    var rispostaKo = '';
    WaitFrame.show();
    setTimeout(function () {
        ajaxAgronicaSync(indirizzohttp + "/PulisciCacheImpostazioni",
            param,
            true,
            function (risposta) {
                rispostaOk = risposta.RispostaStringaCustom;
                var errore = "";
                if (risposta !== undefined && risposta !== null) {
                    if (risposta.Errore !== null && risposta.Errore != undefined)
                        errore = risposta.Errore;
                    else
                        errore = "Errore nella chiamata di ClearCache"

                    if (risposta.ErroriGias !== null && risposta.ErroriGias != undefined && risposta.ErroriGias.length > 0) {
                        errore = risposta.ErroriGias.map(e => e.messaggio).join("");
                    }
                }
                rispostaOk = rispostaOk + errore;
            },
            function (risposta) {
                var errore = "";
                if (risposta !== undefined && risposta !== null) {
                    if (risposta.Errore !== null && risposta.Errore != undefined)
                        errore = risposta.Errore;
                    else
                        errore = "Errore nella chiamata di ClearCache"

                    if (risposta.ErroriGias !== null && risposta.ErroriGias != undefined && risposta.ErroriGias.length > 0) {
                        errore = risposta.ErroriGias.map(e => e.messaggio).join("");
                    }
                }
                else
                    errore = "Errore nella chiamata di ClearCache"
                rispostaKo = errore;
                rispostaOk = risposta.RispostaStringaCustom;
            }
        );

        popolaGrigliaCache();
        WaitFrame.hide();
        if (rispostaOk.length > 0)
            MessaggioOK_Kendo(rispostaOk);
        if (rispostaKo.length > 0)
            MessaggioErrore_Kendo(rispostaKo);
    }, 150);
}

function MessaggioOK_Kendo(str) {
    let dialog = $("#dialogOkKendo").data("kendoDialog");
    dialog.content(str);
    dialog.open();
    WaitFrame.hide();
}

function MessaggioErrore_Kendo(str) {
    let dialog = $("#dialogErrorKendo").data("kendoDialog");
    dialog.content(str);
    dialog.open();
    WaitFrame.hide();
}

function pulisciElemento(chiave, tipoCache)
{
    var param = kendo.stringify(
        {
            chiave: chiave,
            tipoCache: tipoCache
        }
    );

    ajaxAgronica(indirizzohttp + "/PulisciElemento",
        param,
        function (risposta) {
            risp = risposta.RispostaStringa;
            popolaGrigliaCache();
        }, null);
}


function abilitaCache() {

    var abilita = getKendoSwitch("chkCacheEnabled");
    var param = kendo.stringify(
        {
            abilita: abilita
        }
    );

    ajaxAgronica(indirizzohttp + "/AbilitaCache",
        param,
        function (risposta) {
            if (!abilita)
            {
                pulisciCache();
                popolaGrigliaCache();
            }
        }, null);
}

function aggiorna()
{
    popolaGrigliaCache();
}
