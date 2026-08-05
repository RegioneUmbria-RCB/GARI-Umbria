function LeggiDataTabellaLavorazioni(options) {
    var param = kendo.stringify({
        piva: $(cIdPiva).val()
    });

    ajaxAgronica(indirizzohttp + "/LeggiDataTabellaLavorazioni",
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (risp.length > 0)
                options.success(risp);
        }, null);
}

function LavAlt_InviaRigheModificate(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        ajaxAgronica(indirizzohttp + "/LavorazioniAlternative_SalvaGriglia",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}

function Setup_InviaRigheModificate(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        ajaxAgronica(indirizzohttp + "/Setup_SalvaGriglia",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}

function DateRendicontazioni_InviaRigheModificate(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        ajaxAgronica(indirizzohttp + "/DateRendicontazioni_SalvaGriglia",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}

function UMAConfigurazioneAllevamenti_InviaRigheModificate(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        ajaxAgronica(indirizzohttp + "/UMAConfigurazioneAllevamenti_SalvaGriglia",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
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

        ajaxAgronica(indirizzohttp + "/Salva_GrigliaLavorazioniUMA",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}


function LavAlt_Caricale(options) {
    var Inizio = $("#InizioValidita").val();
    var Fine = $("#FineValidita").val();
    //var selDateIni = new Date(Inizio);
    //var selDateFin = new Date(Fine);

    //if (isNaN(selDateIni.getTime()) || isNaN(selDateFin.getTime())) {
    //    return "attenzione date errate";
    //}

    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({ InizioValidita: Inizio, FineValidita: Fine });

        ajaxAgronica(indirizzohttp + "/LavorazioniAlternative_CaricaElenco", parametri,
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}

function Setup_Caricale(options) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({});

        ajaxAgronica(indirizzohttp + "/Setup_CaricaElenco", parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);

                risp = generaDescrizioniTarghe(risp);

                risp = generaDescrizioniStati(risp);

                risp = generaDescrizioniGruppi(risp);

                if (risp.length > 0)
                    options.success(risp);
                resolve(risp);
            }, null, null, false);
    });
}

function UF_Caricale(options) {
    return new Promise((resolve, reject) => {

        var Inizio = $("#InizioValidita").val();
        var Fine = $("#FineValidita").val();

        var parametri = kendo.stringify({ InizioValidita: Inizio, FineValidita: Fine });

        ajaxAgronica(indirizzohttp + "/UF_CaricaElenco", parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);

                //risp = generaDescrizioniTarghe(risp);

                //risp = generaDescrizioniStati(risp);

                //risp = generaDescrizioniGruppi(risp);

                if (risp.length > 0)
                    options.success(risp);
                resolve(risp);
            }, null, null, false);
    });
}

function ElencoMacrousi_Caricale(options) {
    return new Promise((resolve, reject) => {

        var Inizio = $("#InizioValidita").val();
        var Fine = $("#FineValidita").val();

        var parametri = kendo.stringify({ InizioValidita: Inizio, FineValidita: Fine });

        ajaxAgronica(indirizzohttp + "/ElencoMacrousi_Caricale", parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);

                //risp = generaDescrizioniTarghe(risp);

                //risp = generaDescrizioniStati(risp);

                //risp = generaDescrizioniGruppi(risp);

                if (risp.length > 0)
                    options.success(risp);
                resolve(risp);
            }, null, null, false);
    });
}

function ElencoLavorazioni_Caricale(options) {
    return new Promise((resolve, reject) => {

        var Inizio = $("#InizioValidita").val();
        var Fine = $("#FineValidita").val();

        var parametri = kendo.stringify({ InizioValidita: Inizio, FineValidita: Fine });

        ajaxAgronica(indirizzohttp + "/ElencoLavorazioni_Caricale", parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);

                //risp = generaDescrizioniTarghe(risp);

                //risp = generaDescrizioniStati(risp);

                //risp = generaDescrizioniGruppi(risp);

                if (risp.length > 0)
                    options.success(risp);
                resolve(risp);
            }, null, null, false);
    });
}

