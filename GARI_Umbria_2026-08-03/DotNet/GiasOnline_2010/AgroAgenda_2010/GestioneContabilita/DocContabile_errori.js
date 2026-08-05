
function inizializzaKendoValidator(idDivForm, hasDateInput, hasDateCollegate, hasNumberGreaterThan0Input, hasDdlRequiredNoZero, hasTimePickerNoZero, hasVerificaISCC) {

    //Validatore di default per tutti
    let objValidatorConfig = {
        messages: {
            // overrides the built-in message for the required rule
            required: TraduzioneMultiResx(resxObj, "CampoObbligatorio", "Campo obbligatorio")
        },
        validate: function (e) {
            this.hideMessages();
            //$("span.k-invalid-msg").hide();
        },
        validateInput: function (e) {
            console.log("input " + e.input.attr("name") + " changed to valid: " + e.valid);
            if (e.valid === false) {
                impostaClasseErrore(e.input[0].id, "errorClass", true);
                //this.hideMessages();
                //$(e.input[0]).siblings("span.k-tooltip-validation").hide();
                $(e.input[0]).siblings("span.k-tooltip-validation").remove();
            } else if (e.valid === true && e.input.hasClass("errorClass")) {
                impostaClasseErrore(e.input[0].id, "errorClass", false);
                //lo rimuovo anche dalla lista
            }
        }
    };

    if (hasDateInput) {

        if (objValidatorConfig.rules === undefined || objValidatorConfig.rules === null)
            objValidatorConfig.rules = new Object();

        objValidatorConfig.rules.datepicker = function (input) {
            if (input.is("[data-role=datetimepicker]") && $(input).val() !== "") {
                return input.data("kendoDateTimePicker").value();
            } else if (input.is("[data-role=datepicker]") && $(input).val() !== "") {
                return input.data("kendoDatePicker").value();
            } else {
                return true;
            }
        };

        if (objValidatorConfig.messages === undefined || objValidatorConfig.messages === null)
            objValidatorConfig.messages = new Object();

        objValidatorConfig.messages.datepicker = TraduzioneMultiResx(resxObj, "DataNonValida", "Data non valida");

    }

    if (hasNumberGreaterThan0Input) {

        if (objValidatorConfig.rules === undefined || objValidatorConfig.rules === null)
            objValidatorConfig.rules = new Object();

        objValidatorConfig.rules.numeriValorizzati = function (input) {
            if (input.hasClass("GreaterThan0") && $(input).val() !== "") {
                return kendo.parseFloat(input.data("kendoNumericTextBox").value()) > 0;
            } else {
                return true;
            }
        };

        if (objValidatorConfig.messages === undefined || objValidatorConfig.messages === null)
            objValidatorConfig.messages = new Object();

        objValidatorConfig.messages.numeriValorizzati = TraduzioneMultiResx(resxObj, "IlCampoDeveEssereMaggioreDiZero", "Il valore deve essere positivo e maggiore di 0");

    }

    if (hasDateCollegate) {

        if (objValidatorConfig.rules === undefined || objValidatorConfig.rules === null)
            objValidatorConfig.rules = new Object();

        objValidatorConfig.rules.sequenzaDate = function (input) {
            if (input.is("#inDataRegistrazione") && $(input).val() !== "" && $(input).data("kendoDatePicker").value().getTime() !== AGRODATAINIZIO.getTime()) {
                return input.data("kendoDatePicker").value().getTime() >= KendoDate("inDataEmissione").value().getTime();
            } else {
                return true;
            }
        };

        if (objValidatorConfig.messages === undefined || objValidatorConfig.messages === null)
            objValidatorConfig.messages = new Object();

        objValidatorConfig.messages.sequenzaDate = TraduzioneMultiResx(resxObj, "DataRegistrazioneAntecedenteEmissione", "La data di registrazione è antecedente alla data di emissione");
    }

    if (hasDdlRequiredNoZero) {

        if (objValidatorConfig.rules === undefined || objValidatorConfig.rules === null)
            objValidatorConfig.rules = new Object();

        objValidatorConfig.rules.ddlRequiredNoZero = function (input) {
            //if (input.is("[required][data-role=dropdownlist]")) {
            if (input.is("[required][data-role=dropdownlist].DdlRequiredNoZero")) {

                let ddVal = input.data('kendoDropDownList').value();
                if (ddVal !== undefined && ddVal !== null && parseInt(ddVal) !== 0) {
                    return true;
                } else {
                    return false;
                }

            } else {
                return true;
            }
        };

        if (objValidatorConfig.messages === undefined || objValidatorConfig.messages === null)
            objValidatorConfig.messages = new Object();

        objValidatorConfig.messages.ddlRequiredNoZero = TraduzioneMultiResx(resxObj, "CampoObbligatorio", "Campo obbligatorio");

    }

    if (hasTimePickerNoZero) {

        if (objValidatorConfig.rules === undefined || objValidatorConfig.rules === null)
            objValidatorConfig.rules = new Object();

        objValidatorConfig.rules.timePickerNoZero = function (input) {

            if (input.is("[data-role=datetimepicker].TimePickerNoZero")) {

                let dtVal = input.data("kendoDateTimePicker").value();
                if (dtVal !== undefined && dtVal !== null && dtVal.toLocaleTimeString() !== "00:00:00") {
                    return true;
                } else {
                    return false;
                }

            } else {
                return true;
            }
        };

        if (objValidatorConfig.messages === undefined || objValidatorConfig.messages === null)
            objValidatorConfig.messages = new Object();

        objValidatorConfig.messages.timePickerNoZero = TraduzioneMultiResx(resxObj, "SpecificareOrario", "Specificare un orario");

    }


    if (hasVerificaISCC) {

        if (objValidatorConfig.rules === undefined || objValidatorConfig.rules === null)
            objValidatorConfig.rules = new Object();

        objValidatorConfig.rules.ISCCNonConforme = function (input) {
            if (input.is(".ISCCNonConforme")) {

                if (input.val() == "1") {
                    return true;
                } else {
                    return false;
                }

            } else {
                return true;
            }
        };

        if (objValidatorConfig.messages === undefined || objValidatorConfig.messages === null)
            objValidatorConfig.messages = new Object();

        objValidatorConfig.messages.ISCCNonConforme = TraduzioneMultiResx(resxObj, "ISCCNonConforme", "");

    }


    return $("#" + idDivForm).kendoValidator(objValidatorConfig).data("kendoValidator");
}

