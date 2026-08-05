function ws_getRazze(callback, specie, genere) {
    var parametri = kendo.stringify({ "SPE_COD": specie, "GEN_COD": genere });

    ajaxAgronica("Modifica_Multipla_Zoo.aspx/getRazze",
        parametri,
        function (risposta) {
            let r = JSON.parse(risposta.RispostaStringa);
            callback(risposta);
        }, null, null, false);
}


function ws_leggiGiacenze() {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({});
        ajaxAgronica("Modifica_Multipla_Zoo.aspx/leggiGiacenze",
            parametri,
            function (risposta) {
                let r = JSON.parse(risposta.RispostaStringa);
                resolve(r);
            }, null, null, false);
    });
}
//function CaricaComboCmb_Fornitore(callback) {
//    var parametri = kendo.stringify({ });
//
//    ajaxAgronica("Modifica_Multipla_Zoo.aspx/Lettura_Fornitori",
//        parametri,
//        function (risposta) {
//            let r = JSON.parse(risposta.RispostaStringa);
//            callback(risposta);
//        }, null, null, false);
//}
//

function CaricaComboCmb_Fornitore(options) {
     var parametri = kendo.stringify({
            objP_server: objP_server,
            piva: obj_ModificaMultipla.PIVA,
            rapportoAttivo: true,
            cliente: false,
            fornitore: true,
            dipendente: false,
            terzista: false,
            legale: false,
            agente: false,
            consulente: false,
            Cod_Contatto: ""
        });
        ajaxAgronica(pathCoreWS + "Anagrafica/Contatti.asmx/LeggiRapportoSpecifico",
            parametri,
            function (risposta) {
                let detentori = JSON.parse(risposta.RispostaStringa);
                options.success(detentori);
            }, null, null, false);
}

function CaricaComboCmb_FornitoreGriglia(PIVA) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            objP_server: objP_server,
            piva: PIVA,
            rapportoAttivo: true,
            cliente: false,
            fornitore: true,
            dipendente: false,
            terzista: false,
            legale: false,
            agente: false,
            consulente: false,
            Cod_Contatto: ""
        });
        ajaxAgronica(pathCoreWS + "Anagrafica/Contatti.asmx/LeggiRapportoSpecifico",
            parametri,
            function (risposta) {
                let r = JSON.parse(risposta.RispostaStringa);
                resolve(r);
            }, null, null, false)
    });
}

function CaricaComboCmb_FornitoreGrigliaSync(PIVA) {
    var parametri = kendo.stringify({
        objP_server: objP_server,
        piva: PIVA,
        rapportoAttivo: true,
        cliente: false,
        fornitore: true,
        dipendente: false,
        terzista: false,
        legale: false,
        agente: false,
        consulente: false,
        Cod_Contatto: ""
    });

    let r;
    ajaxAgronicaSync(pathCoreWS + "Anagrafica/Contatti.asmx/LeggiRapportoSpecifico",
        parametri, false,
        function (risposta) {
            r = JSON.parse(risposta.RispostaStringa);
        }, null)

    return r;
}

function ws_getRazzeGriglia(specie, genere) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({ "SPE_COD": specie, "GEN_COD": genere });

        ajaxAgronica("Modifica_Multipla_Zoo.aspx/getRazzeGriglia",
            parametri,
            function (risposta) {
                let r = JSON.parse(risposta.RispostaStringa);
                resolve(r);
            }, null, null, false)
    });
}

function ws_getRazzeGrigliaSync(specie, genere) {
    var parametri = kendo.stringify({ "SPE_COD": specie, "GEN_COD": genere });
    let r;
    ajaxAgronicaSync("Modifica_Multipla_Zoo.aspx/getRazzeGriglia",
        parametri, false,
        function (risposta) {
            r = JSON.parse(risposta.RispostaStringa);            
        }, null);
    return r;
}

function ws_getRazzeGrigliaMadre(specie, genere) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({ "SPE_COD": specie, "GEN_COD": genere });

        ajaxAgronica("Modifica_Multipla_Zoo.aspx/getRazzeGrigliaMadre",
            parametri,
            function (risposta) {
                let r = JSON.parse(risposta.RispostaStringa);
                resolve(r);
            }, null, null, false)
    });
}

function ws_getRazzeGrigliaMadreSync(specie, genere) {
    var parametri = kendo.stringify({ "SPE_COD": specie, "GEN_COD": genere });
    let r;

    ajaxAgronicaSync("Modifica_Multipla_Zoo.aspx/getRazzeGrigliaMadre",
        parametri, false,
        function (risposta) {
            r = JSON.parse(risposta.RispostaStringa);
        }, null);
    return r;
}

function ws_getRazzeGrigliaPadre(specie, genere) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({ "SPE_COD": specie, "GEN_COD": genere });

        ajaxAgronica("Modifica_Multipla_Zoo.aspx/getRazzeGrigliaPadre",
            parametri,
            function (risposta) {
                let r = JSON.parse(risposta.RispostaStringa);
                resolve(r);
            }, null, null, false)
    });
}

function ws_getRazzeGrigliaPadreSync(specie, genere) {
    var parametri = kendo.stringify({ "SPE_COD": specie, "GEN_COD": genere });
    let r;

    ajaxAgronicaSync("Modifica_Multipla_Zoo.aspx/getRazzeGrigliaPadre",
        parametri, false,
        function (risposta) {
            r = JSON.parse(risposta.RispostaStringa);
        }, null);
    return r;
}

function ws_salvataggioModificaMultipla() {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({ obj_ModificaMultipla: obj_ModificaMultipla, selected_add: selected_add });
        ajaxAgronica("Modifica_Multipla_Zoo.aspx/salvataggioModificaMultipla",
            parametri,
            function (risposta) {
                if (risposta.RispostaStringa == '"True"') {
                    resolve(true);
                } else {
                    resolve(false);
                }
            }, null, null, false)
    });
}

function ws_savePresetColumns(params) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({ listColumns: kendoEscapeOggetto(params) });
        ajaxAgronica("Modifica_Multipla_Zoo.aspx/salvataggioPresetColumns",
            parametri,
            function (risposta) {
                if (risposta.RispostaStringa == '"True"') {
                    resolve(true);
                } else {
                    resolve(false);
                }
            }, null, null, false)
    });
}

function ws_salvataggioModificaMultiplaGriglia() {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({ updatedRecords: updatedRecords, obj_ModificaMultipla: obj_ModificaMultipla});
        ajaxAgronica("Modifica_Multipla_Zoo.aspx/salvataggioModificaMultiplaGriglia",
            parametri,
            function (risposta) {
                if (risposta.RispostaStringa == '"True"') {
                    resolve(true);
                } else {
                    resolve(false);
                }
            }, null, null, false)
    });
}