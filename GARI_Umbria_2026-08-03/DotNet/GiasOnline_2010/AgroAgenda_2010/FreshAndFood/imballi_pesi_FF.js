
function calcolaQuantitaInCascata(
    piva, MatCod, ImballoCod, ContenitoreCod, ConfezioneCod,
    NrImbMask, NrContenitoriMask, NrConfezioniMask, KgLordiMask, KgNettiMask,
    TaraUnitImballiMask, TaraUnitContenitoriMask, TaraUnitConfezioniMask,
    NrImbTotRiferimento, NrContenitoriTotRiferimento, NrConfezioniTotRiferimento, KgLordiTotRiferimento, KgNettiTotRiferimento,
    CampoModificato, CercaPesoPesoContenutoPerImballoProdotto) {

    // N.B.  Il campo CercaPesoPesoContenutoPerImballoProdotto deve essere passato a false se si vuole ottenere il peso come media della 
    //       giacenza (quindi tipicamente negli scarichi)

    // In base alla specie/varietà del prodotto cerco il peso contenuto per l'imballo più vicino
    var pesoContenutoPerImballo = 0;

    //if (CampoModificato == "prodotto" ||
    //    CampoModificato == "tipoImballo" ||
    //    CampoModificato == "tipoContenitore" ||
    //    CampoModificato == "tipoConfezione" ||
    //    CampoModificato == "NrImballaggi" ||
    //    CampoModificato == "NrContenitori" ||
    //    CampoModificato == "NrConfezioni")
    if (CampoModificato != "KgLordi" && CercaPesoPesoContenutoPerImballoProdotto)
    {

        if (ConfezioneCod != 0) {
            pesoContenutoPerImballo = trovaPesoContenutoPerImballo(piva, 5, ConfezioneCod, MatCod);
        }
        else
            if (ContenitoreCod != 0) {
                pesoContenutoPerImballo = trovaPesoContenutoPerImballo(piva, 8, ContenitoreCod, MatCod);
            }
            else
                if (ImballoCod != 0) {
                    pesoContenutoPerImballo = trovaPesoContenutoPerImballo(piva, 4, ImballoCod, MatCod);
                }
    }

    // Per campi con , come separatore migliaia
    KgLordiMask = kendo.parseFloat(KgLordiMask);
    KgNettiMask = kendo.parseFloat(KgNettiMask);
    TaraUnitImballiMask = kendo.parseFloat(TaraUnitImballiMask);
    TaraUnitContenitoriMask = kendo.parseFloat(TaraUnitContenitoriMask);
    TaraUnitConfezioniMask = kendo.parseFloat(TaraUnitConfezioniMask);
    KgLordiTotRiferimento = kendo.parseFloat(KgLordiTotRiferimento);
    KgNettiTotRiferimento = kendo.parseFloat(KgNettiTotRiferimento);
    
    if (CampoModificato != "NrImballaggi" &&
        CampoModificato != "NrContenitori" &&
        CampoModificato != "NrConfezioni" &&
        CampoModificato != "KgLordi" &&
        CampoModificato != "KgNetti" &&
        CampoModificato != "prodotto" &&
        CampoModificato != "tipoImballo" &&
        CampoModificato != "tipoContenitore" &&
        CampoModificato != "tipoConfezione" &&
        CampoModificato != "taraImballo" &&
        CampoModificato != "taraContenitore" &&
        CampoModificato != "taraConfezione")
    {
        alert("Mi hai passato il CampoModificato errato: " + CampoModificato);
        return;
    }

    // http://jsbin.com/exageg/6/edit?html,js,output

    var contenitoriPerImballo = 0.0;
    var confezioniPerImballo = 0.0;
    var confezioniPerContenitore = 0.0;
    var kgLordiPerImballo = 0.0;
    var kgLordiPerContenitore = 0.0;
    var kgLordiPerConfezione = 0.0;
    var kgNettiPerImballo = 0.0;
    var kgNettiPerContenitore = 0.0;
    var kgNettiPerConfezione = 0.0;

    // Calcolo le medie in base ai campi passati di riferimento (potrebbero ad es.
    // essere la giacenza oppure le quantità di maschera)
    if (NrImbTotRiferimento > 0 && ImballoCod != 0)
    {
        contenitoriPerImballo = kendo.parseFloat(NrContenitoriTotRiferimento / NrImbTotRiferimento);
        confezioniPerImballo = kendo.parseFloat(NrConfezioniTotRiferimento / NrImbTotRiferimento);
        kgLordiPerImballo = kendo.parseFloat(KgLordiTotRiferimento / NrImbTotRiferimento);
        kgNettiPerImballo = kendo.parseFloat(KgNettiTotRiferimento / NrImbTotRiferimento);
    }

    if (NrContenitoriTotRiferimento > 0 && ContenitoreCod != 0) {
        confezioniPerContenitore = kendo.parseFloat(NrConfezioniTotRiferimento / NrContenitoriTotRiferimento);
        kgLordiPerContenitore = kendo.parseFloat(KgLordiTotRiferimento / NrContenitoriTotRiferimento);
        kgNettiPerContenitore = kendo.parseFloat(KgNettiTotRiferimento / NrContenitoriTotRiferimento);
    }

    if (NrConfezioniTotRiferimento > 0 && ConfezioneCod != 0) {
        kgLordiPerConfezione = kendo.parseFloat(KgLordiTotRiferimento / NrConfezioniTotRiferimento);
        kgNettiPerConfezione = kendo.parseFloat(KgNettiTotRiferimento / NrConfezioniTotRiferimento);
    }

    if (CampoModificato == "NrImballaggi" && NrImbMask > 0) {

        if (ImballoCod == 0)
        {
            NrImbMask = 0;
           // return;
        }
        else
        {
            // Aggiorno contenitori / confezioni 
            // Aggiorno lordo e netto, poi alla fine uno dei due verrà ricalcolato in base alle tare
            if (contenitoriPerImballo > 0 && ContenitoreCod != 0)
                {
                    NrContenitoriMask = kendo.parseInt(contenitoriPerImballo * NrImbMask);
                    NrConfezioniMask = kendo.parseInt(confezioniPerContenitore * NrContenitoriMask);
                    KgLordiMask = kendo.parseFloat(kgLordiPerContenitore * NrContenitoriMask);
                    KgNettiMask = kendo.parseFloat(kgNettiPerContenitore * NrContenitoriMask);
                }
                else
                if (confezioniPerImballo > 0 && ConfezioneCod != 0) {
                        NrContenitoriMask = 0;
                        NrConfezioniMask = kendo.parseInt(confezioniPerImballo * NrImbMask);
                        KgLordiMask = kendo.parseFloat(kgLordiPerConfezione * NrConfezioniMask);
                        KgNettiMask = kendo.parseFloat(kgNettiPerConfezione * NrConfezioniMask);
                    }
                    else
                        {
                            NrContenitoriMask = 0;
                            NrConfezioniMask = 0;
                            if (ImballoCod != 0) {
                                KgLordiMask = kendo.parseFloat(kgLordiPerImballo * NrImbMask);
                                KgNettiMask = kendo.parseFloat(kgNettiPerImballo * NrImbMask);
                            }
                        }
            }
    }

    if (CampoModificato == "NrContenitori" && NrContenitoriMask > 0) {

        if (ContenitoreCod == 0) {
            NrContenitoriMask = 0;
          //  return;
        }
        else {

            // Se nr imballi mask = 0 ma è presente anagrafica imballo ...
            if (NrImbMask == 0 && ImballoCod != 0) {

                // se ho i contenitori per imballo, calcolo nr imballi arrotondati all'intero maggiore oppure lo imposto a 1
                if (contenitoriPerImballo > 0 && ContenitoreCod != 0) {
                    NrImballiMask = Math.ceil(contenitoriPerImballo * NrContenitoriMask);
                }
                else
                {
                    NrImballiMask = 1;
                }
            }

            // Aggiorno confezioni 
            // Aggiorno lordo e netto, poi alla fine uno dei due verrà ricalcolato in base alle tare
            if (confezioniPerContenitore > 0 && ConfezioneCod != 0 && ContenitoreCod != 0) {
                NrConfezioniMask = kendo.parseInt(confezioniPerContenitore * NrContenitoriMask);
                KgLordiMask = kendo.parseFloat(kgLordiPerConfezione * NrConfezioniMask);
                KgNettiMask = kendo.parseFloat(kgNettiPerConfezione * NrConfezioniMask);
            }
            else
                if (confezioniPerImballo > 0 && ConfezioneCod != 0 && ImballoCod != 0) {
                    NrConfezioniMask = kendo.parseInt(confezioniPerImballo * NrContenitoriMask);
                    KgLordiMask = kendo.parseFloat(kgLordiPerConfezione * NrConfezioniMask);
                    KgNettiMask = kendo.parseFloat(kgNettiPerConfezione * NrConfezioniMask);
                }
                else
                {
                    NrConfezioniMask = 0;
                    if (ContenitoreCod != 0) {
                        KgLordiMask = kendo.parseFloat(kgLordiPerContenitore * NrContenitoriMask);
                        KgNettiMask = kendo.parseFloat(kgNettiPerContenitore * NrContenitoriMask);
                    }
                }
        }
    }

    if (CampoModificato == "NrConfezioni" && NrConfezioniMask > 0) {

        if (ConfezioneCod == 0) {
            NrConfezioniMask = 0;
         //   return;
        }
        else {
            // Se nr contenitori mask = 0 ma è presente anagrafica contenitore ...
            if (NrContenitoriMask == 0 && ContenitoreCod != 0) {

                // se ho le confezioni per contenitore, calcolo nr contenitori arrotondati all'intero maggiore oppure lo imposto a 1
                if (confezioniPerContenitore > 0 && ContenitoreCod != 0) {
                    NrContenitoriMask = Math.ceil(confezioniPerContenitore * NrConfezioniMask);
                }
                //else {
                //        NrContenitoriMask = 1;
                //     }
            }

            // Se nr imballi mask = 0 ma è presente anagrafica imballo ...
            if (NrImbMask == 0 && ImballoCod != 0) {

                // se ho le confezioni oppure i contenitori per imballo, calcolo nr imballi arrotondati all'intero maggiore oppure lo imposto a 1
                if (contenitoriPerImballo > 0 && ContenitoreCod != 0) {
                    NrImballiMask = Math.ceil(contenitoriPerImballo * NrContenitoriMask);
                }
                else
                    if (confezioniPerImballo > 0 && ConfezioneCod != 0) {
                        NrImballiMask = Math.ceil(confezioniPerImballo * NrConfezioniMask);
                    }
                    else {
                        NrImballiMask = 1;
                    }
            }

            // Aggiorno lordo e netto, poi alla fine uno dei due verrà ricalcolato in base alle tare
            KgLordiMask = kendo.parseFloat(kgLordiPerConfezione * NrConfezioniMask);
            KgNettiMask = kendo.parseFloat(kgNettiPerConfezione * NrConfezioniMask);
        }
    }

    // Calcolo la tara totale
    var totaleTara = 0.0;

    if (ImballoCod != 0)
        totaleTara += kendo.parseFloat(NrImbMask * TaraUnitImballiMask);

    if (ContenitoreCod != 0)
        totaleTara += kendo.parseFloat(NrContenitoriMask * TaraUnitContenitoriMask);

    if (ConfezioneCod != 0)
        totaleTara += kendo.parseFloat(NrConfezioniMask * TaraUnitConfezioniMask);

    // Ricalcolo sempre il netto a partire dagli altri campi a meno che non si sia modificato il campo netto
    // (SOSPESO oppure che la tara non sia a zero)
    // In caso di cambio di taraImballo, taraContenitore o taraConfezione (vale anche per cambio di 
    // tipo imballo, tipo contenitore, tipo confezione) viene verificato se è presente un peso contenuto
    // per imballaggio / specie / varietà; se sì il netto viene determinato dal nr imballaggi * valore trovato
    
    if (CampoModificato != "KgLordi" &&
        //(CampoModificato == "prodotto" ||
        // CampoModificato == "tipoImballo" ||
        // CampoModificato == "tipoContenitore" ||
        //CampoModificato == "tipoConfezione" ||
        //CampoModificato == "NrImballaggi" ||
        //CampoModificato == "NrContenitori" ||
        //CampoModificato == "NrConfezioni") &&
            pesoContenutoPerImballo != 0) {

        if (ConfezioneCod != 0)
            KgNettiMask = kendo.parseFloat(NrConfezioniMask * pesoContenutoPerImballo);
        else
            if (ContenitoreCod != 0)
                KgNettiMask = kendo.parseFloat(NrContenitoriMask * pesoContenutoPerImballo);
            else
                if (ImballoCod != 0)
                    KgNettiMask = kendo.parseFloat(NrImbMask * pesoContenutoPerImballo);

        CampoModificato = "KgNetti";
    }

    if (CampoModificato === "KgLordi"
        //&& totaleTara > 0
    )
    {
        KgNettiMask = kendo.parseFloat(KgLordiMask - totaleTara);
    }
    else
    {
        KgLordiMask = kendo.parseFloat(KgNettiMask + totaleTara);
    } 

    var risultato = {
        NrImbMask: NrImbMask,
        NrContenitoriMask: NrContenitoriMask,
        NrConfezioniMask: NrConfezioniMask,
        KgLordiMask: KgLordiMask,
        KgNettiMask: KgNettiMask,
        KgTaraMask: kendo.parseFloat(KgLordiMask - KgNettiMask)
    };

    return risultato;
}