function impostaClasseErrore(idControllo, nomeClasse, flag) {

    //per le TextBox semplici (e in tutti gli altri casi lo cambio cmq sull'input sottostante)
    $("#" + idControllo).toggleClass(nomeClasse, flag);

    if (yearVersioneKendo >= 2022) {

        if ($("#" + idControllo).parent("span.k-numerictextbox").length > 0) {
            //per le numericTextBox (post 2022.1)
            $("#" + idControllo).parent("span.k-numerictextbox").toggleClass(nomeClasse, flag);
        }
        else if ($("#" + idControllo).parent("span.k-datepicker").length > 0) {
            //per datePicker (post 2022.1)
            $("#" + idControllo).parent("span.k-datepicker").toggleClass(nomeClasse, flag);
        }
        else if ($("#" + idControllo).parent("span.k-datetimepicker").length > 0) {
            //per dateTimePicker (post 2022.1)
            $("#" + idControllo).parent("span.k-datetimepicker").toggleClass(nomeClasse, flag);
        }
        else if ($("#" + idControllo).parent("span.k-picker").length > 0) {
            //per le drop-down (post 2022.1)
            $("#" + idControllo).parent("span.k-picker").toggleClass(nomeClasse, flag);
        }

    } else {

        if ($("#" + idControllo).parent("span.k-numeric-wrap").parent("span.k-widget.k-numerictextbox").length > 0) {
            //per le numericTextBox
            $("#" + idControllo).parent("span.k-numeric-wrap").parent("span.k-widget.k-numerictextbox")
                .toggleClass(nomeClasse, flag);
        }
        else if ($("#" + idControllo).parent("span.k-picker-wrap").parent("span.k-widget.k-datepicker").length > 0) {
            //per datePicker
            $("#" + idControllo).parent("span.k-picker-wrap").parent("span.k-widget.k-datepicker")
                .toggleClass(nomeClasse, flag);
        }
        else if ($("#" + idControllo).parent("span.k-picker-wrap").parent("span.k-widget.k-datetimepicker").length > 0) {
            //per dateTimePicker
            $("#" + idControllo).parent("span.k-picker-wrap").parent("span.k-widget.k-datetimepicker")
                .toggleClass(nomeClasse, flag);
        }
        else if ($("#" + idControllo).parent("span.k-widget.k-dropdown").length > 0) {
            //per le drop-down
            $("#" + idControllo).parent("span.k-widget.k-dropdown").toggleClass(nomeClasse, flag);
        }

    }
}

