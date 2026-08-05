
var indirizzohttp = "./PUA_Piano_Distribuzione.aspx";

function RicercaGrigliaPianoDistribuzione(options) {

    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        saCod: parseInt($(cIdSaCod).val()),
        regolamentoCod: parseInt($(cIdRegCod).val()),
        puaCod: parseInt($(cIdPuaCod).val()),
        puaTipo: parseInt($(cIdPuaTipo).val()),
        dataInizio: kendo.parseDate($(cIdDataInizio).val()),
        dataFine: kendo.parseDate($(cIdDataFine).val()),
        modalita: parseInt($(cIdModalita).val()),
        num_blocco: 100,
        forzadefault: $(cIdAssegnaDefault).val()
    });

    ajaxAgronica(indirizzohttp + "/CaricaGrigliaPianoDistribuzione",
        param,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa.KendoGrid);
            necessarioRicalcolo = false;
            options.success(risp);
            Calcola_MedieAziendali_NZoo();
            if (risposta.RispostaStringa.KendoGridEffluenti !== '' && risposta.RispostaStringa.KendoGridEffluenti !== null) {
                $("#" + id_HD_Effluenti).val(risposta.RispostaStringa.KendoGridEffluenti);
                $("#" + id_HD_Effluenti_Dettagli).val(risposta.RispostaStringa.KendoGridEffluentiDettagli);
                popolaGrigliaEffluenti('kendo_Effluenti');
            }

        },
        function (risposta) {
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
        });
}

function UpdatePianoDistribuzione(options) {

    if (necessarioRicalcolo === true) {
        ricalcola();
    }

    var model2 = gridPD.editable.options.model;
    var uid2 = model2.uid;

    var riga = gridPD.dataSource.getByUid(uid2);

    var params = {
        dati: JSON.stringify(riga)
    };

    ajaxAgronicaSync(indirizzohttp + "/Salva",
        JSON.stringify(params), false,
        function (risposta) {
            MessaggioTuttoOK_Bootstrap("Aggiornamento effettuato correttamente", "DIV_Messaggi");

            necessarioRicalcolo = false;

            var str = risposta.RispostaStringa;
            var j = kendoEscapeOggetto(str);

            var model = gridPD.editable.options.model;
            var uid = model.uid;

            var row = gridPD.tbody.find("tr[data-uid='" + uid + "']");

            var hasChan = gridPD.dataSource.hasChanges();

            var dataItem = gridPD.dataSource.getByUid(uid);

            kendoFastRedrawRow(gridPD, row);

            var hasChan2 = gridPD.dataSource.hasChanges();

            dataItem.dirtyFields = {};
            dataItem.dirty = false;

            //gridPD.dataSource.cancelChanges();

            var hasChan3 = gridPD.dataSource.hasChanges();

            //kendoFastRedrawRow(gridPD, row);

            //var hasChan4 = gridPD.dataSource.hasChanges();


            gridPD.refresh();

        },
        function (risposta) {
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
        });
}

function SalvaModificaMultipla(righe) {

    var params = {
        dati: JSON.stringify(righe)
    };

    ajaxAgronicaSync(indirizzohttp + "/SalvaMultiplo",
        JSON.stringify(params), false,
        function (risposta) {
            MessaggioTuttoOK_Bootstrap("Aggiornamento effettuato correttamente", "DIV_Messaggi");

            necessarioRicalcolo = false;
            selected_add = new Array();
            obj_ModificaMultipla = {};

            $("#gridPD .header-chb")[0].checked = false;

            gridPD.dataSource.read();
            return true;
        },
        function (risposta) {
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
            return false;
        });
}

function MostraAnalisi() {
    
    ajaxAgronica(indirizzohttp + "/MostraAnalisi",
        JSON.stringify({ piva: $(cIdPiva).val() }),
        function (risposta) {
            addAnalisiTerrenoNGListener();
            open_window("analisiWindowContainer", "Analisi", risposta.RispostaStringa);
        }, function (risposta) {
            if (risposta.RispostaStringa !== undefined && risposta.RispostaStringa !== "") {
                //se sono qui non ho i permessi per la pagina
                MessaggioErrore_Bootstrap("Errore: " + risposta.RispostaStringa, "DIV_Messaggi");
            } else {
                MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
            }
        });

}

async function riempiDdlGeneriche() {

    var Reg_Cod = parseInt($(cIdRegCod).val());
    var Pua_Tipo = parseInt($(cIdPuaTipo).val());

    ddlCiclo = riempiDdlCiclo();

    var promises = new Array();

    promises.push(riempiDdlPrecessione(Reg_Cod, Pua_Tipo));
    promises.push(riempiDdlUbicazione(Reg_Cod));
    promises.push(riempiDdlTipoAcqua(Reg_Cod, "v"));
    promises.push(riempiDdlTipoAcqua(Reg_Cod, "n"));

    let resp = await Promise.all(promises);

    ddlPrecessione = resp[0];
    ddlUbicazione = resp[1];
    ddlTipoAcqua["v"] = resp[2];
    ddlTipoAcqua["n"] = resp[3];

}