function trovaPesoContenutoPerImballo(piva, tipoImballo, imballo_cod, Mat_Cod) {

    // imballo_cod passato qui è il mat_cod di materie_prime corrispondente a imballo / contenitore / confezione
    // che corrisponde a mat_cod_generazione_link di otabelle_parametri

    var valore = 0;

    var specie = 0;
    var varieta = 0;
    var found = false;
    var prodotti = RicercaProdotti_FF(false, piva, 0, 210, false, false, true);
    for (let i = 0; i < prodotti.length; i++) {
        if (prodotti[i].Mat_Cod == Mat_Cod) {
            specie = prodotti[i].Veg_Cod;
            varieta = prodotti[i].Cul_Cod;
            found = true;
            break;
        }
    }

    if (found) {

        found = false;
        var configImballiProdotto = RicercaConfigImballiProdotto(piva);

        if (configImballiProdotto !== null && configImballiProdotto.length > 0) {

            // Primo tentativo con varietà
            for (let i = 0; i < configImballiProdotto.length; i++) {
                let cip = configImballiProdotto[i];
                if (cip.Tabella_Cod == tipoImballo &&
                    cip.Mat_Cod == imballo_cod &&
                    cip.Veg_Cod == specie &&
                    cip.Cul_Cod !== null &&
                    cip.Cul_Cod == varieta) {
                    found = true;
                    valore = cip.Valore;
                    break;
                }
            }

            if (!found) {
                for (let i = 0; i < configImballiProdotto.length; i++) {
                    let cip = configImballiProdotto[i];
                    if (cip.Tabella_Cod === tipoImballo &&
                        cip.Mat_Cod === imballo_cod &&
                        cip.Veg_Cod === specie &&
                        cip.Cul_Cod === 0) {
                        found = true;
                        valore = cip.Valore;
                        break;
                    }
                }
            }
        }
    }

    // Se non trovo alcun peso contenuto per imballaggio cerco su anagrafica imballaggio
    if (!found) {
        var valoriParamQual = RicercaValoriParametriQualitativi(tipoImballo, $(cIdPiva).val());
        for (var x = 0; x < valoriParamQual.length; x++) {
            if (valoriParamQual[x].mat_cod === imballo_cod) {
                valore = valoriParamQual[x].qta_extra;
                break;
            }
        }
    }
    
    return valore;
}

