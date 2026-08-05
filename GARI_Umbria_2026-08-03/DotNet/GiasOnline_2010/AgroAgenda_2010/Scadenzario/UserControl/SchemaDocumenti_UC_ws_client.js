var indirizzohttp = "./Scad_Anagrafiche.aspx";


function LeggiDataSchemaDocumenti(options) {
    var param = kendo.stringify({
        piva: $(cIdPiva).val()
    });

    ajaxAgronica(indirizzohttp + "/LeggiDataSchemaDocumenti",
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (risp.length > 0)
                options.success(risp);
        }, null);
}

function ws_leggiConfigurazione() {
    return new Promise((resolve, reject) => {
        var parametri = {};
        ajaxAgronica(indirizzohttp + "/leggiConfigurazione", JSON.stringify(parametri), function (risposta) {
            objP_agenda = risposta.RispostaStringa;
            resolve();
        }, null, null, false);
    });
}


function InviaRigheModificate(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        ajaxAgronica(indirizzohttp + "/SchemaDocumenti_SalvaGriglia",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}

function SchemaDocumenti_Caricale(options) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({});

        ajaxAgronica(indirizzohttp + "/SchemaDocumenti_CaricaElenco", parametri,
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}

function SchemaDocumenti_CaricaConfigurazioniDaDB(options) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({});

        ajaxAgronica(indirizzohttp + "/CaricaConfigurazioniSchemaDocumenti", parametri,
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}

function PopolaElenco_Servizi() {
    return new Promise(function (resolve, reject) {
        var parametri = JSON.stringify({
            elencoRichiesto: "Servizi"
        });

        ajaxAgronica(indirizzohttp + "/Servizi_LeggiElencoDropdown", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    newArr.push({
                        Servizio_Des: "nessuno",
                        Servizio_Cod: null,
                    });
                    for (let i = 0; i < arr.length; i++) {
                        let serv_des = arr[i].Servizio_Des;
                        let serv_cod = arr[i].Servizio_Cod;
                        newArr.push({
                            Servizio_Des: serv_des,
                            Servizio_Cod: serv_cod
                        });
                    }
                    resolve(newArr);
                }
            }, null, null, false);
    });
}

function PopolaElenco_Stato_Da(Servizio_Cod) {
    return new Promise(function (resolve, reject) {
        var parametri = JSON.stringify({
            elencoRichiesto: "Stato_Da",
            Servizio_Cod: Servizio_Cod
        });

        ajaxAgronica(indirizzohttp + "/Stato_Da_LeggiElencoDropdown", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    newArr.push({
                        Stato_Da_Des: "Nessuno",
                        Stato_Da: null,
                    });
                    for (let i = 0; i < arr.length; i++) {
                        let stato_da_des = arr[i].Stato_Da_Des;
                        let stato_da = arr[i].Stato_Da;
                        newArr.push({
                            Stato_Da_Des: stato_da_des,
                            Stato_Da: stato_da
                        });
                    }
                    newArr.unshift({
                        Stato_Da_Des: "Tutti",
                        Stato_Da: -1
                    });
                    resolve(newArr);
                }
            }, null, null, false);
    });
}

function PopolaElenco_Stato_A(Servizio_Cod) {
    return new Promise(function (resolve, reject) {
        var parametri = JSON.stringify({
            elencoRichiesto: "Stato_A",
            Servizio_Cod: Servizio_Cod

        });

        ajaxAgronica(indirizzohttp + "/Stato_A_LeggiElencoDropdown", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    newArr.push({
                        Stato_A_Des: "Nessuno",
                        Stato_A: null,
                    });
                    for (let i = 0; i < arr.length; i++) {
                        let stato_a_des = arr[i].Stato_A_Des;
                        let stato_a = arr[i].Stato_A;
                        newArr.push({
                            Stato_A_Des: stato_a_des,
                            Stato_A: stato_a
                        });
                    }
                    newArr.unshift({
                        Stato_A_Des: "Tutti",
                        Stato_A: -1
                    });
                    resolve(newArr);
                }
            }, null, null, false);
    });
}

function PopolaElenco_Tipologia() {
    return new Promise(function (resolve, reject) {
        var parametri = JSON.stringify({
            elencoRichiesto: "Tipologia"
        });

        ajaxAgronica(indirizzohttp + "/Tipologia_LeggiElencoDropdown", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    newArr.push({
                        Tipologia_Des: "SELEZIONARE",
                        Tipologia: "",
                    });
                    for (let i = 0; i < arr.length; i++) {
                        let tipologia_des = arr[i].Tipologia_Des;
                        let tipologia = arr[i].Tipologia;
                        newArr.push({
                            Tipologia_Des: tipologia_des,
                            Tipologia: tipologia
                        });
                    }
                    resolve(newArr);
                }
            }, null, null, false);
    });
}