function riempiDdlStatoImpianto(veg_cod, Reg_Cod) {
    if (veg_cod === undefined || veg_cod === 0 || Reg_Cod === undefined || Reg_Cod === 0) {
        ddlStatoImpianto = [];
    } else {

        var parametri = kendo.stringify({
            objP_super_server: objP_super_server,
            objP_server: objP_server,
            PrimaRiga_Flag: false,
            PrimaRiga_Text: "",
            PrimaRiga_Value: "",
            Regolamento_Cod: Reg_Cod,
            Veg_Cod: veg_cod,
            xFiltroAggiuntivo: "",
            xOrderBy: ""
        });

        ajaxAgronicaSync(pathCoreWS + "AgronicaCoreDPI/PianoConcimazione.asmx/PUA_FasiCicloColturale_WS",
            parametri, false,
            function (risposta) {
                ddlStatoImpianto = JSON.parse(risposta.RispostaStringa);
            }, null);
    }
}

function riempiDdlCiclo() {
    let ddl = [
        { Ciclo_Cod: 0, Ciclo_Des: "Principale" },
        { Ciclo_Cod: 1, Ciclo_Des: "Secondario" }
    ];
    return ddl;
}

function riempiDdlAnalisiTerreno(piva, saCod, campoCod, appezza, idReg, filtroParticelle) {
    var ddl = [];
    if (piva === undefined || piva === "" || saCod === undefined || saCod === 0) {
        ddl = [];
    } else {

        var parametri = kendo.stringify({
            objP_server: objP_server,
            piva: piva,
            saCod: saCod,
            campoCod: campoCod,
            appezza: appezza,
            idReg: idReg,
            filtroParticelle: filtroParticelle,
            PrimaRiga_Flag: true,
            PrimaRiga_Text: "Nessuna Analisi",
            PrimaRiga_Value: "0",
            Recupera_Dettagli: true
        });

        ajaxAgronicaSync(pathCoreWS + "Anagrafica/Analisi_Testata.asmx/Leggi_Analisi_Testata_Filtrata",
            parametri, false,
            function (risposta) {
                ddl = JSON.parse(risposta.RispostaStringa);
            }, null);
    }
    return ddl;
}

function riempiDdlPrecessione(Reg_Cod, Pua_Tipo) {
    return new Promise((resolve, reject) => {
        ws_riempiDdlPrecessione(Reg_Cod, Pua_Tipo, function (r) {
            resolve(r);
        });
    });
}

function ws_riempiDdlPrecessione(Reg_Cod, Pua_Tipo,callback) {
    if (Reg_Cod === undefined || Reg_Cod === 0) {
        callback([]);
    } else {

        var parametri = kendo.stringify({
            objP_super_server: objP_super_server,
            objP_server: objP_server,
            PrimaRiga_Flag: false,
            PrimaRiga_Text: "",
            PrimaRiga_Value: "",
            Regolamento_Cod: Reg_Cod,
            Pua_Tipo: Pua_Tipo,
            xFiltroAggiuntivo: "",
            xOrderBy: ""
        });

        ajaxAgronica(pathCoreWS + "AgronicaCoreDPI/PianoConcimazione.asmx/PC_PrecessioneColturale_WS",
            parametri,
            function (risposta) {
                callback(JSON.parse(risposta.RispostaStringa));
            }, null, null, false);
    }
}

function riempiDdlFertilizzanteOrganico(Reg_Cod) {
    return new Promise((resolve, reject) => {
        ws_riempiDdlFertilizzanteOrganico(Reg_Cod, function (r) {
            //let p = JSON.parse(r.RispostaStringa);
            //resolve(p);
            resolve(r);
        });
    });
}

function ws_riempiDdlFertilizzanteOrganico(Reg_Cod, callback) {
    if (Reg_Cod === undefined || Reg_Cod === 0) {
        callback([]);
    } else {

        var parametri = kendo.stringify({
            objP_super_server: objP_super_server,
            objP_server: objP_server,
            PrimaRiga_Flag: false,
            PrimaRiga_Text: "",
            PrimaRiga_Value: "",
            Regolamento_Cod: Reg_Cod,
            xFiltroAggiuntivo: "",
            xOrderBy: ""
        });

        ajaxAgronica(pathCoreWS + "AgronicaCoreDPI/PianoConcimazione.asmx/PUA_EffluentiXFrequenza_GetEffluente_WS",
            parametri,
            function (risposta) {
                callback(JSON.parse(risposta.RispostaStringa));
            }, null, null, false);
    }
}