function ElencoAllevamenti_Caricale(options) {
    return new Promise((resolve, reject) => {

        var Inizio = $("#InizioValidita").val();
        var Fine = $("#FineValidita").val();

        var parametri = kendo.stringify({ InizioValidita: Inizio, FineValidita: Fine });

        ajaxAgronica(indirizzohttp + "/ElencoAllevamenti_Caricale", parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);

                //risp = generaDescrizioniTarghe(risp);

                //risp = generaDescrizioniStati(risp);

                //risp = generaDescrizioniGruppi(risp);

                if (risp.length > 0)
                    options.success(risp);
                resolve(risp);
            }, null, null, false);
    });
}

function ElencoAssociazioniMacrousi_Caricale(options) {
    return new Promise((resolve, reject) => {

        var Inizio = $("#InizioValidita").val();
        var Fine = $("#FineValidita").val();

        var parametri = kendo.stringify({ InizioValidita: Inizio, FineValidita: Fine });

        ajaxAgronica(indirizzohttp + "/ElencoAssociazioniMacrousi_Caricale", parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);

                //risp = generaDescrizioniTarghe(risp);

                //risp = generaDescrizioniStati(risp);

                //risp = generaDescrizioniGruppi(risp);

                if (risp.length > 0)
                    options.success(risp);
                resolve(risp);
            }, null, null, false);
    });
}

function getElencoAgea(colonna, cod) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            colonna: colonna,
            mostraDescrizioniVuote: cod
        });

        ajaxAgronica(indirizzohttp + "/UF_CaricaElencoAgea",
            parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                resolve(risp)

            }, null, null, false);
    });
}