function PopolaElenco_Ambito() {
    return new Promise(function (resolve, reject) {
        var parametri = JSON.stringify({
            elencoRichiesto: "Ambito"
        });

        ajaxAgronica(indirizzohttp + "/Ambito_LeggiElencoDropdown", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    newArr.push({
                        Ambito_Des: "SELEZIONARE",
                        Ambito: "",
                    });
                    for (let i = 0; i < arr.length; i++) {
                        let ambito_des = arr[i].Ambito_Des;
                        let ambito = arr[i].Ambito;
                        newArr.push({
                            Ambito_Des: ambito_des,
                            Ambito: ambito
                        });
                    }
                    resolve(newArr);
                }
            }, null, null, false);
    });
}

function PopolaElenco_Fase() {
    return new Promise(function (resolve, reject) {
        var parametri = JSON.stringify({
            elencoRichiesto: "Fase"
        });

        ajaxAgronica(indirizzohttp + "/Fase_LeggiElencoDropdown", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    newArr.push({
                        Fase_Des: "SELEZIONARE",
                        Fase: "",
                    });
                    for (let i = 0; i < arr.length; i++) {
                        let fase_des = arr[i].Fase_Des;
                        let fase = arr[i].Fase;
                        newArr.push({
                            Fase_Des: fase_des,
                            Fase: fase
                        });
                    }
                    resolve(newArr);
                }
            }, null, null, false);
    });
}

function PopolaElenco_Firmato() {
    return new Promise(function (resolve, reject) {
        var parametri = JSON.stringify({
            elencoRichiesto: "Firmato"
        });

        ajaxAgronica(indirizzohttp + "/Firmato_LeggiElencoDropdown", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    //newArr.push({
                    //    Firmato: " ",
                    //    Flag_Firmato_Digit: -1,
                    //});
                    for (let i = 0; i < arr.length; i++) {
                        let firmato = arr[i].Firmato;
                        let flag_firmato_digit = arr[i].Flag_Firmato_Digit;
                        newArr.push({
                            Firmato: firmato,
                            Flag_Firmato_Digit: flag_firmato_digit
                        });
                    }
                    resolve(newArr);
                }
            }, null, null, false);
    });
}

//function PopolaElenco_Obbligatorio() {
//    return new Promise(function (resolve, reject) {
//        var parametri = JSON.stringify({
//            elencoRichiesto: "Obbligatorio"
//        });

//        ajaxAgronica(indirizzohttp + "/Obbligatorio_LeggiElencoDropdown", parametri,
//            function (risposta) {
//                if (risposta.RispostaOK) {
//                    let arr = JSON.parse(risposta.RispostaStringa);
//                    let newArr = new Array();
//                    newArr.push({
//                        Obbligatorio: "",
//                        Flag_Obbligatorio: 0,
//                    });
//                    for (let i = 0; i < arr.length; i++) {
//                        let obbligatorio = arr[i].Obbligatorio;
//                        let flag_obbligatorio = arr[i].Flag_Obbligatorio;
//                        newArr.push({
//                            Obbligatorio: obbligatorio,
//                            Flag_Obbligatorio: flag_obbligatorio
//                        });
//                    }
//                    resolve(newArr);
//                }
//            }, null, null, false);
//    });
//}

function PopolaElenco_Obbligatorio() {
    return new Promise(function (resolve, reject) {
        var parametri = JSON.stringify({
            elencoRichiesto: "Repository"
        });
        let newArr = new Array();
        //newArr.push({
        //    Obbligatorio: " ",
        //    Flag_Obbligatorio: -1,
        //});
        newArr.push({
            Obbligatorio: "No",
            Flag_Obbligatorio: 0,
        });
        newArr.push({
            Obbligatorio: "Si",
            Flag_Obbligatorio: 1,
        });
        resolve(newArr);
    });
}

//è stata selezionata una cella
function modificaSchemaDocumenti(tr_elem, grid_elem) {
    return new Promise((resolve, reject) => {
        let datiGriglia = $(grid_elem).data('kendoGrid');
        let datiRiga = datiGriglia.dataItem(tr_elem);
        var parametri = kendo.stringify({
            data: JSON.stringify(datiRiga)
        });
    });
}