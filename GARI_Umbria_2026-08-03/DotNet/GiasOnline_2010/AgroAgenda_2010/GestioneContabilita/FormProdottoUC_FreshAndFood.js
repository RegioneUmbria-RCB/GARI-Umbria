
var rigaDuplicata_ImballiFormProdottoUCKendoGrid = false;
var rigaDaCopiare_ImballiFormProdottoUCKendoGrid;

function moduloFromElemCod(elem_cod) {

    let w_modulo_anagrafe_log = 0;
    if (elem_cod === TRASFORMATI_VEGETALI &&
        modulo_anagrafe_log.includes(Modulo_FreshFood))
        w_modulo_anagrafe_log = Modulo_FreshFood;
    else
        if (elem_cod === TRASFORMATI_ANIMALI &&
            modulo_anagrafe_log.includes(Modulo_Zoo))
            w_modulo_anagrafe_log = Modulo_Zoo;

    return w_modulo_anagrafe_log;

}

//verifica se è abilitato almeno uno dei moduli FF (Fresh&Food, Tabacco, Zoo)
function is_FF_FormProdottoUC() { 

    let w_is_FF_FormProdottoUC = false;
    if (modulo_anagrafe_log.includes(Modulo_FreshFood) ||
        modulo_anagrafe_log.includes(Modulo_Tabacco) ||
        modulo_anagrafe_log.includes(Modulo_Zoo)) {
            w_is_FF_FormProdottoUC = true;
    }
     
    return w_is_FF_FormProdottoUC;
}

//verifica se è abilitata la gestione FF per i trasformati vegetali/animali 
function is_Trasf_Veg_Anim_FormProdottoUC() {

    let w_is_Trasf_Veg_Anim_FormProdottoUC = false;

    let moduli_TrasfVegetali_presenti = false;
    if (modulo_anagrafe_log.includes(Modulo_FreshFood) ||
        modulo_anagrafe_log.includes(Modulo_Tabacco)) {
        moduli_TrasfVegetali_presenti = true;
    }

    let moduli_TrasfAnimali_presenti = false;
    if (modulo_anagrafe_log.includes(Modulo_Zoo)) {
        moduli_TrasfAnimali_presenti = true;
    }

    if (moduli_TrasfVegetali_presenti === true || moduli_TrasfAnimali_presenti === true) {

        if (KendoDDL("ddlCategorieMagazzino").dataSource._data.length !== 0) {

            let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));

            switch (ddlCategorieMagazzinoValue) {

                case TRASFORMATI_VEGETALI:
                    if (moduli_TrasfVegetali_presenti === true) {
                        w_is_Trasf_Veg_Anim_FormProdottoUC = true;
                    }
                    break;

                case TRASFORMATI_ANIMALI:
                    if (moduli_TrasfAnimali_presenti === true) {
                        w_is_Trasf_Veg_Anim_FormProdottoUC = true;
                    }
                    break;

            }

        }

    }

    return w_is_Trasf_Veg_Anim_FormProdottoUC;

}

/** Restituisce se la categoria prodotto attualmente selezionata è fra quelle che gestiscono il cal_cod */
function is_ElemCod_CalCod() {
    let ddlCategorieMagazzinoValue = 0;

    if (KendoDDL("ddlCategorieMagazzino").dataSource.data().length !== 0) {
        ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));
    }

    return [TRASFORMATI_VEGETALI, SEMILAVORATI_VEGETALI].includes(ddlCategorieMagazzinoValue);
}

function is_210_FormProdottoUC() {

    let w_is_Trasf_Veg_FormProdottoUC = false;
    if (modulo_anagrafe_log.includes(Modulo_FreshFood) ||
        modulo_anagrafe_log.includes(Modulo_Tabacco)) {
        if (KendoDDL("ddlCategorieMagazzino").dataSource._data.length !== 0) {
            let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));
            if (ddlCategorieMagazzinoValue === TRASFORMATI_VEGETALI) {
                w_is_Trasf_Veg_FormProdottoUC = true;
            }
        }
    }

    return w_is_Trasf_Veg_FormProdottoUC;
}

function is_310_FormProdottoUC() {

    let w_is_Trasf_Anim_FormProdottoUC = false;
    if (modulo_anagrafe_log.includes(Modulo_Zoo)) {
        if (KendoDDL("ddlCategorieMagazzino").dataSource._data.length !== 0) {
            let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));
            if (ddlCategorieMagazzinoValue === TRASFORMATI_ANIMALI) {
                w_is_Trasf_Anim_FormProdottoUC = true;
            }
        }
    }

    return w_is_Trasf_Anim_FormProdottoUC;
}

function creaCampiFreshAndFood_FormProdottoUC() {
    
    // Creo comunque tutti i controlli F&F anche se non sono in F&F
    $('input[name$="idQuantita"]').kendoNumericTextBox({ change: UC_idQuantita_change });
    $('input[name$="idQuantitaRiscontrata"]').kendoNumericTextBox();
    $('input[name$="idKgLordi"]').kendoNumericTextBox({ format: enum_Udm.chilogrammi.format, decimals: enum_Udm.chilogrammi.decimals, change: UC_KgLordi_change });
    $('input[name$="idKgNetti"]').kendoNumericTextBox({ format: enum_Udm.chilogrammi.format, decimals: enum_Udm.chilogrammi.decimals, change: UC_KgNetti_change });
    $('input[name$="idKgLordiRiscontrati"]').kendoNumericTextBox({ format: enum_Udm.chilogrammi.format, decimals: enum_Udm.chilogrammi.decimals, change: UC_KgLordiRiscontrati_change });
    $('input[name$="idKgNettiRiscontrati"]').kendoNumericTextBox({ format: enum_Udm.chilogrammi.format, decimals: enum_Udm.chilogrammi.decimals, change: UC_KgNettiRiscontrati_change });
    $('input[name$="idTara"]').kendoNumericTextBox({ change: UC_Tara_change });
    $('input[name$="idTaraRiscontrata"]').kendoNumericTextBox({ change: UC_TaraRiscontrata_change });

    if (FF_gest_materiale_vivaistico) {
        $("#lblKgLordi").text(TraduzioneMultiResx(resxFormProdottoUC, "Numero", "Numero") + ":");
        $("#lblKgNetti").text(TraduzioneMultiResx(resxFormProdottoUC, "Numero", "Numero") + ":");
    }
}

function UC_idQuantita_change() {
    AggiornaDettagliEconomici();
}

function UC_KgLordi_change(e) {

    let tara = kendo.parseFloat(Get_KendoNumTBValue("idTara"));
    let udm = parseInt(Get_KendoDDLValue("ddlUM"));

    switch (udm) {
        case enum_Udm.quintali.value:
            tara = tara / 100;
            break;
        case enum_Udm.tonnellate.value:
            tara = tara / 1000;
    }

    Set_KendoNumTBValue("idKgNetti", kendo.parseFloat(Get_KendoNumTBValue("idKgLordi")) - tara);
    calcolaDegrado_FormProdottoUC(Get_KendoNumTBValue("idKgNetti"), Get_KendoNumTBValue("idDegradoPerc"));

    if (lavCodAccettazione === true && isContattoImpresaGias === true)
        cmbRipartizione_change();

    AggiornaDettagliEconomici();
}

function UC_KgNetti_change(e) {

    let tara = kendo.parseFloat(Get_KendoNumTBValue("idTara"));
    let udm = parseInt(Get_KendoDDLValue("ddlUM"));

    switch (udm) {
        case enum_Udm.quintali.value:
            tara = tara / 100;
            break;
        case enum_Udm.tonnellate.value:
            tara = tara / 1000;
    }

    Set_KendoNumTBValue("idKgLordi", kendo.parseFloat(Get_KendoNumTBValue("idKgNetti")) + tara);
    calcolaDegrado_FormProdottoUC(Get_KendoNumTBValue("idKgNetti"), Get_KendoNumTBValue("idDegradoPerc"));

    if (lavCodAccettazione === true && isContattoImpresaGias === true)
        cmbRipartizione_change();

    AggiornaDettagliEconomici();
}

function calcolaDegrado_FormProdottoUC(netto, degradoPerc) {
    if (netto === null)
        netto = 0;
    if (degradoPerc === null)
        degradoPerc = 0;

    // Per ora il degrado è senza decimali
    //$('#lblDegradoRisultatoCalc').text(Math.round((netto / 100 * kendo.parseFloat(degradoPerc)) * 100) / 100);
    //$('#lblKgEffettiviRisultatoCalc').text(Math.round((netto / 100 * (100 - kendo.parseFloat(degradoPerc))) * 100) / 100);
    let udm = parseInt(Get_KendoDDLValue("ddlUM"));

    switch (udm) {
        case enum_Udm.quintali.value:
            netto = netto * 100;
            break;
        case enum_Udm.tonnellate.value:
            netto = netto * 1000;
    }

    let w_degradoRisultatoCalc = Math.round(netto / 100 * kendo.parseFloat(degradoPerc));
    $('#lblDegradoRisultatoCalc').text(w_degradoRisultatoCalc);
    $('#lblKgEffettiviRisultatoCalc').text(Math.round(netto) - w_degradoRisultatoCalc);
}

function UC_Tara_change(e) {

    let tara = kendo.parseFloat(Get_KendoNumTBValue("idTara"));
    let udm = parseInt(Get_KendoDDLValue("ddlUM"));

    switch (udm) {
        case enum_Udm.quintali.value:
            tara = tara / 100;
            break;
        case enum_Udm.tonnellate.value:
            tara = tara / 1000;
    }

    Set_KendoNumTBValue("idKgNetti", kendo.parseFloat(Get_KendoNumTBValue("idKgLordi")) - tara);
    calcolaDegrado_FormProdottoUC(Get_KendoNumTBValue("idKgNetti"), Get_KendoNumTBValue("idDegradoPerc"));

    if (lavCodAccettazione === true && isContattoImpresaGias === true)
        cmbRipartizione_change();
}

function UC_KgLordiRiscontrati_change(e) {

    let kgLordiRiscontrati = e.sender.value();
    let taraTotRiscontrata = Get_KendoNumTBValue("idTaraRiscontrata");

    if (kgLordiRiscontrati === null) {
        Set_KendoNumTBValue("idKgLordiRiscontrati", 0); // Non esegue l'evento di change
        kgLordiRiscontrati = 0;
    }

    if (kgLordiRiscontrati > 0 && taraTotRiscontrata !== null && taraTotRiscontrata !== 0) {

        let tara = kendo.parseFloat(taraTotRiscontrata);
        let udm = Get_KendoDDLValue("ddlUM");

        switch (udm) {
            case enum_Udm.quintali.value:
                tara = tara / 100;
                break;
            case enum_Udm.tonnellate.value:
                tara = tara / 1000;
        }

        Set_KendoNumTBValue("idKgNettiRiscontrati", kendo.parseFloat(kgLordiRiscontrati) - tara);
    }
    else {
        Set_KendoNumTBValue("idKgNettiRiscontrati", kendo.parseFloat(kgLordiRiscontrati));
    }

    calcolaDegrado_FormProdottoUC(Get_KendoNumTBValue("idKgNettiRiscontrati"), Get_KendoNumTBValue("idDegradoPerc"));
}

function UC_KgNettiRiscontrati_change(e) {
    //Non faccio nulla
    //Set_KendoNumTBValue("idKgLordiRiscontrati", kendo.parseFloat(Get_KendoNumTBValue("idKgNettiRiscontrati")) + kendo.parseFloat(Get_KendoNumTBValue("idTaraRiscontrata")));
    //calcolaDegrado_FormProdottoUC(Get_KendoNumTBValue("idKgNettiRiscontrati"), Get_KendoNumTBValue("idDegradoPerc"));
}

function UC_TaraRiscontrata_change(e) {

    let taraTotRiscontrata = e.sender.value();
    let kgLordiRiscontrati = Get_KendoNumTBValue("idKgLordiRiscontrati");

    // Se kgLordiRiscontrati
    // è null => imposto 0 su kg lordi e netti
    // è 0 => imposto i kg netti uguali ai lordi
    // è > 0 => se taraRiscontrata è null o zero imposto i kg netti uguali ai lordi, altrimenti imposto i kg netti alla sottrazione fra i due valori
    if (kgLordiRiscontrati === null) {
        Set_KendoNumTBValue("idKgLordiRiscontrati", 0); // Non esegue l'evento di change
        kgLordiRiscontrati = 0;
    }

    if (kgLordiRiscontrati > 0 && taraTotRiscontrata !== null && taraTotRiscontrata !== 0) {

        let tara = kendo.parseFloat(Get_KendoNumTBValue("idTara"));
        let udm = Get_KendoDDLValue("ddlUM");

        switch (udm) {
            case enum_Udm.quintali.value:
                tara = tara / 100;
                break;
            case enum_Udm.tonnellate.value:
                tara = tara / 1000;
        }

        Set_KendoNumTBValue("idKgNettiRiscontrati", kendo.parseFloat(kgLordiRiscontrati) - tara);
    }
    else {
        Set_KendoNumTBValue("idKgNettiRiscontrati", kendo.parseFloat(kgLordiRiscontrati));
    }

    // TODO: Se l'unità di misura non è kg e quindi il campo di input è quantità riscontrata al posto di kgNettiRiscontrati, come si deve comportare questa funzione?

    calcolaDegrado_FormProdottoUC(Get_KendoNumTBValue("idKgNettiRiscontrati"), Get_KendoNumTBValue("idDegradoPerc"));
}

function impostaVisibilitaFreshAndFood_FormProdottoUC(mostraControlliFF) {
    let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));
   
    if (mostraControlliFF) {

        if (!lavCodAccettazionePomodoro && !lavCodMovMagazzino && (gestitoImballaggio_FF || gestitoContenitore_FF || gestitoConfezione_FF)) {
            $("#id_row_imballaggio").show();
            KendoNumTB("idTara").enable(false);
            KendoNumTB("idTaraRiscontrata").enable(false);
        } else {
            $("#id_row_imballaggio").hide();
            KendoNumTB("idTara").enable(true);
            KendoNumTB("idTaraRiscontrata").enable(true);
        }

        Gestione_Visibilita_Degrado(GetPropertyFromJson($(cIdOpzioniContab).val(), "Degrado_Visibilita_Obbligatorieta"));
         
        // TODO U.M.
        // Per gestione materiale vivaistico con F&F non mostro il lordo, il degrado e i kg riscontrati
        if (FF_gest_materiale_vivaistico) {
            Visibilita_Degrado(false, false);
            Visibilita_KgLordi(false, false);    
            Visibilita_KgNettiRiscontrati(false, false);
            Visibilita_KgLordiRiscontrati(false, false);
            Visibilita_BtnRiscontrati(false);
        }

    } else {

        // Non mostro i parametri qualitativi e gli imballaggi
        $("#id_parametri_qualitativi_list").hide();
        $("#id_row_imballaggio").hide(); 
    }

    //visualizzo le note solo se F&F attivo e si tratta di un carico
    if (mostraControlliFF && Qs_CaricoScarico === CAU_CARICO) {

        $("#idNote").show();

    } else {

        $("#idNote").hide();

    }

}   

