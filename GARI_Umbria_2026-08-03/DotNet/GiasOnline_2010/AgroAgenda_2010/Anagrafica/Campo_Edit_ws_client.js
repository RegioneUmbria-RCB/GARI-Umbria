function ControlloDataInizio(data) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            data: data
        });

        ajaxAgronica("Campo_Edit.aspx/ControlloDataInizio",
            parametri,
            function (risposta) {
                if (risposta.RispostaStringa == "True" || risposta.RispostaStringa == "true") {
                    resolve(true);
                } else {
                    resolve(false);
                }
            }, null, null, false);
    });
}

function ControlloDataFine(data) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            data: data
        });

        ajaxAgronica("Campo_Edit.aspx/ControlloDataFine",
            parametri,
            function (risposta) {
                if (risposta.RispostaStringa == "True" || risposta.RispostaStringa == "true") {
                    resolve(true);
                } else {
                    resolve(false);
                }
            }, null, null, false);
    });
}


function Aggiorna_Appezzamenti_DaDateWS(data_inizio, data_fine, piva, sa_cod, campo_cod) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            data_inizio: data_inizio,
            data_fine: data_fine,
            piva: piva,
            sa_cod: sa_cod,
            campo_cod: campo_cod
        });

        ajaxAgronica("Campo_Edit.aspx/Aggiorna_Appezzamenti_DaDateWS",
            parametri,
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, null, false);
    });
}

function Imposta_Serra(tipo) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            tipo: tipo
        });

        ajaxAgronica("Campo_Edit.aspx/Imposta_Serra",
            parametri,
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, null, false);
    });
}

function AggiornajsCodici(str) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            strJsCodici: str
        });

        ajaxAgronica("Campo_Edit.aspx/AggiornajsCodici",
            parametri,
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, null, false);
    });
}


function Check_PianoConcimazione_EntitaxTestata() {
    var prosegui = true

    var parametri = {
        Campo_Cod: (JSON.parse(objP_agenda).Campo_Cod == 0 ? -1 : JSON.parse(objP_agenda).Campo_Cod)
    }
       

    ajaxAgronicaSync("Campo_Edit.aspx/Check_PianoConcimazione_EntitaxTestata",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            if (risposta.RispostaStringa == "") {
                prosegui =  true
            } else {
                kendo.alert(risposta.RispostaStringa)
                prosegui =  false
            }
        }, null);

    return prosegui

}