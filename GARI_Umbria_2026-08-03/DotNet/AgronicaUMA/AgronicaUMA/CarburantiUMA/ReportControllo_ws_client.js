var indirizzohttp = "./ReportControllo.aspx";
var indirizzohttpWSGenerali = "./CdG_WS.aspx";
var indirizzohttpRichiestaCarburanti = "./RichiestaCarburanti.aspx";

//ELAS
function CercaReportELAS(options) {

    anno_elas = isNaN(parseInt($('#anno')[0].value)) ? 0 : parseInt($('#anno')[0].value);
    bimestre_elas = Get_KendoDDLValue("bimestre");

    var parametri = {
        anno: anno_elas,
        bimestre: bimestre_elas
    }

    ajaxAgronica(indirizzohttp + "/CercaReportELAS", JSON.stringify(parametri), function (risposta) {
        risp = JSON.parse(risposta.RispostaStringa);
        options.success(risp);
    }, null);
}

function SalvaDocumento(file, nomefile, estensioneFile, tipologia, desc) {

    var id_area = 7;
    var id_elenco = -1;
    var id_alert_entita = -1;

    var id_tipologia = tipologia;

    var nome_file = nomefile + "_" + username_master + "_." + estensioneFile;
    var file_allegato = file;

    var data_upload = Date.now();
    var username_upload = username_master;

    var validazione_flag = 0;

    var chkstorico = 0;

    var cod_contatto = ""
    var sa_cod = 0 // $('#ddlCentro').data("kendoDropDownList").value();
    var appezza = 0// $('#ddlAppezzamento').data("kendoDropDownList").value();                
    var pc_testata_cod = 0 // $('#ddlPianoConcimazione').data("kendoDropDownList").value();
    var pua_cod = 0 // $('#ddlPua').data("kendoDropDownList").value();
    var mac_cod = "";
    var analisi_testata_cod = 0;
    var id_agenda = '0';
    var Ricetta_Operazione_Cod = '0';
    var Id_ImpresexParticelle = '0';
    var descrizione = desc;
    var id_schema_template = 0;
    
    //chiamo il web service
    var strObjJSON = JSON.stringify({
        "id_elenco": id_elenco,
        "id_alert_entita": id_alert_entita,
        "allegati_documenti_cod": 0,
        "id_area": id_area,
        "id_tipologia": id_tipologia,
        "ballegato_modificato": true,
        "piva": cIdPivaSuperUser,
        "sa_cod": sa_cod,
        "appezza": appezza,
        "mac_cod": mac_cod,
        "cod_contatto": cod_contatto,
        "analisi_testata_cod": analisi_testata_cod,
        "id_agenda": id_agenda,
        "pc_testata_cod": pc_testata_cod,
        "pua_cod": pua_cod,
        "richiesta_cod": 0,
        "id_schema_template": id_schema_template,
        "num_documento": 0,
        "testo": descrizione,
        "data": data_upload,
        "nome_file": nome_file,
        "file_allegato": file_allegato,
        "validazione_flag": validazione_flag,
        "username_upload": username_upload,
        "data_upload": data_upload,
        "note": "",
        "chkstorico": chkstorico,
        "entitaxindici": undefined,
        "multiTipologia": "",
        "CompressoDaGIAS": false,
        "Ricetta_Operazione_Cod": Ricetta_Operazione_Cod,
        "Id_ImpresexParticelle": Id_ImpresexParticelle
    });

    var parametri = JSON.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "strObjJSON": strObjJSON });

    ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert.asmx/Scrivi",
        parametri,
        function (risposta) {
            if (risposta.RispostaOK) {
                // MessaggioTuttoOK_Bootstrap("Allegato salvato correttamente", 'DIV_Messaggi');
                // kendo.alert("Allegato salvato correttamente");
                // parent.chiudiGestioneAllegati();
                MessaggioTuttoOK_Bootstrap("Allegato salvato correttamente", "DIV_Messaggi"); 
            } else {
                // MessaggioErrore_Bootstrap("Errore durante salvataggio allegato", "DIV_Messaggi");
                MessaggioErrore_Bootstrap("Errore durante salvataggio allegato", "DIV_Messaggi"); 
            }            
        }, null);
    
}


//Elenco Inadempienti 
function CercaElencoInadempienti(options) {

    anno_elenco_inadempienti = isNaN(parseInt($('#annoIna')[0].value)) ? 0 : parseInt($('#annoIna')[0].value);

    var parametri = {
        anno: anno_elenco_inadempienti,
        prov: $('#prov')[0].value,
        com: $('#citta')[0].value,
        statoPrat: $('#statoPratica')[0].value,
        conto: $('#conto')[0].value,
    }

    ajaxAgronica(indirizzohttp + "/CercaElencoInadempienti", JSON.stringify(parametri), function (risposta) {
        risp = JSON.parse(risposta.RispostaStringa);
        options.success(risp);
    }, null);
}

//Elenco Trasferimenti
function CercaElencoTrasferimenti(options) {

    anno_Trasf = isNaN(parseInt($('#annoTrasf')[0].value)) ? 0 : parseInt($('#annoTrasf')[0].value);

    var parametri = {
        anno: anno_Trasf
    }

    ajaxAgronica(indirizzohttp + "/CercaElencoTrasferimenti", JSON.stringify(parametri), function (risposta) {
        risp = JSON.parse(risposta.RispostaStringa);
        options.success(risp);
    }, null);
}