/**
 * Modalità udm usata per la movimentazione
 * @readonly
 * @enum {string}
 */
var ENUM_MOD_UDM = {
    /** Sono nella vecchia situazione: movimento a kg + conf + eventuali contenitori e/o imballi (pur avendo movimentato a Kg, su db viene salvato a Nr) */
    KG_CONF: "KG_Conf",
    /** Sono nella vecchia situazione: movimento a kg + NO conf + eventuali contenitori e/o imballi */
    KG: "KG",
    /** Ho movimentato a numero o altre Udm */
    ALTRA_UDM: "Altra_Udm"
}

/**
 * Restituisce la modalità di udm usata
 * @param {any} dataItem riga o altro elemento che contenga e proprietà Udm_Cod e (volendo) FF_confezione_Tipo_Cod
 * @returns {ENUM_MOD_UDM} modalità di udm usata
 */
function getModalitaUdm(dataItem) {
    let confezioneCod = 0;
    if (dataItem.FF_confezione_Tipo_Cod !== undefined &&
        dataItem.FF_confezione_Tipo_Cod !== null &&
        dataItem.FF_confezione_Tipo_Cod !== 0) {
        confezioneCod = dataItem.FF_confezione_Tipo_Cod;
    }

    if (confezioneCod !== 0) {
        //Sono nella vecchia situazione: movimento a kg + conf + eventuali contenitori e/o imballi
        //pur avendo movimentato a Kg, su db viene salvato a Nr
        return ENUM_MOD_UDM.KG_CONF;

    } else if (parseInt(dataItem.Udm_Cod) === 2) {
        //Sono nella vecchia situazione: movimento a kg + NO conf + eventuali contenitori e/o imballi
        return ENUM_MOD_UDM.KG;

    } else {
        //Ho movimentato a numero o altre Udm
        return ENUM_MOD_UDM.ALTRA_UDM;
    }
}


