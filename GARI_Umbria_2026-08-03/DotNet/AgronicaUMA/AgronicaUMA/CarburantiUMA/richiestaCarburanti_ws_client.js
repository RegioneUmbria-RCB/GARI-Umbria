

var indirizzohttp = "./richiestaCarburanti.aspx";
var indirizzohttpWSGenerali = "./CdG_WS.aspx";

function EmptyRead(options) { }
function EmptySubmit(options) { }

function LeggiRichiesta(check, copia) {
    return new Promise(function (resolve, reject) {
        var programmazione_cod = 0;

        //if (QS_Richiesta == 0 || QS_Richiesta == -1) {
        try {
            var index = $("#fascicolo").data("kendoDropDownList").selectedIndex;
            var dataItem = $("#fascicolo").data("kendoDropDownList").dataItem(index);

            if (index != -1) {
                if (isNumeric(dataItem.strProgrammazioniCod.replaceAll("|", "").trim())) {
                    programmazione_cod = parseInt(dataItem.strProgrammazioniCod.replaceAll("|", "").trim())
                }
            }
        } catch (e) {

        }
        
        //}

        if (copia) {
            programmazione_cod = 0;
            check = false;
        }
        if ($("#anno").val() >= 2025) {
            programmazione_cod = codiceFascicoloPianoColturale;
        }
        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            richiesta_cod: richiesta_cod,
            check: check,
            Programmazione_Cod: programmazione_cod,
            avanzamento: QS_Avanzamento
        }

        ajaxAgronica(indirizzohttp + "/Trova_Richieste", JSON.stringify(parametri), function (risposta) {
            WaitFrame.hide();
            pivaSelezionata = KendoDDL("ddlAzienda").value();
            dtRichiesta = JSON.parse(risposta.RispostaStringa);
            resolve();
        }, null)
    });
}


function terzistaIntegrazione(anno) {
    return new Promise(function (resolve, reject) {
        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            anno: anno
        }

        ajaxAgronica(indirizzohttp + "/Terzista_Abilitato_Per_Integrazione", JSON.stringify(parametri), function (risposta) {
            WaitFrame.hide();

            //resolve(JSON.parse(risposta.RispostaStringa));

            let res = JSON.parse(risposta.RispostaStringa)
            if (res.split("|")[0].toLowerCase() == 'true') {
                resolve(true)
            } else {
                resolve(false)
            }
            percentualePrelievo = res.split("|")[1]

        }, null);
    });
}

function LeggiRichiestaDettaglio(options) {
    return new Promise(function (resolve, reject) {
        if ($("#griglia_dettagliColture").data("kendoGrid").dataSource._data.length === 0) {
            WaitFrame.show();
            var parametri = {
                piva: KendoDDL("ddlAzienda").value(),
                richiesta_cod: richiesta_cod,
                check: true,
                macrouso_UMA_Cod: macro_cod,
                Programmazione_Cod: program_cod,
                avanzamento: QS_Avanzamento
            }

            ajaxAgronica(indirizzohttp + "/Trova_Richieste_Dettaglio", JSON.stringify(parametri), function (risposta) {
                WaitFrame.hide();
                dtRichiestaDettaglio = JSON.parse(risposta.RispostaStringa);
                options.success(dtRichiestaDettaglio);
                resolve();
            }, null);
        } else {
            options.success(dtRichiestaDettaglio);
            resolve();
        }

    });
}

function LeggiAppezzamentiProgrammazione(callback) {
    
    WaitFrame.show();
    var parametri
    if ($("#anno").val() >= 2025) { 
        parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            programmazione_cod: -4,
            richiesta_Cod: richiesta_cod

        }
    } else {
        var index = $("#fascicoloric").data("kendoDropDownList").selectedIndex;
        var dataItem = $("#fascicoloric").data("kendoDropDownList").dataItem(index);
        //var pippo= parseInt(dataItem.strProgrammazioniCod.replaceAll("|", "").trim())

        parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            programmazione_cod: parseInt(dataItem.strProgrammazioniCod.replaceAll("|", "").trim()),
            richiesta_Cod: richiesta_cod

            //programmazione_cod: parseInt(QS_Programmazione_Cod_Fascicolo)//parseInt($("#fascicolo")[0].value.replaceAll("|", "").trim())

        }
    }


    console.log(indirizzohttp)
    ajaxAgronica(indirizzohttp + "/Leggi_Appezzamenti_Programmazione",
        JSON.stringify(parametri),

        function (risposta) {
            callback(risposta);
        }, null);

}

function VerificaAppezzamenti() {
    return new Promise(function (resolve, reject) {
        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            richiesta_cod: richiesta_cod,
            tipo_richiesta: QS_Type,
            avanzamento: QS_Avanzamento
        }

        ajaxAgronica(indirizzohttp + "/Verifica_Appezzamenti",
            JSON.stringify(parametri),
            function (risposta) {
                anomalieSuperficiAppezzamenti = JSON.parse(risposta.RispostaStringa);
                if (anomalieSuperficiAppezzamenti.length == 0) {
                    anomalieSuperficiAppezzamenti = null;
                }
                resolve(true);
            }, null);
    });
}

function CorreggiSuperficiAppezzamenti() {
    return new Promise(function (resolve, reject) {
        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            richiesta_cod: richiesta_cod,
            tipo_richiesta: QS_Type,
            avanzamento: QS_Avanzamento,
            Regione_Cod: '010',
            appezzamentiDataStr: JSON.stringify(anomalieSuperficiAppezzamenti)
        }

        ajaxAgronica(indirizzohttp + "/Correggi_Superfici_Appezzamenti",
            JSON.stringify(parametri),
            function (risposta) {                
                anomalieSuperficiAppezzamenti = null;
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);            
            }, null, null, false);
    });
}


function RicercaAgenda(options) {
    options.success(dtRichiesta);
}

function CercaDettagliAzienda(piva, r_cod) {
    return new Promise(function (resolve, reject) {
        if (r_cod == 0) {
            r_cod = -1;
        }

        let parametri = {
            piva: piva,
            Richiesta_Cod: r_cod,
            Tipo_Richiesta: QS_Type,
            Avanzamento_Richiesta: QS_Avanzamento,
            annoR: $("#anno").val()
        }

        ajaxAgronica(indirizzohttp + "/Cerca_Dettagli_Azienda",
            JSON.stringify(parametri), function (risposta) {
                //alert(risposta.RispostaStringa);
                risp = JSON.parse(risposta.RispostaStringa);

                resolve(risp);
            }, null);
    });
}

function CreaNuovaRichiestaWS(anno, tipo, isTerz, recuperaRimanenze, integrativa) {

    let anticipoGasolio_ = parseFloat($("#gasolioAnticipo")[0].value);
    let anticipoBenzina_ = parseFloat($("#benzinaAnticipo")[0].value);
    let anticipoGasolioSerra_ = parseFloat($("#gasolioSerraAnticipo")[0].value);
    let percentuale_anticipo_carb_ = (QS_Anticipo == 1) ? parseFloat((($("#txtVarPercentualeAnticipo")[0].value).replace(',', '.') / 100)) : Percentuale_Anticipo_Carb;

    return new Promise(function (resolve, reject) {
        let parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            cuaa: $('#CUAA')[0].value,
            isTerzista: isTerz,
            anno: anno,
            avanzamento: tipo,
            recuperaRimanenze: recuperaRimanenze,
            integrativa: integrativa,
            anticipo: QS_Anticipo,
            percentuale_anticipo_carb: percentuale_anticipo_carb_,//Percentuale_Anticipo_Carb,
            tipo_azienda: get_TipoAzienda(),
            anticipoGasolio: isNaN(anticipoGasolio_) ? 0 : anticipoGasolio_,
            anticipoBenzina: isNaN(anticipoBenzina_) ? 0 : anticipoBenzina_,
            anticipoGasolioSerra: isNaN(anticipoGasolioSerra_) ? 0 : anticipoGasolioSerra_
        };
        ajaxAgronica(indirizzohttp + "/Nuova_Richiesta",
            JSON.stringify(parametri), function (risposta) {
                //richiesta_cod = JSON.parse(risposta.RispostaStringa);
                let arr = JSON.parse(risposta.RispostaStringa);

                richiesta_cod = arr[0].richiesta_cod;
                carburante_Richiesto_Benzina = arr[0].carburante_Richiesto_Benzina;
                carburante_Richiesto_Gasolio = arr[0].carburante_Richiesto_Gasolio;
                carburante_Richiesto_Gasolio_Serra = arr[0].carburante_Richiesto_Gasolio_Serra;

                //richiesta_cod = arr[0];
                resolve(richiesta_cod);
            }, null);

    });

}