function creaParametriQualitativi_FF_Zoo(elem_cod, veg_cod, cul_cod) {
    $("#id_parametri_qualitativi_list div").html("");
    let container = document.getElementById("id_parametri_qualitativi_list");
    let contaRighe = 0;
    let nrRighe = 0;
    let newRowDiv = null;
    let foundParamQual = false;

    let w_modulo_anagrafe_log = moduloFromElemCod(elem_cod);

    if (w_modulo_anagrafe_log !== 0) {
        // La variabile paramQual_FF_filtrospevar è globale
        paramQual_FF_filtrospevar = RicercaParametriQualitativiFiltroSpecieVarieta(true, $(cIdPiva).val(), w_modulo_anagrafe_log, veg_cod, cul_cod);
        for (let ipar = 0; ipar < paramQual_FF_filtrospevar.length; ipar++) {

            let tabella_id = paramQual_FF_filtrospevar[ipar].Tabella_ID;

            if (tabella_id !== 0) {
                if (tabella_id !== "4" &&
                    tabella_id !== "5" &&
                    tabella_id !== "8") {

                    // Creo il controllo solo per i parametri a libera imputazione oppure se c'è almeno un  
                    // valore da caricare nella DDL(se length === 1 significa che c'è solo il record vuoto)
                    if (paramQual_FF_filtrospevar[ipar].Tipo === 3 ||
                        paramQual_FF_filtrospevar[ipar].Tipo === 4 ||
                        paramQual_FF_filtrospevar[ipar].Tipo === 5 ||
                        RicercaValoriParametriQualitativiFiltratiSpecieVarieta(tabella_id, $(cIdPiva).val(), true, veg_cod, cul_cod).length > 1) {

                        // 3 colonne per riga
                        if (contaRighe === 999) {
                            contaRighe = 0;
                        }

                        if (contaRighe === 0) {
                            if (newRowDiv !== null) {
                                container.appendChild(newRowDiv);
                            }
                            nrRighe++;
                            newRowDiv = creaNewRowDiv("id" + nrRighe.toString);
                        }

                        //Se è ordine, considero tutti i parametri come non obbligatori
                        let required = false;
                        if (paramQual_FF_filtrospevar[ipar].ChkObbligatorio === 1 && lavCodOrdine === false) {
                            required = true;
                        }

                        // Tipo 3 sono i parametri a libera imputazione numerici
                        // Tipo 4 sono i parametri a libera imputazione stringa
                        // Tipo 5 sono i parametri a libera imputazione data
                        let tipo_param = "";
                        switch (paramQual_FF_filtrospevar[ipar].Tipo) {
                            case 3:
                                foundParamQual = true;
                                tipo_param = "txt";
                                break;

                            case 4:
                                foundParamQual = true;
                                tipo_param = "txtStr";
                                break;

                            case 5:
                                foundParamQual = true;
                                tipo_param = "date";
                                break;

                            default:
                                tipo_param = "ddl";
                                break;
                        }

                        var newColumnDiv = creaNewColumnBS(4, 6, 12);
                        var newDivInputGroup = creaDIV("input-group");

                        let tabella_cod_des = paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des;
                        let tabella_des = paramQual_FF_filtrospevar[ipar].Tabella_Des;

                        let paramQualValorizzatoInLav = paramQualValorizzatiInLavorazioni.find(function (elem) { return elem.tabellaCodDes === tabella_cod_des; });
                        let isParamQualValorizzatoInLav = paramQualValorizzatoInLav !== undefined;
                        //let paramQualValorizzatoInLavorazione = paramQualValorizzatiInLavorazioni.some(function (elem) { return elem.tabellaCodDes === tabella_cod_des; });

                        if (lavCodAccettazione && isParamQualValorizzatoInLav) {
                            newColumnDiv.style.display = "none";
                        }

                        //la label mi serve in questo formato per eventuali segnalazioni di errore
                        let testoLabel = tabella_des.charAt(0).toUpperCase() + tabella_des.substring(1);
                        let classeLabel = "input-group-addon";
                        if (required === true) {
                            classeLabel = classeLabel + " campiObbligatori";
                        }
                        newDivInputGroup.appendChild(creaLabel("lbl" + tabella_cod_des, testoLabel, classeLabel, tipo_param + tabella_cod_des));

                        let nuovoInputHtml = null;
                        switch (paramQual_FF_filtrospevar[ipar].Tipo) {
                            case 3:
                                nuovoInputHtml = creaInputGenerico(tipo_param + tabella_cod_des, tipo_param + tabella_cod_des, "form-control", txtParamQual_change, required, null, tabella_id);

                                nuovoInputHtml.disabled = paramQualISCC.some(function (elem) { return elem === tabella_cod_des; });

                                newDivInputGroup.appendChild(nuovoInputHtml);

                                if (lavCodAccettazione && isParamQualValorizzatoInLav && required) {
                                    nuovoInputHtml.value = paramQualValorizzatoInLav.valDefault;
                                }
                                break;

                            case 4:
                                nuovoInputHtml = creaInputGenerico(tipo_param + tabella_cod_des, tipo_param + tabella_cod_des, "form-control", txtParamQual_change, required, null, tabella_id);

                                nuovoInputHtml.disabled = paramQualISCC.some(function (elem) { return elem === tabella_cod_des; });

                                newDivInputGroup.appendChild(nuovoInputHtml);

                                if (lavCodAccettazione && isParamQualValorizzatoInLav && required) {
                                    nuovoInputHtml.value = paramQualValorizzatoInLav.valDefault;
                                }
                                break;

                            case 5:
                                nuovoInputHtml = creaInputGenerico(tipo_param + tabella_cod_des, tipo_param + tabella_cod_des, "kendoCalendarParamQual", txtParamQual_change, required, null, tabella_id);

                                nuovoInputHtml.disabled = paramQualISCC.some(function (elem) { return elem === tabella_cod_des; });

                                newDivInputGroup.appendChild(nuovoInputHtml);

                                if (lavCodAccettazione && isParamQualValorizzatoInLav && required) {
                                    nuovoInputHtml.value = paramQualValorizzatoInLav.valDefault;
                                }
                                break;

                            default:
                                var classeInput = "form-control";
                                if (required === true && tipo_param === "ddl") {
                                    classeInput = classeInput + " DdlRequiredNoZero";
                                }
                                nuovoInputHtml = creaInputGenerico(tipo_param + tabella_cod_des, tipo_param + tabella_cod_des, classeInput, null, required, null, tabella_id);

                                nuovoInputHtml.disabled = paramQualISCC.some(function (elem) { return elem === tabella_cod_des; });

                                newDivInputGroup.appendChild(nuovoInputHtml);

                                if (lavCodAccettazione && isParamQualValorizzatoInLav && required) {
                                    nuovoInputHtml.value = paramQualValorizzatoInLav.valDefault;
                                }
                                break;
                        }

                        newColumnDiv.appendChild(newDivInputGroup);
                        newRowDiv.appendChild(newColumnDiv);

                        contaRighe++;

                        //KendoDDL("ddl" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des).bind("change", idCalibro2_change);
                    }

                }
            }

            // ultimo giro
            if (ipar === paramQual_FF_filtrospevar.length - 1 && newRowDiv !== null) {
                container.appendChild(newRowDiv);
            }

        }

        for (let ipar = 0; ipar < paramQual_FF_filtrospevar.length; ipar++) {

            let tabella_id = paramQual_FF_filtrospevar[ipar].Tabella_ID;
            let tabella_cod_des = paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des;

            if (tabella_id !== 0) {
                if (tabella_id !== enum_IdParamQual.Imballaggio &&
                    tabella_id !== enum_IdParamQual.Confezione &&
                    tabella_id !== enum_IdParamQual.Contenitore) {

                    let tipo_param = "";
                    switch (paramQual_FF_filtrospevar[ipar].Tipo) {

                        case enum_TipoParamQual.Numero:
                            // Tipo 3 sono i parametri a libera imputazione numerici
                            var numDecParam = parseInt(paramQual_FF_filtrospevar[ipar].NumDecimali_Maximo);
                            var formatParam = "0." + "#".repeat(numDecParam);
                            tipo_param = "txt";
                            $("#" + tipo_param + tabella_cod_des).kendoNumericTextBox({ format: formatParam, decimals: numDecParam }); //, change: txtParamQual_change
                            break;

                        case enum_TipoParamQual.Stringa:
                            // Tipo 4 sono i parametri a libera imputazione stringa
                            tipo_param = "txtStr";
                            // NULLA DA FARE
                            break;

                        case enum_TipoParamQual.Data:
                            // Tipo 5 sono i parametri a libera imputazione data
                            tipo_param = "date";
                            // NULLA DA FARE, la classe l'ho già impostata sopra
                            break;

                        default:
                            // DDL
                            tipo_param = "ddl";
                            var risultato_lettura_SpecieVarieta = RicercaValoriParametriQualitativiFiltratiSpecieVarieta(tabella_id, $(cIdPiva).val(), true, veg_cod, cul_cod);
                            if (risultato_lettura_SpecieVarieta.length > 1) {
                                let jQueryDDL = creaKendoDropDownList(tipo_param + tabella_cod_des, { read: LeggiValoriParametriQualitativiFiltratiSpecieVarieta, data: { elencoValori: risultato_lettura_SpecieVarieta } }, "val_des", "val_cod").bind("change", ddlParamQual_change);
                                miaKendoDDL = jQueryDDL.data("kendoDropDownList");
                                //miaKendoDDL.bind("change", ddlParamQual_change);

                                if (paramQualISCC.some(function (elem) { return elem === tabella_cod_des; })) {
                                    miaKendoDDL.enable(false);
                                }

                                // Se c'è solo un valore ed il parametro qualitativo è obbligatorio lo imposto in automatico
                                if (paramQual_FF_filtrospevar[ipar].ChkObbligatorio === 1 &&
                                    miaKendoDDL.dataSource.data().length === 2 &&
                                    lavCodOrdine === false) {
                                    miaKendoDDL.value(miaKendoDDL.dataSource.data()[1].val_cod);
                                }
                                if (!foundParamQual && miaKendoDDL.dataSource.data().length > 1) {
                                    foundParamQual = true;
                                }
                            }
                            break;
                    }
                }
            }
        }
    }

    // Andrebbe fatto in imposta visibilità ma ce l'ho a disposizione qui
    if (foundParamQual) {
        $("#id_parametri_qualitativi_list").show();
        $(".kendoCalendarParamQual").kendoDatePicker({
            footer: "#: kendo.toString(data, 'd')#", //Template per il footer
            max: new Date(2100, 11, 31) //,  Larghezza calendario come il campo di input...
            //        open: function () {
            //            var calendar = this.dateView.calendar;
            //            calendar.wrapper.width(this.wrapper.width() - 6);
            //        }
        });
    } else {
        $("#id_parametri_qualitativi_list").hide();
    }

    //newdiv.innerHTML = "Entry " + (counter + 1) + " <br><input type='text' name='myInputs[]'>";
    //document.getElementById(divName).appendChild(newdiv);

    
}



