
//function RiempiDdlMacrousi(options) {

//    var parametri = kendo.stringify({ "piva": $('#piva')[0].value });

//    ajaxAgronicaSync(indirizzohttp + "/Trova_Macrousi_Superfici",
//        parametri,
//        false,
//        function (risposta) {
//            risp = JSON.parse(risposta.RispostaStringa);
//            options.success(risp);
//            macroSuperfici = risp;
//        }, null);
//}

function AggiornaRichiesteCarburanti(piva, benzina, gasolio, gasolioSerra, Gasolio_Approvato, Benzina_Approvato, Gasolio_Serra_Approvato) {
    return new Promise(function (resolve, reject) {
        var anno = isNaN(parseInt($('#anno')[0].value)) ? reject() : parseInt($('#anno')[0].value);

        if (parseInt(anno) > 1900 && parseInt(anno) < 2100) {
            setYear(parseInt(anno));
        }

        var parametri = {
            piva: piva,
            r_cod: richiesta_cod,
            anno: anno,
            benzina: benzina,
            gasolio: gasolio,
            gasolioSerra: gasolioSerra,
            Approvato_Gasolio: Gasolio_Approvato,
            Approvato_Benzina: Benzina_Approvato,
            Approvato_Gasolio_Serra: Gasolio_Serra_Approvato,
            Percentuale_Anticipo_Carb: Percentuale_Anticipo_Carb,  //aggiunto Gloria percentuale di anticipo
            Tipo_Azienda: get_TipoAzienda()
        }

        ajaxAgronicaSync(indirizzohttp + "/Aggiorna_Richiesta_Carburanti",
            JSON.stringify(parametri),
            false,
            function (risposta) {
                resolve();
            }, null);
    });
}

//function LeggiNumeroIscrizioneCdC(piva) {
//    var risp = 0
//    var param = kendo.stringify({ "piva": piva });

//    ajaxAgronicaSync(indirizzohttp + "/LeggiNumeroIscrizioneCdC",
//        param,
//        false,
//        function (risposta) {
//            risp = JSON.parse(risposta.RispostaStringa);
//        }, null);

//    return risp;
//}

//function ws_Inserisci_Richieste_Terzisti(piva, r_cod, righeInserite, righeModificate, righeCancellate) {
//    return new Promise(function (resolve, reject) {
//        let parametri = {
//            piva: piva,
//            r_cod: r_cod,
//            righeInserite: JSON.stringify(righeInserite),
//            righeModificate: JSON.stringify(righeModificate),
//            righeCancellate: JSON.stringify(righeCancellate)
//        };
//        ajaxAgronicaSync(indirizzohttp + "/Aggiorna_Richieste_Terzisti",
//            JSON.stringify(parametri),
//            false,
//            function (risposta) {
//                richiesta_cod = JSON.parse(risposta.RispostaStringa);
//                resolve();
//            }, null);
//    });
//}

//function LeggiRichiesteTerzistaParziale(piva) {
//    return new Promise(function (resolve, reject) {

//        var anno;
//        if ($('#anno')[0].value == undefined || $('#anno')[0].value == "") {
//            anno = 0;
//        } else {
//            anno = parseInt($('#anno')[0].value);
//        }

//        var parametri = {
//            piva: piva,
//            richiesta_cod: richiesta_cod,
//            anno: anno
//        }

//        ajaxAgronica(indirizzohttp + "/Trova_Richieste_Terzisti_Lavorazioni_Parziali", JSON.stringify(parametri), function (risposta) {
//            WaitFrame.hide();
//            dtRichiestaTerzistaParziale = JSON.parse(risposta.RispostaStringa);
//            resolve();
//        }, null);
//    });
//}

//function RichiesteTerzistaParziale(options) {
    
//        options.success(dtRichiestaTerzistaParziale);
    
//}

//function TrovaAziendaDaCUAA(CUAA) {
//    var rag_soc;

//    var parametri = {
//        CUAA: CUAA,
//    }

//    ajaxAgronicaSync(indirizzohttp + "/Trova_Azienda_Da_CUAA", JSON.stringify(parametri), false, function (risposta) {
//        WaitFrame.hide();
//        rag_soc = JSON.parse(risposta.RispostaStringa);
//    }, null);

//    return rag_soc;
//}

function RicercaLavorazioniParzialiTerzisti(options) {

    //if (parametriPerLettura[0] !== undefined && parametriPerLettura.length > 0 && parametriPerLettura[0] !== null) {

    
        var anno;
        if ($('#anno')[0].value == undefined || $('#anno')[0].value == "") {
            anno = 0;
        } else {
            anno = parseInt($('#anno')[0].value);
        }
        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            richiesta_cod: richiesta_cod,
            anno:anno
        }

        //ajaxAgronica(indirizzohttp + "/Trova_Richieste_Terzisti_Lavorazioni_Parziali", JSON.stringify(parametri), function (risposta) {
        //    resolve(risp);
        //}, null);
        ajaxAgronicaSync(indirizzohttp + "/Trova_Richieste_Terzisti_Lavorazioni_Parziali",
            JSON.stringify(parametri),
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                
                    options.success(risp);
                
            }, null);
    //}
}

//    if (
//        parametriPerLettura[0] !== undefined && parametriPerLettura.length > 0 && parametriPerLettura[0] !== null) {

//        var parametri = {
//            piva: pivaSelezionata,
//            Richiesta_Cod: richiesta_cod
//        }


//        ajaxAgronicaSync(indirizzohttp + "/Trova_Richieste_Terzisti_Lavorazioni_Parziali",
//            JSON.stringify(parametri),
//            false,
//            function (risposta) {
//                risp = JSON.parse(risposta.RispostaStringa);
//                if (check) {
//                    colt_elem = risp.length;
//                } else {
//                    options.success(risp);
//                }
//            }, null);
//    }
//}