function UF_InviaRigheModificate(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        ajaxAgronica(indirizzohttp + "/UF_SalvaGriglia",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}

function ElencoMacrousi_InviaRigheModificate(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        console.log(jsonData);

        ajaxAgronica(indirizzohttp + "/ElencoMacrousi_SalvaGriglia",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}

function ElencoLavorazioni_InviaRigheModificate(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        ajaxAgronica(indirizzohttp + "/ElencoLavorazioni_SalvaGriglia",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}

function ElencoAllevamenti_InviaRigheModificate(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        ajaxAgronica(indirizzohttp + "/ElencoAllevamenti_SalvaGriglia",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}

function ElencoAssociazioniMacrousi_InviaRigheModificate(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        ajaxAgronica(indirizzohttp + "/ElencoAssociazioniMacrousi_SalvaGriglia",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}

function generaDescrizioniTarghe(risp) {
    //Generazione Descrizione per macchine targhe obbligatorie
    var class_Cod_Letti = "";
    for (var x = 0; x < risp.length; x++) {
        //Aggiungo il campo Costi_Cod per la multiselect dei costi 
        class_Cod_Letti = risp[x].Macchine_Targa_ObbligatoriaString.replaceAll("%", "").split("|");
        risp[x].Macchine_Targa_Obbligatoria = [];
        for (var y = 0; y < class_Cod_Letti.length; y++) {
            if (class_Cod_Letti[y] !== "") {
                risp[x].Macchine_Targa_Obbligatoria.push(class_Cod_Letti[y].toString());
            }
        }

        risp[x].Macchine_Targa_ObbligatoriaDes = [];
        risp[x].Macchine_Targa_ObbligatoriaDesString = "";
        for (var y = 0; y < class_Cod_Letti.length; y++) {
            for (var b = 0; b < elencoMacchine.length; b++) {
                if (class_Cod_Letti[y] === elencoMacchine[b].Macchine_Targa_Obbligatoria) {
                    risp[x].Macchine_Targa_ObbligatoriaDes.push(elencoMacchine[b].Macchine_Targa_ObbligatoriaDes);
                    risp[x].Macchine_Targa_ObbligatoriaDesString += elencoMacchine[b].Macchine_Targa_ObbligatoriaDes + ",";
                }
            }
        }

        //Rimuovo l'ultima virgola
        if (risp[x].Macchine_Targa_ObbligatoriaDesString !== "") {
            risp[x].Macchine_Targa_ObbligatoriaDesString = risp[x].Macchine_Targa_ObbligatoriaDesString.slice("0", risp[x].Macchine_Targa_ObbligatoriaDesString.length - 1);
        }

    }
    return risp
}

function generaDescrizioniStati(risp) {
    //Generazione Descrizione per macchine targhe obbligatorie
    var class_Cod_Letti = "";
    for (var x = 0; x < risp.length; x++) {
        //Aggiungo il campo Costi_Cod per la multiselect dei costi 
        class_Cod_Letti = risp[x].Stati_Invio_MailString.replaceAll("%", "").split("|");
        risp[x].Stati_Invio_Mail = [];
        for (var y = 0; y < class_Cod_Letti.length; y++) {
            if (class_Cod_Letti[y] !== "") {
                risp[x].Stati_Invio_Mail.push(class_Cod_Letti[y].toString());
            }
        }

        risp[x].Stati_Invio_MailDes = [];
        risp[x].Stati_Invio_MailDesString = "";
        for (var y = 0; y < class_Cod_Letti.length; y++) {
            for (var b = 0; b < elencoStati.length; b++) {
                if (class_Cod_Letti[y] === elencoStati[b].Stati_Invio_Mail.toString()) {
                    risp[x].Stati_Invio_MailDes.push(elencoStati[b].Stati_Invio_MailDes);
                    risp[x].Stati_Invio_MailDesString += elencoStati[b].Stati_Invio_MailDes + ",";
                }
            }
        }

        //Rimuovo l'ultima virgola
        if (risp[x].Stati_Invio_MailDesString !== "") {
            risp[x].Stati_Invio_MailDesString = risp[x].Stati_Invio_MailDesString.slice("0", risp[x].Stati_Invio_MailDesString.length - 1);
        }

    }
    return risp
}

function generaDescrizioniGruppi(risp) {
    //Generazione Descrizione per macchine targhe obbligatorie
    var class_Cod_Letti = "";
    for (var x = 0; x < risp.length; x++) {
        //Aggiungo il campo Costi_Cod per la multiselect dei costi 
        class_Cod_Letti = risp[x].Gruppi_Utenti_Invio_MailString.replaceAll("%", "").split("|");
        risp[x].Gruppi_Utenti_Invio_Mail = [];
        for (var y = 0; y < class_Cod_Letti.length; y++) {
            if (class_Cod_Letti[y] !== "") {
                risp[x].Gruppi_Utenti_Invio_Mail.push(class_Cod_Letti[y].toString());
            }
        }

        risp[x].Gruppi_Utenti_Invio_MailDes = [];
        risp[x].Gruppi_Utenti_Invio_MailDesString = "";
        for (var y = 0; y < class_Cod_Letti.length; y++) {
            for (var b = 0; b < elencoGruppi.length; b++) {
                if (class_Cod_Letti[y] === elencoGruppi[b].Gruppi_Utenti_Invio_Mail.toString()) {
                    risp[x].Gruppi_Utenti_Invio_MailDes.push(elencoGruppi[b].Gruppi_Utenti_Invio_MailDes);
                    risp[x].Gruppi_Utenti_Invio_MailDesString += elencoGruppi[b].Gruppi_Utenti_Invio_MailDes + ",";
                }
            }
        }

        //Rimuovo l'ultima virgola
        if (risp[x].Gruppi_Utenti_Invio_MailDesString !== "") {
            risp[x].Gruppi_Utenti_Invio_MailDesString = risp[x].Gruppi_Utenti_Invio_MailDesString.slice("0", risp[x].Gruppi_Utenti_Invio_MailDesString.length - 1);
        }

    }
    return risp
}

function DateRendicontazioni_Caricale(options) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({});

        ajaxAgronica(indirizzohttp + "/DateRendicontazione_CaricaElenco", parametri,
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}

function UMAConfigurazioneAllevamenti_Caricale(options) {

    var Inizio = $("#InizioValidita").val();
    var Fine = $("#FineValidita").val();

    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({ InizioValidita: Inizio, FineValidita: Fine });

        ajaxAgronica(indirizzohttp + "/UMAConfigurazioneAllevamenti_CaricaElenco", parametri,
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}

function CaricaConfigurazioniDaDB(options) {

    var Inizio = $("#InizioValidita").val();
    var Fine = $("#FineValidita").val();

    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({ InizioValidita: Inizio, FineValidita: Fine });

        ajaxAgronica(indirizzohttp + "/CaricaConfigurazioniUMA", parametri,
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}

function LavAlt_PopolaElenco_Gruppo_Colturale_UMA() {
    return new Promise(function (resolve, reject) {
        var parametri = JSON.stringify({
            elencoRichiesto: "Gruppo_Colturale_UMA"
        });

        ajaxAgronica(indirizzohttp + "/LavorazioniAlternative_LeggiElencoDropdown", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    newArr.push({
                        Macrouso_UMA_Des: "Non Selezionato",
                        Gruppo_Colturale_UMA: "Non Selezionato",
                    });
                    for (let i = 0; i < arr.length; i++) {
                        let uma_des = arr[i].Macrouso_UMA_Des;
                        let uma_cod = arr[i].Gruppo_Colturale_UMA;
                        newArr.push({
                            Macrouso_UMA_Des: uma_des,
                            Gruppo_Colturale_UMA: uma_cod
                        });
                    }
                    resolve(newArr);
                }
            }, null, null, false);
    });
}

function LavAlt_PopolaElenco_Lavorazione_UMA() {
    return new Promise(function (resolve, reject) {
        var parametri = JSON.stringify({
            elencoRichiesto: "Lavorazione_UMA"
        });

        ajaxAgronica(indirizzohttp + "/LavorazioniAlternative_LeggiElencoDropdown", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    newArr.push({
                        LavUMA_Lav_UMA_Des: "Non Selezionato",
                        Lavorazione_UMA: "",
                    });
                    for (let i = 0; i < arr.length; i++) {
                        let uma_des = arr[i].LavUMA_Lav_UMA_Des;
                        let uma_cod = arr[i].Lavorazione_UMA;
                        newArr.push({
                            LavUMA_Lav_UMA_Des: uma_des,
                            Lavorazione_UMA: uma_cod
                        });
                    }
                    resolve(newArr);
                }
            }, null, null, false);
    });
}

function LavAlt_PopolaElenco_Lavorazione_UMA_Alt() {
    return new Promise(function (resolve, reject) {
        var parametri = JSON.stringify({
            elencoRichiesto: "Lavorazione_UMA_Alt"
        });

        ajaxAgronica(indirizzohttp + "/LavorazioniAlternative_LeggiElencoDropdown", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    newArr.push({
                        LavUMAAlt_Lav_UMA_Des: "Non Selezionato",
                        Lavorazione_UMA_Alt: "",
                    });
                    for (let i = 0; i < arr.length; i++) {
                        let uma_des = arr[i].LavUMAAlt_Lav_UMA_Des;
                        let uma_cod = arr[i].Lavorazione_UMA_Alt;
                        newArr.push({
                            LavUMAAlt_Lav_UMA_Des: uma_des,
                            Lavorazione_UMA_Alt: uma_cod
                        });
                    }
                    resolve(newArr);
                }
            }, null, null, false);
    });
}

function PopolaElenco_MacrousoUMACod() {
    return new Promise(function (resolve, reject) {
        var parametri = JSON.stringify({
            elencoRichiesto: "uma_macrousi"
        });

        ajaxAgronica(indirizzohttp + "/LeggiElenco_MacrousoUMACod", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    newArr.push({
                        Macrouso_UMA_Des: "Non Selezionato",
                        Macrouso_UMA_Cod: 0,
                    });
                    for (let i = 0; i < arr.length; i++) {
                        let uma_des = arr[i].Macrouso_UMA_Des;
                        let uma_cod = arr[i].Macrouso_UMA_Cod;
                        newArr.push({
                            Macrouso_UMA_Des: uma_des,
                            Macrouso_UMA_Cod: parseInt(uma_cod)
                        });
                    }
                    resolve(newArr);
                }
            }, null, null, false);
    });
}