function SvuotaSegnalazioniErrori(flagIntestazione, flagTabTestata, flagTabDettaglio) {

    //if (flagIntestazione === true) {
        $("#erroriMsgIntestazione ul.errorMessages").empty();
        $("#erroriMsgIntestazione").hide();
    //}

    if (flagTabTestata === true) {
        $("#erroriMsgTabTestata ul.errorMessages").empty();
        $("#erroriMsgTabTestata").hide();
        $("#bdgErrorTestata").remove();
    }

    if (flagTabDettaglio === true) {
        $("#erroriMsgTabDettaglio ul.errorMessages").empty();
        $("#erroriMsgTabDettaglio").hide();
        $("#bdgErrorDettaglio").remove();

        let listElementsError = [];
        if (yearVersioneKendo >= 2022) {
            listElementsError = $("#tabDettagliDoc").find(".k-invalid").children().not("span,button").filter("[id]");
        } else {
            listElementsError = $("#tabDettagliDoc").find(".k-invalid").not("span");
        }

        if (listElementsError.length > 0) {
            listElementsError.each(function (index, node) {
                delete validatorTabDettaglio._errors[node.id];
                impostaClasseErrore(node.id, "errorClass", false);
                $(node).removeClass("k-invalid");
            });
        }
    }
}

function EvidenziaErroriIntestazione() {

    let listElementsError = $("#intestazione").find(".k-invalid").not("span");
    if (yearVersioneKendo >= 2022) {
        listElementsError = $.merge(listElementsError, $("#intestazione").find(".k-invalid").children().not("span,button").filter("[id]"));
    }

    if (listElementsError.length > 0) {
        listElementsError.each(function(index, node) {
            let label = $("label[for=" + node.id + "]");
            let message = validatorIntestazione._errors[node.id];
            $("#erroriMsgIntestazione ul.errorMessages")
                .append("<li><span>" + label.html() + "</span> " + message + "</li>");
        });
    }

    let numErroriIntestazione = $("#erroriMsgIntestazione ul.errorMessages").children().length;
    if (numErroriIntestazione > 0) {
        $("#erroriMsgIntestazione").show();
    }

    return numErroriIntestazione;
}

function EvidenziaErroriTabTestata() {

    let listElementsError = $("#tabTestataDoc").find(".k-invalid").not("span");
    if (yearVersioneKendo >= 2022) {
        listElementsError = $.merge($("#tabTestataDoc").find(".k-invalid").children().not("span,button").filter("[id]"), listElementsError);
    }

    if (listElementsError.length > 0) {
        listElementsError.each(function (index, node) {
            let label = $("label[for=" + node.id + "]");
            let message = validatorTabTestata._errors[node.id];
            $("#erroriMsgIntestazione ul.errorMessages")
                .append("<li><span> " + TraduzioneMultiResx(resxObj, "Testata", "Testata") + " → " + label.html() + "</span> " + message + "</li>");
        });
    }

    let numErroriTabTestata = $("#erroriMsgIntestazione ul.errorMessages").children().length;
    if (numErroriTabTestata > 0) {
        $("#erroriMsgIntestazione").show();
        $("#a_tabTestataDoc").append('  <span id="bdgErrorTestata" class="badge" style="background-color: red;">' + numErroriTabTestata + '</span>');
    }

    return numErroriTabTestata;
}