function creaParametriQualitativi_Indici_GHG() {
    
    let contaRighe = 0;
    let nrRighe = 0;
    let newRowDiv = null;
    let valore = "";

    foundParamQualIndici = false;   
    parametri_indici_creati = true;

    chiaveETD = "";
    chiaveETD_UDM = "";
    chiaveETD_QTY = "";
    chiaveGHG_Total = "";
    paramQual_FF_indici_GHG = "";

    $("#id_parametri_indici_list_GHG div").html("");
    let container = document.getElementById("id_parametri_indici_list_GHG");

    if (cIdLavCod === enum_LavCod.DDT_Emesso.value) {
        
        // La variabile paramQual_FF_indici_GHG è globale
        paramQual_FF_indici_GHG = RicercaParametriIndici(false, $(cIdPiva).val(), "GHG");

        for (let ipar = 0; ipar < paramQual_FF_indici_GHG.length; ipar++) {

            foundParamQualIndici = true;
            let tipocampo = paramQual_FF_indici_GHG[ipar].TipoCampo;
       
            // 3 colonne per riga
            if (contaRighe === 3) {
                contaRighe = 0;
            }

            if (contaRighe === 0) {
                if (newRowDiv !== null) {
                    container.appendChild(newRowDiv);
                }
                nrRighe++;
                newRowDiv = creaNewRowDiv("id" + nrRighe.toString);
            }

            //parametri come obbligatori
            let required = false;
            if (paramQual_FF_indici_GHG[ipar].ChkObbligatorio === 1) {
                required = true;
            }
         
            var newColumnDiv = creaNewColumnBS(4, 4, 12);
            var newDivInputGroup = creaDIV("input-group");
        
            let titoloindice = paramQual_FF_indici_GHG[ipar].TitoloIndice;                
            let id_indice = paramQual_FF_indici_GHG[ipar].ID_Indice;
            var nomecampo = paramQual_FF_indici_GHG[ipar].Nome_Campo.toUpperCase();
            //la label mi serve in questo formato per eventuali segnalazioni di errore
            let testoLabel = titoloindice.charAt(0).toUpperCase() + titoloindice.substring(1);
                   
            var classeLabel = "input-group-addon";
            if (required === true) {
                classeLabel = classeLabel + " campiObbligatori";
            }
            var newKey = paramQual_FF_indici_GHG[ipar].TipoIndice + "_" + tipocampo + "_" + id_indice;
        
            let tipo_param = "";

            if (tipocampo == 0) {
          
                /*libera imputazione  */                      
                switch (paramQual_FF_indici_GHG[ipar].TipoDato) {

                    case "string":

                        tipo_param = "txt";
                        newDivInputGroup.appendChild(creaLabel("lbl" + newKey, titoloindice, classeLabel, tipo_param + newKey));
                        newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, "form-control", null, required));

                        break;

                    case "date":

                        tipo_param = "txt";
                        newDivInputGroup.appendChild(creaLabel("lbl" + newKey, titoloindice, classeLabel, tipo_param + newKey));
                        newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, "kendoCalendar", null, required));

                        break;

                    case "numeric":

                        tipo_param = "txt";
                        newDivInputGroup.appendChild(creaLabel("lbl" + newKey, titoloindice, classeLabel, tipo_param + newKey));

                        switch (nomecampo) {

                            case "QTY_TRASPORTO_ATTUALE":
                           
                                chiaveETD_QTY = tipo_param + newKey;
                                newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, "form-control", CalcolaETD_GHG, required, null, null));
                                break;

                            default:

                                newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, "form-control", null, required, null, null));
                                break;
                        }
                        break;

                    case "boolean":

                        tipo_param = "chk";
                        newDivInputGroup.appendChild(creaLabel("lbl" + newKey, titoloindice, classeLabel, tipo_param + newKey));
                        newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, "kendoSwitch", null, false));

                        break;
                }
            }
            else {
                tipo_param = "ddl";

                newDivInputGroup.appendChild(creaLabel("lbl" + newKey, titoloindice, classeLabel, tipo_param + newKey));

                var classeInput = "form-control";

                if (required === true) {
                    classeInput = classeInput + " DdlRequiredNoZero";
                }
           
                switch (nomecampo) {

                    case "UDM_TRASPORTO_ATTUALE":
                        newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, classeInput, CalcolaETD_GHG, required, null, null));
                       break;

                    default:
                        newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, classeInput, null, required, null, null));
                    break;
                }
            }


            newColumnDiv.appendChild(newDivInputGroup);
            newRowDiv.appendChild(newColumnDiv);

            contaRighe++;

            // ultimo giro
            if (ipar == paramQual_FF_indici_GHG.length - 1 && newRowDiv !== null) {
                container.appendChild(newRowDiv);
            }          
       
        }



        //Creazione DDL Kendo
        for (let ipar = 0; ipar < paramQual_FF_indici_GHG.length; ipar++) {
            if (paramQual_FF_indici_GHG[ipar].ID_Indice !== 0) {

                let chiave = paramQual_FF_indici_GHG[ipar].TipoIndice + "_" + paramQual_FF_indici_GHG[ipar].TipoCampo + "_" + paramQual_FF_indici_GHG[ipar].ID_Indice;
                let nDecimali = paramQual_FF_indici_GHG[ipar].nDecimali;
                let ChkDisabilitato = paramQual_FF_indici_GHG[ipar].ChkDisabilitato;
                let nomecampo = paramQual_FF_indici_GHG[ipar].Nome_Campo.toUpperCase();
                let controllo = "";

                switch (parseInt(paramQual_FF_indici_GHG[ipar].TipoCampo)) {

                    case 0:

                        switch (paramQual_FF_indici_GHG[ipar].TipoDato) {

                            case "string":
                                if (ChkDisabilitato == 1) {
                                    $("#txt" + chiave).attr('disabled', 'disabled');
                                }
                                break;

                            case "date":
                                controllo = "txt" + chiave + "";
                                $('#' + controllo).kendoDatePicker({
                                    footer: "#: kendo.toString(data, 'd')#",
                                    max: new Date(2100, 11, 31)
                                });

                                if (ChkDisabilitato == 1) {
                                    KendoDate(controllo).enable(false);
                                }

                                break;

                            case "numeric":
                                controllo = "txt" + chiave + "";
                                $('#' + controllo).kendoNumericTextBox({ format: "0.##", decimals: nDecimali });

                                if (ChkDisabilitato == 1) {
                                    KendoNumericBox(controllo).enable(false);
                                }

                                break;

                            case "boolean":
                                controllo = "chk" + chiave + "";
                                creaKendoSwitch(controllo);

                                if (ChkDisabilitato == 1) {
                                    KendoSwitch(controllo).enable(false);
                                }
                                break;
                        }
                        break;

                    default:

                    
                        paramQual_FF_indici_dettagli_GHG = RicercaDDLParametriIndice($(cIdPiva).val(), paramQual_FF_indici_GHG[ipar].ID_Indice, paramQual_FF_indici_GHG[ipar].TipoCampo, paramQual_FF_indici_GHG[ipar].Elenco_Tipo, paramQual_FF_indici_GHG[ipar].Elenco_Cod, "", paramQual_FF_indici_GHG[ipar].FiltroAggiuntivo);

                        if (paramQual_FF_indici_dettagli_GHG.length > 0) {

                            let jQueryDDLDettagli = creaKendoDropDownList("ddl" + chiave, { read: LeggiValoriParametriIndiciDettagli, data: { elencoValori: paramQual_FF_indici_dettagli_GHG } }, "Valore_Des", "Valore_Cod");

                            let ddl = "ddl" + chiave;
                            if (paramQual_FF_indici_GHG[ipar].ChkObbligatorio === 1) {
                                if (KendoDDL(ddl).dataSource._data.length === 2) {
                                    Set_KendoDDLValue(ddl, KendoDDL(ddl).dataSource._data[1].Valore_Cod);
                                }
                            }

                            if (ChkDisabilitato == 1) {
                                KendoDDL(ddl).enable(false);
                            }
                        }
                        break;
                 }              

            }

        }


        //Impostazione Parametri        
        for (let ipar = 0; ipar < paramQual_FF_indici_GHG.length; ipar++) {
            if (paramQual_FF_indici_GHG[ipar].ID_Indice !== 0) {

                let chiave = paramQual_FF_indici_GHG[ipar].TipoIndice + "_" + paramQual_FF_indici_GHG[ipar].TipoCampo + "_" + paramQual_FF_indici_GHG[ipar].ID_Indice;
                let nDecimali = paramQual_FF_indici_GHG[ipar].nDecimali;
                let ChkDisabilitato = paramQual_FF_indici_GHG[ipar].ChkDisabilitato;
                let nomecampo = paramQual_FF_indici_GHG[ipar].Nome_Campo.toUpperCase();

                switch (nomecampo) {

                    case "UDM_TRASPORTO_ATTUALE":

                        chiaveETD_UDM = "ddl" + chiave;
                        valore = Get_KendoDDLValue("ddlUnitaMisuraTrasporto", 0);
                        Set_KendoDDLValue(chiaveETD_UDM, valore);
                        CalcolaETD_GHG();
                        break;

                    case "ETD_TRASPORTO_ATTUALE":

                        chiaveETD = "txt" + chiave;
                        CalcolaETD_GHG();
                        break;

                    case "QTY_TRASPORTO_ATTUALE":

                        valore = Get_KendoNumTBValue("ntbDistanzaTrasporto", true);
                        Set_KendoNumTBValue("txt" + chiave, valore); 
                        CalcolaETD_GHG();
                        break;


                    case "GHG_TOTAL":

                        chiaveGHG_Total = "txt" + chiave;
                        $("#txt" + chiave).attr('disabled', 'disabled');
                        break;

                    default:

                        break;

                }
            }
        }

    }

    // Andrebbe fatto in imposta visibilità ma ce l'ho a disposizione qui
    if (foundParamQualIndici) {
        $("#id_parametri_indici_list_GHG").show();
        $("#panelBar_ParametriGhG").show();
        //$(".kendoCalendarParamQual").kendoDatePicker({
        //    footer: "#: kendo.toString(data, 'd')#", //Template per il footer
        //    max: new Date(2100, 11, 31) //,  Larghezza calendario come il campo di input...
        //    //        open: function () {
        //    //            var calendar = this.dateView.calendar;
        //    //            calendar.wrapper.width(this.wrapper.width() - 6);
        //    //        }
        //});
    } else {
        $("#id_parametri_indici_list_GHG").hide();
        $("#panelBar_ParametriGhG").hide();
        
    }

    ////newdiv.innerHTML = "Entry " + (counter + 1) + " <br><input type='text' name='myInputs[]'>";
    ////document.getElementById(divName).appendChild(newdiv);

}

function CalcolaETD_GHG() {

    if (chiaveETD !== "" && chiaveETD_UDM !== "" && chiaveETD_QTY !== "") {       
        var valore = CalcoloStandardFactor($("#" + chiaveETD_QTY).val(), 0, Get_KendoDDLValue(chiaveETD_UDM, 0), 1, $("#inStatoCC1").val());      
        Set_KendoNumTBValue(chiaveETD, valore);

        AggiornaGHGTotal();
    }
}


function CalcolaETD_PQ() {

    if (lavCodAccettazione === true && paramQual_FF_filtrospevar.length > 0 && $('input[name$="txtkmdistance"]').val() !== undefined) {
         var valore = CalcoloStandardFactor(kendo.parseFloat($('input[name$="txtkmdistance"]').val()), 0, 305, 1, $("#inStatoCC1").val());
         Set_KendoNumTBValue("ghgforetd", valore);
    }

    AggiornaGHGTotal();

}

// Creazione controlli parametri pomodoro i18n
function creaParametriQualitativiPomodoro_FF(veg_cod, cul_cod, cal_cod) {

    $("#id_parametri_qualitativi_list div").html("");
    let container = document.getElementById("id_parametri_qualitativi_list");
    let tipo_param = "txt";
    let parametri = {}
    
    paramQual_FF_filtrospevar = RicercaParametriQualitativiFiltroSpecieVarieta(true, $(cIdPiva).val(), Modulo_FreshFood, veg_cod, cul_cod);

    for (var ipar = 0; ipar < paramQual_FF_filtrospevar.length; ipar++) {
        if (paramQual_FF_filtrospevar[ipar].Tabella_ID !== 0 && paramQual_FF_filtrospevar[ipar].Tipo === 3) {
            let tabella_cod_des = paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des;
            let tabella_des = paramQual_FF_filtrospevar[ipar].Tabella_Des;
            let required = paramQual_FF_filtrospevar[ipar].ChkObbligatorio === 1;
            let valoreMin = paramQual_FF_filtrospevar[ipar].Valore_Minimo;
            let valoreMax = paramQual_FF_filtrospevar[ipar].Valore_Maximo;
            let testoLabel = tabella_des.charAt(0).toUpperCase() + tabella_des.substring(1);
            let classeLabel = "input-group-addon" + (required ? " campiObbligatori" : "");
            if (valoreMax != null && valoreMax != "") {
                parametriPomodoro[tabella_cod_des + "_MAX"] = valoreMax.replace(",", ".");
                testoLabel += (" (Max " + valoreMax + "%)").replace(".", ",");
            }
            var newDivInputGroup = creaDIV("input-group");
            newDivInputGroup.appendChild(creaLabel("lbl" + tabella_cod_des, testoLabel, classeLabel, tipo_param + tabella_cod_des));
            newDivInputGroup.appendChild(creaInputGenerico(tipo_param + tabella_cod_des, tipo_param + tabella_cod_des, "form-control", txtParamQual_change, required));
            parametri[tabella_cod_des] = newDivInputGroup;
        }
    }

    var newRowDiv = creaNewRowDiv("id_parametri_qualitativi_pomodoro");
    var newColumnDiv1 = creaNewColumnBS(6, 6, 12);
    var newColumnDiv2 = creaNewColumnBS(6, 6, 12);

    var divDifettiMaggiori = creaDIV("bd-callout bd-callout-danger");
    divDifettiMaggiori.innerHTML = "<h5>Difetti Maggiori</h5>";
    newColumnDiv1.appendChild(divDifettiMaggiori);
    newColumnDiv1.appendChild(parametri["inerti"]);
    newColumnDiv1.appendChild(parametri["verde"]);
    newColumnDiv1.appendChild(parametri["marcio"]);

    var divCalcolaPrezzo = creaDIV("");
    divCalcolaPrezzo.innerHTML = "<div id='btnCalcolaPrezzo' class='btn btn-success' onclick='Calcola_Prezzo_Pomodoro();'><span class='fa fa-calculator'></span>Dati Riepilogativi</div>";
    newColumnDiv1.appendChild(divCalcolaPrezzo);
    newRowDiv.appendChild(newColumnDiv1);

    var divDifettiMinori = creaDIV("bd-callout bd-callout-warning");
    divDifettiMinori.innerHTML = "<h5>Difetti Minori</h5>";
    newColumnDiv2.appendChild(divDifettiMinori);
    newColumnDiv2.appendChild(parametri["gradobrix"]);
    newColumnDiv2.appendChild(parametri["fruttischiacciati"]);
    newColumnDiv2.appendChild(parametri["fruttiimmaturi"]);
    newColumnDiv2.appendChild(parametri["fruttiscottati"]);
    newColumnDiv2.appendChild(parametri["fruttilesionati"]);
    newRowDiv.appendChild(newColumnDiv2);

    container.appendChild(newRowDiv);
    
    for (let ipar = 0; ipar < paramQual_FF_filtrospevar.length; ipar++) {
        if (paramQual_FF_filtrospevar[ipar].Tabella_ID !== 0 && paramQual_FF_filtrospevar[ipar].Tipo === 3) {
            $("#" + tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des).kendoNumericTextBox({ decimals: 2, format: "0.00\\%", min: 0, max: 100, change: Calcola_Prezzo_Pomodoro });
        }
    }
    
    $("#id_parametri_qualitativi_list div").show();
}