/**
 * Blocca/sblocca i controlli (NumericTextBox e DropDownList), come da array indicato
 * @param {HTMLElement[]} inputs array di DomElement (creato con find) dei controlli
 * @param {string[]} arrayControlliDaSbloccare array dei controlli che vanno abilitati (gli altri saranno disabilitati)
 * @param {string[]} [arrayControlliDaBloccare=null] opzionale = se presente, vengono bloccati esplicitamente solo questi, in alternativa blocca tutto quanto non compreso nell'array da sbloccare
 */
function SbloccaControlliPesi(inputs, arrayControlliDaSbloccare, arrayControlliDaBloccare) {

    for (let i = 0; i < inputs.length; i++) {

        let nameC = inputs[i].name;

        let inputNTB = KendoNumTB(nameC);
        let inputDDL = KendoDDL(nameC);
        let input = undefined;

        if (inputNTB != undefined) {
            //console.log("NumericTextBox", inputNTB);
            input = inputNTB;
        } else if (inputDDL != undefined) {
            //console.log("DDL", inputDDL);
            input = inputDDL;
        }

        if (input != undefined) {

            if (arrayControlliDaSbloccare.includes(nameC)) {
                input.enable(true);
            } else {

                if (arrayControlliDaBloccare !== null &&
                    arrayControlliDaBloccare !== undefined) {

                    if (arrayControlliDaBloccare.includes(nameC)) {
                        input.enable(false);
                    }
                } else {
                    input.enable(false);
                }

            }
        }
    }
}