function LeggiNumeroRichiesta(richiesta_cod) {
    return new Promise(function (resolve, reject) {
        let parametri = {
            richiesta_cod: richiesta_cod
        };

        ajaxAgronica(indirizzohttp + "/LeggiNumeroRichiesta",
            JSON.stringify(parametri), function (risposta) {
                numero = risposta.RispostaStringa;
                resolve(numero);
            }, null);

    });
}

function LeggiLavorazioniAlternative(gruppo_colturale, regolamento_cod) {
    return new Promise(function (resolve, reject) {

        let annoSelezionato = getAnnoSelezionato();

        let parametri = {
            gruppo_colturale: gruppo_colturale,
            anno: annoSelezionato
        };

        ajaxAgronica(indirizzohttp + "/Leggi_Lavorazioni_Alternative",
            JSON.stringify(parametri), function (risposta) {
                dt = JSON.parse(risposta.RispostaStringa);
                if (dt !== null) {
                    dtlav = dt.filter(function (e) { return e.Regolamento_Cod === (regolamento_cod == RegolamentoConvenzionale ? RegolamentoConvenzionale : RegolamentoBiologico) });
                } else {
                    dtlav = dt;
                }
                lav_alt[gruppo_colturale] = dtlav;
                resolve();
            }, null);

    });

}

function LeggiLavorazioniAlternativeLimitate(gruppo_colturale) {
    return new Promise(function (resolve, reject) {
        let parametri = {
            gruppo_colturale: gruppo_colturale
        };

        ajaxAgronica(indirizzohttp + "/Leggi_Lavorazioni_Alternative_Limitate",
            JSON.stringify(parametri), function (risposta) {
                dt = JSON.parse(risposta.RispostaStringa);

                lav_alt_lim_sup = dt;
                resolve();
            }, null);

    });

}

function PopolaElencoLavUMA(flagVuoto, Macrouso_UMA_Cod, Terzista, regolamento_Cod) {
    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = false;

    if (Terzista == undefined) {
        Terzista = 0;
    }

    //SE Forma giuridica = CONSORZIO DI BONIFICA (19) --> filtro solo le lavorazioni con Utilizzata_Da_Consorzio_Bonifica = 1
    let Utilizzata_Da_Consorzio_Bonifica = (KendoDDL("ddlAzienda").dataItem().forma_giuridica == Consorzio_Di_Bonifica) ? 1 : 0

    let annoSelezionato = getAnnoSelezionato();

    let storage_key = "PopolaElencoLavUMA_" + flagVuoto + "_" + Macrouso_UMA_Cod + "_" + Terzista + "_" + Utilizzata_Da_Consorzio_Bonifica + "_" + annoSelezionato + "_" + regolamento_Cod;

    if (!storageExistItem(storage_key)) {
        var param = {
            Macrouso_UMA_Cod: Macrouso_UMA_Cod,
            Terzista: Terzista,
            Utilizzata_Da_Consorzio_Bonifica: Utilizzata_Da_Consorzio_Bonifica,
            Anno: annoSelezionato,
            Regolamento_Cod: regolamento_Cod
        };

        ajaxAgronicaSync(indirizzohttp + "/ElencoLavUMA",
            JSON.stringify(param),
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "Lav_UMA_Cod": 0,
                        "LavUMA": "",
                        "Regolamento_Cod": 0
                    };
                    risp.unshift(objVuoto);
                }
                elencoLavUMA = risp;
                storageSetItem(storage_key, JSON.stringify(risp));
            }, null);
    } else {
        elencoLavUMA = JSON.parse(storageGetItem(storage_key));
    }
    if (elencoLavUMA !== null) {
        elencoLavUMA = (regolamento_Cod === RegolamentoBiologico ?
            elencoLavUMA.filter(function (e) { return e.Regolamento_Cod === RegolamentoBiologico || e.Regolamento_Cod === RegolamentoEntrambi }) :
            elencoLavUMA.filter(function (e) { return e.Regolamento_Cod === RegolamentoConvenzionale || e.Regolamento_Cod === RegolamentoEntrambi }))
    }
    elencoLavUMA = filtraLavorazioniEccedenzaAnticipi(elencoLavUMA, Macrouso_UMA_Cod);
    return elencoLavUMA;
}

function filtraLavorazioniEccedenzaAnticipi(elencoLavUMA, Macrouso_UMA_Cod) {
    if (Macrouso_UMA_Cod == macrousoCod_EccedenzaAnticipi) {
        //Richiesta
        if (QS_Avanzamento == 0) {
            elencoLavUMA = elencoLavUMA.filter(function (e) { return e.Lav_UMA_Cod !== lavorazioneCod_QuotaAnticipoEccedenteAccisePagate })
        }
        //Rendicontazione
        if (QS_Avanzamento == 1) {
            elencoLavUMA = elencoLavUMA.filter(function (e) { return e.Lav_UMA_Cod !== lavorazioneCod_QuotaAnticipoEccedente })
        }
    }
    return elencoLavUMA;
}

function PopolaElencoLavGIAS(flagVuoto, Macrouso_UMA_Cod, Lav_UMA_Cod, Regolamento_Cod) {
    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = false;

    if (Macrouso_UMA_Cod == "" || Lav_UMA_Cod == "") {
        elencoLavGIAS = new Array();
        return elencoLavGIAS;
    }

    let annoSelezionato = getAnnoSelezionato();

    if (Regolamento_Cod === undefined || Regolamento_Cod === null)
        Regolamento_Cod = RegolamentoConvenzionale;

    let storage_key = "PopolaElencoLavGIAS_" + flagVuoto + "_" + Macrouso_UMA_Cod.trim() + "_" + Regolamento_Cod + "_" + Lav_UMA_Cod + "_" + annoSelezionato;

    if (!storageExistItem(storage_key)) {
        var param = {
            Lav_UMA_Cod: Lav_UMA_Cod,
            Macrouso_UMA_Cod: Macrouso_UMA_Cod.trim(),
            Regolamento_Cod: parseInt(Regolamento_Cod),
            Anno: annoSelezionato
        };
        ajaxAgronicaSync(indirizzohttp + "/ElencoLavGIAS",
            JSON.stringify(param),
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "LAV_COD": 0,
                        "LavGIAS": ""

                    };
                    risp.unshift(objVuoto);
                }
                elencoLavGIAS = risp;
                storageSetItem(storage_key, JSON.stringify(risp));
            }, null);
    } else {
        elencoLavGIAS = JSON.parse(storageGetItem(storage_key));
    }
    return elencoLavGIAS;
}

function PopolaElencoLavUMAMultiple(flagVuoto, Macrouso_UMA_Cod, Terzista, Regolamento_Cod) {
    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = false;

    if (Terzista == undefined) {
        Terzista = 0;
    }

    if (Regolamento_Cod == undefined) {
        Regolamento_Cod = RegolamentoConvenzionale;
    }

    //SE Forma giuridica = CONSORZIO DI BONIFICA (19) --> filtro solo le lavorazioni con Utilizzata_Da_Consorzio_Bonifica = 1
    let Utilizzata_Da_Consorzio_Bonifica = (KendoDDL("ddlAzienda").dataItem().forma_giuridica == Consorzio_Di_Bonifica) ? 1 : 0

    let annoSelezionato = getAnnoSelezionato();

    let storage_key = "PopolaElencoLavUMA_" + flagVuoto + "_" + Macrouso_UMA_Cod + "_" + Terzista + "_" + Utilizzata_Da_Consorzio_Bonifica + "_" + annoSelezionato + "_" + Regolamento_Cod;

    if (!storageExistItem(storage_key)) {
        var param = {
            Macrouso_UMA_Cod: Macrouso_UMA_Cod,
            Terzista: Terzista,
            Utilizzata_Da_Consorzio_Bonifica: Utilizzata_Da_Consorzio_Bonifica,
            Anno: annoSelezionato,
            Regolamento_Cod: Regolamento_Cod
        };

        ajaxAgronicaSync(indirizzohttp + "/ElencoLavUMA",
            JSON.stringify(param),
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "Lav_UMA_Cod": 0,
                        "LavUMA": "",
                        "LavGIAS": "",
                        "LAV_COD": 0,
                        "Regolamento_Cod": 0
                    };
                    risp.unshift(objVuoto);
                }
                elencoLavUMA = risp;
                storageSetItem(storage_key, JSON.stringify(risp));
            }, null);
    } else {
        elencoLavUMA = JSON.parse(storageGetItem(storage_key));
    }
    if (elencoLavUMA !== null) {
        elencoLavUMA = elencoLavUMA.filter(function (e) { return e.Regolamento_Cod === Regolamento_Cod || e.Regolamento_Cod === RegolamentoEntrambi })
    }
    elencoLavUMA = filtraLavorazioniEccedenzaAnticipi(elencoLavUMA, Macrouso_UMA_Cod);
    return elencoLavUMA;
}