// Per i conferimenti ed i prodotti referenza del F&F valorizza i parametri qualitativi con i dati impostati in anagrafica
function impostaDefaultParametriQualitativi_FF() {

    if ((lavCodAccettazione === true || cIdLavCod === enum_LavCod.DDT_Emesso.value) && paramQual_FF_filtrospevar.length > 0 && inizializzaFormDettaglioRiga === false) {

        let prodMatCod = parseInt(Get_KendoDDLValue("ddlProdottoDes", 0)) * -1; // So che il codice del prodotto viene usato come value della ddl
        if (prodMatCod !== 0) {
            // Faccio una lettura a partire dal mat_cod per prelevare il cal_cod, poi effettuo un'altra lettura per leggere dalla materie_prime_campionature
            let objProd = RicercaProdotti_FF(false, $(cIdPiva).val(), prodMatCod, 0, false, true, false)[0];
            let arrCampionatureProd = [];
            if (objProd !== undefined && objProd.Mat_Cod_Referenza !== 0 && objProd.Cal_Cod !== 0) {
                arrCampionatureProd = Leggi_MateriePrimeCampionature($(cIdPiva).val(), objProd.Cal_Cod);
            }
            if (arrCampionatureProd.length > 0) {

                // 1 - Imposto i valori di default dei parametri qualitativi
                let objCampionatureProd = {};
                let oImballaggioCod = 0;
                let oContenitoreCod = 0;
                let oConfezioneCod = 0;

                let numImballi = 1; // Impostato fisso ad uno
                let numContenInImballo = 1; // Indica il numero di contenitori che l'imballaggio può contenere
                let numConfInContenitore = 1; // Indica il numero di confezioni che il contenitore può contenere

                arrCampionatureProd.forEach(function (row) {
                    if (row.Tipo.startsWith("o")) {
                        //let tipoCod = row.Tipo_Cod === 0 ? null : row.Tipo_Cod;
                        //let valCod = row.Val_Cod === "" || row.Val_Cod === "0" ? null : row.Val_Cod;

                        objCampionatureProd["FF_" + row.Tipo.substring(1) + "_Tipo_Cod"] = row.Tipo_Cod;
                        objCampionatureProd["FF_" + row.Tipo.substring(1) + "_Val_Cod"] = row.Val_Cod;

                        if (row.Tipo === "oimballaggio") {
                            oImballaggioCod = row.Tipo_Cod;
                        }
                        if (row.Tipo === "ocontenitore") {
                            oContenitoreCod = row.Tipo_Cod;
                            let valCod = parseInt(row.Val_Cod);
                            numContenInImballo = isNaN(valCod) || valCod === 0 ? 1 : valCod;
                        }
                        if (row.Tipo === "oconfezione") {
                            oConfezioneCod = row.Tipo_Cod;
                            let valCod = parseInt(row.Val_Cod);
                            numConfInContenitore = isNaN(valCod) || valCod === 0 ? 1 : valCod;
                        }
                    }
                });
                //for (let prop in objCampionatureProd) {
                //    console.log(prop + " " + objCampionatureProd[prop]);
                //}
                ImpostaParametriQualitativi(objCampionatureProd);

                // 2 - Imposto le tipologie degli imballi del prodotto
                if (oImballaggioCod !== 0 || oContenitoreCod !== 0 || oConfezioneCod !== 0) {
                    
                    let kGridImballi = KendoGrid("tab_imballaggi_formProdottoUC");
                    if (kGridImballi !== undefined) {

                        kGridImballi.dataSource.data([]); // Dal momento che l'utente cambia prodotto con uno che possiede un imballaggio di default, sovrascrivo quest'ultimo a quello presente

                        $(".k-grid-add").trigger("click");
                        let rowImballiEdit = $("#tab_imballaggi_formProdottoUC").find(".k-grid-content").find(".k-grid-edit-row");

                        if (rowImballiEdit.length === 1) {
                            let ddlEditImballaggi = KendoDDL("FF_imballaggio_Tipo_Cod");
                            let ddlEditContenitore = KendoDDL("FF_contenitore_Tipo_Cod");
                            let ddlEditConfezione = KendoDDL("FF_confezione_Tipo_Cod");

                            if (ddlEditImballaggi !== undefined && oImballaggioCod !== 0) {
                                KendoNumTB("NrImballaggi").value(numImballi);
                                KendoNumTB("NrImballaggi").trigger("change");
                                ddlEditImballaggi.value(oImballaggioCod);
                                ddlEditImballaggi.trigger("change");
                            }

                            if (ddlEditContenitore !== undefined && oContenitoreCod !== 0) {
                                KendoNumTB("NrContenitori").value(numImballi * numContenInImballo);
                                KendoNumTB("NrContenitori").trigger("change");
                                ddlEditContenitore.value(oContenitoreCod);
                                ddlEditContenitore.trigger("change");
                            }

                            if (ddlEditConfezione !== undefined) {
                                let prodottoDataItem = KendoDDL("ddlProdottoDes").dataItem();
                                let prodottoUm = prodottoDataItem.Udm_Cod;
                                let ddlNrConf = KendoNumTB("NrConfezioni");

                                // Imposto la confezione di default, solo se l'unità di misura del prodotto selezionato
                                // non è numero, perché in questo caso non posso avere delle confezioni
                                if (prodottoUm !== enum_Udm.numero.value) {
                                    if (oConfezioneCod !== 0) {
                                        ddlNrConf.value(numImballi * numContenInImballo * numConfInContenitore);
                                        ddlNrConf.trigger("change");
                                        ddlEditConfezione.value(oConfezioneCod);
                                        ddlEditConfezione.trigger("change");
                                    }
                                }
                                else {
                                    ddlEditConfezione.enable(false);
                                    ddlNrConf.enable(false);

                                    let kntbConfTara = KendoNumTB("FF_confezione_Tara_Campionatura");
                                    if (kntbConfTara !== undefined) {
                                        kntbConfTara.enable(false);
                                    }

                                    if (gestionePesiRiscontrati) {
                                        KendoNumTB("Num_Conf_Riscontrate").enable(false);
                                        KendoNumTB("Tara_Unit_Conf_Riscontrata").enable(false);
                                    }
                                }
                            }


                            let rowImballiEditLockedCol = $("#tab_imballaggi_formProdottoUC").find(".k-grid-content-locked").find(".k-grid-edit-row");

                            if (ddlEditImballaggi === undefined && ddlEditContenitore === undefined && ddlEditConfezione === undefined) {
                                // In questo caso non considero la riga valida e ne annullo le modifiche
                                rowImballiEditLockedCol.find(".k-grid-cancel").trigger("click");
                            }
                            else {
                                rowImballiEditLockedCol.find(".k-grid-update").trigger("click");
                            }
                        }
                    }
                }
                // 2 - Fine Imposto le tipologie degli imballi del prodotto
            }
            // - Fine controllo su presenza di materie_prime_campionature
        }

        CalcolaETD_PQ();
    }
}

function popola_ImballiFormProdottoUC(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: kRead_ImballiFormProdottoUC_rows,
        funzioneSubmit: { funzione: SubmitAggiorna_ImballiFormProdottoUC, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };
    var idModel = "key_mov_dett";

    var campiKendoModel = kRead_ImballiFormProdottoUC_mod();
    var colonneKendoGrid = kRead_ImballiFormProdottoUC_col();
    var parametriPerLettura = null;
    var parametriDataSource = {
        //pagesize: 50,
        aggregate: [
            { field: "NrImballaggi", aggregate: "sum" },
            { field: "NrContenitori", aggregate: "sum" },
            { field: "NrConfezioni", aggregate: "sum" },
        //    { field: "Num_Imballi_Riscontrati", aggregate: "sum" },
        //    { field: "Num_Colli_Riscontrati", aggregate: "sum" },
        //    { field: "Num_Conf_Riscontrate", aggregate: "sum" }
        ]
    };

    var colCustKendoGrid = [
        {
            command: [
                {
                    iconClass: "fa fa-pencil fa-xs", className: "blockModifica", name: "edit", text: { edit: "", update: " ", cancel: " " }
                },
                {
                    iconClass: "fa fa-trash fa-xs", className: "blockCancella", name: "destroy", text: ""
                }
            ],
            title: TraduzioneMultiResx(resxFormProdottoUC, "Operazioni", "Operazioni"), locked: true
        }
    ];

    // Se l'utente non è abilitato in modifica non mostro il pulsante di duplicazione
    // Le colonne modifica / cancellazione e inserimento nuova riga sono già gestite nel GiasBase
    if (UteAbilitatoInsMod) {
        colCustKendoGrid[0].command.push(
            {
                iconClass: "fa fa-files-o fa-xs", className: "blockDuplica", name: "duplica", text: "", click: duplicaRiga_ImballiFormProdottoUCKendoGrid
            });
    }

    var parametriKendoGrid = {
        excel: false, pdf: false, groupable: false, pageable: false,
        editable: {
            mode: "inline"
        },
        colonneCustomKendoGrid: colCustKendoGrid,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundRighe_ImballiFormProdottoUC, funzioneDaChiamareDopoEdit: onEdit_ImballiFormProdottoUC, funzioneDaChiamareDopoSave: onSave_ImballiFormProdottoUC, funzioneDaChiamareDopoAnnulla: onCancel_ImballiFormProdottoUC };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );

    var grid = $("#" + IDControllo).data("kendoGrid");
    controllaSeAggiungereNuovaRigaImballi(grid);
    grid.bind("remove", onRemove_ImballiFormProdottoUC);

    sonoInModificaImballi = false;
}

function kRead_ImballiFormProdottoUC_rows(options) {

    var data = $('input[name$="hdKendo_Imballi_formProdottoUC"]').val();
    var jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kRead_ImballiFormProdottoUC_col() {

    var data = $('input[name$="hdKendo_Imballi_formProdottoUC"]').val();
    var jSonParsed_Kendo = JSON.parse(data);
    var mostraSigla = true;

    //Cella
    var k = 0;
    ////kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "key_Dest", "Mag. o Cella", k, "Ubic_Des", celle_Template, null, null, filterable_celle_Template, true);
 
    if (jSonParsed_Kendo.kendo_model.FF_imballaggio_Tipo_Cod !== undefined) {
        mostraSigla = false;
        filterTemplate = getfilterable_paramQualTemplate($(cIdPiva).val(), 4, mostraSigla);
        kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "FF_imballaggio_Tipo_Cod", TraduzioneMultiResx(resxFormProdottoUC, "ImballaggioAbbr", "Imb."), k, "FF_imballaggio_Descrizione", paramQual_Template, null, null, filterTemplate, true);
        k++;
    }

    if (jSonParsed_Kendo.kendo_model.FF_contenitore_Tipo_Cod !== undefined) {
        mostraSigla = false;
        filterTemplate = getfilterable_paramQualTemplate($(cIdPiva).val(), 8, mostraSigla);
        kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "FF_contenitore_Tipo_Cod", TraduzioneMultiResx(resxFormProdottoUC, "ContenitoreAbbr", "Conten."), k, "FF_contenitore_Descrizione", paramQual_Template, null, null, filterTemplate, true);
        k++;
    }

    if (jSonParsed_Kendo.kendo_model.FF_confezione_Tipo_Cod !== undefined) {
        mostraSigla = false;
        filterTemplate = getfilterable_paramQualTemplate($(cIdPiva).val(), 5, mostraSigla);
        kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "FF_confezione_Tipo_Cod", TraduzioneMultiResx(resxFormProdottoUC, "ConfezioneAbbr", "Conf."), k, "FF_confezione_Descrizione", paramQual_Template, null, null, filterTemplate, true);
        k++;
    }

    for (var i = 0; i < jSonParsed_Kendo.kendo_columns.length; i++) {
        if (jSonParsed_Kendo.kendo_columns[i].format === "{0:n0}" ||
            jSonParsed_Kendo.kendo_columns[i].format === "{0:n1}" ||
            jSonParsed_Kendo.kendo_columns[i].format === "{0:n2}" ||
            jSonParsed_Kendo.kendo_columns[i].format === "{0:n3}" ||
            jSonParsed_Kendo.kendo_columns[i].format === "{0:n4}" ||
            jSonParsed_Kendo.kendo_columns[i].format === "{0:n5}") {
            jSonParsed_Kendo.kendo_columns[i].editor = editKendoNumericTextBoxForGridInline;
        }
    }

    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}

function kRead_ImballiFormProdottoUC_mod() {

    var data = $('input[name$="hdKendo_Imballi_formProdottoUC"]').val();
    var jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_model;
}

function onDataBoundRighe_ImballiFormProdottoUC(e) {
    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");
    kendo_AggiustaDimensioneColonne("#" + gridId);
    
}

function controllaSeAggiungereNuovaRigaImballi(grid) {

    let altriMovimentiStessoCalCod = "";

    // Righe imballi multiple ammesse solo in accettazione prodotto F&F
    // 9/1/20120 e per facilitare la parte di scrittura dati relativa ai parametri qualitativi per ora solo in inserimento 
    
        $('.k-grid-add').unbind("click");
        // TODO U.M.
        $(".k-grid-add", grid.element).bind("click", function (ev) {
            if (lavCodAccettazione) {
                altriMovimentiStessoCalCod = Verifica_Utilizzo_CalCod(); 
            }
            var gridImb = $("#tab_imballaggi_formProdottoUC").data("kendoGrid");
            if (gridImb.dataSource.data().length < 1 ||
                (lavCodAccettazione && altriMovimentiStessoCalCod === "" &&
                    raccolteXConferimenti_dsSelezionate.length === 0)) {
                gridImb.addRow();
            } else {
                let msg = "";
                if (altriMovimentiStessoCalCod !== "")
                    msg = TraduzioneMultiResx(resxFormProdottoUC, "ProdUtilizzatoImpossibileInserireAltriImballi",
                        "Questo prodotto è già stato utilizzato, non è possibile inserire altri imballaggi");
                else {
                    if (raccolteXConferimenti_dsSelezionate.length > 0) {
                        msg = TraduzioneMultiResx(resxFormProdottoUC, "SoloUnImballoSeAssociateRaccolte",
                            "Non è possibile aggiungere più righe di imballi se si associano una o più raccolte");
                    }
                    else {
                        msg = TraduzioneMultiResx(resxFormProdottoUC, "SoloUnImballoPerQuestoTipoDiMovimento",
                            "Non è possibile aggiungere più righe di imballi per questo tipo di movimento");
                    }
                }
                $("<div></div>").kendoAlert({
                    title: TraduzioneMultiResx(resxFormProdottoUC, "OperazioneNonConsentita", "Operazione non consentita"),
                    content: msg,
                    actions: [
                        { text: 'OK', action: onOK }
                    ]
                }).data("kendoAlert").open();
            }
        }); 


    function onOK(e) {
        var gridImb = $("#tab_imballaggi_formProdottoUC").data("kendoGrid");
        gridImb.cancelChanges();
    }
}


function ddlParamQual_change(e) {

    let procediAzzeraCalCod = true;

    let idParamQual = $(e.target).data("idparamqual");
    if (idParamQual !== null && idParamQual !== undefined) {

        idParamQual = idParamQual.toString();

        let currentParamQual = paramQual_FF_filtrospevar.find(function (elem) { return elem.Tabella_ID === idParamQual; });

        if (currentParamQual !== undefined) {
            procediAzzeraCalCod = !paramQualSenzaResetCalCod.some(function (elem) { return elem === currentParamQual.Tabella_Cod_Des; });
        }
    }

    if (procediAzzeraCalCod) {
        seAzzeraCalCod();
    }
}


function txtParamQual_change(e) {

    let procediAzzeraCalCod = true;

    let idParamQual = $(e.target).data("idparamqual");
    if (idParamQual !== null && idParamQual !== undefined) {

        idParamQual = idParamQual.toString();

        let currentParamQual = paramQual_FF_filtrospevar.find(function (elem) { return elem.Tabella_ID === idParamQual; });

        if (currentParamQual !== undefined) {
            procediAzzeraCalCod = !paramQualSenzaResetCalCod.some(function (elem) { return elem === currentParamQual.Tabella_Cod_Des; });
        }

        if (lavCodAccettazione) {

            //Controllo Distanza
            if (idParamQual == "69") {
                CalcolaETD_PQ();
            }

        } else {

            var updateGHGtotal = false;
            switch (idParamQual) {
                case "53":
                    //GHG For Ec
                    updateGHGtotal = true;
                    break;
                case "54":
                    //GHG For El
                    updateGHGtotal = true;
                    break;
                case "55":
                    //GHG For Esca
                    updateGHGtotal = true;
                    break;
                case "57":
                    //GHG for Etd
                    updateGHGtotal = true;
                    break;

                default:
                    break;

            }

            if (updateGHGtotal = true) {
                AggiornaGHGTotal();
            }

        }

    }

    if (procediAzzeraCalCod) {
        seAzzeraCalCod();
    }

    if (lavCodAccettazione) {

        //TODO  ora è fisso sul fatto che se modifico il punteggio o il grado tenderometrico ci sia da ricalcolare il calibro
        // occorre modificare per andare in modo dinamico in base a OModuli_Referenze_Config_Dettagli.tabella_key_rif
        
        // In caso di cambio del punteggio o del grado tenderometrico aggiorno il calibro se presente
        let indice = 0;
        if (this.id === "txtpunteggio" || this.id === "txtgradotenderom") {
            if (this.id === "txtpunteggio")
                indice = parseInt($('input[name$="txtpunteggio"]').val());
            else
                if (this.id === "txtgradotenderom")
                    indice = parseInt($('input[name$="txtgradotenderom"]').val());

            if (KendoDDL("ddlcalibro") !== undefined &&
                KendoDDL("ddlcalibro").dataSource !== undefined) {
                for (let i = 0; i < KendoDDL("ddlcalibro").dataSource._data.length; i++) {
                    let elem = KendoDDL("ddlcalibro").dataSource._data[i];
                    if (elem.valore_min <= indice &&
                        (elem.valore_max >= indice || (elem.valore_max === 0 && elem.valore_min !== 0))) {
                        Set_KendoDDLValueNoDef("ddlcalibro", elem.val_cod);
                        break;
                    }
                }
            }
        }
        
    }

}

