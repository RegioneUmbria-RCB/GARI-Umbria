function RicercaMacchine(piva, richiesta_cod) {
    return new Promise(function (resolve, reject) {
        var parametri = {
            piva: piva,
            richiesta_cod: richiesta_cod,
            anno: $("#anno").val()
        }

        ajaxAgronica(indirizzohttp + "/Cerca_Macchine", JSON.stringify(parametri), function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            resolve(risp);
        }, null);
    });
}

function AggiornaMacchine(piva, richiesta_cod, strMacchine) {
    return new Promise(function (resolve, reject) {
        var parametri = {
            piva: piva,
            richiesta_cod: richiesta_cod,
            strMacchine: JSON.stringify(strMacchine)
        }

        ajaxAgronica(indirizzohttp + "/Aggiorna_Macchine", JSON.stringify(parametri), function (risposta) {
            resolve(risp);
        }, null);
    });
}