function RiempiElencoLavorazioniValidita() {
    let annoSelezionato = getAnnoSelezionato();
    let param = {
        Anno: annoSelezionato
    };
    ajaxAgronicaSync(indirizzohttp + "/ElencoLavValidita",
        JSON.stringify(param),
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            elencoLavValidita = risp;
        }, null);
}

function getAnnoSelezionato() {
    let annoSelezionato = $("#anno").val();
    if (annoSelezionato === undefined) {
        annoSelezionato = "";
    }
    return annoSelezionato;
}

function RecuperaDataPassaggioDiStato() {
    ajaxAgronicaSync(indirizzohttp + "/RecuperaDataPassaggioDiStato",
        JSON.stringify({ richiesta_Cod: richiesta_cod }),
        false,
        function (risposta) {
            if (risposta.RispostaStringa != '') {
                risp = JSON.parse(risposta.RispostaStringa);
                dataPassaggioDiStato = risp.ItemArray[0];
            }
        }, null);
}

function PopolaElencoProgrammazione_Des() {
    return new Promise(function (resolve, reject) {
        var param = {
            piva: KendoDDL("ddlAzienda").value(),
            anno: parseInt($('#anno')[0].value)
        }
        ajaxAgronica("./richiestaCarburanti.aspx/Leggi_Fascicoli",
            JSON.stringify(param),
            function (risposta) {
                let arr = JSON.parse(risposta.RispostaStringa);
                let emptyEl = {
                    Programmazione_Des: 'SELEZIONA',
                    Programmazione_Cod: 0
                };
                let nonImpu = {
                    Programmazione_Des: descrizioneFascicoloColtureNonImputabili,
                    Programmazione_Cod: codiceFascicoloColtureNonImputabili
                };
                let anticipi = {
                    Programmazione_Des: descrizioneFascicoloAnticipi,
                    Programmazione_Cod: codiceFascicoloAnticipi
                };
                let trasferimenti = {
                    Programmazione_Des: descrizioneFascicoloTrasferimenti,
                    Programmazione_Cod: codiceFascicoloTrasferimenti
                };
                let pianoColt = {
                    Programmazione_Des: descrizioneFascicoloPianoColturale,
                    Programmazione_Cod: codiceFascicoloPianoColturale
                }; 
                let newArr = new Array();
                newArr.push(emptyEl);
                newArr.push(nonImpu);
                newArr.push(anticipi);
                newArr.push(pianoColt);
                if (QS_Avanzamento == 1) {
                    newArr.push(trasferimenti);
                }
                for (let i = 0; i < arr.length; i++) {
                    let prg_cod = arr[i].strProgrammazioniCod.trim().replaceAll("|", "");
                    let prg_des = arr[i].strProgrammazioniDes.trim();
                    newArr.push({
                        Programmazione_Des: prg_des,
                        Programmazione_Cod: parseInt(prg_cod)
                    });
                }
                resolve(newArr);
            }, null);
    });
}

function PopolaElencoCarb(flagVuoto) {

    if (elencoCarburanti === null) {
        var param = "";
        ajaxAgronicaSync(indirizzohttp + "/ElencoCarburanti",
            param,
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                elencoCarburanti = risp;
            }, null);
    }
    return elencoCarburanti;
}

function RicercaLavorazioni(options, parametriPerLettura) {

    if (
        parametriPerLettura[0] !== undefined && parametriPerLettura.length > 0 && parametriPerLettura[0] !== null) {

        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            gruppo: gruppo_col,
            Programmazione_Cod: prog_cod,
            Richiesta_Cod: richiesta_cod
        }


        ajaxAgronicaSync(indirizzohttp + "/Trova_Lavorazioni",
            JSON.stringify(parametri),
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                options.success(risp);
            }, null);
    }
}

function NuovaPraticaUMA(piva, cuaa, options) {

    var params = "{ piva: '" + piva + "'," +
        " cuaa: '" + cuaa + "'} ";

    ajaxAgronicaSync(indirizzohttp + "/Nuova_Pratica_UMA",
        params,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);

}

function PopolaTabellaCalcoloCosti() {
    let anno = $("#anno").val();
    if (anno === undefined) {
        anno = "";
    }
    let storage_key = "PopolaTabellaCalcoloCosti_" + anno;
    if (!storageExistItem(storage_key)) {
        var param = {
            Regione_Cod: '010',
            Anno: anno
        };
        ajaxAgronicaSync(indirizzohttp + "/CalcoloCosti",
            JSON.stringify(param),
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                tabellaCalcoloCosti = risp;
                storageSetItem(storage_key, JSON.stringify(risp));
            }, null);
    } else {
        tabellaCalcoloCosti = JSON.parse(storageGetItem(storage_key));
    }
    return tabellaCalcoloCosti;
}

function CercaPivaReale(piva) {
    return new Promise(function (resolve, reject) {
        let parametri = {
            piva: piva
        };
        ajaxAgronica(indirizzohttp + "/CercaPivaReale",
            JSON.stringify(parametri),
            function (risposta) {
                resolve(JSON.parse(risposta.RispostaStringa));
            }, null);
    });
}

function ws_Inserisci_Lavorazioni(piva, Richiesta_Cod, Programmazione_Cod, Macrouso_UMA_Cod, righeInserite, righeModificate, righeCancellate, regolamento_Cod) {
    return new Promise(function (resolve, reject) {
        let parametri = {
            piva: piva,
            Richiesta_Cod: Richiesta_Cod,
            isTerzista: QS_Type == -1,
            Programmazione_Cod: Programmazione_Cod,
            righeInserite: JSON.stringify(righeInserite),
            righeModificate: JSON.stringify(righeModificate),
            righeCancellate: JSON.stringify(righeCancellate),
            avanzamento: QS_Avanzamento,
            regolamento_Cod: regolamento_Cod
        };
        ajaxAgronicaSync(indirizzohttp + "/AggiornaRichieste",
            JSON.stringify(parametri),
            false,
            function (risposta) {
                resolve();
            }, null);
    });
}

function AggiornaRichieste_UMA(piva, Richiesta_Cod, righe) {
    return new Promise(function (resolve, reject) {
        let parametri = {
            piva: piva,
            Richiesta_Cod: Richiesta_Cod,
            righe: righe
        };
        ajaxAgronica(indirizzohttp + "/AggiornaRichieste_UMA",
            JSON.stringify(parametri),
            function (risposta) {
                resolve();
            }, null);
    });
}

function ContaRichiesteWS(anno, Avanzamento_Richiesta, isTerz, bocciate = false) {
    return new Promise(function (resolve, reject) {
        let parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            cuaa: $('#CUAA')[0].value,
            isTerzista: isTerz,
            anno: anno,
            Avanzamento_Richiesta: Avanzamento_Richiesta,
            Tipo_Azienda: get_TipoAzienda(),
            bocciate: bocciate
        };
        ajaxAgronica(indirizzohttp + "/Conta_Richieste",
            JSON.stringify(parametri),
            function (risposta) {
                resolve(JSON.parse(risposta.RispostaStringa));
            }, null);
    });
}

function AggiornaRichieste(pivaSelezionata, righeInserite, righeModificate, righeCancellate) {
    return new Promise(function (resolve, reject) {
        var param = {
            piva: pivaSelezionata,
            righeInserite: righeInserite,
            righeModificate: righeModificate,
            righeCancellate: righeCancellate
        }


        ajaxAgronicaSync("./richiestaCarburanti.aspx/AggiornaRichieste",
            JSON.stringify(param), false,
            function (risposta) {
                resolve();
            }, null);
    });
}