function AggiornaGHGTotal() {

    if (cIdLavCod === enum_LavCod.DDT_Emesso.value && chiaveETD !== "" && chiaveGHG_Total !== "") {

        var ghgtotal = 0;

        ghgtotal = ghgtotal + kendo.parseFloat($('input[name$="txtghgforec"]').val());
        ghgtotal = ghgtotal + kendo.parseFloat($('input[name$="txtghgforel"]').val());
        ghgtotal = ghgtotal + kendo.parseFloat($('input[name$="txtghgforesca"]').val());
        ghgtotal = ghgtotal + kendo.parseFloat($('input[name$="txtghgforetd"]').val());
        ghgtotal = ghgtotal + kendo.parseFloat($("#" + chiaveETD).val());

        //Aggiornamento GHG Total
        Set_KendoNumTBValue(chiaveGHG_Total, ghgtotal);
    }
}


function changedParamQual(e) {
    // Funzione di change per gli editor della griglia Imballi

    seAzzeraCalCod();

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_imballaggi_formProdottoUC").data("kendoGrid");
    var model = grid.dataItem(e.sender.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    var valoriParamQual = RicercaValoriParametriQualitativi(e.sender.dataItem().val_tabella_cod, $(cIdPiva).val());
    for (var valP = 0; valP < valoriParamQual.length; valP++) {
        // Imballaggio
        if (valoriParamQual[valP].val_tabella_cod === 4) {
            if (model.FF_imballaggio_Tipo_Cod == valoriParamQual[valP].val_cod) {
                model.FF_imballaggio_Tara_Campionatura = valoriParamQual[valP].tara;
                //grid.dataSource.data()[valP].FF_imballaggio_Tara_Campionatura = valoriParamQual[valP].tara;
                model.FF_imballaggio_Codice_Generazione_Link = 0;
                model.FF_imballaggio_Mat_Cod_Generazione_Link = valoriParamQual[valP].mat_cod;
                if (KendoNumTB("FF_imballaggio_Tara_Campionatura") !== undefined)
                    KendoNumTB("FF_imballaggio_Tara_Campionatura").value(valoriParamQual[valP].tara);
                /* se non dobbiamo calcolare i contenitori partendo da un nr di cont * imballo o le confezioni partendo 
                 * da un nr di confez * contenitore non serve
                aggiornaQtaMovimento(e, "tab_imballaggi_formProdottoUC", "tipoImballo", model.FF_imballaggio_Tara_Campionatura);
                */
                break;
            }
        }

        // Contenitore
        if (valoriParamQual[valP].val_tabella_cod === 8) {
            if (model.FF_contenitore_Tipo_Cod == valoriParamQual[valP].val_cod) {
                model.FF_contenitore_Tara_Campionatura = valoriParamQual[valP].tara;
                //grid.dataSource.data()[valP].FF_contenitore_Tara_Campionatura = valoriParamQual[valP].tara;
                model.FF_contenitore_Codice_Generazione_Link = 0;
                model.FF_contenitore_Mat_Cod_Generazione_Link = valoriParamQual[valP].mat_cod;
                if (KendoNumTB("FF_contenitore_Tara_Campionatura") !== undefined)
                    KendoNumTB("FF_contenitore_Tara_Campionatura").value(valoriParamQual[valP].tara);
                /* se non dobbiamo calcolare i contenitori partendo da un nr di cont * imballo o le confezioni partendo
                 * da un nr di confez * contenitore non serve
                 * aggiornaQtaMovimento(e, "tab_imballaggi_formProdottoUC", "tipoContenitore", model.FF_contenitore_Tara_Campionatura);
                 */
                break;
            }
        }

        // Confezione
        if (valoriParamQual[valP].val_tabella_cod === 5) {
            if (model.FF_confezione_Tipo_Cod == valoriParamQual[valP].val_cod) {
                model.FF_confezione_Tara_Campionatura = valoriParamQual[valP].tara;
                //grid.dataSource.data()[valP].FF_confezione_Tara_Campionatura = valoriParamQual[valP].tara;
                model.FF_confezione_Codice_Generazione_Link = 0;
                model.FF_confezione_Mat_Cod_Generazione_Link = valoriParamQual[valP].mat_cod;
                if (KendoNumTB("FF_confezione_Tara_Campionatura") !== undefined)
                    KendoNumTB("FF_confezione_Tara_Campionatura").value(valoriParamQual[valP].tara);
                /* se non dobbiamo calcolare i contenitori partendo da un nr di cont * imballo o le confezioni partendo
                 * da un nr di confez * contenitore non serve
                 * aggiornaQtaMovimento(e, "tab_imballaggi_formProdottoUC", "tipoConfezione", model.FF_confezione_Tara_Campionatura);
                 */
                break;
            }
        }

    }
    
    kendo_AggiustaDimensioneColonne("#tab_imballaggi_formProdottoUC");
}

function onEdit_ImballiFormProdottoUC(e) {

    sonoInModificaImballi = true;

    let enableModConfezioni = true;
    let kddlConfezioni = KendoDDL("FF_confezione_Tipo_Cod");
    if (kddlConfezioni !== undefined) {

        let kddlUM = KendoDDL("ddlUM");
        if (kddlUM !== undefined) {

            let kntbConfezioni = KendoNumTB("NrConfezioni");
            let kntbConfTara = KendoNumTB("FF_confezione_Tara_Campionatura");
            if (parseInt(kddlUM.value()) === enum_Udm.numero.value) {

                enableModConfezioni = false;

                kddlConfezioni.enable(false);
                kddlConfezioni.value(0);
                kddlConfezioni.trigger("change");

                kntbConfezioni.enable(false);
                kntbConfezioni.value(0);
                kntbConfezioni.trigger("change");

                // La tara potrebbe essere bloccata in caso sia presa direttamente dall'anagrafica imballo o dalla giacenza,
                // in questo caso imposto a zero la tara nel dataItem della riga
                if (kntbConfTara !== undefined) {
                    kntbConfTara.enable(false);
                    kntbConfTara.value(0);
                    kntbConfTara.trigger("change");
                }
                else {
                    e.model.set("FF_confezione_Tara_Campionatura", 0);
                }

            }
            else {
                enableModConfezioni = true;

                kddlConfezioni.enable(true);
                kntbConfezioni.enable(true);

                if (kntbConfTara !== undefined) {
                    kntbConfTara.enable(true);
                }
            }
        }
    }

    if (gestionePesiRiscontrati) {
        let enableModRisc = VerificaValorizzazionePesiRiscontrati();

        if (gestitoImballaggio_FF) {
            KendoNumTB("Num_Imballi_Riscontrati").enable(enableModRisc);
            KendoNumTB("Tara_Unit_Imballo_Riscontrata").enable(enableModRisc);
        }

        if (gestitoContenitore_FF) {
            KendoNumTB("Num_Colli_Riscontrati").enable(enableModRisc);
            KendoNumTB("Tara_Unit_Collo_Riscontrata").enable(enableModRisc);
        }

        if (gestitoConfezione_FF) {
            let kntbConfRisc = KendoNumTB("Num_Conf_Riscontrate");
            let kntbTaraConfRisc = KendoNumTB("Tara_Unit_Conf_Riscontrata");

            if (enableModConfezioni) {
                kntbConfRisc.enable(enableModRisc);
                kntbTaraConfRisc.enable(enableModRisc);
            }
            else {
                kntbConfRisc.enable(false);
                kntbConfRisc.value(0);

                kntbTaraConfRisc.enable(false);
                kntbTaraConfRisc.value(0);
            }
        }
    }

    // Gestione della duplicazione di una riga
    //      Vengono copiati solo i valori delle colonne marcate come "Da duplicare"
    //      Questa funzione viaggia in coppia con la funzione Duplica
    if (rigaDuplicata_ImballiFormProdottoUCKendoGrid && rigaDaCopiare_ImballiFormProdottoUCKendoGrid != null && e.model.isNew() && !e.model.dirty) {

        var gridId = e.sender.element[0].id;
        var grid = $("#" + gridId).data("kendoGrid");

        rigaDaCopiare_ImballiFormProdottoUCKendoGrid.forEach(function (valore, campo) {
            //console.log('[' + campo + '] ' + valore);
            var duplicazioneEffettuata = false;
            for (let r = 0; r < grid.columns.length; r++) {
                let col = grid.columns[r];
                if (col.daDuplicare && col.field !== undefined && col.field === campo) {
                    if (e.model.get(campo) !== valore) {
                        e.container.find("input[name=" + campo + "]").val(valore).change();
                        e.model.set(campo, valore);
                    }
                    duplicazioneEffettuata = true;
                }
            }

            if (!duplicazioneEffettuata) {
                // Se non ho trovato il campo fra le colonne della griglia cerco se è una DropDownList (i campi 
                // codice in questo caso non fanno parte delle colonne quindi non li trova)
                if (e.container.find("input[name=" + campo + "]").data("kendoDropDownList") !== undefined) {
                    var keyCampo = e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.dataValueField;
                    var textCampo = null;

                    // nomeCampoEffettivo è un campo valorizzato solo per i parametri qualitativi
                    if (e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.nomeCampoEffettivo !== undefined)
                        textCampo = e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.nomeCampoEffettivo;
                    else
                        textCampo = e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.dataTextField;

                    if (textCampo != null) {
                        for (let r = 0; r < grid.columns.length; r++) {
                            let col = grid.columns[r];
                            if (col.daDuplicare && col.field !== undefined && col.field === textCampo) {
                                if (e.model.get(campo) !== valore) {
                                    e.container.find("input[name=" + keyCampo + "]").val(valore).change();
                                    e.model.set(campo, valore);
                                }
                                duplicazioneEffettuata = true;
                            }
                        }
                    }
                }
            }

            if (!duplicazioneEffettuata) {
                //Gestione dei campi che non voglio duplicare per evitare che vengano impostati con i default
                for (var f in grid.dataSource.options.schema.model.fields) {
                    if (grid.dataSource.options.schema.model.fields.hasOwnProperty(f) &&
                        f === campo &&
                        grid.dataSource.options.schema.model.fields[f].defaultValue !== undefined) {
                        //console.log(f + " -> " + grid.dataSource.options.schema.model.fields[f]);
                        if (grid.dataSource.options.schema.model.fields[f].type === "string") {
                            e.container.find("input[name=" + f + "]").val("").change();
                            e.model.set(f, "");
                        }
                        if (grid.dataSource.options.schema.model.fields[f].type === "date") {
                            e.container.find("input[name=" + f + "]").val("").change();
                            e.model.set(f, "");
                        }
                        if (grid.dataSource.options.schema.model.fields[f].type === "number") {
                            e.container.find("input[name=" + f + "]").val(0).change();
                            e.model.set(f, 0);
                        }
                        if (grid.dataSource.options.schema.model.fields[f].type === "boolean") {
                            e.container.find("input[name=" + f + "]").val(false).change();
                            e.model.set(f, false);
                        }
                    }
                }
            }
        });

        rigaDuplicata_ImballiFormProdottoUCKendoGrid = false;
        rigaDaCopiare_ImballiFormProdottoUCKendoGrid = null;

    }

    var input = e.container.find(".k-input");
    var value = input.val();
    input.keyup(function () {
        value = input.val();
    });
    
}

function onRemove_ImballiFormProdottoUC(e) {

    seAzzeraCalCod();

}

function onCancel_ImballiFormProdottoUC(e) {
    // Ripristino la griglia come era in precedenza perchè non avendola effettivamente 
    // salvata all'annulla modifiche perderebbe tutte le righe
    var grid = $("#tab_imballaggi_formProdottoUC").data("kendoGrid");
    grid.refresh();

    sonoInModificaImballi = false;
}

function onSave_ImballiFormProdottoUC(e) {
    var x = 0;
    for (x = 0; x < elencoImballaggi.length; x++) {
        if (elencoImballaggi[x].val_cod === e.model.FF_imballaggio_Tipo_Cod) {
            e.model.FF_imballaggio_Descrizione = elencoImballaggi[x].val_des;
            e.model.FF_imballaggio_Sigla = elencoImballaggi[x].val_sigla;
            break;
        }
    }

    for (x = 0; x < elencoContenitori.length; x++) {
        if (elencoContenitori[x].val_cod === e.model.FF_contenitore_Tipo_Cod) {
            e.model.FF_contenitore_Descrizione = elencoContenitori[x].val_des;
            e.model.FF_contenitore_Sigla = elencoContenitori[x].val_sigla;
            break;
        }
    }

    for (x = 0; x < elencoConfezioni.length; x++) {
        if (elencoConfezioni[x].val_cod === e.model.FF_confezione_Tipo_Cod) {
            e.model.FF_confezione_Descrizione = elencoConfezioni[x].val_des;
            e.model.FF_confezione_Sigla = elencoConfezioni[x].val_sigla;
            break;
        }
    } 

    sonoInModificaImballi = false; 
}

function duplicaRiga_ImballiFormProdottoUCKendoGrid(e) {

    var grid = $("#tab_imballaggi_formProdottoUC").data("kendoGrid");
    var row = $(e.target).closest("tr");

    // Qui non posso testarlo perché il salvataggio avviene con un unico pulsante globale
    // e quindi dopo la prima riga avrei sempre hasChange true
    //var hasChanges = grid.dataSource.hasChanges();
    
    //if (!hasChanges) {

    if (lavCodAccettazione || grid.dataSource.data().length < 1) {

            e.preventDefault();

            rigaDaCopiare_ImballiFormProdottoUCKendoGrid = grid.dataItem(row);
            rigaDuplicata_ImballiFormProdottoUCKendoGrid = true;
            grid.addRow();

        } else {
            // Righe imballi multiple ammesse solo in accettazione prodotto F&F
            $("<div></div>").kendoAlert({
                title: TraduzioneMultiResx(resxFormProdottoUC, "OperazioneNonConsentita", "Operazione non consentita"),
                content: TraduzioneMultiResx(resxFormProdottoUC, "SoloUnImballoPerQuestoTipoDiMovimento",
                    "Non è possibile aggiungere altre righe imballi per questo tipo di movimento")
            }).data("kendoAlert").open();
        }
        

    //}
    //else {
    //    alert("Sono presenti righe non salvate: procedere prima con il salvataggio");
    //}
}

function SubmitAggiorna_ImballiFormProdottoUC(options) {

    var grid = $("#tab_imballaggi_formProdottoUC").data("kendoGrid");
    var allOk = true;
    var currentData = grid.dataSource.data();

    var righeDaControllareImballi = [];
    var tutteLeRigheImballi = [];
    for (var d = 0; d < currentData.length; d++) {
        tutteLeRigheImballi.push(currentData[d].toJSON());
        if (currentData[d].isNew())
            righeDaControllareImballi.push(currentData[d].toJSON());
        else if (currentData[d].dirty)
            righeDaControllareImballi.push(currentData[d].toJSON());
    }

    // controllo che tutte le righe CREATE e MODIFICATE siano complete e coerenti
    var errMessage = controlla_ImballiFormProdottoUC(tutteLeRigheImballi, righeDaControllareImballi, "carico");

    if (errMessage !== "") {
        $("<div></div>").kendoAlert({
            title: TraduzioneMultiResx(resxFormProdottoUC, "OperazioneNonConsentita", "Operazione non consentita"),
            content: errMessage
        }).data("kendoAlert").open();
    } else {

        // Non ci sono errori, procedo con aggiornamenti

        var updatedRecords = [];
        var newRecords = [];
        var deletedRecords = [];

        var data = $('input[name$="hdKendo_Imballi_formProdottoUC"]').val();
        var jSonParsed_Kendo = JSON.parse(data);

        for (let c = 0; c < grid.dataSource._destroyed.length; c++) {

            // Locate item in original datasource and remove it.
            let indexRow = getIndexById(jSonParsed_Kendo.kendo_rows, grid.dataSource._destroyed[c].key_mov_dett);
            jSonParsed_Kendo.kendo_rows.splice(indexRow, 1);
            $('input[name$="hdKendo_Imballi_formProdottoUC"]').val(JSON.stringify(jSonParsed_Kendo));
            // On success.
 

            //console.log(data);
            //////options.success(jSonParsed_Kendo.kendo_rows);
        } 

        for (let d = 0; d < currentData.length; d++) {
            if (currentData[d].isNew()) {
                currentData[d].key_mov_dett = currentData[d].uid;
                jSonParsed_Kendo.kendo_rows.push(currentData[d].toJSON());
                $('input[name$="hdKendo_Imballi_formProdottoUC"]').val(JSON.stringify(jSonParsed_Kendo));     
            } else if (currentData[d].dirty) {
                let indexRow = getIndexById(jSonParsed_Kendo.kendo_rows, currentData[d].key_mov_dett);
                jSonParsed_Kendo.kendo_rows.splice(indexRow, 1, currentData[d].toJSON());
                $('input[name$="hdKendo_Imballi_formProdottoUC"]').val(JSON.stringify(jSonParsed_Kendo));
            }
        }

        let tare_da_griglia = CalcolaTaraTotaleImballi();
        Set_KendoNumTBValue("idTara", tare_da_griglia.taraTotale);

        let tara = tare_da_griglia.taraTotale;
        let udm = parseInt(Get_KendoDDLValue("ddlUM"));

        switch (udm) {
            case enum_Udm.quintali.value:
                tara = tara / 100;
                break;
            case enum_Udm.tonnellate.value:
                tara = tara / 1000;
        }
        // TODO Valutare se cambiare idKgNetti usando il change di idTara piuttosto che ripetere questo codice
        Set_KendoNumTBValue("idKgNetti", kendo.parseFloat(Get_KendoNumTBValue("idKgLordi")) - tara);
        if (lavCodAccettazione === true && isContattoImpresaGias === true)
            cmbRipartizione_change();

        calcolaDegrado_FormProdottoUC(Get_KendoNumTBValue("idKgNetti"), Get_KendoNumTBValue("idDegradoPerc"));

        if (gestionePesiRiscontrati) {
            let ntbTaraRiscontrata = KendoNumTB("idTaraRiscontrata");
            ntbTaraRiscontrata.value(tare_da_griglia.taraTotaleRiscontrata);
            ntbTaraRiscontrata.trigger("change");
        }

        if (allOk) {
            options.success();
            grid.dataSource._destroyed = [];
            grid.refresh();

            AggiornaDettagliEconomici();
            // popola_ImballiFormProdottoUC("tab_imballaggi_formProdottoUC");
            //grid.dataSource.read();
        }
    }
}

// Legge tutte le righe della griglia imballi e calcola la tara totale
function CalcolaTaraTotaleImballi() {

    let objTara = { taraTotale: 0, taraTotaleRiscontrata: null };

    var grid = $("#tab_imballaggi_formProdottoUC").data("kendoGrid");

    if (grid !== undefined) {
        var currentData = grid.dataSource.data();

        for (let d = 0; d < currentData.length; d++) {
            if (gestitoImballaggio_FF) {
                objTara.taraTotale += currentData[d].FF_imballaggio_Tara_Campionatura * currentData[d].NrImballaggi;
                if (currentData[d].Tara_Unit_Imballo_Riscontrata !== null) {
                    objTara.taraTotaleRiscontrata += currentData[d].Tara_Unit_Imballo_Riscontrata * currentData[d].Num_Imballi_Riscontrati;
                }
            }

            if (gestitoContenitore_FF) {
                objTara.taraTotale += currentData[d].FF_contenitore_Tara_Campionatura * currentData[d].NrContenitori;
                if (currentData[d].Tara_Unit_Collo_Riscontrata !== null) {
                    objTara.taraTotaleRiscontrata += currentData[d].Tara_Unit_Collo_Riscontrata * currentData[d].Num_Colli_Riscontrati;
                }
            }

            if (gestitoConfezione_FF) {
                objTara.taraTotale += currentData[d].FF_confezione_Tara_Campionatura * currentData[d].NrConfezioni;
                if (currentData[d].Tara_Unit_Conf_Riscontrata !== null) {
                    objTara.taraTotaleRiscontrata += currentData[d].Tara_Unit_Conf_Riscontrata * currentData[d].Num_Conf_Riscontrate;
                }
            }
        }
    }

    return objTara;
}

function getIndexById(kendo_rows, id) {
    var idx,
        l = kendo_rows.length;

    for (var j = 0; j < l; j++) {
        if (kendo_rows[j].key_mov_dett == id) {
            return j;
        }
    }
    return null;
}

function controlla_ImballiFormProdottoUC(tutteLeRigheImballi, righeDaControllare, tipoOper) {

    // tipoOper può valere "carico" o "scarico"

    var errMessage = "";
    var x = 0;

    // Inizio controllo coerenza fra campi digitati
    for (x = 0; x < righeDaControllare.length; x++) {
       
        let item = righeDaControllare[x];

        if (item.NrImballaggi === null ||
            item.NrImballaggi === undefined)
            item.NrImballaggi = 0;
        if (item.NrContenitori === null ||
            item.NrContenitori === undefined)
            item.NrContenitori = 0;
        if (item.NrConfezioni === null ||
            item.NrConfezioni === undefined)
            item.NrConfezioni = 0;
        if (item.FF_imballaggio_Tara_Campionatura === null ||
            item.FF_imballaggio_Tara_Campionatura === undefined)
            item.FF_imballaggio_Tara_Campionatura = 0;
        if (item.FF_contenitore_Tara_Campionatura === null ||
            item.FF_contenitore_Tara_Campionatura === undefined)
            item.FF_contenitore_Tara_Campionatura = 0;
        if (item.FF_confezione_Tara_Campionatura === null ||
            item.FF_confezione_Tara_Campionatura === undefined)
            item.FF_confezione_Tara_Campionatura = 0;

        // Imballaggio
        if ((item.NrImballaggi !== 0 && (item.FF_imballaggio_Tipo_Cod === undefined || item.FF_imballaggio_Tipo_Cod === 0)) ||
            (item.NrImballaggi === 0 && item.FF_imballaggio_Tipo_Cod !== undefined && item.FF_imballaggio_Tipo_Cod !== 0)
            //&& tipoOper === "carico"
        ) {
            errMessage += TraduzioneMultiResx(resxFormProdottoUC, "TipoENumeroImballaggioDevonoEssereValorizzati", "Tipo e nr imballaggio devono essere entrambi valorizzati") + "<br/>";
        }

        if (FF_gest_materiale_vivaistico) {
            if (item.FF_imballaggio_Tara_Campionatura !== 0 && (item.FF_imballaggio_Tipo_Cod === undefined || item.FF_imballaggio_Tipo_Cod === 0)) {
                errMessage += TraduzioneMultiResx(resxFormProdottoUC, "IndicareTaraETipoImballaggio", "Non è possibile indicare la tara senza indicare il tipo imballaggio") + "<br/>";
            }
        } else {
            if ((item.FF_imballaggio_Tara_Campionatura !== 0 && (item.FF_imballaggio_Tipo_Cod === undefined || item.FF_imballaggio_Tipo_Cod === 0)) ||
                (item.FF_imballaggio_Tara_Campionatura === 0 && item.FF_imballaggio_Tipo_Cod !== undefined && item.FF_imballaggio_Tipo_Cod !== 0)
                //&& tipoOper === "carico"
            ) {
                errMessage += TraduzioneMultiResx(resxFormProdottoUC, "TipoETaraImballaggioDevonoEssereValorizzati", "Tipo e tara imballaggio devono essere entrambi valorizzati") + "<br/>";
            }
        }
        

        // Contenitore
        if ((item.NrContenitori !== 0 && (item.FF_contenitore_Tipo_Cod === undefined || item.FF_contenitore_Tipo_Cod === 0)) ||
            (item.NrContenitori === 0 && item.FF_contenitore_Tipo_Cod !== undefined && item.FF_contenitore_Tipo_Cod !== 0)
            //&& tipoOper === "carico"
        ) {
            errMessage += TraduzioneMultiResx(resxFormProdottoUC, "TipoContenitoreENumeroDevonoEssereValorizzati", "Tipo contenitore e nr devono essere entrambi valorizzati") + "<br/>";
        }

        if (FF_gest_materiale_vivaistico) {
            if (item.FF_contenitore_Tara_Campionatura !== 0 && (item.FF_contenitore_Tipo_Cod === undefined || item.FF_contenitore_Tipo_Cod === 0)) {
                errMessage += TraduzioneMultiResx(resxFormProdottoUC, "IndicareTaraETipoContenitore", "Non è possibile indicare la tara senza indicare il tipo contenitore") + "<br/>";
            }
        } else {
            if ((item.FF_contenitore_Tara_Campionatura !== 0 && (item.FF_contenitore_Tipo_Cod === undefined || item.FF_contenitore_Tipo_Cod === 0)) ||
                (item.FF_contenitore_Tara_Campionatura === 0 && item.FF_contenitore_Tipo_Cod !== undefined && item.FF_contenitore_Tipo_Cod !== 0)
                //&& tipoOper === "carico"
            ) {
                errMessage += TraduzioneMultiResx(resxFormProdottoUC, "TipoETaraContenitoreDevonoEssereValorizzati", "Tipo e tara contenitore devono essere entrambi valorizzati") + "<br/>";
            }
        }

        // Confezione
        if ((item.NrConfezioni !== 0 && (item.FF_confezione_Tipo_Cod === undefined || item.FF_confezione_Tipo_Cod === 0)) ||
            (item.NrConfezioni === 0 && item.FF_confezione_Tipo_Cod !== undefined && item.FF_confezione_Tipo_Cod !== 0)
            //&& tipoOper === "carico"
        ) {
            errMessage += TraduzioneMultiResx(resxFormProdottoUC, "TipoConfezioneENumeroDevonoEssereValorizzati", "Tipo confezione e nr devono essere entrambi valorizzati") + "<br/>";
        }
        //if ((item.FF_confezione_Tara_Campionatura != 0 && (item.FF_confezione_Tipo_Cod === undefined || item.FF_confezione_Tipo_Cod === 0)) ||
        //    (item.FF_confezione_Tara_Campionatura === 0 && item.FF_confezione_Tipo_Cod !== undefined && item.FF_confezione_Tipo_Cod !== 0)
        //&& tipoOper === "carico"
        //)
        //{
        //    errMessage += "Tipo e tara confezione devono essere entrambi valorizzati<br/>";
        //}

        if (item.NrImballaggi !== 0 &&
            item.NrContenitori !== 0 &&
            item.NrImballaggi > item.NrContenitori ) {
            errMessage += TraduzioneMultiResx(resxFormProdottoUC, "ContenitoriInferioriImballaggi", "Il nr di contenitori non può essere minore del nr di imballaggi") + "<br/>";
        }

        if (item.NrContenitori !== 0 &&
            item.NrConfezioni !== 0 &&
            item.NrContenitori > item.NrConfezioni) {
            errMessage += TraduzioneMultiResx(resxFormProdottoUC, "ConfezioniInferioriContenitori", "Il nr di confezioni non può essere minore del nr di contenitori") + "<br/>";
        }
    }
    // Fine controllo coerenza fra campi digitati

    if (errMessage === "") {

        // Inizio Controllo che su tutte le righe siano stati scelti gli stessi tipi di imballaggi
        let wImballiGestitiPrimaRiga = "";
        let wImballiGestitiAltraRiga = "";

        for (x = 0; x < tutteLeRigheImballi.length; x++) {

            let item = tutteLeRigheImballi[x];

            if (x === 0) {
                if (item.NrImballaggi != 0)
                    wImballiGestitiPrimaRiga = TraduzioneMultiResx(resxFormProdottoUC, "UnImballaggio", "un Imballaggio");
                if (item.NrContenitori != 0)
                    wImballiGestitiPrimaRiga = TraduzioneMultiResx(resxFormProdottoUC, "UnContenitore", "un Contenitore");
                if (item.NrConfezioni != 0)
                    wImballiGestitiPrimaRiga = TraduzioneMultiResx(resxFormProdottoUC, "UnaConfezione", "una Confezione");
            } else {
                wImballiGestitiAltraRiga = "";
                if (item.NrImballaggi != 0)
                    wImballiGestitiAltraRiga = TraduzioneMultiResx(resxFormProdottoUC, "UnImballaggio", "un Imballaggio");
                if (item.NrContenitori != 0)
                    wImballiGestitiAltraRiga = TraduzioneMultiResx(resxFormProdottoUC, "UnContenitore", "un Contenitore");
                if (item.NrConfezioni != 0)
                    wImballiGestitiAltraRiga = TraduzioneMultiResx(resxFormProdottoUC, "UnaConfezione", "una Confezione");
            }
        }

        if (wImballiGestitiPrimaRiga !== wImballiGestitiAltraRiga && wImballiGestitiAltraRiga !== "") {
            
            errMessage += kendo.format(TraduzioneMultiResx(resxFormProdottoUC, "TutteLeRigheDevonoAvereGliStessiTipiImballi",
                "Tutte le righe devono avere gli stessi tipi di imballi<br/>La riga appena inserita contiene {0} mentre la riga precedente contiene {1}<br />"),
                wImballiGestitiPrimaRiga, wImballiGestitiAltraRiga);
        }
        // Fine Controllo che su tutte le righe siano stati scelti gli stessi tipi di imballaggi

        // Inizio Controllo che se ci sono più righe il valore di riferimento sia impostato a prezzo
        var ddlValoreRiferimento = KendoDDL("ddlValoreRiferimento");
        if (ddlValoreRiferimento.value() !== "0" && tutteLeRigheImballi.length > 1) {
            errMessage += TraduzioneMultiResx(resxFormProdottoUC, "CalcoliConPrezzoUnitarioSeMultipleRigheImballi",
                "Se si inseriscono più righe di imballi i calcoli devono partire dal Prezzo Unitario") + "<br/>";
        }
        // Fine Controllo che se ci sono più righe il valore di riferimento sia impostato a prezzo
    }
    

    return errMessage;
}


function popola_SceltaDaGiacenza_FormProdottoUC(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
     
    var funzioniCRUD = {
        funzioneRead: kRead_SceltaDaGiacenza_FormProdottoUC_rows,
        funzioneInsert: null,
        funzioneUpdate: null,
        funzioneDelete: null,
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };

    var idModel = "Cal_Cod";
    // TODO U.M.
    var campiKendoModel = {
        Cal_Cod: { editable: false, type: "number" },
        Elem_Cod: { editable: false, type: "number" },
        Mat_Cod: { editable: false, type: "number" },
        Sa_Cod: { editable: false, type: "number" },
        Id_Destinazione: { editable: false, type: "number" },
        Udm_Cod: { editable: false, type: "number" },
        Udm_Cod_Extra: { editable: false, type: "number" },
        Udm_Des: { editable: false, type: "string" },
        Lotto: { editable: false, type: "string" },
        Cod_Progetto: { editable: false, type: "number" },
        Lotto_Int: { editable: false, type: "string" },
        KgLordi: { editable: false, type: "number" },
        TaraTotale: { editable: false, type: "number" },
        KgNetti: { editable: false, type: "number" },
        NrConfezioni: { editable: false, type: "number" } 
         
        //Flag_Distinta_Chiusa: { editable: false, type: "boolean" }
    };

    var colonneKendoGrid = [
        { field: "Lotto", title: TraduzioneMultiResx(resxFormProdottoUC, "LottoProdotto", "Lotto Prodotto"), filterable: { multi: true, search: true }, width: "125px" },
        { field: "Udm_Des", title: TraduzioneMultiResx(resxObj, "UnitàDiMisuraAbbr", "U.M."), filterable: { multi: true, search: true }, width: "125px" },
        { field: "Lotto_Int", title: TraduzioneMultiResx(resxObj, "LottoEsercizio", "Lotto Esercizio"), filterable: { multi: true, search: true }, width: "125px" },
        //, 
        //{ field: "Validita_Fine_Distinta", title: "Fine", format: "{0:dd/MM/yyyy}" },
        //{ field: "Superficie", title: "Ha", format: "{0:n4}" }
    ];

    if (paramQual_FF_filtrospevar !== null) {

        // TODO U.M.
        if (FF_gest_materiale_vivaistico) {
            colonneKendoGrid.push({ field: "KgNetti", title: TraduzioneMultiResx(resxFormProdottoUC, "Numero", "Numero"), format: "{0:n0}", width: "125px" });
        } else {
            colonneKendoGrid.push({ field: "NrConfezioni", title: TraduzioneMultiResx(resxFormProdottoUC, "NrConfezioni", "Nr Confezioni"), width: "125px" });
            colonneKendoGrid.push({ field: "KgLordi", title: TraduzioneMultiResx(resxFormProdottoUC, "KgLordi", "Kg Lordi"), format: `{0:${enum_Udm.chilogrammi.format}}`, width: "125px" });
            colonneKendoGrid.push({ field: "TaraTotale", title: TraduzioneMultiResx(resxFormProdottoUC, "Tara", "Tara"), format: `{0:${enum_Udm.chilogrammi.format}}`, width: "125px" });
            colonneKendoGrid.push({ field: "KgNetti", title: TraduzioneMultiResx(resxFormProdottoUC, "KgNetti", "Kg Netti"), format: `{0:${enum_Udm.chilogrammi.format}}`, width: "125px" });
        }

        // Aggiungo dinamicamente i parametri qualitativi
        for (let x = 0; x < paramQual_FF_filtrospevar.length; x++) {
            if (parseInt(paramQual_FF_filtrospevar[x].Tabella_ID) !== 0) {
                let w_tab = paramQual_FF_filtrospevar[x].Tabella_Cod_Des;
                campiKendoModel["FF_" + w_tab + "_Tabella_ID"] = { editable: false, type: "string" };
                campiKendoModel["FF_" + w_tab + "_Tipo"] = { editable: false, type: "number" };

                if (paramQual_FF_filtrospevar[x].Tipo === 1) {
                    campiKendoModel["FF_" + w_tab + "_Tipo_Cod"] = { editable: false, type: "number" };
                    campiKendoModel["FF_" + w_tab + "_Sigla"] = { editable: false, type: "string" };
                    campiKendoModel["FF_" + w_tab + "_Descrizione"] = { editable: false, type: "string" };
                    colonneKendoGrid.push({ field: "FF_" + w_tab + "_Sigla", title: paramQual_FF_filtrospevar[x].Tabella_Des, width: "125px" });
                    if (w_tab === "imballaggio") {
                        campiKendoModel["NrImballaggi"] = { editable: false, type: "number" };
                        colonneKendoGrid.push({ field: "NrImballaggi", title: TraduzioneMultiResx(resxFormProdottoUC, "NrImballaggi", "Nr Imballaggi"), width: "125px" });
                    } else if (w_tab === "contenitore") {
                        campiKendoModel["NrContenitori"] = { editable: false, type: "number" };
                        colonneKendoGrid.push({ field: "NrContenitori", title: TraduzioneMultiResx(resxFormProdottoUC, "NrContenitori", "Nr Contenitori"), width: "125px" });
                    }
                }

                if (paramQual_FF_filtrospevar[x].Tipo === 3) {
                    campiKendoModel["FF_" + w_tab + "_Val_Cod"] = { editable: false, type: "number" };
                    var numDecParam = parseInt(paramQual_FF_filtrospevar[x].NumDecimali_Maximo);
                    var formatParam = `{0:0.${"#".repeat(numDecParam)}}`;
                    colonneKendoGrid.push({ field: "FF_" + w_tab + "_Val_Cod", title: paramQual_FF_filtrospevar[x].Tabella_Des, format: formatParam, width: "125px" });
                }

                if (paramQual_FF_filtrospevar[x].Tipo === 4) {
                    campiKendoModel["FF_" + w_tab + "_Val_Cod"] = { editable: false, type: "string" };
                    colonneKendoGrid.push({ field: "FF_" + w_tab + "_Val_Cod", title: paramQual_FF_filtrospevar[x].Tabella_Des, width: "125px" });
                }

                if (paramQual_FF_filtrospevar[x].Tipo === 5) {
                    campiKendoModel["FF_" + w_tab + "_Val_Cod"] = { editable: false, type: "date" };
                    colonneKendoGrid.push({ field: "FF_" + w_tab + "_Val_Cod", title: paramQual_FF_filtrospevar[x].Tabella_Des, format: "{0:dd/MM/yyyy}", width: "125px" });
                }

            }
        }
    }
    else {
        campiKendoModel["Giacenza"] = { editable: false, type: "number" };
        colonneKendoGrid.push({ field: "Giacenza", title: "Giacenza", width: "125px"});
    }


    var parametriPerLettura = null;
    var parametriDataSource = {
        pagesize: 10
        //////aggregate: [
        //////    { field: "NrImballaggi", aggregate: "sum" },
        //////    { field: "NrContenitori", aggregate: "sum" },
        //////    { field: "NrConfezioni", aggregate: "sum" }
        //////]
    };


    var parametriKendoGrid = {
        selectable: "row",
        excel: false, pdf: false, groupable: false, pageable: true, btnEliminaTuttiFiltri: false,
        columnMenu: true,
        reorderable: true,
        scrollable: true
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoChange: rigaGiacenza_FormProdottoUC_Selezionata };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );

    var grid = $("#" + IDControllo).data("kendoGrid"); 
    $("#" + IDControllo + " .k-grid-toolbar").hide();
}