function PopolaElenco_UMALavorazioni() {
    return new Promise(function (resolve, reject) {
        var parametri = JSON.stringify({
            elencoRichiesto: "uma_lavorazioni"
        });

        ajaxAgronica(indirizzohttp + "/LeggiElenco_MacrousoUMACod", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    newArr.push({
                        UMA_Lavorazioni_Des: "Non Selezionato",
                        UMA_Lavorazioni_Cod: 0,
                    });

                    for (let i = 0; i < arr.length; i++) {
                        let lav_des = arr[i].UMA_Lavorazioni_Des;
                        let lav_cod = arr[i].UMA_Lavorazioni_Cod;
                        newArr.push({
                            UMA_Lavorazioni_Des: lav_des,
                            UMA_Lavorazioni_Cod: parseInt(lav_cod)
                        });
                    }
                    resolve(newArr);
                }
            }, null, null, false);
    });
}

function PopolaElenco_Operazioni() {
    return new Promise(function (resolve, reject) {
        var parametri = JSON.stringify({
            elencoRichiesto: "operazioni"
        });

        ajaxAgronica(indirizzohttp + "/LeggiElenco_MacrousoUMACod", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    newArr.push({
                        Operazioni_Des: "Non Selezionato",
                        Operazioni_Cod: 0,
                    });

                    for (let i = 0; i < arr.length; i++) {
                        let op_des = arr[i].Operazioni_Des;
                        let op_cod = arr[i].Operazioni_Cod;
                        newArr.push({
                            Operazioni_Des: op_des,
                            Operazioni_Cod: parseInt(op_cod)
                        });
                    }
                    resolve(newArr);
                }
            }, null, null, false);
    });
}