function SalvaAnticipi() {
    return new Promise(function (resolve, reject) {
        var param = {
            richiestaCod: richiesta_cod,
            percAnticipo: parseFloat((($("#txtVarPercentualeAnticipoTerzisti")[0].value).replace(',', '.') / 100)).toFixed(4),//$("#txtVarPercentualeAnticipoTerzisti")[0].value / 100,
            gasolioAnticipo: parseFloat($("#gasolioTerzisti")[0].value),
            benzinaAnticipo: parseFloat($("#benzinaTerzisti")[0].value),
            gasolioSerraAnticipo: parseFloat($("#gasolioSerraTerzisti")[0].value)
        }


        ajaxAgronica("./richiestaCarburanti.aspx/Salva_Anticipi",
            JSON.stringify(param), //false,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, //null,
            function (errore) {
                MessaggioErrore_Bootstrap(errore.Errore, "DIV_Messaggi");
                resolve(false);
            },
            null, false);
    });
}


function WS_Fascicoli() {
    return new Promise(function (resolve, reject) {
        var param = {
            piva: KendoDDL("ddlAzienda").value(),
            anno: parseInt($('#anno')[0].value)
        }


        ajaxAgronicaSync("./richiestaCarburanti.aspx/Leggi_Fascicoli",
            JSON.stringify(param), false,
            function (risposta) {
                resolve(JSON.parse(risposta.RispostaStringa));
            }, null);
    });
}

function get_ListaCUAA_Richiesti(TipoAzienda, ignoraEliminatiRichiesta) {
    if (TipoAzienda == Cooperativa_Agricola) {
        if (ignoraEliminatiRichiesta) {
            //Siamo in richiesta, controllo se posso Avanzare la pratica
            //Leggo i CUAA, tranne quelli eliminati
            if ($("#tab_griglia_terzisti").data("kendoGrid") !== undefined) {
                var rendicontazioni = $("#tab_griglia_terzisti").data("kendoGrid"),
                    items = rendicontazioni.dataSource.data();

                items.forEach(function (richiesta) {
                    //Aggiungo alla lista solo se:
                    // - !richiesta.deleted: deletd is false (appare solo se si è eliminato e poi diseliminato)
                    // - richiesta.deleted == undefined: il flag deleted compare solo se si è eliminato una volta l'elemento 
                    if (jQuery.inArray(richiesta.CUAA, listaCUAARendicontati) == -1 && (!richiesta.deleted || richiesta.deleted == undefined)) {
                        listaCUAARendicontati.push(richiesta.CUAA)
                    }
                }, this);
            }

        } else {
            //LE COOP SONO OBBLIGATE AD INSERIRLE IN RICHIESTA, QUINDI CONTROLLO DALLE RICHIESTE
            var parametri = kendo.stringify({
                piva_coop: KendoDDL("ddlAzienda").value(),
                anno: parseInt($('#anno')[0].value),
                tipo_azienda: get_TipoAzienda()
            })

            //LEGGO TUTTI I CUAA PER CUI HO FATTO UNA RICHIESTA
            ajaxAgronicaSync(indirizzohttp + "/get_ListaCUAA_Richiesti",
                parametri, false,
                function (risposta) {
                    listaCUAAPresentiInRichiesta = JSON.parse(risposta.RispostaStringa);
                }, null);


            //LEGGO TUTTI I CUAA PER CUI HO GIA' FATTO UNA RENDICONTAZIONE (record già presenti in tabella)
            if ($("#tab_griglia_terzisti").data("kendoGrid") !== undefined) {
                let rendicontazioni = $("#tab_griglia_terzisti").data("kendoGrid"),
                    items = rendicontazioni.dataSource.data();
                items.forEach(function (richiesta) {
                    if (jQuery.inArray(richiesta.CUAA, listaCUAARendicontati) == -1) {
                        listaCUAARendicontati.push(richiesta.CUAA)
                    }
                }, this);
            }
        }

        ////CONFRONTO I DUE ARRAY E SELEZIONO SOLO GLI ELEMENBTI PRESENTI IN ENTRAMBI GLI INSIEMI
        //tempCUAA_PresentiInRichiesta.each(function (CUAA_PresenteInRichiesta) {
        //    tempCUAA_Rendicontati.each(function (CUAA_Rendicontato) {
        //        if (CUAA_PresenteInRichiesta == CUAA_Rendicontato) {
        //            listaCUAARendicontati.push(CUAA_PresenteInRichiesta)
        //        }
        //    }, this);
        //}, this);

    } else if (TipoAzienda == Azienda_Terzista) {
        //QUESTO CASO MI SERVE PER I TERZISTI --> DOPO UNA CERTA DATA NON POSSSONO PIù INSERIRE CUAA NUOVI
        //Quindi mi salvo tutti quelli inseriti in precedenza
        if ($("#tab_griglia_terzisti").data("kendoGrid") !== undefined) {
            var rendicontazioni = $("#tab_griglia_terzisti").data("kendoGrid"),
                items = rendicontazioni.dataSource.data();

            items.forEach(function (richiesta) {
                if (jQuery.inArray(richiesta.CUAA, listaCUAARendicontati) == -1) {
                    listaCUAARendicontati.push(richiesta.CUAA)
                }
            }, this);
        }
    }
}

function WS_AssegnaAutomaticamenteCarburante(richiesta_cod) {
    return new Promise(function (resolve, reject) {
        var param = {
            piva: KendoDDL("ddlAzienda").value(),
            richiesta_cod: richiesta_cod,
            Percentuale_Decurtamento: Percentuale_Decurtamento,
            lav_Parziali: QS_Avanzamento == 0 && QS_TipoAzienda == 2
        }


        ajaxAgronicaSync("./richiestaCarburanti.aspx/Assegna_Automaticamente_Carburante",
            JSON.stringify(param), false,
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null);
    });
}

function Leggi_UMA_Macrousi(Programmazione_Cod) {
    return new Promise(function (resolve, reject) {
        if (Programmazione_Cod == codiceFascicoloColtureNonImputabili ||
            Programmazione_Cod == codiceFascicoloAnticipi ||
            Programmazione_Cod == codiceFascicoloTrasferimenti) {
            let annoSelezionato = getAnnoSelezionato();
            let parametri = {
                Programmazione_Cod: Programmazione_Cod,
                Anno: annoSelezionato,
                CUAA: $('#CUAA')[0].value,
                Piva: KendoDDL("ddlAzienda").value(),
                Avanzamento: QS_Avanzamento,
                Terzista: QS_Type
            };
            ajaxAgronica(indirizzohttp + "/LeggiUMA_Macrousi",
                JSON.stringify(parametri),
                function (risposta) {
                    let arrResp = JSON.parse(risposta.RispostaStringa);
                    arrResp.unshift({
                        Macrouso_UMA_Cod: 0,
                        Macrouso_UMA_Des: 'SELEZIONA'
                    });
                    resolve(arrResp);
                }, null);
        } else {
            let parametri = {
                piva: pivaSelezionata,
                Programmazione_Cod: Programmazione_Cod,
                anno: parseInt($('#anno')[0].value),
                richiesta_Cod: richiesta_cod,
                avanzamento: QS_Avanzamento,
                terzista: QS_Type
            };
            ajaxAgronica(indirizzohttp + "/Trova_Macrousi_Superfici_Fascicolo",
                JSON.stringify(parametri),
                function (risposta) {
                    let arrResp = JSON.parse(risposta.RispostaStringa);
                    arrResp.unshift({
                        Macrouso_UMA_Cod: 0,
                        Macrouso_UMA_Des: 'SELEZIONA'
                    });
                    resolve(arrResp);
                }, null);
        }
    });
}

function Leggi_UMA_Lavorazioni() {
    return new Promise(function (resolve, reject) {
        let storage_key = "LeggiUMA_Macrousi_";
        if (!storageExistItem(storage_key)) {
            let parametri = {
                Lav_UMA_Cod: '',
                Ordinamento: '',
                objP_server: objP_server
            };
            ajaxAgronica(pathCoreWS + "Metaschema/UMA.asmx/LeggiUMA_Lavorazioni",
                JSON.stringify(parametri),
                function (risposta) {
                    resolve(JSON.parse(risposta.RispostaStringa));
                }, null);
        } else {
            resolve(JSON.parse(storageGetItem(storage_key)));
        }

    });
}