function kRead_SceltaDaGiacenza_FormProdottoUC_rows(options) {

    var data = $('input[name$="hdKendo_SceltaDaGiacenza_FormProdottoUC"]').val();

    var jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    if (jSonParsed_Kendo.righe_dettaglio !== undefined &&
        jSonParsed_Kendo.righe_dettaglio.length > 0) {
        options.success(jSonParsed_Kendo.righe_dettaglio);
    } else {
        options.success([]);
    }
}

function rigaGiacenza_FormProdottoUC_Selezionata() {

    var gridGiacenza_FormProdottoUC = $("#tab_grid_scelta_da_giacenza_formProdottoUC").data("kendoGrid");
    var selectedItem = gridGiacenza_FormProdottoUC.dataItem(gridGiacenza_FormProdottoUC.select());

    let gestitoImballaggioSpeVar = false;
    let gestitoContenitoreSpeVar = false;
    let gestitoConfezioneSpeVar = false;
    if (paramQual_FF_filtrospevar !== null) {
        for (let ipar = 0; ipar < paramQual_FF_filtrospevar.length; ipar++) {
            if (paramQual_FF_filtrospevar[ipar].Tabella_ID !== 0) {
                if (paramQual_FF_filtrospevar[ipar].Tabella_ID === "4")
                    gestitoImballaggioSpeVar = true;
                if (paramQual_FF_filtrospevar[ipar].Tabella_ID === "8")
                    gestitoContenitoreSpeVar = true;
                if (paramQual_FF_filtrospevar[ipar].Tabella_ID === "5")
                    gestitoConfezioneSpeVar = true;
            }
        }
    }

    // Imposto il Cal_Cod che poi verrà modificato in caso di cambio di un parametro qualitativo
    $('input[name$="hf_Cal_Cod"]').val(selectedItem.Cal_Cod);
    $('input[name$="hf_Cod_Progetto"]').val(selectedItem.Cod_Progetto);

    if (selectedItem !== undefined && selectedItem !== null) {

        let umDaImpostare = selectedItem.Udm_Cod;

        if (is_Trasf_Veg_Anim_FormProdottoUC()) {

            if (gestitoConfezioneSpeVar && selectedItem.FF_confezione_Tipo_Cod !== 0) {
                // prodotto in confezioni
                // in questo caso l'unità di misura è numero, ma devo impostare i kg perché così è espressa la giacenza
                umDaImpostare = enum_Udm.chilogrammi.value;

                // Tramite questa "impostazione" la funzione di change della um, richiamata da impostaDftValueDdlUM, non azzera il lotto e non ricarica la giacenza,
                // da fare perché in questo caso il lotto è coerente con la giacenza anche se l'unità di misura principale è diversa
                $("#ddlUM").data("conservalottoconfezioniff", true);

                let kddlUm = KendoDDL("ddlUM");

                let hasUmKg = kddlUm.dataSource.data().some(function (dataItem) { return parseInt(dataItem.Udm_Cod) === umDaImpostare });
                if (!hasUmKg) {
                    kddlUm.dataSource.add({
                        Udm_Cod: enum_Udm.chilogrammi.value,
                        Udm_Des: enum_Udm.chilogrammi.name
                    });
                }
            }

            if (umDaImpostare === enum_Udm.chilogrammi.value) {
                Set_KendoNumTBValue("idKgLordi", selectedItem.KgLordi);
                Set_KendoNumTBValue("idKgNetti", selectedItem.KgNetti);
                Set_KendoNumTBValue("idKgLordiRiscontrati", null);
                Set_KendoNumTBValue("idKgNettiRiscontrati", null);
                Set_KendoNumTBValue("idTaraRiscontrata", null);
                Set_KendoNumTBValue("idTara", selectedItem.KgLordi - selectedItem.KgNetti);
            }
            else {
                Set_KendoNumTBValue("idQuantita", umDaImpostare === enum_Udm.numero.value ? selectedItem.NrConfezioni : selectedItem.KgNetti);
                Set_KendoNumTBValue("idTara", selectedItem.TaraTotale);
            }

        }
        else {
            Set_KendoNumTBValue("idQuantita", selectedItem.Giacenza);
            Set_KendoNumTBValue("idTara", selectedItem.TaraTotale);
        }

        // U.M.
        impostaDftValueDdlUM(umDaImpostare);

        // Se il lotto non è impostato lo prendo dalla griglia
        if (Get_KendoDDLValue("ddlLottoAccettazione") === "" &&
            selectedItem.Lotto !== "")
            Set_KendoDDLValue("ddlLottoAccettazione", selectedItem.Lotto);

        $("#tab_grid_scelta_da_giacenza_formProdottoUC").hide();
        $("#titolo_grid_scelta_da_giacenza").hide();
        $("#btn_scelta_da_giacenza_formProdottoUC").show();

        // Imballaggi
        // Prima carico gli imballi senza righe valorizzate passando la sola Piva
        Ricerca_ImballiFormProdottoUC($(cIdPiva).val(), 0, 0);

        // Poi mi prendo i dati che devono essere vuoti e costruisco io la riga
        let data = $('input[name$="hdKendo_Imballi_formProdottoUC"]').val();
        let jSonParsed_Kendo = JSON.parse(data);
        let rigaImb = jSonParsed_Kendo.kendo_rows;
        if (rigaImb !== undefined) {
            rigaImb = [];
            let obj = {};
            obj.key_mov_dett = "";
            obj.DataOraUltimaLettura = "";
            obj.key_Dest = "";
            //obj.key_Dest: "16_132775937_132775938"  TODO
            //Ubic_Des: "Cisterna Celle"   TODO

            let creaGrigliaImballaggi = false;
            if (selectedItem.FF_imballaggio_Tipo_Cod !== undefined && selectedItem.FF_imballaggio_Tipo_Cod !== 0) {
                creaGrigliaImballaggi = true;

                let imbScelto = [];

                if (Array.isArray(elencoImballaggi)) {
                    imbScelto = elencoImballaggi.filter(function (x) { return (x.val_cod === selectedItem.FF_imballaggio_Tipo_Cod); });
                }

                obj.FF_imballaggio_Tipo_Cod = selectedItem.FF_imballaggio_Tipo_Cod;
                obj.FF_imballaggio_Descrizione = selectedItem.FF_imballaggio_Descrizione;
                obj.FF_imballaggio_Sigla = selectedItem.FF_imballaggio_Sigla;
                obj.FF_imballaggio_Codice_Generazione_Link = 0;
                if (imbScelto.length === 1)
                    obj.FF_imballaggio_Mat_Cod_Generazione_Link = imbScelto[0].mat_cod;
                else
                    obj.FF_imballaggio_Mat_Cod_Generazione_Link = 0;
                obj.NrImballaggi = selectedItem.NrImballaggi;
                obj.FF_imballaggio_Tara_Campionatura = selectedItem.FF_imballaggio_Tara_Campionatura;
                obj.Num_Imballi_Riscontrati = null;
                obj.Tara_Unit_Imballo_Riscontrata = null;
            } else {
                if (gestitoImballaggioSpeVar) {
                    let gestitoContenitoreSpeVar = false;
                    let gestitoConfezioneSpeVar = false;
                    obj.FF_imballaggio_Tipo_Cod = 0;
                    obj.FF_imballaggio_Descrizione = "";
                    obj.FF_imballaggio_Sigla = "";
                    obj.FF_imballaggio_Codice_Generazione_Link = 0;
                    obj.FF_imballaggio_Mat_Cod_Generazione_Link = 0;
                    obj.NrImballaggi = 0;
                    obj.FF_imballaggio_Tara_Campionatura = 0;
                    obj.Num_Imballi_Riscontrati = null;
                    obj.Tara_Unit_Imballo_Riscontrata = null;
                }
            }
            if (selectedItem.FF_contenitore_Tipo_Cod !== undefined && selectedItem.FF_contenitore_Tipo_Cod !== 0) {
                creaGrigliaImballaggi = true;

                let contScelto = [];

                if (Array.isArray(elencoContenitori)) {
                    contScelto = elencoContenitori.filter(function (x) { return (x.val_cod === selectedItem.FF_contenitore_Tipo_Cod); });
                }

                obj.FF_contenitore_Tipo_Cod = selectedItem.FF_contenitore_Tipo_Cod;
                obj.FF_contenitore_Descrizione = selectedItem.FF_contenitore_Descrizione;
                obj.FF_contenitore_Sigla = selectedItem.FF_contenitore_Sigla;
                obj.FF_contenitore_Codice_Generazione_Link = 0; 
                if (contScelto.length === 1)
                    obj.FF_contenitore_Mat_Cod_Generazione_Link = contScelto[0].mat_cod;
                else
                    obj.FF_contenitore_Mat_Cod_Generazione_Link = 0;
                obj.NrContenitori = selectedItem.NrContenitori;
                obj.FF_contenitore_Tara_Campionatura = selectedItem.FF_contenitore_Tara_Campionatura;
                obj.Num_Colli_Riscontrati = null;
                obj.Tara_Unit_Collo_Riscontrata = null;
            } else {
                if (gestitoContenitoreSpeVar) {
                    obj.FF_contenitore_Tipo_Cod = 0;
                    obj.FF_contenitore_Descrizione = "";
                    obj.FF_contenitore_Sigla = "";
                    obj.FF_contenitore_Codice_Generazione_Link = 0;
                    obj.FF_contenitore_Mat_Cod_Generazione_Link = 0;
                    obj.NrContenitori = 0;
                    obj.FF_contenitore_Tara_Campionatura = 0;
                    obj.Num_Colli_Riscontrati = null;
                    obj.Tara_Unit_Collo_Riscontrata = null;
                }
            }
            if (selectedItem.FF_confezione_Tipo_Cod !== undefined && selectedItem.FF_confezione_Tipo_Cod !== 0) {
                creaGrigliaImballaggi = true;

                let confezScelta = [];

                if (Array.isArray(elencoContenitori)) {
                    confezScelta = elencoContenitori.filter(function (x) { return (x.val_cod === selectedItem.FF_confezione_Tipo_Cod); });
                }

                obj.FF_confezione_Tipo_Cod = selectedItem.FF_confezione_Tipo_Cod;
                obj.FF_confezione_Descrizione = selectedItem.FF_confezione_Descrizione;
                obj.FF_confezione_Sigla = selectedItem.FF_confezione_Sigla;
                obj.FF_confezione_Codice_Generazione_Link = 0; 
                if (confezScelta.length === 1)
                    obj.FF_confezione_Mat_Cod_Generazione_Link = confezScelta[0].mat_cod;
                else
                    obj.FF_confezione_Mat_Cod_Generazione_Link = 0;
                obj.NrConfezioni = selectedItem.NrConfezioni;
                obj.FF_confezione_Tara_Campionatura = selectedItem.FF_confezione_Tara_Campionatura;
                obj.Num_Conf_Riscontrate = null;
                obj.Tara_Unit_Conf_Riscontrata = null;
            } else {
                if (gestitoConfezioneSpeVar) {
                    obj.FF_confezione_Tipo_Cod = 0;
                    obj.FF_confezione_Descrizione = "";
                    obj.FF_confezione_Sigla = "";
                    obj.FF_confezione_Codice_Generazione_Link = 0;
                    obj.FF_confezione_Mat_Cod_Generazione_Link = 0;
                    obj.NrConfezioni = 0;
                    obj.FF_confezione_Tara_Campionatura = 0;
                    obj.Num_Conf_Riscontrate = null;
                    obj.Tara_Unit_Conf_Riscontrata = null;
                }
            }
            
            if (creaGrigliaImballaggi) {
                rigaImb.push(obj);
            } else {
                rigaImb = [];
            }

            //TODO: da rivedere perchè non sono sicura che sia corretto così...
            // se rigaImb diventa = [] poi si impianta nel popola_imballi a causa delle colonne locked
            // però il ricaricamento sarebbe cmq da far fare, immagino, se la riga scelta precedentemente aveva almeno un confezionamento
            // e poi ne scelgo una che invece non ha nulla (quindi rigaImb == [])

            //se non sono gestiti i confezionamenti mi ritrovo senza colonne
            if (jSonParsed_Kendo.kendo_columns.length > 0) {
                // se l'array è vuoto non posso lasciargli ricaricare la griglia perché va in errore per la presenza di colonne locked
                jSonParsed_Kendo.kendo_rows = rigaImb;
                $('input[name$="hdKendo_Imballi_formProdottoUC"]').val(kendo.stringify(jSonParsed_Kendo));
                popola_ImballiFormProdottoUC("tab_imballaggi_formProdottoUC");
            }

        }


        // Parametri qualitativi
        ImpostaParametriQualitativi(selectedItem);

        // Parametri Indici GHG
        ImpostaParametriIndiciGHG(selectedItem);

        // Aggiornamento GHG Totale
        AggiornaGHGTotal();

    }
}