function PopolaElenco_Attivita() {
    return new Promise(function (resolve, reject) {
        var parametri = JSON.stringify({
            elencoRichiesto: "attivita"
        });

        ajaxAgronica(indirizzohttp + "/LeggiElenco_MacrousoUMACod", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    newArr.push({
                        Attivita_Des: "Non Selezionato",
                        Attivita_Cod: 0,
                    });

                    for (let i = 0; i < arr.length; i++) {
                        let at_des = arr[i].Attivita_Des;
                        let at_cod = arr[i].Attivita_Cod;
                        newArr.push({
                            Attivita_Des: at_des,
                            Attivita_Cod: parseInt(at_cod)
                        });
                    }
                    resolve(newArr);
                }
            }, null, null, false);
    });
}

//function PopolaElenco_TipologiaReport() {
//    var parametri = kendo.stringify({});
//    ajaxAgronicaSync(indirizzohttp + "/Setup_LeggiTipologieReport", parametri, false,
//        function (risposta) {
//            if (risposta.RispostaOK) {
//                let arr = JSON.parse(risposta.RispostaStringa);
//                let newArr = new Array();
//                newArr.push({
//                    Tipologia_Report_ElasDes: "Nessuna",
//                    Tipologia_Report_Elas: 0,
//                });
//                for (let i = 0; i < arr.length; i++) {
//                    let tipol_des = arr[i].Nome;
//                    let tipol_cod = arr[i].ID_Tipologia;
//                    newArr.push({
//                        Tipologia_Report_ElasDes: tipol_des,
//                        Tipologia_Report_Elas: parseInt(tipol_cod)
//                    });
//                }
//                risp = newArr;
//            }
//        }, null);

//    return risp;
//}