function PopolaElencoAttivitaGIAS(flagVuoto, Macrouso_UMA_Cod, Lav_UMA_Cod, LAV_COD, Regolamento_Cod) {
    return new Promise(function (resolve, reject) {

        if (flagVuoto === undefined || flagVuoto === null)
            flagVuoto = false;

        if (Macrouso_UMA_Cod == "" || Lav_UMA_Cod == "") {
            elencoLavGIAS = new Array();
            return elencoLavGIAS;
        }

        let annoSelezionato = getAnnoSelezionato();

        if (Regolamento_Cod === undefined || Regolamento_Cod === null)
            Regolamento_Cod = RegolamentoConvenzionale;

        let storage_key = "PopolaElencoAttivitaGIAS_" + flagVuoto + "_" + Macrouso_UMA_Cod + "_" + Regolamento_Cod + "_" + Lav_UMA_Cod + "_" + LAV_COD + "_" + annoSelezionato;

        if (!storageExistItem(storage_key)) {
            var parametri = {
                Lav_UMA_Cod: Lav_UMA_Cod,
                Macrouso_UMA_Cod: Macrouso_UMA_Cod,
                Regolamento_Cod: parseInt(Regolamento_Cod),
                Lav_Cod: parseInt(LAV_COD),
                Anno: annoSelezionato
            };
            ajaxAgronica(indirizzohttp + "/ElencoAttivitaGIAS",
                JSON.stringify(parametri),
                function (risposta) {
                    let resp = JSON.parse(risposta.RispostaStringa);
                    if (flagVuoto) {
                        resp.unshift({
                            Attivita_Cod: 0,
                            Attivita_Des: ''
                        })
                    }
                    resolve(resp);
                }, null);
        } else {
            resolve(JSON.parse(storageGetItem(storage_key)));
        }
    });
}

function TrovaAttivitaGIAS(flagVuoto, Macrouso_UMA_Cod, Lav_UMA_Cod, LAV_COD, Regolamento_Cod) {

    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = false;

    let att = null;

    let annoSelezionato = getAnnoSelezionato();

    var parametri = {
        Lav_UMA_Cod: Lav_UMA_Cod,
        Macrouso_UMA_Cod: Macrouso_UMA_Cod,
        Lav_Cod: parseInt(LAV_COD),
        Anno: annoSelezionato,
        Regolamento_Cod: Regolamento_Cod
    };

    ajaxAgronicaSync(indirizzohttp + "/ElencoAttivitaGIAS",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            let resp = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto) {
                resp.unshift({
                    Attivita_Cod: 0,
                    Attivita_Des: ''
                })
            }
            att = resp[0];
        }, null);

    return att;
}

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

function aggiornaTestata(piva, richiesta_cod, Rimanenza_Gasolio, Rimanenza_Benzina, Rimanenza_Gasolio_Serra) {
    return new Promise(function (resolve, reject) {

        var parametri = {
            piva: piva,
            richiesta_cod: richiesta_cod,
            Rimanenza_Gasolio: Rimanenza_Gasolio,
            Rimanenza_Benzina: Rimanenza_Benzina,
            Rimanenza_Gasolio_Serra: Rimanenza_Gasolio_Serra
        }

        ajaxAgronica(indirizzohttp + "/Aggiorna_Testata",
            JSON.stringify(parametri),
            function (risposta) {
                let resp = risposta.RispostaStringa;
                resolve(resp);
            }, null);
    });
}

function aggiornaTestataGestRima(piva, richiesta_cod, stato) {
    var causalicar = KendoMultisel("DdlCausalidelnonutilizzo").value().join("|");
    return new Promise(function (resolve, reject) {

        var parametri = {
            piva: piva,
            richiesta_cod: richiesta_cod,
            rim_riass_gasolio: getValoreNumericoControlloConInizializzazione('TxtriassegnabiliGasolio'),
            rim_riass_benzina: getValoreNumericoControlloConInizializzazione('TxtriassegnabiliBenzina'),
            rim_riass_gasolio_serra: getValoreNumericoControlloConInizializzazione('TxtriassegnabiliGasolio_Serra'),
            rim_riass_conf_gasolio: getValoreNumericoControlloConInizializzazione('TxtriassegnabiliGasolioConferma'),
            rim_riass_conf_benzina: getValoreNumericoControlloConInizializzazione('TxtriassegnabiliBenzinaConferma'),
            rim_riass_conf_gasolio_serra: getValoreNumericoControlloConInizializzazione('TxtriassegnabiliGasolio_SerraConferma'),
            rec_acc_dich_gasolio: getValoreNumericoControlloConInizializzazione('TxtrecuperoacciseGasolio'),
            rec_acc_dich_benzina: getValoreNumericoControlloConInizializzazione('TxtrecuperoacciseBenzina'),
            rec_acc_dich_gasolio_serra: getValoreNumericoControlloConInizializzazione('TxtrecuperoacciseGasolio_Serra'),
            rec_acc_conf_gasolio: getValoreNumericoControlloConInizializzazione('TxtrecuperoacciseGasolioConferma'),
            rec_acc_conf_benzina: getValoreNumericoControlloConInizializzazione('TxtrecuperoacciseBenzinaConferma'),
            rec_acc_conf_gasolio_serra: getValoreNumericoControlloConInizializzazione('TxtrecuperoacciseGasolio_SerraConferma'),
            causale_non_utilizzo: causalicar,
            stato: stato,
            avanzamento: QS_Avanzamento,
            Rim_Dich_Gasolio: getValoreNumericoControlloConInizializzazione('TxtNonUtilizzatoGasolio'),
            Rim_Dich_Benzina: getValoreNumericoControlloConInizializzazione('TxtNonUtilizzatoBenzina'),
            Rim_Dich_Gasolio_Serra: getValoreNumericoControlloConInizializzazione('TxtNonUtilizzatoGasolio_Serra')
        }

        ajaxAgronica(indirizzohttp + "/Aggiorna_Carburante",
            JSON.stringify(parametri),
            function (risposta) {
                let resp = JSON.parse(risposta.RispostaStringa).Item1;
                resolve(resp);
            },
            function (risposta) {
                let resp = JSON.parse(risposta.RispostaStringa)
                kendo.alert(resp.Item2);
                resolve(resp.Item1);
            });
    });
}

function getValoreNumericoControlloConInizializzazione(IdControllo) {
    let nomeControllo = '#' + IdControllo;
    let valoreControlloNumerico = parseFloat($(nomeControllo).val())
    if (isNaN(valoreControlloNumerico)) {
        $(nomeControllo).val("0");
        valoreControlloNumerico = 0;
    }
    return valoreControlloNumerico;
}

function aggiornaPermessiAcqua(piva, richiesta_cod, permessoAcqua, notePermessoAcqua) {
    return new Promise(function (resolve, reject) {

        var parametri = {
            piva: piva,
            richiesta_cod: richiesta_cod,
            isTerzista: QS_Type == -1,
            permessoAcqua: isNaN(parseInt(permessoAcqua)) ? 0 : parseInt(permessoAcqua),
            notePermessoAcqua: notePermessoAcqua,
            isIntegrativaAcqua: permessoAcquaGiaRichiesto > 0
        }

        ajaxAgronica(indirizzohttp + "/Aggiorna_PermessiAcqua",
            JSON.stringify(parametri),
            function (risposta) {
                let resp = risposta.RispostaStringa;
                resolve(resp);
            },
            function (risposta) { 
                let resp = risposta.RispostaStringa;
                kendo.alert(resp);
            });
    });
}

function ws_CopiaRichiesta(richiesta_cod) {

    return new Promise(function (resolve, reject) {

        let file_allegato = $('#File_Caricato').val();
        let nome_file = $('#Txt_Documento_Allegato').val();
        if (file_allegato == undefined) {
            file_allegato = "";
        }

        if (nome_file == undefined) {
            nome_file = "";
        }

        var parametri = {
            richiesta_cod: richiesta_cod,
            nome_file: nome_file,
            file_allegato: file_allegato
        }

        ajaxAgronica(indirizzohttp + "/CopiaRichiesta",
            JSON.stringify(parametri),
            function (risposta) {
                let resp = risposta.RispostaStringa;
                resolve(resp);
            }, null);
    });
}