function ImpostaParametriQualitativi(dataItem) {
    // In modifica quando arrivo qui paramQual_FF_filtrospevar deve essere già stato caricato perchè il prodotto è già stato letto
    if (dataItem !== null && paramQual_FF_filtrospevar !== null && paramQual_FF_filtrospevar.length !== 0) {

        for (let ipar = 0; ipar < paramQual_FF_filtrospevar.length; ipar++) {
            const paramQual = paramQual_FF_filtrospevar[ipar];

            if (paramQual.Tabella_ID !== 0) {
                if (paramQual.Tabella_ID !== "4" &&
                    paramQual.Tabella_ID !== "5" &&
                    paramQual.Tabella_ID !== "8") {

                    if (dataItem !== null) {
                        for (let campo in dataItem) {
                            if (campo === "FF_" + paramQual.Tabella_Cod_Des + "_Bool" && paramQual.Tipo === 1) {
                                Set_KendoDDLValue_From_Sigla("ddl" + paramQual.Tabella_Cod_Des, dataItem[campo] ? "Yes" : "No", 0);
                            }
                            if (campo === "FF_" + paramQual.Tabella_Cod_Des + "_Tipo_Cod" &&
                                paramQual.Tipo === 1) {
                                Set_KendoDDLValue("ddl" + paramQual.Tabella_Cod_Des, dataItem[campo]);
                            }
                            if (campo === "FF_" + paramQual.Tabella_Cod_Des + "_Val_Cod" &&
                                paramQual.Tipo === 3) {
                                Set_KendoNumTBValue("txt" + paramQual.Tabella_Cod_Des, dataItem[campo]);
                            }
                            if (campo === "FF_" + paramQual.Tabella_Cod_Des + "_Val_Cod" &&
                                paramQual.Tipo === 4) {
                                let nomeCampoStr = "txtStr" + paramQual.Tabella_Cod_Des;
                                $('input[name$=' + nomeCampoStr + ']').val(dataItem[campo]);
                            }
                            if (campo === "FF_" + paramQual.Tabella_Cod_Des + "_Val_Cod" &&
                                paramQual.Tipo === 5) {
                                set_data("date" + paramQual.Tabella_Cod_Des, dataItem[campo], null);
                            }
                        }
                    } else {
                        if (paramQual.Tipo === 1)
                            Set_KendoDDLValue("ddl" + paramQual.Tabella_Cod_Des, 0);
                    }
                }
            }
        }

        //Inizializzazione Distanza = Testata
        if (lavCodAccettazione == true) {
            for (let ipar2 = 0; ipar2 < paramQual_FF_filtrospevar.length; ipar2++) {
                const paramQual = paramQual_FF_filtrospevar[ipar2];

                if (paramQual.Tabella_Cod_Des === "kmdistance") {
                    var valore = Get_KendoNumTBValue("ntbDistanzaTrasporto", true);

                    //Controllo Impostazione Miglia
                    if (Get_KendoDDLValue("ddlUnitaMisuraTrasporto", 0) === "306") {
                        valore = valore * 1.60934;
                    }
                    Set_KendoNumTBValue("txt" + paramQual.Tabella_Cod_Des, valore);
                    CalcolaETD_PQ();
                }
            }
        }

    }
}