function PopolaElenco_TipologiaReport(elas, accise) {
    var parametri = kendo.stringify({});
    ajaxAgronicaSync(indirizzohttp + "/Setup_LeggiTipologieReport", parametri, false,
        function (risposta) {
            if (risposta.RispostaOK) {
                let arr = JSON.parse(risposta.RispostaStringa);
                let newArr = new Array();
                if (elas) {
                    newArr.push({
                        Tipologia_Report_ElasDes: "Nessuna",
                        Tipologia_Report_Elas: 0,
                    });
                    for (let i = 0; i < arr.length; i++) {
                        let tipol_des = arr[i].Nome;
                        let tipol_cod = arr[i].ID_Tipologia;
                        newArr.push({
                            Tipologia_Report_ElasDes: tipol_des,
                            Tipologia_Report_Elas: parseInt(tipol_cod)
                        });
                    }
                } else if (accise) {
                    newArr.push({
                        Tipologia_Report_SegnalazioneAcciseDes: "Nessuna",
                        Tipologia_Report_SegnalazioneAccise: 0,
                    });
                    for (let i = 0; i < arr.length; i++) {
                        let tipol_des = arr[i].Nome;
                        let tipol_cod = arr[i].ID_Tipologia;
                        newArr.push({
                            Tipologia_Report_SegnalazioneAcciseDes: tipol_des,
                            Tipologia_Report_SegnalazioneAccise: parseInt(tipol_cod)
                        });
                    }
                } else {
                    newArr.push({
                        Tipologia_Elenco_InadempientiDes: "Nessuna",
                        Tipologia_Elenco_Inadempienti: 0,
                    });
                    for (let i = 0; i < arr.length; i++) {
                        let tipol_des = arr[i].Nome;
                        let tipol_cod = arr[i].ID_Tipologia;
                        newArr.push({
                            Tipologia_Elenco_InadempientiDes: tipol_des,
                            Tipologia_Elenco_Inadempienti: parseInt(tipol_cod)
                        });
                    }
                }
                risp = newArr;
            }
        }, null);

    return risp;
}

function PopolaElenco_GruppiAllevamentoUMA() {
    return new Promise(function (resolve, reject) {
        var parametri = kendo.stringify({});

        ajaxAgronica(indirizzohttp + "/LeggiDataTabellaGruppiAllevamento", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    newArr.push({
                        UMA_AllGru_Des: "Non Selezionato",
                        UMA_AllGru_Cod: 0,
                    });

                    for (let i = 0; i < arr.length; i++) {
                        let allGru_des = arr[i].UMA_AllGru_Des;
                        let allGru_cod = arr[i].UMA_AllGru_Cod;
                        newArr.push({
                            UMA_AllGru_Des: allGru_des,
                            UMA_AllGru_Cod: allGru_cod
                        });
                    }
                    resolve(newArr);
                }
            }, null, null, false);
    });
}

function PopolaElenco_MacrousiUMA() {
    return new Promise(function (resolve, reject) {
        var parametri = kendo.stringify({});

        ajaxAgronica(indirizzohttp + "/LeggiDataTabellaMacrousiUMA", parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    let arr = JSON.parse(risposta.RispostaStringa);
                    let newArr = new Array();
                    newArr.push({
                        Macrouso_UMA_Des: "Non Selezionato",
                        Macrouso_UMA_Cod: 0,
                    });

                    for (let i = 0; i < arr.length; i++) {
                        let macrouso_UMA_Des = arr[i].Macrouso_UMA_Des;
                        let macrouso_UMA_Cod = arr[i].Macrouso_UMA_Cod;
                        newArr.push({
                            Macrouso_UMA_Des: macrouso_UMA_Des,
                            Macrouso_UMA_Cod: macrouso_UMA_Cod
                        });
                    }
                    resolve(newArr);
                }
            }, null); //, null, false);
    });
}

function RiempiElencoMacrousiUMA(options) {
    PopolaElenco_MacrousiUMA().then(
        elenco_MacrousiUMA => {
            options.success(elenco_MacrousiUMA);
        }
    )
}

