let indirizzoVenditaCarburanti = "./VenditeCarburantiUMA.aspx";

function EseguireChiamataAjax(target, argomenti) {
    let parametri = argomenti != null ? JSON.stringify(argomenti) : JSON.stringify({});

    return new Promise((resolve, reject) => {
        ajaxAgronica(indirizzoVenditaCarburanti + target, parametri, function (risposta) {
            let risultato = JSON.parse(risposta.RispostaStringa);
            resolve(risultato);
        }, null, null, false);
    });
}
function EseguireChiamataAjaxSync(target, argomenti) {
    let parametri = argomenti != null ? JSON.stringify(argomenti) : JSON.stringify({});
    let result = null;

    ajaxAgronicaSync(indirizzoVenditaCarburanti + target, parametri, false,
        function (risposta) {
            let risultato = JSON.parse(risposta.RispostaStringa);
            result = risultato;
        }, null);
    return result;
}
function EseguireChiamataAjaxSenzaMessaggioErrore(target, argomenti) {
    let parametri = argomenti != null ? JSON.stringify(argomenti) : JSON.stringify({});

    return new Promise((resolve, reject) => {
        ajaxAgronica(indirizzoVenditaCarburanti + target, parametri, function (risposta) {
            risposta.RispostaStringa = JSON.parse(risposta.RispostaStringa);
            resolve(risposta);
        }, null, null, false);
    });
}