function StampaRichRendicon(_piva, richiesta_avanz, _richiestaCod) {
    return new Promise(function (resolve, reject) {

        var parametri = {
            piva: _piva,
            richiestaAvanz: richiesta_avanz,
            richiestaCod: _richiestaCod
        };

        ajaxAgronica(indirizzohttp + "/StampaRichiestaRendicon",
            JSON.stringify(parametri),
            function (risposta) {
                let resp = risposta.RispostaStringa;
                resolve(resp);
            }, null);
    });
}

function StampaIstruttoria(_piva, richiesta_avanz, _richiestaCod, flagModDati, flagSegnMacchine, codEsito, note) {
    return new Promise(function (resolve, reject) {

        var parametri = {
            piva: _piva,
            richiestaAvanz: richiesta_avanz,
            richiestaCod: _richiestaCod,
            modificaDati: flagModDati,
            segnalazioniMacchine: flagSegnMacchine,
            esito: codEsito,
            noteEsito: note
        };

        ajaxAgronica(indirizzohttp + "/StampaIstruttoria",
            JSON.stringify(parametri),
            function (risposta) {
                let resp = risposta.RispostaStringa;
                resolve(resp);
            }, null);
    });
}

function isPrimaRichiesta(anno, ric_cod) {
    return new Promise(function (resolve, reject) {

        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            richiestaAvanz: QS_Avanzamento,
            richiestaCod: ric_cod,
            anno: ((isNumeric(anno) && parseInt(anno) >= 1900 && parseInt(anno) <= 2100) ? parseInt(anno) : 0),
            terzista: QS_Type == -1
        }

        ajaxAgronica(indirizzohttp + "/CheckPrimaRichiesta",
            JSON.stringify(parametri),
            function (risposta) {
                let resp = risposta.RispostaStringa;
                resolve(resp);
            }, null);
    });
}

function controlloIncrociato(piva, programmazione_cod, gruppo_colturale, anno, soloContoTerzi) {
    return new Promise(function (resolve, reject) {

        if (soloContoTerzi === undefined) {
            soloContoTerzi = false;
        }

        var parametri = {
            piva: piva,
            programmazione_cod: programmazione_cod,
            pivaChiamante: KendoDDL("ddlAzienda").value(),
            gruppo_colturale: gruppo_colturale,
            anno: anno,
            isTerzista: QS_Type == -1,
            avanzamento: QS_Avanzamento,
            integrativa: integrativa,
            soloContoTerzi: soloContoTerzi,
            richiesta_cod: richiesta_cod
        }

        ajaxAgronica(indirizzohttp + "/Leggi_Lavorazioni_Controllo_Incrociato",
            JSON.stringify(parametri),
            function (risposta) {
                let resp = JSON.parse(risposta.RispostaStringa);
                resolve(resp);
            }, null);
    });
}

function RecuperaAnticipazioniColturali(piva, anno) {
    return new Promise(function (resolve, reject) {

        var parametri = {
            piva: piva,
            anno: anno
        }

        ajaxAgronica(indirizzohttp + "/Leggi_Anticipazioni_Colturali",
            JSON.stringify(parametri),
            function (risposta) {
                let resp = JSON.parse(risposta.RispostaStringa);
                resolve(resp);
            }, null);
    });
}

///Gloria
function controlloRichiesteLavorazioni(piva, programmazione_cod, gruppo_colturale, anno) {
    return new Promise(function (resolve, reject) {

        var parametri = {
            piva: piva,
            programmazione_cod: programmazione_cod,
            pivaChiamante: KendoDDL("ddlAzienda").value(),
            gruppo_colturale: gruppo_colturale,
            anno: anno,
            isTerzista: QS_Type == -1
        }

        ajaxAgronica(indirizzohttp + "/Leggi_Richieste_Lavorazioni_Testa",
            JSON.stringify(parametri),
            function (risposta) {
                let resp = JSON.parse(risposta.RispostaStringa);
                resolve(resp);
            }, null);
    });
}
///fine Gloria

function Controlla_Permessi_Inserimento_Nuovo_Documento(piva) {
    var risp = "";

    var parametri = {
        piva: piva
    }

    ajaxAgronicaSync(indirizzohttp + "/Controlla_Permessi_Inserimento_Nuovo_Documento",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            risp = risposta.RispostaStringa;
        }, function (risposta) {
            risp = risposta.Errore;
        });

    return risp;
}

function GetUrlSintesiRendicontazione(piva) {
    var parametri = kendo.stringify({
        "piva": piva
    });

    var risp = "";

    ajaxAgronicaSync(indirizzohttp + "/GetUrlSintesiRendicontazione", parametri, false,
        function (risposta) {
            risp = risposta.RispostaStringa;
        }, null);

    return risp;
}

function GetUrlSintesiUMA(piva, anno, type) {
    var parametri = kendo.stringify({
        "piva": piva,
        "anno": anno,
        "type": type
    });

    var risp = "";

    ajaxAgronicaSync(indirizzohttp + "/GetUrlSintesiUMA", parametri, false,
        function (risposta) {
            risp = risposta.RispostaStringa;
        }, null);

    return risp;
}

function VisualizzaLavTerzisti(options) {
    //console.log(JSON.parse($("#LavorazioniTerzistiDett").val()))
    options.success(JSON.parse($("#LavorazioniTerzistiDett").val()));
}

function VisualizzaAnticipiTerzisti(options) {
    options.success(JSON.parse($("#AnticipazioniDett").val()));
}

function ws_Inserisci_Lavorazioni_Parziali(piva, Richiesta_Cod, righeInserite, righeModificate, righeCancellate) {
    return new Promise(function (resolve, reject) {
        let parametri = {
            piva: piva,
            Richiesta_Cod: Richiesta_Cod,
            isTerzista: QS_Type == -1,
            righeInserite: JSON.stringify(righeInserite),
            righeModificate: JSON.stringify(righeModificate),
            righeCancellate: JSON.stringify(righeCancellate)
        };
        ajaxAgronicaSync(indirizzohttp + "/AggiornaLavorazioniParziali",
            JSON.stringify(parametri),
            false,
            function (risposta) {
                resolve();
            }, null);
    });
}

function LeggiNoProssimaRichiesta() {
    let parametri = {
        richiesta_Cod: richiesta_cod
    };
    ajaxAgronicaSync(indirizzohttp + "/LeggiNoProssimaRichiesta",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            noRichiestaSuccessiva = JSON.parse(risposta.RispostaStringa);
            if (getKendoSwitch("noRichiestaAnnoProxCheck") != noRichiestaSuccessiva)
                setKendoSwitch("noRichiestaAnnoProxCheck", noRichiestaSuccessiva);
        }, null);
}

function salvaNoProssimaRichiesta(noProssimaRichiesta) {
    return new Promise(function (resolve, reject) {
        let parametri = {
            richiesta_Cod: richiesta_cod,
            noProxRichiesta: noProssimaRichiesta,
            isTerzista: QS_Type === -1
        };
        ajaxAgronicaSync(indirizzohttp + "/SalvaNoProssimaRichiesta",
            JSON.stringify(parametri),
            false,
            function (risposta) {
                resolve();
            },
            function (errore) {
                if (errore.Errore !== '') {
                    setKendoSwitch("noRichiestaAnnoProxCheck", noRichiestaSuccessiva);
                    ajaxAgronicaWaitFrame(false);
                    MessaggioErrore_Bootstrap("Si e' verificato un problema lato server (Errore 500): " + errore.Errore, "DIV_Messaggi");
                };
            })
    });
}

function Leggi_Date_Ins_Rendicontazione() {
    var parametri = kendo.stringify({
        "anno": $("#anno").val(),
        "tipo_azienda": QS_TipoAzienda,
        "piva": KendoDDL("ddlAzienda").value(),
        "richiesta_cod": richiesta_cod
    });

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Date_Ins_Rendicontazione",
        parametri, false,
        function (risposta) {
            let res = JSON.parse(risposta.RispostaStringa)
            if (res.split("|")[0].toLowerCase() == 'true') {
                consentitoAggiungereModificareRendicontazione_daSetup = true
            } else {
                consentitoAggiungereModificareRendicontazione_daSetup = false
            }
            Data_Inizio_Rendicontazione = res.split("|")[1]
            Data_Fine_Rendicontazione = res.split("|")[2]
        }, function (risposta) {
            consentitoAggiungereModificareRendicontazione_daSetup = false
            Data_Inizio_Rendicontazione = ""
            Data_Fine_Rendicontazione = ""
            kendo.alert(risposta.RispostaStringa)
        });
}

