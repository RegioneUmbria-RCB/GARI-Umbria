function ucUmaAllevamenti_ricerca() {
    return new Promise(function (resolve, reject) {

        if (richiesta_cod <= 0) {
            resolve([]);
        }
        else {
            
            var storage_key = "ucUmaAllevamenti_ricerca_" + pivaSelezionata + "_" + richiesta_cod;
            if (!storageExistItem(storage_key)) {
                var parametri = {
                    piva: pivaSelezionata,
                    richiestaCod: richiesta_cod
                };

                ajaxAgronica(indirizzohttp + "/UC_Allevamenti_CercaRichiesteAllevamenti", JSON.stringify(parametri),
                    function (risposta) {
                        try {
                            var arrRisp = JSON.parse(risposta.RispostaStringa);

                            storageSetItem(storage_key, risposta.RispostaStringa);
                            resolve(arrRisp);
                        } catch (e) {
                            reject(e.message);
                        }
                    },
                    function (risposta) {
                        reject(risposta.Errore);
                    }
                );
            }
            else {
                resolve(JSON.parse(storageGetItem(storage_key)));
            }
        }
    });
}

/** Legge la tabella uma_configurazione_allevamenti */
function ucUmaAllevamenti_elencoConfig() {
    var risp = [];
    var annoSelezionato = getAnnoSelezionato();
    var storage_key = "ucUmaAllevamenti_elencoConfig_" + annoSelezionato;
    if (!storageExistItem(storage_key)) {
      
        var param = {
            anno: annoSelezionato
        };
        ajaxAgronicaSync(indirizzohttp + "/UC_Allevamenti_ElencoConfigurazioni", JSON.stringify(param), false, function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            storageSetItem(storage_key, risposta.RispostaStringa);
        }, null);
    }
    else {
        risp = JSON.parse(storageGetItem(storage_key));
    }

    return risp;
}

/** Legge la vista uma_allevamenti */
function ucUmaAllevamenti_elencoTipi() {
    var risp = [];

    var anno = $("#anno").val();

    if (anno === undefined) {
        anno = "";
    }

    var storage_key = "ucUmaAllevamenti_elencoTipi_" + anno;

    if (!storageExistItem(storage_key)) {
        var param = {
            anno: anno
        };
        ajaxAgronicaSync(indirizzohttp + "/UC_Allevamenti_ElencoTipi", JSON.stringify(param), false, function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            storageSetItem(storage_key, risposta.RispostaStringa);
        }, null);
    }
    else {
        risp = JSON.parse(storageGetItem(storage_key));
    }

    return risp;
}

function ucUmaAllevamenti_AggiornaAllevamenti() {
    return new Promise(function (resolve, reject) {

        var kGridAllevamenti = KendoGrid("tab_griglia_allevamenti");
        var grigliaDaRicaricare = false;

        if (kGridAllevamenti !== undefined && kGridAllevamenti !== null) {
            var dtAllevamenti = kGridAllevamenti.dataSource.data();

            var _righeInserite = dtAllevamenti.filter((el) => { return el.dirty == true && el.Richiesta_Cod == 0 && (!el.deleted) });
            var _righeModificate = dtAllevamenti.filter((el) => { return el.dirty == true && el.Richiesta_Cod != 0 && (!el.deleted) });
            var _righeCancellate = dtAllevamenti.filter((el) => { return el.Richiesta_Cod != 0 && el.deleted == true });

            var modificheFatte = false;

            if (_righeInserite.length > 0 || _righeModificate.length > 0 || _righeCancellate.length > 0) {
                modificheFatte = true;
            }

            if (modificheFatte) {
                grigliaDaRicaricare = true;

                var parametri = {
                    piva: pivaSelezionata,
                    richiestaCod: richiesta_cod,
                    righeInserite: JSON.stringify(_righeInserite),
                    righeModificate: JSON.stringify(_righeModificate),
                    righeCancellate: JSON.stringify(_righeCancellate)
                };

                ajaxAgronica(indirizzohttp + "/UC_Allevamenti_Aggiorna", JSON.stringify(parametri),
                    function (risposta) {
                        if (risposta.RispostaOK == true) {
                            resolve(grigliaDaRicaricare);
                        }
                        else {
                            reject(risposta.Errore);
                        }
                    },
                    function (risposta) {
                        reject(risposta.Errore);
                    }
                );
            }
            else {
                resolve(grigliaDaRicaricare);
            }
        }
        else {
            resolve(grigliaDaRicaricare);
        }

    });
}