//Elenco Segnalazione Accise
function CercaElencoSegnalazioni(options) {

    anno_elenco_segnalazioni = parseInt($('#annoAcc')[0].value);

    var parametri = {
        anno: anno_elenco_segnalazioni,
        prov: $('#provAcc')[0].value,
        com: $('#cittaAcc')[0].value,
        tipoPrat: $('#tipoPraticaAcc')[0].value,
        conto: $('#contoAcc')[0].value,
        giaSegnalate: $('#giaSegnAcc')[0].value == "1" ? true : false, 
        segnalateDal: $("#segnDaAcc").val(),
        rimanenze: rimanenze,
    }

    ajaxAgronica(indirizzohttp + "/CercaElencoSegnalazioni", JSON.stringify(parametri), function (risposta) {
        risp = JSON.parse(risposta.RispostaStringa);
        options.success(risp);
    }, null);
}

function SegnalaSelezionati(selected) {
    WaitFrame.show();
    return new Promise(function (resolve, reject) {
        let parametri = {
            anno: anno_elenco_segnalazioni,
            data: $("#dataSegnAcc").val(),
            righeSegnalate: JSON.stringify(selected),
        };
        ajaxAgronicaSync(indirizzohttp + "/SegnalaSelezionate",
            JSON.stringify(parametri),
            false,
            function (risposta) {
                WaitFrame.hide();
                resolve();
            }, null, null, false);
    });

}

function AnnullaSegnalazioneSelezionati(selected) {
    WaitFrame.show();
    return new Promise(function (resolve, reject) {
        let parametri = {
            anno: anno_elenco_segnalazioni,
            righeSegnalate: JSON.stringify(selected),
        };
        ajaxAgronicaSync(indirizzohttp + "/AnnullaSegnalazioneSelezionati",
            JSON.stringify(parametri),
            false,
            function (risposta) {
                WaitFrame.hide();
                resolve();
            }, null, null, false);
    });

}


//Utility

function LeggiSetup(anno) {
    return new Promise(function (resolve, reject) {
        let storage_key = "LeggiSetup_" + anno;
        if (!storageExistItem(storage_key)) {
            var parametri = {
                anno: anno
            };
            ajaxAgronica(indirizzohttp + "/Leggi_UMA_Setup",
                JSON.stringify(parametri),
                function (risposta) {
                    storageSetItem(storage_key, risposta.RispostaStringa);
                    let resp = JSON.parse(risposta.RispostaStringa);
                    resolve(resp);
                }, null);
        } else {
            resolve(JSON.parse(storageGetItem(storage_key)));
        }
    });
}

function GetUrlDocAgenda2010(piva, id_Tipologia, richiesta_Cod, pratica_Cod, id_Schema_Template, operazione, Documentale1_Anagrafica2) {
    var parametri = kendo.stringify({
        "piva": piva,
        "id_Tipologia": id_Tipologia,
        "richiesta_Cod": richiesta_Cod,
        "pratica_Cod": pratica_Cod,
        "id_Schema_Template": id_Schema_Template,
        "operazione": operazione,
        "Documentale1_Anagrafica2": Documentale1_Anagrafica2
    });

    var risp = "";

    ajaxAgronicaSync(indirizzohttpRichiestaCarburanti + "/GetUrlDocAgenda2010", parametri, false,
        function (risposta) {
            risp = risposta.RispostaStringa;
        }, null);

    return risp;}
    
function RiempiStatiPratiche(options) {

    var parametri = {
    }

    ajaxAgronicaSync("./elencoRichieste.aspx/RiempiStatiPratiche",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            /*objVuoto = { "cod": "-1", "stato": "TUTTI" };
            risp.unshift(objVuoto);*/
            options.success(risp);
        }, null);
}

function RiempiCitta(options) {

    return new Promise(function (resolve, reject) {
        var parametri = {
            prov: KendoDDL(cittaTab.replace("citta","prov").slice(1)).value()
        }

        ajaxAgronica("./elencoRichieste.aspx/RiempiCitta",
            JSON.stringify(parametri),
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                objVuoto = { "proCom": "-1", "citta": "TUTTE" };
                risp.unshift(objVuoto);
                WaitFrame.hide();
                options.success(risp);
            }, null);
    });
}

function RiempiProv(options) {

    var parametri = {
    }

    ajaxAgronica("./elencoRichieste.aspx/RiempiProv",
        JSON.stringify(parametri),
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "PROV": "-1", "PROVINCIA": "TUTTE" };
            risp.unshift(objVuoto);
            options.success(risp);
        }, null);
}

function LeggiTermineUltimoRendicontazione(anno) {
    return new Promise(function (resolve, reject) {
        
        var parametri = {
            anno: anno
        };
        ajaxAgronica(indirizzohttp + "/LeggiTermineUltimoRendicontazione",
            JSON.stringify(parametri),
            function (risposta) {
                let resp = JSON.parse(risposta.RispostaStringa);
                resolve(resp);
            }, null);
       
    });
}