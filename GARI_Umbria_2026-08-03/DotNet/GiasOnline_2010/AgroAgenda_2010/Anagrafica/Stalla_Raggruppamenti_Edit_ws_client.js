function ws_get_permessi(permesso_cod, callback) {
    var parametri = kendo.stringify({ "permesso_cod": permesso_cod});

    ajaxAgronica("Stalla_Raggruppamenti_Edit.aspx/get_permesso",
        parametri,
        function (risposta) {
            callback(risposta);
        }, null, null, false);
}

function ws_get_stalla(callback) {
    var parametri = kendo.stringify({});

    ajaxAgronica("Stalla_Raggruppamenti_Edit.aspx/get_stalla",
        parametri,
        function (risposta) {
            callback(risposta);
        }, null, null, false);
}

function ws_get_raggruppamento_stalla(callback) {
    var parametri = kendo.stringify({});

    ajaxAgronica("Stalla_Raggruppamenti_Edit.aspx/get_raggruppamento_stalla",
        parametri,
        function (risposta) {
            callback(risposta);
        }, null, null, false);
}

function CaricaComboCmb_tipo(options) {
    var parametri = kendo.stringify({  });

    ajaxAgronica("Stalla_Raggruppamenti_Edit.aspx/get_lista_tipi_raggruppamenti",
        parametri,
        function (risposta) {
            let raggruppamenti = JSON.parse(risposta.RispostaStringa);
            //objVuoto = { "Raggruppamento_Cod": "", "Raggruppamento_Des": "" };
            //raggruppamenti.unshift(objVuoto);
            options.success(raggruppamenti);
        }, null, null, false);
}

function ws_salva(retOp) {
    return new Promise((resolve, reject) => {
        if (obj_stalla_raggruppamento.STATO_COD === undefined) {
            obj_stalla_raggruppamento.STATO_COD = 0;
        }
        if (obj_stalla_raggruppamento.Mq === undefined) {
            obj_stalla_raggruppamento.Mq = 0;
        }

        var parametri = kendo.stringify({
            obj_str_raggruppamento_Stalla: JSON.stringify(obj_stalla_raggruppamento),
            RetOp: retOp
        });


        ajaxAgronica("Stalla_Raggruppamenti_Edit.aspx/salva",
            parametri,
            function (risposta) {
                resolve(risposta);
            }, null, null, false);
    });
}

function ws_impostaTipoOperazioneObj_Agenda(tipoOperazione, callback) {

    var parametri = kendo.stringify({ tipoOperazione: tipoOperazione});

    ajaxAgronica("Stalla_Raggruppamenti_Edit.aspx/impostaTipoOperazioneObj_Agenda",
        parametri,
        function (risposta) {
            callback(risposta);
        }, null, null, false);

}

function CaricaComboCmb_Specie(options) {
    let gen_cod = obj_stalla.s.GEN_COD;
    if (gen_cod !== "" && gen_cod !== undefined) {
        var parametri = kendo.stringify({ gen_cod: gen_cod, objP_server: objP_server, objP_utenti: objP_utenti });
        ajaxAgronica(pathCoreWS + "Metaschema/Lista_Specie_Animali.asmx/get_Lista_Specie_Animali",
            parametri,
            function (risposta) {
                let specie = JSON.parse(risposta.RispostaStringa);
                options.success(specie);
            }, null, null, false);
    } else {
        options.success([]);
    }
}

function CaricaComboCmb_Razza(options) {
    let gen_cod = obj_stalla.s.GEN_COD;
    let spe_cod = obj_stalla.s.SPE_COD;
    if (gen_cod !== "" && spe_cod !== "" && gen_cod !== undefined && spe_cod !== undefined) {
        var parametri = kendo.stringify({ gen_cod: gen_cod, spe_cod: spe_cod, objP_server: objP_server, objP_utenti: objP_utenti });
        ajaxAgronica(pathCoreWS + "Metaschema/Lista_Razze_Animali.asmx/get_Lista_Razze_Animali",
            parametri,
            function (risposta) {
                let razze = JSON.parse(risposta.RispostaStringa);
                options.success(razze);
            }, null, null, false);
    } else {
        options.success([]);
    }
}

function CaricaComboCmb_StatoAccrescimento(options) {
    let gen_cod = obj_stalla.s.GEN_COD;
    let spe_cod = obj_stalla.s.SPE_COD;
    if (gen_cod !== "" && spe_cod !== "" && gen_cod !== undefined && spe_cod !== undefined) {
        var parametri = kendo.stringify({ GEN_COD: gen_cod, SPE_COD: spe_cod })
        ajaxAgronica("Stalla_Raggruppamenti_Edit.aspx/Carica_Zoo_Animali_Lista_Stati_Accrescimento",
            parametri,
            function (risposta) {
                let detentori = JSON.parse(risposta.RispostaStringa);
                options.success(detentori);
            }, null, null, false);
    } else {
        options.success([]);
    }
}