function ImpostaParametriIndiciGHG(dataItem) {
    // In modifica quando arrivo qui paramQual_FF_indici_GHG deve essere già stato caricato perché il prodotto è già stato letto
    if (dataItem.length > 0 && paramQual_FF_indici_GHG !== null && paramQual_FF_indici_GHG.length !== 0) {
        for (let ipar = 0; ipar < paramQual_FF_indici_GHG.length; ipar++) {
            if (paramQual_FF_indici_GHG[ipar].TipoIndice == "GHG") {
               
                let titoloindice = paramQual_FF_indici_GHG[ipar].TitoloIndice;
                let nomecampo = paramQual_FF_indici_GHG[ipar].Nome_Campo.toUpperCase();
                let id_indice = paramQual_FF_indici_GHG[ipar].ID_Indice;
                let tipocampo = paramQual_FF_indici_GHG[ipar].TipoCampo;
                let chiave2 = paramQual_FF_indici_GHG[ipar].TipoIndice + "_" + tipocampo + "_" + id_indice;
                let valore = dataItem[0][paramQual_FF_indici_GHG[ipar].Nome_Campo];

                let tipo_param = "";
                
                switch (nomecampo) {

                    case "UDM_TRASPORTO_ATTUALE":

                        //Già impostato al momento della creazione
                        break;
                    case "ETD_TRASPORTO_ATTUALE":

                        //Già impostato al momento della creazione
                        break;

                    case "QTY_TRASPORTO_ATTUALE":

                        //Già impostato al momento della creazione
                        break;


                    default:

                        switch (tipocampo) {

                            case 0: //Valore Libero

                                /*libera imputazione  */
                                switch (paramQual_FF_indici_GHG[ipar].TipoDato) {

                                    case "numeric":

                                        Set_KendoNumTBValue("txt" + chiave2, valore);
                                        break;

                                    case "boolean":

                                        var Valore_Boolean = valore === "true";

                                        setKendoSwitch("chk" + chiave2, Valore_Boolean);
                                        break;

                                    case "date":

                                        //Controllo agrodatainizio
                                        if (kendo.parseDate(valore) !== null) {
                                            if (kendo.parseDate(valore).getFullYear() !== 1900) {
                                                set_data("txt" + chiave2, valore, null);
                                            }
                                        }
                                        
                                        break;

                                    default:

                                        $("#txt" + chiave2).val(valore);
                                        break;

                                }

                                break;

                            case 1: //Valori Dettagli

                                Set_KendoDDLValue("ddl" + chiave2, valore);
                                break;

                            case 2: //Elenco

                                Set_KendoDDLValue("ddl" + chiave2, valore);
                                break;
                        }

                }

            }
        }

        AggiornaGHGTotal();

    }
}



function GrigliaImballiImpostaRiscontrati() {
    let kGridImballi = KendoGrid("tab_imballaggi_formProdottoUC");
    if (kGridImballi !== undefined) {

        for (let datarow of kGridImballi.dataSource.data()) {

            if (gestitoImballaggio_FF) {
                datarow.Num_Imballi_Riscontrati = datarow.NrImballaggi;
                datarow.Tara_Unit_Imballo_Riscontrata = datarow.FF_imballaggio_Tara_Campionatura;

                datarow.dirty = true;
                datarow.dirtyFields.Num_Imballi_Riscontrati = true;
                datarow.dirtyFields.Tara_Unit_Imballo_Riscontrata = true;
            }

            if (gestitoContenitore_FF) {
                datarow.Num_Colli_Riscontrati = datarow.NrContenitori;
                datarow.Tara_Unit_Collo_Riscontrata = datarow.FF_contenitore_Tara_Campionatura;

                datarow.dirty = true;
                datarow.dirtyFields.Num_Colli_Riscontrati = true;
                datarow.dirtyFields.Tara_Unit_Collo_Riscontrata = true;
            }

            if (gestitoConfezione_FF) {
                datarow.Num_Conf_Riscontrate = datarow.NrConfezioni;
                datarow.Tara_Unit_Conf_Riscontrata = datarow.FF_confezione_Tara_Campionatura;

                datarow.dirty = true;
                datarow.dirtyFields.Num_Conf_Riscontrate = true;
                datarow.dirtyFields.Tara_Unit_Conf_Riscontrata = true;
            }

        }

        kGridImballi.refresh();

    }
}

function GrigliaImballiResettaRiscontrati() {
    let kGridImballi = KendoGrid("tab_imballaggi_formProdottoUC");
    if (kGridImballi !== undefined) {

        for (let datarow of kGridImballi.dataSource.data()) {

            if (gestitoImballaggio_FF) {
                datarow.Num_Imballi_Riscontrati = null;
                datarow.Tara_Unit_Imballo_Riscontrata = null;

                datarow.dirtyFields.Num_Imballi_Riscontrati = false;
                datarow.dirtyFields.Tara_Unit_Imballo_Riscontrata = false;
            }

            if (gestitoContenitore_FF) {
                datarow.Num_Colli_Riscontrati = null;
                datarow.Tara_Unit_Collo_Riscontrata = null;

                datarow.dirtyFields.Num_Colli_Riscontrati = false;
                datarow.dirtyFields.Tara_Unit_Collo_Riscontrata = false;
            }

            if (gestitoConfezione_FF) {
                datarow.Num_Conf_Riscontrate = null;
                datarow.Tara_Unit_Conf_Riscontrata = null;

                datarow.dirtyFields.Num_Conf_Riscontrate = false;
                datarow.dirtyFields.Tara_Unit_Conf_Riscontrata = false;
            }

        }

        kGridImballi.refresh();

    }
}