function leggiAcquistatoAnnoPrecedente() {
    var parametri = kendo.stringify({
        "anno": $("#anno").val(),
        "terzista": QS_Type,
        "piva": KendoDDL("ddlAzienda").value(),
    });

    ajaxAgronicaSync(indirizzohttp + "/leggiAcquistatoAnnoPrecedente",
        parametri, false,
        function (risposta) {
            let res = JSON.parse(risposta.RispostaStringa)
            AcquistatoAnnoPrecedente_Gasolio = res.ltGasolio
            AcquistatoAnnoPrecedente_Benzina = res.ltBenzina
            AcquistatoAnnoPrecedente_Gasolio_Serra = res.ltGasolioSerra
        }, null);

    if (AcquistatoAnnoPrecedente_Gasolio == 0 &&
        AcquistatoAnnoPrecedente_Benzina == 0 &&
        AcquistatoAnnoPrecedente_Gasolio_Serra == 0) {
        if ($("#anno").val() >= 2025) {
            //Non è stato acquistato carburante, consento un inserimento forfettario
            //$("#txtVarPercentualeAnticipo").attr("disabled", "disabled");
            $("#row_percentuale").hide();
            //$("#btn_proseguiAnticipo").hide()
            $("#btn_proseguiAnticipo").show()

            $("#row_MsgErrore").attr("style", "display:block; color:red");
            //$("#lblMsgAliquotaSup").html("Impossibile procedere con la richiesta di anticipo: l'azienda non ha acquistato carburante nell'anno " + ($("#anno").val() - 1).toString() + ".");
            $("#lblMsgAliquotaSup").html("Indicare i litri di carburante in base al fascicolo 2025, calcolando 60 l/ha o 1,2 l/m3 di serra (autodichiarazione resa ai sensi del D.P.R.n. 445 / 2000 - articoli 46 e 47) ");
            $("#gasolioAnticipo").val(AcquistatoAnnoPrecedente_Gasolio)
            $("#benzinaAnticipo").val(AcquistatoAnnoPrecedente_Benzina)
            $("#gasolioSerraAnticipo").val(AcquistatoAnnoPrecedente_Gasolio_Serra)
            $("#gasolioAnticipo").removeAttr("disabled");
            $("#benzinaAnticipo").removeAttr("disabled");
            $("#gasolioSerraAnticipo").removeAttr("disabled");
            let window = $('#Imposta_PercAnticipo').data("kendoWindow");
            if (window != null) {
                window.title("Anticipo forfettario per aziende senza acquisti di carburante nel 2024")
            }

            //proseguiAnticipo = false

            if (QS_Anticipo == 1 && QS_TipoOp == 2) {
                $("#lbl_Richiesta").html("Indicare i litri di carburante in base al fascicolo 2025, calcolando 60 l/ha o 1,2 l/m3 di serra (autodichiarazione resa ai sensi del D.P.R.n. 445 / 2000 - articoli 46 e 47) ");
                gasolioTerz.enable(true);
                benzinaTerz.enable(true);
                gasolioSerraTerz.enable(true);
            }
        } else {
            $("#txtVarPercentualeAnticipo").attr("disabled", "disabled");
            $("#btn_proseguiAnticipo").hide();
            $("#row_MsgErrore").attr("style", "display:block; color:red");
            $("#lblMsgAliquotaSup").html("Impossibile procedere con la richiesta di anticipo: l'azienda non ha acquistato carburante nell'anno " + ($("#anno").val() - 1).toString() + ".");
        }
    } else {
        var soloGasolio = AcquistatoAnnoPrecedente_Gasolio > 0 && AcquistatoAnnoPrecedente_Benzina == 0 && AcquistatoAnnoPrecedente_Gasolio_Serra == 0
        var soloBenzina = AcquistatoAnnoPrecedente_Gasolio == 0 && AcquistatoAnnoPrecedente_Benzina > 0 && AcquistatoAnnoPrecedente_Gasolio_Serra == 0
        var soloGasolioSerra = AcquistatoAnnoPrecedente_Gasolio == 0 && AcquistatoAnnoPrecedente_Benzina == 0 && AcquistatoAnnoPrecedente_Gasolio_Serra > 0
        var piuCarburanti = !soloGasolio && !soloBenzina && !soloGasolioSerra

        $("#gasolioAnticipo").val(Math.floor(AcquistatoAnnoPrecedente_Gasolio * Percentuale_Anticipo_Carb))
        $("#benzinaAnticipo").val(Math.floor(AcquistatoAnnoPrecedente_Benzina * Percentuale_Anticipo_Carb))
        $("#gasolioSerraAnticipo").val(Math.floor(AcquistatoAnnoPrecedente_Gasolio_Serra * Percentuale_Anticipo_Carb))
        
        if (AcquistatoAnnoPrecedente_Gasolio > 0)
            $("#gasolioAnticipo").removeAttr("disabled");
        else
            $("#gasolioAnticipo").attr("disabled", "disabled");

        if (AcquistatoAnnoPrecedente_Benzina > 0)
            $("#benzinaAnticipo").removeAttr("disabled");
        else
            $("#benzinaAnticipo").attr("disabled", "disabled");

        if (AcquistatoAnnoPrecedente_Gasolio_Serra > 0)
            $("#gasolioSerraAnticipo").removeAttr("disabled");
        else
            $("#gasolioSerraAnticipo").attr("disabled", "disabled");

        if (piuCarburanti) {
            $("#txtVarPercentualeAnticipoTerzisti").attr("disabled", "disabled");
            $("#row_percentuale").hide();

            $("#lbl_RichiestaAnticipo").html("Quantità acquistate nell'anno precedente: Gasolio = " + AcquistatoAnnoPrecedente_Gasolio.toString() + ", Benzina = " + AcquistatoAnnoPrecedente_Benzina.toString() + ", Gasolio Serra = " + AcquistatoAnnoPrecedente_Gasolio_Serra.toString() + ").");
        }
        else {
            $("#txtVarPercentualeAnticipoTerzisti").removeAttr("disabled");
            $("#row_percentuale").show();

            $("#lbl_RichiestaAnticipo").html("");
        }
        
        //Ripristino i controlli
        $("#row_MsgErrore").attr("style", "display:none; color:black");
        $("#txtVarPercentualeAnticipo").removeAttr("disabled");
        $("#btn_proseguiAnticipo").show()

        if (QS_Anticipo == 1 && QS_TipoOp == 2) {
            $("#row_percentualeTerzisti").show();

            gasolioTerz.enable(true);
            benzinaTerz.enable(true);
            gasolioSerraTerz.enable(true);

            if (AcquistatoAnnoPrecedente_Gasolio > 0)
                $("#gasolioTerzisti").removeAttr("disabled");
            else
                $("#gasolioTerzisti").attr("disabled", "disabled");
            
            if (AcquistatoAnnoPrecedente_Benzina > 0)
                $("#benzinaTerzisti").removeAttr("disabled");
            else
                $("#benzinaTerzisti").attr("disabled", "disabled");

            if (AcquistatoAnnoPrecedente_Gasolio_Serra > 0)
                $("#gasolioSerraTerzisti").removeAttr("disabled");
            else
                $("#gasolioSerraTerzisti").attr("disabled", "disabled");

            if (piuCarburanti) {
                $("#txtVarPercentualeAnticipoTerzisti").attr("disabled", "disabled");
                $("#row_percentualeTerzisti").hide();

                $("#lbl_Richiesta").html("Quantità acquistate nell'anno precedente: Gasolio = " + AcquistatoAnnoPrecedente_Gasolio.toString() + ", Benzina = " + AcquistatoAnnoPrecedente_Benzina.toString() + ", Gasolio Serra = " + AcquistatoAnnoPrecedente_Gasolio_Serra.toString() + ").");
            }
            else {
                $("#txtVarPercentualeAnticipoTerzisti").removeAttr("disabled");
                $("#row_percentualeTerzisti").show();

                $("#lbl_Richiesta").html("");
            }
        }
    }
}

function Leggi_Data_Fine_Inserimento_Richieste_Anticipo() {
    var parametri = kendo.stringify({
        "anno": $("#anno").val(),
        "tipo_azienda": QS_TipoAzienda
    });

    var risp = "";

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Data_Limite_Inserimento_Richiesta_Anticipo",
        parametri, false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
        }, null);

    return risp;
}

