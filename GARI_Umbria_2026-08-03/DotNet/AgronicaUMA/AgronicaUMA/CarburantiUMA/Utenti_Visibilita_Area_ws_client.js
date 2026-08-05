var indirizzohttpUtentiVisibilitaArea = "./Utenti_Visibilita_Area.aspx";


function Visibilita_Area_Read(options) {

    let parametri = {
        area: QS_Area
    }

    ajaxAgronica(indirizzohttpUtentiVisibilitaArea + "/Leggi_Visibilita_Area",
        JSON.stringify(parametri), function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);

}

function RiempiDdlArea(options) {
    var parametri = {
    }

    ajaxAgronicaSync(indirizzohttpUtentiVisibilitaArea + "/LeggiAree",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "area_Cod": "-1", "area_Desc": "TUTTE" };
            if (QS_Area != "") {
                let tmp = risp.filter(x => x.area_Cod == QS_Area)[0];
                risp.splice(risp.indexOf(tmp), 1);
                risp.unshift(tmp);
            }
            //risp.unshift(objVuoto);
            elencoAree = risp;
            options.success(risp);
        }, null);
}

function ws_InserisciVisibilita(righeInserite, righeModificate, righeCancellate) {
    return new Promise(function (resolve, reject) {
        let parametri = {
            righeInserite: JSON.stringify(righeInserite),
            righeModificate: JSON.stringify(righeModificate),
            righeCancellate: JSON.stringify(righeCancellate)
        };
        ajaxAgronicaSync(indirizzohttpUtentiVisibilitaArea + "/AggiornaVisibilita",
            JSON.stringify(parametri),
            false,
            function (risposta) {
                resolve();
            }, null);
    });
}

function PopolaElencoGruppo() {
    return new Promise(function (resolve, reject) {
        let parametri = {};
        ajaxAgronicaSync(indirizzohttpUtentiVisibilitaArea + "/LeggiGruppi",
            JSON.stringify(parametri),
            false,
            function (risposta) {
                let risp = JSON.parse(risposta.RispostaStringa)
                resolve(risp);
            }, null);
    });
}

function TrovaAziendaDaCUAA(cuaa) {
    var rag_soc;

    var parametri = {
        CUAA: cuaa,
    }

    ajaxAgronicaSync(indirizzohttpUtentiVisibilitaArea + "/Trova_Azienda_Da_CUAA", JSON.stringify(parametri), false, function (risposta) {
        WaitFrame.hide();
        rag_soc = JSON.parse(risposta.RispostaStringa);
    }, null);

    return rag_soc;
}

function CheckVisibilita(Gruppo_Cod, UserName, Area, PIVA) {

    var check = true;

    var parametri = {
        Gruppo: Gruppo_Cod == null ? 0 : Gruppo_Cod,
        Username: UserName == null ? "" : UserName,
        Area: Area == null ? -1 : Area,
        Piva: PIVA,
    }

    ajaxAgronicaSync(indirizzohttpUtentiVisibilitaArea + "/CheckVisibilita", JSON.stringify(parametri), false, function (risposta) {
        WaitFrame.hide();
        check = JSON.parse(risposta.RispostaStringa);
    }, null);

    return check;

}

function CheckUtente(UserName) {

    var check = true;

    var parametri = {
        Username: UserName == null ? "" : UserName,
    }

    ajaxAgronicaSync(indirizzohttpUtentiVisibilitaArea + "/CheckUtente", JSON.stringify(parametri), false, function (risposta) {
        WaitFrame.hide();
        check = JSON.parse(risposta.RispostaStringa);
    }, null);

    return check;

}