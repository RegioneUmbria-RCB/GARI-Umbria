/*
* Creato il 2017-07-18
* 
* 
* Contiene tutti i WS alla pagina Impianto_edit.vb
*
* Author: Drudi Riccardo
*/


function TestSuperficieCfrGis() {

    var oToTestGisEntita = {
        Piva: obj_Impianto.piva,
        Sa_Cod: obj_Impianto.sa_cod,
        Appezza: obj_Impianto.appezza,
        Id_Imp: obj_Impianto.id_reg,
        TipoEntita_Cod: 19
    }


    $("#msgCfrGisSuperficieSenzaCatasto").html("");

    ajaxAgronica("../GIS/GIS.aspx/TestSuperficieCfrGis", JSON.stringify({ ToTestGisEntita: oToTestGisEntita, SuperficieDaTestare: $("#TxtSuperficie").val().replace(",", ".") }),
        function (risposta) {
            if (risposta.RispostaStringa !== "") {
                $("#msgCfrGisSuperficieSenzaCatasto").html(risposta.RispostaStringa);
            }
        }, null);

}

function RiempiCmb_Specie(options) {

    var parametri = kendo.stringify({ "objP_utenti": objP_utenti, "Gru_Cod": 0, "LetteraIniziale": "", "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

    ajaxAgronica(pathCoreWS + "Metaschema/SpecieVegetali.asmx/CaricaComboSpecieVegetali_conFiltroUtente",
        parametri,
        function (risposta) {
            let specievegetali = JSON.parse(risposta.RispostaStringa);

            // ordina i risultati per descrizione
            specievegetali.sort(function (a, b) {
                if (a.veg_des > b.veg_des) return 1;
                else if (a.veg_des < b.veg_des) return -1;
                return 0;
            });

            objVuoto = { "veg_cod": "", "veg_des": "" };
            specievegetali.unshift(objVuoto);
            options.success(specievegetali);
        }, null, null, false);

}

function RiempiCmb_CodiciTerreno(options) {

    var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "LetteraIniziale": "", "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

    ajaxAgronica(pathCoreWS + "Metaschema/SpecieVegetali.asmx/CaricaComboCodici_Terreno",
        parametri,
        function (risposta) {
            let specievegetali = JSON.parse(risposta.RispostaStringa);

            // ordina i risultati per descrizione
            specievegetali.sort(function (a, b) {
                if (a.descrizione > b.descrizione) return 1;
                else if (a.descrizione < b.descrizione) return -1;
                return 0;
            });

            objVuoto = { "codice": "", "descrizione": "" };
            specievegetali.unshift(objVuoto);
            options.success(specievegetali);
        }, null, null, false);

}

function CaricaComboFinalita(options) {

    if (obj_Impianto.veg_cod !== null && obj_Impianto.veg_cod !== 0 && obj_Impianto.veg_cod !== "" && obj_Impianto.veg_cod !== "0") {

        var parametri = kendo.stringify({ "objP_server": objP_server, "Veg_Cod": obj_Impianto.veg_cod, "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

        ajaxAgronica(pathCoreWS + "Metaschema/GruppoFinalita.asmx/CaricaComboFinalita",
            parametri,
            function (risposta) {
                let specievegetali = JSON.parse(risposta.RispostaStringa);
                options.success(specievegetali);
            }, null, null, false);

    } else {
        options.success([]);
    }

}

function CaricaComboCultivar_conFiltroUtente(options) {

    if (obj_Impianto.veg_cod !== null && obj_Impianto.veg_cod !== 0 && obj_Impianto.veg_cod !== "" && obj_Impianto.veg_cod !== "0") {
        var parametri = kendo.stringify({ "objP_utenti": objP_utenti, "Veg_Cod": obj_Impianto.veg_cod, "LetteraIniziale": "", "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

        ajaxAgronica(pathCoreWS + "Metaschema/Cultivar.asmx/CaricaComboCultivar_conFiltroUtente",
            parametri,
            function (risposta) {
                let specievegetali = JSON.parse(risposta.RispostaStringa);
                options.success(specievegetali);
            }, null, null, false);
    } else {
        options.success([]);
    }

}

function CaricaComboCmb_Copertura(options) {

    if (obj_Impianto.veg_cod !== null && obj_Impianto.veg_cod !== 0 && obj_Impianto.veg_cod !== "" && obj_Impianto.veg_cod !== "0") {
        var parametri = kendo.stringify({ "objP_server": objP_server, "Veg_Cod": obj_Impianto.veg_cod, "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

        ajaxAgronica(pathCoreWS + "Metaschema/Copertura.asmx/CaricaComboCopertura",
            parametri,
            function (risposta) {
                let specievegetali = JSON.parse(risposta.RispostaStringa);
                options.success(specievegetali);
            }, null, null, false);
    } else {
        options.success([]);
    }

}

function CaricaComboTipologiaVarietale(options) {

    if (obj_Impianto.veg_cod !== null && obj_Impianto.veg_cod !== 0 && obj_Impianto.veg_cod !== "" && obj_Impianto.veg_cod !== "0") {
        var parametri = kendo.stringify({ "objP_server": objP_server, "Veg_Cod": obj_Impianto.veg_cod, "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

        ajaxAgronica(pathCoreWS + "Metaschema/GruppoVarietale.asmx/CaricaComboGruppoVarietale",
            parametri,
            function (risposta) {
                let specievegetali = JSON.parse(risposta.RispostaStringa);
                objVuoto = { "Grva_Cod": "", "Grva_Des": "" };
                specievegetali.unshift(objVuoto);
                options.success(specievegetali);
            }, null, null, false);
    } else {
        options.success([]);
    }

}

function CaricaComboCmb_ImpIrrigazione(options) {

    if (obj_Impianto.veg_cod !== null && obj_Impianto.veg_cod !== 0 && obj_Impianto.veg_cod !== "" && obj_Impianto.veg_cod !== "0") {

        var parametri = kendo.stringify({ "objP_server": objP_server, "Veg_Cod": obj_Impianto.veg_cod, "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

        ajaxAgronica(pathCoreWS + "Metaschema/ImpiantiIrrigazioni.asmx/CaricaImpiantiIrrigazioni",
            parametri,
            function (risposta) {
                let specievegetali = JSON.parse(risposta.RispostaStringa);
                objVuoto = { "Imp_Cod": "", "Imp_Des": "" };
                specievegetali.unshift(objVuoto);
                options.success(specievegetali);
            }, null, null, false);

    } else {
        options.success([]);
    }

}

function CaricaComboCmb_FormaAllevamento(options) {

    if (obj_Impianto.veg_cod !== null && obj_Impianto.veg_cod !== 0 && obj_Impianto.veg_cod !== "" && obj_Impianto.veg_cod !== "0") {

        var parametri = kendo.stringify({ "objP_server": objP_server, "Veg_Cod": obj_Impianto.veg_cod, "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

        ajaxAgronica(pathCoreWS + "Metaschema/FormeAllevamento.asmx/CaricaFormeAllevamento",
            parametri,
            function (risposta) {
                let specievegetali = JSON.parse(risposta.RispostaStringa);
                objVuoto = { "Foral_Cod": "", "Foral_Des": "" };
                specievegetali.unshift(objVuoto);
                options.success(specievegetali);
            }, null, null, false);

    } else {
        options.success([]);
    }

}

function CaricaComboCmb_Portinnesto(options) {

    if (obj_Impianto.veg_cod !== null && obj_Impianto.veg_cod !== 0 && obj_Impianto.veg_cod !== "" && obj_Impianto.veg_cod !== "0") {
        var parametri = kendo.stringify({ "objP_server": objP_server, "Veg_Cod": obj_Impianto.veg_cod, "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

        ajaxAgronica(pathCoreWS + "Metaschema/Portinnesti.asmx/CaricaComboPortinnesti",
            parametri,
            function (risposta) {
                let specievegetali = JSON.parse(risposta.RispostaStringa);
                objVuoto = { "Port_Cod": "", "Port_Des": "" };
                specievegetali.unshift(objVuoto);
                options.success(specievegetali);
            }, null, null, false);
    } else {
        options.success([]);
    }

}

function CaricaComboCmb_SeminaTrapianto(options) {

}

function CaricaComboCmb_DettaglioVarietaPersonalizzato(options) {
    var parametri = kendo.stringify({ "objP_server": objP_server, "Argomento_Cod": 2, "InfoAgg_Cod": "", "Tipo_Codifica": 0, "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

    ajaxAgronica(pathCoreWS + "Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiCAC_Codifica_InfoAggiuntive",
        parametri,
        function (risposta) {
            let specievegetali = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "InfoAgg_Cod": "", "InfoAgg_Des": "" };
            specievegetali.unshift(objVuoto);
            options.success(specievegetali);
        }, null, null, false);
}
function CaricaComboCmb_CodiceZona(options) {
    var toDay = new Date().toJSON().slice(0, 10).replace(/-/g, '/');
    /*toDay = formattedDate(new Date(), "/");*/

    var parametri = kendo.stringify({
        "objP_server": objP_server,
        "objP_utenti": objP_utenti,
        "objP_super_server": objP_super_server,
        "Cau_Mov": "7300",
        "Piva": obj_Impianto.piva,
        "Sa_Cod": 0,
        "Id_Destinazione": 0,
        "Elem_Cod": 700,
        "Flag_Negativo": false,
        "RicercaTesto": "",
        "RicercaTestoJArray": "",
        "Flag_VisualizzaProCod": false,
        "Flag_CaricaUdmCod": false,
        "Pro_Cod": 0,
        "Udm_Cod": 0,
        "DataFiltroFormulati": toDay,
        "PUA_RegolamentoCod": 0,
        "TipoRichiesto": 0,
        "Flag_LeggiGiacenze": false,
        "Flag_FiltraRevocati": false,
        "RegolamentoCod_Operazioni": 0,
        "Tipo_PuaRegolamento": 0,
        "Flag_IncludiNPK_Desc": false,
        "Flag_IncludiClassificazione": false,
        "Flag_QtaNoZero": false,
        "xFiltroAggiuntivo": "",
        "Flag_Filtra_MateriePrime_Per_Piva": true,
        "Flag_Filtra_MateriePrime_Pubblici": true,
        "Flag_CodArticolo_In_Descrizione": false
    });

    ajaxAgronica(pathCoreWS + "Anagrafica/Prodotti.asmx/Prodotti_x_CAC",
        parametri,
        function (risposta) {
            let specievegetali = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "pro_cod": "", "pro_des": "" };
            specievegetali.unshift(objVuoto);
            options.success(specievegetali);
        }, null, null, false);
}

function formattedDate(date, sep) {

    var d = new Date(date || Date.now()),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [day, month, year].join(sep);

}

function CaricaComboCmb_ConduzioneSu(options) {

    if (obj_Impianto.veg_cod !== null && obj_Impianto.veg_cod !== 0 && obj_Impianto.veg_cod !== "" && obj_Impianto.veg_cod !== "0") {
        var parametri = kendo.stringify({ "Veg_Cod": obj_Impianto.veg_cod });

        ajaxAgronica("Impianto_Edit2.aspx/CaricaCombo_ConduzioneSu",
            parametri,
            function (risposta) {
                let specievegetali = JSON.parse(risposta.RispostaStringa);
                objVuoto = { "Tecn_Cod": "", "Tecn_Des": "" };
                specievegetali.unshift(objVuoto);
                options.success(specievegetali);
            }, null, null, false);
    } else {
        options.success([]);
    }


}

function CaricaComboCmb_ConduzioneTra(options) {

    if (obj_Impianto.veg_cod !== null && obj_Impianto.veg_cod !== 0 && obj_Impianto.veg_cod !== "" && obj_Impianto.veg_cod !== "0") {
        var parametri = kendo.stringify({ "Veg_Cod": obj_Impianto.veg_cod });

        ajaxAgronica("Impianto_Edit2.aspx/CaricaCombo_ConduzioneTra",
            parametri,
            function (risposta) {
                let specievegetali = JSON.parse(risposta.RispostaStringa);
                objVuoto = { "Tecn_Cod": "", "Tecn_Des": "" };
                specievegetali.unshift(objVuoto);
                options.success(specievegetali);
            }, null, null, false);
    } else {
        options.success([]);
    }


}

function CaricaComboCmb_Regolamento(options) {
    var parametri = kendo.stringify({ "xFiltroAggiuntivo": "", "xOrderBy": "", "objP_server": objP_server });

    ajaxAgronica(pathCoreWS + "Metaschema/Regolamenti.asmx/CaricaComboRegolamenti",
        parametri,
        function (risposta) {
            let specievegetali = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "Reg_Cod": "1", "Reg_Des": "" };
            specievegetali.unshift(objVuoto);
            options.success(specievegetali);
        }, null, null, false);
}

function CaricaComboCmb_Disciplinare(options) {

    if (obj_Impianto.veg_cod !== 0 && obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Regolamento_Cod !== undefined) {
        //var data = obj_Distinta_Selezionata.Validita_Inizio.toLocaleDateString();
        var data = "";
        var flag_pp;
        if (obj_Distinta_Selezionata.Disciplinare_PubblicoPrivato === 0) {
            flag_pp = false;
        } else {
            flag_pp = true;
        }
        //var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "veg_cod": obj_Impianto.veg_cod, "data": data, "flag_disciplinareprivato": flag_pp, "reg_cod": obj_Distinta_Selezionata.Regolamento_Cod });
        var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "veg_cod": obj_Impianto.veg_cod, "data": data, "flag_disciplinareprivato": flag_disciplinareprivato, "reg_cod": 0 });

        ajaxAgronica(pathCoreWS + "AgronicaCoreDPI/DPI.asmx/CaricaComboDisciplinare",
            parametri,
            function (risposta) {
                let specievegetali = JSON.parse(risposta.RispostaStringa);
                options.success(specievegetali);
            }, null, null, false);
    } else {
        options.success([]);
    }
}

function CaricaComboCmb_IAF(options) {

    if (obj_Impianto.veg_cod !== 0 && obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Disciplinare_Cod !== undefined) {
        var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "veg_cod": obj_Impianto.veg_cod, "data": "", "flag_disciplinareprivato": flag_disciplinareprivato, "disciplinare_cod": obj_Distinta_Selezionata.Disciplinare_Cod });

        ajaxAgronica(pathCoreWS + "AgronicaCoreDPI/DPI.asmx/CaricaComboIAF",
            parametri,
            function (risposta) {
                let specievegetali = JSON.parse(risposta.RispostaStringa);
                options.success(specievegetali);
            }, null, null, false);
    } else {
        options.success([]);
    }

}

function CaricaComboCmb_CapitolatoPrivato(options) {

    var parametri = kendo.stringify({ "objP_server": objP_server, "Argomento_Cod": 1, "InfoAgg_Cod": "", "Tipo_Codifica": 0, "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

    ajaxAgronica(pathCoreWS + "Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiCAC_Codifica_InfoAggiuntive",
        parametri,
        function (risposta) {
            let specievegetali = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "InfoAgg_Cod": "", "InfoAgg_Des": "" };
            specievegetali.unshift(objVuoto);
            options.success(specievegetali);
        }, null, null, false);

}


function CaricaComboCmb_OrganismoReferente(options) {

    var parametri = kendo.stringify({ "objP_server": objP_server, "piva": JSON.parse(objP_agenda).Piva });

    ajaxAgronica(pathCoreWS + "Anagrafica/Contatti.asmx/CaricaComboCmb_OrganismoReferente",
        parametri,
        function (risposta) {
            let specievegetali = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "text": "", "value": "" };
            specievegetali.unshift(objVuoto);
            options.success(specievegetali);
        }, null, null, false);

}

//  - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
function CaricaComboCmb_LicenzaColtivazione(options) {
    var valoriParamQual = RicercaValoriParametriQualitativi(1330, obj_Impianto.piva, true);

    options.success(valoriParamQual);


    //ajaxAgronica(pathCoreWS + "/CaricaComboCmb_LicenzaColtivazione",
    //    parametri,
    //    function (risposta) {
    //        let LicenzaColtivazione = RicercaValoriParametriQualitativi(1330, obj_Impianto.piva);
    //        objVuoto = { "text": "", "value": "" };
    //        LicenzaColtivazione.unshift(objVuoto);
    //        options.success(LicenzaColtivazione);
    //    }, null, null, false);
}
// - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

function CaricaComboCmb_Riferimento_Trasferimento_Dati(options) {

    var parametri = kendo.stringify({ "objP_super_server": objP_super_server, "objP_server": objP_server, "objP_utenti": objP_utenti, "piva": JSON.parse(objP_agenda).Piva });

    ajaxAgronica(pathCoreWS + "Anagrafica/Contatti.asmx/CaricaComboCmb_Riferimento_Trasferimento_Dati",
        parametri,
        function (risposta) {
            let trasferimentoDati = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "text": "", "value": "" };
            trasferimentoDati.unshift(objVuoto);
            options.success(trasferimentoDati);
        }, null, null, false);

}

function CaricaComboCmb_MagazzinoConferimento(options) {

    var parametri = kendo.stringify({ "objP_server": objP_server, "piva": "" });

    ajaxAgronica(pathCoreWS + "Anagrafica/Contatti.asmx/CaricaComboCmb_MagazzinoConferimento",
        parametri,
        function (risposta) {
            let specievegetali = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "text": "", "value": "" };
            specievegetali.unshift(objVuoto);
            options.success(specievegetali);
        }, null, null, false);

}

function CaricaComboCmb_RegolamentoConc(options) {


    if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Regolamento_Cod !== undefined) {
        var filtro = "";
        if (obj_Distinta_Selezionata.Validita_Inizio !== undefined && obj_Distinta_Selezionata.Validita_Fine !== undefined) {
            filtro = "PUA_Regolamenti.Validita_inizio <= '" + obj_Distinta_Selezionata.Validita_Fine + "' AND PUA_Regolamenti.Validita_Fine >= '" + obj_Distinta_Selezionata.Validita_Inizio + "'";
        }
        var parametri = kendo.stringify({ "objP_super_server": objP_super_server, "objP_server": objP_server, "objP_utenti": objP_utenti, "Regolamento_Cod": 0, "xFiltroAggiuntivo": filtro, "xOrderBy": "Regolamento_Des" });

        ajaxAgronica(pathCoreWS + "Metaschema/PUA_Regolamenti.asmx/Leggi_PUA_RegolamentixEditImpianto",
            parametri,
            function (risposta) {
                let specievegetali = JSON.parse(risposta.RispostaStringa);
                objVuoto = { "Reg_Des": "", "Reg_Cod": "1" };
                specievegetali.unshift(objVuoto);
                options.success(specievegetali);
            }, null, null, false);
    } else {
        options.success([]);
    }

}


function CaricaComboCmb_FinalitaConc(options) {

    if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Regolamento_Concimazione_Cod !== "0" && obj_Distinta_Selezionata.Regolamento_Concimazione_Cod !== "" && obj_Impianto.veg_cod !== 0 && obj_Impianto.grfi_cod !== 0) {

        var parametri = kendo.stringify({
            objP_super_server: objP_super_server,
            objP_server: objP_server,
            PrimaRiga_Flag: false,
            PrimaRiga_Text: "",
            PrimaRiga_Value: "",
            Regolamento_Cod: obj_Distinta_Selezionata.Regolamento_Concimazione_Cod,
            Veg_Cod: obj_Impianto.veg_cod,
            Grfi_Cod: obj_Impianto.grfi_cod,
            xFiltroAggiuntivo: "",
            xOrderBy: ""
        });

        ajaxAgronica(pathCoreWS + "AgronicaCoreDPI/PianoConcimazione.asmx/PC_Finalita_Rer_WS",
            parametri,
            function (risposta) {
                let finalitaconc = JSON.parse(risposta.RispostaStringa);
                objVuoto = { "Descrizione": "", "Codice": "" };
                finalitaconc.unshift(objVuoto);
                options.success(finalitaconc);
            }, null, null, false);

    } else {

        options.success([]);

    }
}


function CaricaComboCmb_Stato(options) {

    if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Regolamento_Concimazione_Cod !== undefined && obj_Impianto.veg_cod !== 0 && obj_Impianto.grfi_cod !== 0) {
        let parametri;
        switch (obj_Distinta_Selezionata.Regolamento_Concimazione_Cod) {
            case 1: case 2: case -1: case -2:

                parametri = kendo.stringify({ "objP_super_server": objP_super_server, "objP_server": objP_server, "Veg_Cod": obj_Impianto.veg_cod, "Grfi_Cod": obj_Impianto.grfi_cod, "Regolamento_Cod": Math.abs(obj_Distinta_Selezionata.Regolamento_Concimazione_Cod), "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

                ajaxAgronica(pathCoreWS + "Metaschema/GruppoFinalita.asmx/CaricaComboFinalita2",
                    parametri,
                    function (risposta) {
                        let specievegetali = JSON.parse(risposta.RispostaStringa);
                        options.success(specievegetali);
                    }, null, null, false);

                break;
            default:

                parametri = kendo.stringify({ "objP_super_server": objP_super_server, "objP_server": objP_server, "Veg_Cod": obj_Impianto.veg_cod, "Grfi_Cod": obj_Impianto.grfi_cod, "Regolamento_Cod": Math.abs(obj_Distinta_Selezionata.Regolamento_Concimazione_Cod), "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

                ajaxAgronica(pathCoreWS + "Metaschema/GruppoFinalita.asmx/CaricaComboFinalita2",
                    parametri,
                    function (risposta) {
                        let specievegetali = JSON.parse(risposta.RispostaStringa);
                        var cont102 = false;
                        for (let i = 0; i < specievegetali.length; i++) {
                            if (specievegetali[i].grfi_cod === 102) {
                                cont102 = true;
                            }
                        }
                        if (!cont102) {
                            obj102 = { "grfi_cod": "102", "grfi_des": TraduzioneMultiResx(impiantoEditResx, "ImpiantoInProduzione", "Impianto in Produzione") };
                            specievegetali.unshift(obj102);
                        }
                        options.success(specievegetali);
                    }, null, null, false);

                break;
        }

    } else {
        options.success([]);
    }

}

function CaricaComboCmb_PianoSemina(options) {
    var parametri = kendo.stringify({ "objP_server": objP_server, "Argomento_Cod": 5, "InfoAgg_Cod": "", "Tipo_Codifica": 0, "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

    ajaxAgronica(pathCoreWS + "Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiCAC_Codifica_InfoAggiuntive",
        parametri,
        function (risposta) {
            let specievegetali = JSON.parse(risposta.RispostaStringa);
            for (let i = 0; i < specievegetali.length; i++) {
                specievegetali[i].InfoAgg_Des = specievegetali[i].InfoAgg_Cod + " - " + specievegetali[i].InfoAgg_Des
            }
            objVuoto = { "InfoAgg_Cod": "", "InfoAgg_Des": "" };
            specievegetali.unshift(objVuoto);
            options.success(specievegetali);
        }, null, null, false);
}

function get_gru_cod(veg_cod, callback) {

    var parametri = kendo.stringify({ "veg_cod": veg_cod });

    ajaxAgronica("Impianto_Edit2.aspx/get_gru_cod",
        parametri,
        function (risposta) {
            callback(risposta.RispostaStringa);
        }, null, null, false);

}

function WS_Controlla_Dati(obj_impianto_str, saltaPrimoControllo, saltaSecondoControllo) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            "obj_Impianto_str": obj_impianto_str,
            "saltaPrimoControllo": saltaPrimoControllo,
            "saltaSecondoControllo": saltaSecondoControllo
        });
        ajaxAgronica("Impianto_Edit2.aspx/Controlla_Dati",
            parametri,
            function (risposta) {
                let obj_risposta = JSON.parse(risposta.RispostaStringa);
                resolve(obj_risposta)
            }, null, null, false);
    });
}

function WS_Controlla_DatixEliminazioneDistinta(obj_distinta) {
    var risp = null
    var parametri = kendo.stringify({
        "obj_Impianto": kendo.stringify( obj_Impianto),
        "obj_distinta": obj_distinta
    });

    ajaxAgronicaSync("Impianto_Edit2.aspx/Controlla_DatixCancellazioneDistinta",
        parametri, false,
        function (risposta) {
             risp = JSON.parse(risposta.RispostaStringa);            
        }, null);

    return risp
}

function WS_Controlla_DatixModificaDistinta(obj_distinta) {
    var risp = null
    var parametri = kendo.stringify({
        "obj_Impianto": kendo.stringify(obj_Impianto),
        "obj_distinta": obj_distinta
    });

    ajaxAgronicaSync("Impianto_Edit2.aspx/Controlla_DatixModificaDistinta",
        parametri, false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
        }, null);

    return risp
}

function WS_GeneraProgressivo() {
    let parametri = kendo.stringify({ "piva": JSON.parse(objP_agenda).Piva, "data": obj_Distinta_Selezionata.Validita_Inizio });

    return new Promise((resolve, reject) => {
        ajaxAgronica("Impianto_Edit2.aspx/GeneraProgressivo",
            parametri,
            function (risposta) {
                resolve(risposta);
            }, null, null, false);
    });
}

function WS_LeggiDescrizioni(options) {
    var parametri = kendo.stringify({ "piva": JSON.parse(objP_agenda).Piva });
    ajaxAgronica("Impianto_Edit2.aspx/LeggiDescrizioni", parametri, function (risposta) {
        var data = JSON.parse(risposta.RispostaStringa);
        risp = JSON.parse(risposta.RispostaStringa);
        var objVuoto = { "des": "", "val": "" };
        data.unshift(objVuoto);
        options.success(data);
    }, null); // , null, false
}

function LeggiDisciplinarePrivato() {
    return new Promise((resolve, reject) => {

        var param = {};

        ajaxAgronica("Impianto_Edit2.aspx/LeggiDisciplinarePrivato", JSON.stringify(param),
            function (risposta) {
                if (risposta.RispostaStringa == "true") {
                    resolve(true);
                } else {
                    resolve(false);
                }
            }, null, null, false);
    });
}

function impostaNPK(Regolamento, Veg_Cod, Grfi_Cod, Stato_Cod) {
    var npkObject = null;
    var veg_cod_S = Veg_Cod != 0 ? Veg_Cod.split("|")[0] : 0;
    if (Regolamento > 0 && veg_cod_S > 0 && Grfi_Cod > 0 && Stato_Cod > 0) {
        var parametri = kendo.stringify({
            "objP_super_server": objP_super_server,
            "objP_server": objP_server,
            "Regolamento": Regolamento,
            "Veg_Cod": veg_cod_S,
            "Grfi_Cod": Grfi_Cod,
            "Stato_Cod": Stato_Cod
        });
        ajaxAgronicaSync(pathCoreWS + "AgronicaCoreDPI/PianoConcimazione.asmx/CalcoloNPK_GrfiCod_StatoCod",
            parametri, false,
            function (risposta) {
                npkObject = JSON.parse(risposta.RispostaStringa);
            }, null);
    }
    return npkObject;
}

function CaricaComboCmb_Operazioni(options) {

    var parametri = kendo.stringify({
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        FiltraImpostazioniUtente: true,
        Tipo_GruppoOperazioni: "'C'"
    });

    ajaxAgronica(pathCoreWS + "Metaschema/Operazioni.asmx/CaricaComboLavorazioni",
        parametri,
        function (risposta) {
            let operazioni = JSON.parse(risposta.RispostaStringa);
            if (obj_Impianto.cul_cod == 0) {
                toRemove = [107, 122, 150, 116, 118, 121, 110, 79, 109, 13, 113, 1, 119, 126];
                operazioni = operazioni.filter((el) => !toRemove.includes(el.lav_cod));
            }
            options.success(operazioni);
        }, null, null, false);

}

function LeggiImpostazioniUtente(impostazione_cod) {
    return new Promise((resolve, reject) => {
        var param = {
            objP_Utenti: objP_utenti,
            impostazione_cod: impostazione_cod
        }

        ajaxAgronica(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_Impostazioni", JSON.stringify(param),
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, null, false);
    });
}