function EvidenziaErroriTabDettaglio() {

    let listElementsError = [];
    if (yearVersioneKendo >= 2022) {
        listElementsError = $("#tabDettagliDoc").find(".k-invalid").children().not("span,button").filter("[id]");
    } else {
        listElementsError = $("#tabDettagliDoc").find(".k-invalid").not("span");
    }

    if (listElementsError.length > 0) {
        listElementsError.each(function (index, node) {
            let label = $("label[for=" + node.id + "]");
            let message = validatorTabDettaglio._errors[node.id];
            $("#erroriMsgIntestazione ul.errorMessages")
                .append("<li><span> " + TraduzioneMultiResx(resxObj, "Righe", "Righe") + " → " + label.html() + "</span> " + message + "</li>");
        });
    }

    let numErroriTabDettaglio = $("#erroriMsgIntestazione ul.errorMessages").children().length;
    if (numErroriTabDettaglio > 0) {
        $("#erroriMsgIntestazione").show();
        $("#a_tabDettagliDoc").append('  <span id="bdgErrorDettaglio" class="badge" style="background-color: red;">' + numErroriTabDettaglio + '</span>');
    }

    return numErroriTabDettaglio;
}

function ControlliFormDettaglio() {

    let w_elem_cod = parseInt(KendoDDL("ddlCategorieMagazzino").value());

    //TODO: implementa controlli form
    let errMsg = "";

    if (Qs_CaricoScarico === CAU_TRASFERIMENTO &&
        Get_KendoDDLValue("ddlUbicDestinazione") !== "" && Get_KendoDDLValue("ddlUbicProvenienza") !== "" &&
        Get_KendoDDLValue("ddlUbicProvenienza") === Get_KendoDDLValue("ddlUbicDestinazione")) {
        errMsg += TraduzioneMultiResx(resxObj, "MagazziniDiProvenienzaEDestinazioneCoincidono",
            "Il magazzino di provenienza e quello di destinazione coincidono! Modificare uno dei due magazzini.") + " <br/>";
    }

    if (w_elem_cod !== RIGA_DESCRIZIONE_LIBERA &&
        w_elem_cod !== ALTRI_BENI &&
        Get_KendoDDLValue("ddlProdottoDes") === "") {
        errMsg += TraduzioneMultiResx(resxObj, "SelezionareUnProdotto", "È necessario selezionare un prodotto!") + " <br/>";
    }

    let w_um = 0;
    if (Get_KendoDDLValue("ddlUM") !== undefined && Get_KendoDDLValue("ddlUM") !== null && Get_KendoDDLValue("ddlUM") !== "")
        w_um = parseInt(Get_KendoDDLValue("ddlUM"));

    let udm_multipli_g = [enum_Udm.chilogrammi.value, enum_Udm.quintali.value, enum_Udm.tonnellate.value];

    if (udm_multipli_g.includes(w_um) && is_Trasf_Veg_Anim_FormProdottoUC()) {
        if (kendo.parseFloat(Get_KendoNumTBValue("idKgLordi")) <= 0) {
            errMsg += TraduzioneMultiResx(resxObj, "PesoLordoMaggioreDiZero", "Il peso lordo deve essere maggiore di 0 ") + " <br/>";
        }
        if (kendo.parseFloat(Get_KendoNumTBValue("idKgNetti")) <= 0) {
            errMsg += TraduzioneMultiResx(resxObj, "PesoNettoMaggioreDiZero", "Il peso netto deve essere maggiore di 0 ") + " <br/>";
        }
        if (kendo.parseFloat(Get_KendoNumTBValue("idKgLordi")) < kendo.parseFloat(Get_KendoNumTBValue("idKgNetti")) ) {
            errMsg += TraduzioneMultiResx(resxObj, "PesoLordoMinoreNetto", "Il peso lordo non può essere minore del peso netto ") + " <br/>";
        }  
    } else {
        if (w_elem_cod !== RIGA_DESCRIZIONE_LIBERA &&
            kendo.parseFloat(Get_KendoNumTBValue("idQuantita")) <= 0) {
            errMsg += TraduzioneMultiResx(resxObj, "QuantitaMaggioreDiZero", "La quantità deve essere maggiore di 0 ") + " <br/>";
        } 
    }
    

    // Controlli sull'utilizzo della stessa entrata in caso di cambio dei parametri qualitativi
    if (is_Trasf_Veg_Anim_FormProdottoUC()) {
        let w_cal_cod = parseInt($('input[name$="hf_Cal_Cod"]').val());

        if (lavCodAccettazione && w_cal_cod !== 0) {

            // Il prodotto con le caratteristiche che aveva è già stato scaricato (vendita, lavorazione, scarico di mag)

            // Verifico se uno dei campi chiave è cambiato
            if (riga_originale_entrata_FormProdottoUC !== null) {
                let altriMovimentiStessoCalCod = Verifica_Utilizzo_CalCod();

                // Per bloccare il salvataggio,
                // se il prodotto è cambiato controllo che nel vecchio ci siano movimenti
                // se il prodotto è lo stesso controllo anche che i parametri qualitativi siano cambiati
                if (altriMovimentiStessoCalCod !== "") {
                    let elenMov = null;

                    // Modifica non possibile: il prodotto j è già stato incluso nel/i documento/i di vendita - nella/e lavorazione/i x,y,z
                    if (riga_originale_entrata_FormProdottoUC.Mat_Cod !== parseInt(Get_KendoDDLValue("ddlProdottoDes") * -1) ||
                        riga_originale_entrata_FormProdottoUC.Lotto !== $("#txtLottoAccettazione").val()) {
                        elenMov = ottieniElenchiDaStrMovimentiCalCod(altriMovimentiStessoCalCod);

                        errMsg += " " + kendo.format(TraduzioneMultiResx(resxObj, "MessErr_ModProdUtilizzato",
                            "<b>MODIFICA NON POSSIBILE</b><br/>Il prodotto {0} era già stato incluso"), riga_originale_entrata_FormProdottoUC.Referenza) + " ";

                    }
                    else {
                        let dummy = {};
                        AggiungiValoriParametriQualitativi(dummy);
                        let arrParamQual = dummy.MateriePrimeCampionature;

                        if (arrParamQual !== undefined && arrParamQual !== null && Array.isArray(arrParamQual)) {
                            // Se il prodotto è stato movimentato, ma non sono presenti parametri qualitativi, allora lascio libera la modifica

                            for (let item of arrParamQual) {
                                let nomeParamQual = item.Tipo.substring(1); // tolgo la 'o' iniziale dal nome del campo

                                // Il parametro qualitativo utilizza la colonna Val_Cod
                                if (riga_originale_entrata_FormProdottoUC.hasOwnProperty("FF_" + nomeParamQual + "_Val_Cod")) {
                                    if (item.Val_Cod != riga_originale_entrata_FormProdottoUC["FF_" + nomeParamQual + "_Val_Cod"]) {

                                        elenMov = ottieniElenchiDaStrMovimentiCalCod(altriMovimentiStessoCalCod);

                                        errMsg += " " + kendo.format(TraduzioneMultiResx(resxObj, "MessErr_ModParamQualProdUtilizzato",
                                            "<b>MODIFICA NON POSSIBILE</b><br/>Si sta cercando di modificare i parametri qualitativi del prodotto {0} che è già stato incluso"),
                                            riga_originale_entrata_FormProdottoUC.Referenza) + " ";

                                        break;

                                    }
                                }

                                // Il parametro qualitativo utilizza la colonna Tipo_Cod
                                if (riga_originale_entrata_FormProdottoUC.hasOwnProperty("FF_" + nomeParamQual + "_Tipo_Cod")) {
                                    if (item.Tipo_Cod != riga_originale_entrata_FormProdottoUC["FF_" + nomeParamQual + "_Tipo_Cod"]) {

                                        elenMov = ottieniElenchiDaStrMovimentiCalCod(altriMovimentiStessoCalCod);

                                        //errMsg += " <b>MODIFICA NON POSSIBILE</b><br/>Si sta cercando di modificare i parametri qualitativi del prodotto "
                                        //    + riga_originale_entrata_FormProdottoUC.Referenza + " che è già stato incluso ";

                                        errMsg += " " + kendo.format(TraduzioneMultiResx(resxObj, "MessErr_ModParamQualProdUtilizzato",
                                            "<b>MODIFICA NON POSSIBILE</b><br/>Si sta cercando di modificare i parametri qualitativi del prodotto {0} che è già stato incluso"),
                                            riga_originale_entrata_FormProdottoUC.Referenza) + " ";

                                        break;

                                    }
                                }
                            }
                        }
                        
                    }

                    if (elenMov !== null) {
                        if (elenMov.vendite.length > 0) {
                            if (elenMov.vendite.length === 1) {
                                errMsg += TraduzioneMultiResx(resxObj, "MesErr_ProdUtilizzatoInDocumento", "<br />nel documento");
                            }
                            else {
                                errMsg += TraduzioneMultiResx(resxObj, "MesErr_ProdUtilizzatoInDocumenti", "<br />nei documenti");
                            }
                            errMsg += " " + TraduzioneMultiResx(resxObj, "MesErr_DocVenditaAcquisto", "di vendita/acquisto: <br />") +
                                elenMov.vendite.join("<br />");
                        }
                        if (elenMov.lavorazioni.length > 0) {
                            if (elenMov.lavorazioni.length === 1) {
                                errMsg += TraduzioneMultiResx(resxObj, "MesErr_ProdUtilizzatoInLavorazione", "<br />nella lavorazione");
                            }
                            else {
                                errMsg += TraduzioneMultiResx(resxObj, "MesErr_ProdUtilizzatoInLavorazioni", "<br />nelle lavorazioni");
                            }
                            errMsg += ": <br />" + elenMov.lavorazioni.join("<br />");
                        }
                    }
                }
                // Fine controllo altriMovimentiStessoCalCod
            }
        }
        // Fine controllo Conferimento e cal_cod ------------------
    }
    // Fine controllo Trasf Veg/Anim --------------------------


    if (isRaccolteXConferimentiAttive() && raccolteXConferimenti_dsSelezionate.length > 0) {
        if (raccolteConfUC_verificaCoerenzaRigheSelezionate(raccolteXConferimenti_dsSelezionate) === false) {
            errMsg += TraduzioneMultiResx(resxObj, "RaccolteSelezionateNonCoerenti",
                "Le righe di raccolta selezionate si riferiscono a impianti appartenenti a centri aziendali diversi oppure con specie o unità di misura diverse") + " <br/>";
        }
    }
    else {
        //TODO: verifica se accettazione e griglia impianti, i kg totali impianti per raccolta deve essere uguale ai kg netti
        if (lavCodAccettazione === true &&
            isContattoImpresaGias === true &&
            KendoGrid("tab_griglia_impianti") !== undefined) {

            let sumImpianti = kendo.parseFloat(KendoGrid("tab_griglia_impianti").dataSource.aggregates().Qta.sum);

            if (sumImpianti !== 0 && getKendoSwitch("ChkImpiantiIndefiniti") === false) {

                let sumImpiantiRound = Math.round(sumImpianti * 100) / 100;
                let qta = 0;
                if (Get_KendoNumTBValue("idQuantita") !== null)
                    qta = kendo.parseFloat(Get_KendoNumTBValue("idQuantita"));
                else {
                    if (Get_KendoNumTBValue("idKgNetti") !== null)
                        qta = kendo.parseFloat(Get_KendoNumTBValue("idKgNetti"));
                }

                var udm = parseInt(Get_KendoDDLValue("ddlUM"));
                switch (udm) {
                    case enum_Udm.quintali.value:
                        qta = qta * 100;
                        break;
                    case enum_Udm.tonnellate.value:
                        qta = qta * 1000;
                }

                if (sumImpiantiRound !== qta) {
                    errMsg += TraduzioneMultiResx(resxObj, "QuantitaImpiantiIncoerentiKgNetti",
                        "Il totale delle quantità di prodotto impostate sugli impianti per la raccolta differiscono dai kg netti di prodotto caricato.") + " <br/>";
                }
            }


            if (sumImpianti == 0 && getKendoSwitch("ChkImpiantiIndefiniti") == false && parseInt(Obbligo_Ripartizione) == 1) {
                errMsg += TraduzioneMultiResx(resxObj, "QuantitaImpiantiNonCorretta", "Quantità non impostata correttamente su griglia impianti.") + " <br/>";
            }

        }
    }

    // Controllo non ci siano record nella griglia degli imballi con confezioni selezionate in caso si sia scelta "numero" come unità di misura
    let kddlUM = KendoDDL("ddlUM");
    if (kddlUM !== undefined && parseInt(kddlUM.value()) === enum_Udm.numero.value) {
        let kgridImballi = KendoGrid("tab_imballaggi_formProdottoUC");
        if (kgridImballi !== undefined) {
            // Tramite la funzione filter di javascript ottengo un nuovo array con tutti gli elementi che soddisfano la condizione
            let arrImballiConf = kgridImballi.dataSource.data().filter(function (elem) {
                return elem.FF_confezione_Tipo_Cod !== undefined && elem.FF_confezione_Tipo_Cod !== 0;
            });
            if (arrImballiConf.length > 0) {
                errMsg += TraduzioneMultiResx(resxObj, "ImpossibileInserireConfezioniNumero", "Non è possibile inserire confezioni utilizzando l'unità di misura 'numero'");
            }
        }
    }

    // Controllo che siano coerenti i dati dei pesi riscontrati
    let kgLordiRiscontrati = Get_KendoNumTBValue("idKgLordiRiscontrati", true);
    //let taraTotRiscontrata = Get_KendoNumTBValue("idTaraRiscontrata");
    let kgNettiRiscontrati = Get_KendoNumTBValue("idKgNettiRiscontrati", true);
    if (kgLordiRiscontrati !== 0 && kgNettiRiscontrati !== 0) {
        if (kgNettiRiscontrati > kgLordiRiscontrati) {
            errMsg += TraduzioneMultiResx(resxObj, "KgRiscontratiLordiMinoriNetti", "I kg riscontrati lordi sono minori dei rispettivi netti");
        }
    }



    return errMsg;
}


function ottieniElenchiDaStrMovimentiCalCod(strMovimenti) {
    let elencoMovimenti = {
        vendite: [],
        lavorazioni: []
    };

    let movimenti = strMovimenti.split("|");
    let lav_cod = 0;
    let lav_des = "";

    for (let i = 0; i < movimenti.length; i++) {

        lav_cod = parseInt(movimenti[i].split("_")[0]);
        lav_des = movimenti[i].split("_")[1];

        // Distinguo il movimento fra vendite e lavorazioni
        if (lav_cod === enum_LavCod.Ordine_Vendita_Emesso.value ||
            lav_cod === enum_LavCod.DDT_Emesso.value ||
            lav_cod === enum_LavCod.Fattura_Emessa.value ||
            lav_cod === enum_LavCod.DDT_Contabilizzato_Emesso.value ||
            lav_cod === enum_LavCod.Ordine_Acquisto.value) {

            elencoMovimenti.vendite.push(lav_des);

        } else {

            if (lav_cod === enum_LavCod.Lavorazione.value) {
                elencoMovimenti.lavorazioni.push(lav_des);
            }
        }
    }

    return elencoMovimenti;
}