//è stata selezionata una cella
function modificaLavorazione(tr_elem, grid_elem) {
    return new Promise((resolve, reject) => {
        let datiGriglia = $(grid_elem).data('kendoGrid');
        let datiRiga = datiGriglia.dataItem(tr_elem);
        var parametri = kendo.stringify({
            data: JSON.stringify(datiRiga)
        });

        ajaxAgronica(indirizzohttp + "/EditConfigurazioneUMA",
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

function cancellaLavorazione(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    var chiave = datiRiga.ID;
    var richiestaCod = datiRiga.richiestaCod;

    //if (datiRiga.CodstatoAv == 2001) {

    //    if (dialogCanc == undefined) {

    //        dialogCanc = $("#dialogCanc").kendoDialog({
    //            width: "400px",
    //            title: Traduzione(gestioneCarbResx, "SeiSicuroDiEliminareQuestoElemento", "Sei sicuro di voler eliminare questo elemento?"),
    //            buttonLayout: "stretched",
    //            content: "<p>Stai per cancellare questa richiesta e tutte le lavorazioni ad essa assegnate.<p>",
    //            actions: [
    //                { text: Traduzione(gestioneCarbResx, "Annulla", "Annulla"), primary: true, },
    //                {
    //                    text: Traduzione(gestioneCarbResx, "SiSonoSicuro", "Si sono sicuro"),

    //                    action: function (e) {
    //                        WaitFrame.show();
    //                        ajaxAgronicaSync("./ElencoRichieste.aspx/DeleteRichiesta",
    //                            "{Piva: '" + chiave + "' , " +
    //                            "richiestaCod: '" + richiestaCod + "' }",
    //                            false,
    //                            function (data) {
    //                                //kendo.alert(JSON.parse(data.RispostaStringa));
    //                                WaitFrame.hide();
    //                                ConfiguraGrigliaElenco("tab_griglia_elencoRichieste");
    //                            }, null);
    //                        return true;
    //                    },
    //                }
    //            ],
    //        });

    //    }

    //    dialogCanc.data("kendoDialog").open();

    //} else if (datiRiga.CodstatoAv >= 2007) {
    //    if (dialogRinuncia == undefined) {

    //        dialogRinuncia = $("#dialogRinuncia").kendoDialog({
    //            width: "400px",
    //            title: Traduzione(gestioneCarbResx, "SeiSicuroDiRinunciare", "Sei sicuro di voler richiedere la rinuncia di questa richiesta?"),
    //            buttonLayout: "stretched",
    //            content: "<p>Stai per richiedere la rinuncia di questa richiesta (i litri relativi ad essa non saranno conteggianti nella rendicontazione).<p>",
    //            actions: [
    //                { text: Traduzione(gestioneCarbResx, "Annulla", "Annulla"), primary: true, },
    //                {
    //                    text: Traduzione(gestioneCarbResx, "SiSonoSicuro", "Si sono sicuro"),

    //                    action: function (e) {
    //                        WaitFrame.show();
    //                        ajaxAgronicaSync("./ElencoRichieste.aspx/RinunciaRichiesta",
    //                            "{Piva: '" + chiave + "' , " +
    //                            "richiestaCod: '" + richiestaCod + "' }",
    //                            false,
    //                            function (data) {
    //                                //kendo.alert(JSON.parse(data.RispostaStringa));
    //                                WaitFrame.hide();
    //                                ConfiguraGrigliaElenco("tab_griglia_elencoRichieste");
    //                            }, null);
    //                        return true;
    //                    },
    //                }
    //            ],
    //        });

    //    }

    //    dialogRinuncia.data("kendoDialog").open();
    //}


}

/** Legge la vista uma_allevamenti */
function UMAConfigurazioneAllevamenti_elencoAllevamenti() {
    var risp = [];

    var storage_key = "elencoAllevamenti";
    if (!storageExistItem(storage_key)) {

        ajaxAgronicaSync(indirizzohttp + "/ElencoAllevamenti", null, false, function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            storageSetItem(storage_key, risposta.RispostaStringa);
        }, null);
    }
    else {
        risp = JSON.parse(storageGetItem(storage_key));
    }

    return risp;
}

function getElencoTipoMacchine() {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
        });

        ajaxAgronica(indirizzohttp + "/getElencoTipoMacchine",
            parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                resolve(risp)

            }, null, null, false);
    });
}

function getElencoStati() {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
        });

        ajaxAgronica(indirizzohttp + "/getElencoStati",
            parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                resolve(risp)

            }, null, null, false);
    });
}

function getElencoGruppiUtente() {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
        });

        ajaxAgronica(indirizzohttp + "/getElencoGruppiUtente",
            parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                resolve(risp)

            }, null, null, false);
    });
}

function getTipologieDocumenti() {
    var parametri = kendo.stringify({
        "objP_server": objP_server,
        "piva": "", "soloPrivate": false,
        "id_area": "7", "controllaSeUtenteAutorizzato": false,
        "tipoPermessoDaControllare": 2,
        "listaIndici": "",
        "xMultiSelect": false,
        "id_tipologia": 0
    });

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Alert_Tipologia.asmx/Leggi_Tipologie",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

        }, null);

}