function riempiDdlFrequenza(Reg_Cod) {
    return new Promise((resolve, reject) => {
        ws_riempiDdlFrequenza(Reg_Cod, function (r) {
            //let p = JSON.parse(r.RispostaStringa);
            //resolve(p);
            resolve(r);
        });
    });
}

function ws_riempiDdlFrequenza(Reg_Cod, callback) {
    if (Reg_Cod === undefined || Reg_Cod === 0) {
        callback([]);
    } else {

        var parametri = kendo.stringify({
            objP_super_server: objP_super_server,
            objP_server: objP_server,
            PrimaRiga_Flag: false,
            PrimaRiga_Text: "",
            PrimaRiga_Value: "",
            Regolamento_Cod: Reg_Cod,
            xFiltroAggiuntivo: "",
            xOrderBy: ""
        });

        ajaxAgronica(pathCoreWS + "AgronicaCoreDPI/PianoConcimazione.asmx/PC_Frequenza_WS",
            parametri,
            function (risposta) {
                callback(JSON.parse(risposta.RispostaStringa));
            }, null, null, false);
    }
}

function riempiDdlUbicazione(Reg_Cod) {
    return new Promise((resolve, reject) => {
        ws_riempiDdlUbicazione(Reg_Cod, function (r) {
            //let p = JSON.parse(r.RispostaStringa);
            //resolve(p);
            resolve(r);
        });
    });
}

function ws_riempiDdlUbicazione(Reg_Cod, callback) {
    if (Reg_Cod === undefined || Reg_Cod === 0) {
        callback([]);
    } else {

        var parametri = kendo.stringify({
            objP_super_server: objP_super_server,
            objP_server: objP_server,
            PrimaRiga_Flag: false,
            PrimaRiga_Text: "",
            PrimaRiga_Value: "",
            Regolamento_Cod: Reg_Cod,
            xFiltroAggiuntivo: "",
            xOrderBy: ""
        });

        ajaxAgronica(pathCoreWS + "AgronicaCoreDPI/PianoConcimazione.asmx/PC_Ubicazione_WS",
            parametri,
            function (risposta) {
                callback(JSON.parse(risposta.RispostaStringa));
            }, null, null, false);
    }
}

function riempiDdlTipoAcqua(Reg_Cod, TipoZona) {
    return new Promise((resolve, reject) => {
        ws_riempiDdlTipoAcqua(Reg_Cod, TipoZona, function (r) {
            //let p = JSON.parse(r.RispostaStringa);
            //resolve(p);
            resolve(r);
        });
    });
}

function ws_riempiDdlTipoAcqua(Reg_Cod, TipoZona, callback) {
    if (Reg_Cod === undefined || Reg_Cod === 0 || TipoZona === undefined || TipoZona === "") {
        callback([]);
    } else {

        var parametri = kendo.stringify({
            objP_super_server: objP_super_server,
            objP_server: objP_server,
            PrimaRiga_Flag: false,
            PrimaRiga_Text: "",
            PrimaRiga_Value: "",
            Regolamento_Cod: Reg_Cod,
            TipoZona: TipoZona,
            xFiltroAggiuntivo: "",
            xOrderBy: ""
        });

        ajaxAgronica(pathCoreWS + "AgronicaCoreDPI/PianoConcimazione.asmx/PC_TipoAcqua_WS",
            parametri,
            function (risposta) {
                callback(JSON.parse(risposta.RispostaStringa));
            }, null, null, false);
    }
}

function riempiDdlFinalitaRER(regCod, vegCod, grfiCod) {
    var ddl = [];
    //if (regCod === undefined || regCod === 0 || vegCod === undefined || vegCod === 0 || grfiCod === undefined || grfiCod === 0) {
        if (regCod === undefined || regCod === 0 || vegCod === undefined || vegCod === 0) {
        ddl = [];
    } else {

        var parametri = kendo.stringify({
            objP_super_server: objP_super_server,
            objP_server: objP_server,
            PrimaRiga_Flag: false,
            PrimaRiga_Text: "",
            PrimaRiga_Value: "",
            Regolamento_Cod: regCod,
            Veg_Cod: vegCod,
            Grfi_Cod: grfiCod,
            xFiltroAggiuntivo: "",
            xOrderBy: ""
        });

        //ajaxAgronicaSync(pathCoreWS + "AgronicaCoreDPI/PianoConcimazione.asmx/PC_Finalita_Rer_BPerc_WS",
            ajaxAgronicaSync(pathCoreWS + "AgronicaCoreDPI/PianoConcimazione.asmx/PC_Finalita_Rer_BPercNMasResa_WS",
            parametri, false,
            function (risposta) {
                ddl = JSON.parse(risposta.RispostaStringa);
            }, null);
    }
    return ddl;
}