function checkRendicontazioneTerzista(fascicolo, coltura, lavorazione, terzista) {
    var parametri = kendo.stringify({
        "piva": KendoDDL("ddlAzienda").value(),
        "programmazione_cod": parseInt(fascicolo),
        "macrouso_UMA": coltura,
        "lav_UMA": lavorazione,
        "anno": $("#anno").val(),
        "terzista_CUAA": terzista
    });

    var risp = false;

    ajaxAgronicaSync(indirizzohttp + "/Controllo_Congruenza_Rendicontazione_Terzista",
        parametri, false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa) > 0;
        }, null);

    return risp;
}

function controllaPermesso_InserimentoRendicontazione_ContoProprio() {
    var parametri = kendo.stringify({
        "anno": $("#anno").val(),
        "piva": $("#ddlAzienda").val()
    });

    ajaxAgronicaSync(indirizzohttp + "/controllaPermesso_InserimentoRendicontazione_ContoProprio",
        parametri, false,
        function (risposta) {
            let res = JSON.parse(risposta.RispostaStringa)
            if (res.split("|")[0].toLowerCase() == 'true') {
                consentitoInserireRendicontazioneContoProprio = true
            } else {
                consentitoInserireRendicontazioneContoProprio = false
                nonConsentitoInserireRendicontazioneContoProprio_Mess = res.split("|")[1]
            }
        }, function (risposta) {
            kendo.alert(risposta.RispostaStringa)
        });
}

function leggiSommeCarburanti_daDB(objSommeLt, Programmazione_Cod, Macrouso_UMA_Cod, PivaLavorazione) {
    var parametri = {
        Piva: PivaLavorazione,
        GruppoColturaleUMA: Macrouso_UMA_Cod,
        ProgrammazioneCod: Programmazione_Cod,
        RichiestaCod: richiesta_cod
    }

    ajaxAgronicaSync(indirizzohttp + "/leggiSommeCarburanti_daDB",
        JSON.stringify(parametri),
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            objSommeLt.somma_LtGasolioAssegnati += risp.ltGasolio
            objSommeLt.somma_LtGasolioSerraAssegnati += risp.ltGasolioSerra
            objSommeLt.somma_LtGasolioBenzinaAssegnati += risp.ltBenzina
        }, null);
}
function checkPerc() {

    var parametri = {
        anno: $("#anno").val()
    }

    ajaxAgronica(indirizzohttp + "/CheckPerc",
        JSON.stringify(parametri),
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (risp == 0) {
                $("#lbl_Richiesta").html("Richiesta iniziale (i litri carburante sono già stati decurtati a norma di legge)")
                $("#legenda").show()
            }
            else {
                $("#lbl_Richiesta").html("Richiesta iniziale")
                $("#legenda").hide()
            }
        }, null);
}


function CaricatrasferimentiCarburanteDaDB(options) {



    return new Promise((resolve, reject) => {
        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            Richiesta_Cod: richiesta_cod,
        };

        ajaxAgronica(indirizzohttp + "/Leggi_TrasferimentiCarburanti", JSON.stringify(parametri),
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}
async function InviaTrasferimentiModificati(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        ajaxAgronica(indirizzohttp + "/Salva_Trasferimenti",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}
async function InviaRestituzioniModificati(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        ajaxAgronica(indirizzohttp + "/Salva_Restituzioni",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}


function CaricaRestituzioniCarburanteDaDB(options) {



    return new Promise((resolve, reject) => {
        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            Richiesta_Cod: richiesta_cod,
        };

        ajaxAgronica(indirizzohttp + "/Leggi_RestituzioniCarburanti", JSON.stringify(parametri),
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}

function Leggi_Causali_UMA(options) {



    return new Promise((resolve, reject) => {
        var parametri = {};

        ajaxAgronica(indirizzohttp + "/Leggi_Causali_UMA", JSON.stringify(parametri),
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}

function LeggiPraticheSuccessive() {
    return new Promise(function (resolve, reject) {
        
        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            richiesta_cod: richiesta_cod,
            avanzamento: QS_Avanzamento
        }

        ajaxAgronica(indirizzohttp + "/LeggiPraticheSuccessive", JSON.stringify(parametri), function (risposta) {
            dtPraticheSuccessive = JSON.parse(risposta.RispostaStringa);
            resolve();
        }, null)
    });
}

function CaricaLavNoteObbligatorie() {

    var parametri = {
    }

    ajaxAgronica(indirizzohttp + "/LavNoteObbligatorie",
        JSON.stringify(parametri),
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            lav_note_obbligatorie = risp;
        }, null);
}


function CaricatrasferimentiCarburanteDaDB(options) {



    return new Promise((resolve, reject) => {
        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            Richiesta_Cod: richiesta_cod,
        };

        ajaxAgronica(indirizzohttp + "/Leggi_TrasferimentiCarburanti", JSON.stringify(parametri),
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}
async function InviaTrasferimentiModificati(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        ajaxAgronica(indirizzohttp + "/Salva_Trasferimenti",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}
async function InviaRestituzioniModificati(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        ajaxAgronica(indirizzohttp + "/Salva_Restituzioni",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}


function CaricaRestituzioniCarburanteDaDB(options) {



    return new Promise((resolve, reject) => {
        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            Richiesta_Cod: richiesta_cod,
        };

        ajaxAgronica(indirizzohttp + "/Leggi_RestituzioniCarburanti", JSON.stringify(parametri),
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}

function Leggi_Causali_UMA(options) {

    return new Promise((resolve, reject) => {
        var parametri = {};

        ajaxAgronica(indirizzohttp + "/Leggi_Causali_UMA", JSON.stringify(parametri),
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}

function LeggiPraticheSuccessive() {
    return new Promise(function (resolve, reject) {
        
        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            richiesta_cod: richiesta_cod,
            avanzamento: QS_Avanzamento
        }

        ajaxAgronica(indirizzohttp + "/LeggiPraticheSuccessive", JSON.stringify(parametri), function (risposta) {
            dtPraticheSuccessive = JSON.parse(risposta.RispostaStringa);
            resolve();
        }, null)
    });
}

function CaricaLavNoteObbligatorie() {

    var parametri = {
    }

    ajaxAgronica(indirizzohttp + "/LavNoteObbligatorie",
        JSON.stringify(parametri),
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            lav_note_obbligatorie = risp;
        }, null);
}


function CaricatrasferimentiCarburanteDaDB(options) {



    return new Promise((resolve, reject) => {
        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            Richiesta_Cod: richiesta_cod,
        };

        ajaxAgronica(indirizzohttp + "/Leggi_TrasferimentiCarburanti", JSON.stringify(parametri),
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}
async function InviaTrasferimentiModificati(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        ajaxAgronica(indirizzohttp + "/Salva_Trasferimenti",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}
async function InviaRestituzioniModificati(newRecords, updatedRecords, deletedRecords) {
    return new Promise((resolve, reject) => {
        var param = {
            righeInserite: JSON.stringify(newRecords),
            righeModificate: JSON.stringify(updatedRecords),
            righeCancellate: JSON.stringify(deletedRecords)
        };
        var jsonData = JSON.stringify(param);

        ajaxAgronica(indirizzohttp + "/Salva_Restituzioni",
            jsonData,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                resolve(true);
            }, null, null, false);
    });
}


function CaricaRestituzioniCarburanteDaDB(options) {



    return new Promise((resolve, reject) => {
        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            Richiesta_Cod: richiesta_cod,
        };

        ajaxAgronica(indirizzohttp + "/Leggi_RestituzioniCarburanti", JSON.stringify(parametri),
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}

function Leggi_Causali_UMA(options) {



    return new Promise((resolve, reject) => {
        var parametri = {};

        ajaxAgronica(indirizzohttp + "/Leggi_Causali_UMA", JSON.stringify(parametri),
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}

function LeggiPraticheSuccessive() {
    return new Promise(function (resolve, reject) {
        
        var parametri = {
            piva: KendoDDL("ddlAzienda").value(),
            richiesta_cod: richiesta_cod,
            avanzamento: QS_Avanzamento
        }

        ajaxAgronica(indirizzohttp + "/LeggiPraticheSuccessive", JSON.stringify(parametri), function (risposta) {
            dtPraticheSuccessive = JSON.parse(risposta.RispostaStringa);
            resolve();
        }, null)
    });
}