function calcolaFabbisognoN(model) {

    //mi passo direttamente il model della riga che sto editando

    let calcoloOK = false;

    let param = kendo.stringify({
        objP_super_server: objP_super_server,
        objP_server: objP_server,
        Pua_Tipo: parseInt($(cIdPuaTipo).val()),
        Regolamento_Cod: parseInt($(cIdRegCod).val()),
        PuaAppezzamento: JSON.stringify(model)
    });

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreDPI/PianoConcimazione.asmx/PUAFabbisognoSingolo",
        param, false,
        function (risposta) {
            calcoloOK = true;

            let risp = JSON.parse(risposta.RispostaStringa);

            AggiornaModelloPerN(model.uid, risp);

            //riga.N_Fabbisogno = risp.Fabbisogno_N;
            //riga.N_FabbisognoComplessivo = risp.Fabbisogno_N_Complessivo;
            //riga.set("N_Fabbisogno", risp.Fabbisogno_N);
            //riga.set("N_FabbisognoComplessivo", risp.Fabbisogno_N_Complessivo);

        },
        function (risposta) {
            calcoloOK = false;
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
        });

    return calcoloOK;
}

function aggiornaN(uid, nFabbisogno, saCod, appezza, idReg, progettoCod) {

    var risultato;

    var param = kendo.stringify({
        Pua_Tipo: parseInt($(cIdPuaTipo).val()),
        Regolamento_Cod: parseInt($(cIdRegCod).val()),
        N_Fabbisogno: parseFloat(nFabbisogno),
        Piva: $(cIdPiva).val(),
        Sa_Cod: parseInt(saCod),
        Appezza: parseInt(appezza),
        Id_Reg: parseInt(idReg),
        Progetto_Cod: parseInt(progettoCod)
    });

    ajaxAgronicaSync(indirizzohttp + "/AggiornaN",
        param, false,
        function (risposta) {

            //devo aggiornare la riga
            let row = gridPD.tbody.find("tr[data-uid='" + uid + "']");
            let dataItem = gridPD.dataSource.getByUid(uid);

            dataItem.N_Fabbisogno_Database = nFabbisogno;
            dataItem.Selected = false;

            let chk = $("#gridPD_" + dataItem.id);
            if ($(chk).closest("tr").is("." + GIAS_K_STATE_SELECTED)) {
                $(chk).click();
            }
            row.removeClass(GIAS_K_STATE_SELECTED);

            let riga_visibile = gridPD.dataSource.view().filter(function (item) {
                return item.uid === uid;
            });

            if (riga_visibile.length > 0) {
                kendoFastRedrawRow(gridPD, row);
            }

            dataItem.dirtyFields = {};
            dataItem.dirty = false;

            MessaggioTuttoOK_Bootstrap("Aggiornamento effettuato correttamente", "DIV_Messaggi");
            risultato = "";
        },
        function (risposta) {
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
            risultato = risposta.Errore;
        });

    return risultato;
}

function aggiornaN_Organico(uid, nFabbisogno, saCod, appezza, idReg, progettoCod) {

    var param = kendo.stringify({
        N_Fabbisogno: parseFloat(nFabbisogno),
        Piva: $(cIdPiva).val(),
        Sa_Cod: parseInt(saCod),
        Appezza: parseInt(appezza),
        Id_Reg: parseInt(idReg),
        Progetto_Cod: parseInt(progettoCod)
    });

    ajaxAgronicaSync(indirizzohttp + "/AggiornaNOrganico",
        param, false,
        function (risposta) {

            //Dopo aver fatto update devo ridisegnare la riga e togliere il dirty...
            let dataItem = gridPD.dataSource.getByUid(uid);
            let row = gridPD.tbody.find("tr[data-uid='" + uid + "']");
            kendoFastRedrawRow(gridPD, row);
            dataItem.dirtyFields = {};
            dataItem.dirty = false;

            //TODO: mi serve sta roba?!?
            let hasChan = gridPD.dataSource.hasChanges();
            gridPD.refresh();

            let hasChan2 = gridPD.dataSource.hasChanges();

            ////devo aggiornare la riga
            //let row = gridPD.tbody.find("tr[data-uid='" + uid + "']");
            //let dataItem = gridPD.dataSource.getByUid(uid);

            //dataItem.N_Fabbisogno_Database = nFabbisogno;
            //dataItem.Selected = false;

            //let chk = $("#gridPD_" + dataItem.id);
            //if ($(chk).closest("tr").is(".k-state-selected")) {
            //    $(chk).click();
            //}
            //row.removeClass("k-state-selected");

            //kendoFastRedrawRow(gridPD, row);

            //dataItem.dirtyFields = {};
            //dataItem.dirty = false;

            MessaggioTuttoOK_Bootstrap("Aggiornamento effettuato correttamente", "DIV_Messaggi");
        },
        function (risposta) {
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
        });

}


