// #region API User Control

/**
 * Inizializza o rilegge la griglia delle raccolte collegabili
 * @param {string} piva Impresa di Campagna a cui è intestata la raccolta
 * @param {Date} dataMov Data del conferimento, viene usata per mostrare le raccolte effettuate nel periodo temporale
 * @param {any} idMovDetConf Se specificato, estrae anche le raccolte collegate alla specifica riga di conferimento
 */
function raccolteConfUC_set(piva, dataMov, idMovDetConf) {
    let numRaccolte = 0;
    if (raccolteConfUC_isInit === false) {
        if (piva !== undefined && piva !== null && dataMov !== undefined && dataMov !== null) {
            // Il parametro della partita iva è obbligatorio in inizializzazione
            raccolteConfUC_isInit = true;
            raccolteConfUC_azioniPostLettura = true;

            if (resxObj !== null && resxObj !== undefined) {
                resxRaccolteConfUC = resxObj;
                resxRaccolteConfUC.unshift(readResxFile("GestioneContabilita/App_LocalResources/FormProdottoUC.ascx.resx"));
            }
            else {
                resxRaccolteConfUC.push(readResxFile("GestioneContabilita/App_LocalResources/FormProdottoUC.ascx.resx"));
                resxRaccolteConfUC.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx"));
            }

            raccolteConfUC_piva = piva;
            raccolteConfUC_dataMov = dataMov;
            if (idMovDetConf !== undefined) {
                raccolteConfUC_idMovDetConf = idMovDetConf;
            }

            raccolteConfUC_impProponiPeso = null;

            $("#raccolteConfUC_container").show();

            //$("#raccolteConfUC_btnAssocia").on("click", raccolteConfUC_btnAssociaClick);

            raccolteConfUC_popolaGrigliaRaccolte();
            numRaccolte = KendoGrid("raccolteConfUC_boxGrigliaRaccolte").dataSource.data().length;
        }
    }
    else {
        if (piva !== undefined && piva !== null && dataMov !== undefined && dataMov !== null) {
            // Rilettura da db
            raccolteConfUC_azioniPostLettura = true;

            raccolteConfUC_piva = piva;
            raccolteConfUC_dataMov = dataMov;
            if (idMovDetConf !== undefined) {
                raccolteConfUC_idMovDetConf = idMovDetConf;
            }

            raccolteConfUC_impProponiPeso = null;

            let kgridRaccolte = KendoGrid("raccolteConfUC_boxGrigliaRaccolte");
            kgridRaccolte.dataSource.read();
            numRaccolte = kgridRaccolte.dataSource.data().length;
        }
        else {
            // Ricaricamento grafico degli elementi della pagina
            let kgridRaccolte = KendoGrid("raccolteConfUC_boxGrigliaRaccolte");
            kgridRaccolte.refresh();
            numRaccolte = kgridRaccolte.dataSource.data().length;
        }
    }

    return numRaccolte;
}

// #region Eventi Selezione Raccolte

function raccolteConfUC_registraEventoPrimaRaccoltaSel(jqueryElem, funzioneDelegata) {
    jqueryElem.on("primaRaccoltaSel", funzioneDelegata);
    raccolteConfUC_observerUnaRigaSel.push(jqueryElem);
}

function raccolteConfUC_rimuoviEventoPrimaRaccoltaSel(jqueryElem) {
    jqueryElem.off("primaRaccoltaSel");
    let indexToRemove = raccolteConfUC_observerUnaRigaSel.indexOf(jqueryElem);
    if (indexToRemove !== -1) {
        raccolteConfUC_observerUnaRigaSel.splice(indexToRemove, 1);
    }
}

function raccolteConfUC_registraEventoNessunaRaccoltaSel(jqueryElem, funzioneDelegata) {
    jqueryElem.on("nessunaRaccoltaSel", funzioneDelegata);
    raccolteConfUC_observerNoRigheSel.push(jqueryElem);
}

function raccolteConfUC_rimuoviEventoNessunaRaccoltaSel(jqueryElem) {
    jqueryElem.off("nessunaRaccoltaSel");
    let indexToRemove = raccolteConfUC_observerNoRigheSel.indexOf(jqueryElem);
    if (indexToRemove !== -1) {
        raccolteConfUC_observerNoRigheSel.splice(indexToRemove, 1);
    }
}

function raccolteConfUC_registraEventoRaccoltaRimossa(jqueryElem, funzioneDelegata) {
    jqueryElem.on("raccoltaRimossa", funzioneDelegata);
    raccolteConfUC_observerRaccoltaRimossa.push(jqueryElem);
}

function raccolteConfUC_rimuoviEventoRaccoltaRimossa(jqueryElem) {
    jqueryElem.off("raccoltaRimossa");
    let indexToRemove = raccolteConfUC_observerRaccoltaRimossa.indexOf(jqueryElem);
    if (indexToRemove !== -1) {
        raccolteConfUC_observerRaccoltaRimossa.splice(indexToRemove, 1);
    }
}

// #endregion

function raccolteConfUC_ottieniRigheSelezionate() {
    let arrSel = [];

    // Non uso la funzione select() della kendogrid per ottenere le righe in quanto questa restituisce solo quelle selezionate nella pagina corrente
    let kgrid = KendoGrid("raccolteConfUC_boxGrigliaRaccolte");
    if (kgrid !== undefined) {
        let dsKGrid = kgrid.dataSource.data();
        for (let riga of dsKGrid) {
            if (riga.Selected === true) {
                arrSel.push(riga);
            }
        }
    }

    return arrSel;
}

function raccolteConfUC_verificaCoerenzaRigheSelezionate(dsRigheSel) {
    let isSelezioneCoerente = true;

    for (let i = 0; i < dsRigheSel.length - 1; i++) {
        //if (dsRigheSel[i].Impianto_Veg_Cod !== dsRigheSel[i + 1].Impianto_Veg_Cod ||
        //    dsRigheSel[i].Impianto_Cul_Cod !== dsRigheSel[i + 1].Impianto_Cul_Cod ||
        //    dsRigheSel[i].Esercizio_Lotto !== dsRigheSel[i + 1].Esercizio_Lotto) {
        //    isSelezioneCoerente = false;
        //    break;
        //}

        // Sa_Cod_Campagna per impostazione "ProponiPesoRaccolta"
        // Impianto_Veg_Cod per scelta prodotto su conferimento
        // Udm_Cod_Campagna per scelta unità di misura su conferimento
        // Mat_Cod_Campagna per contro aggiornamento qta su raccolta (cod_progetto valorizzato)
        // Lotto_Campagna per tracciabilità e contro aggiornamento qta su raccolta (cod_progetto valorizzato)
        /*
         ||
            (dsRigheSel[i].Lotto_Campagna !== "" && dsRigheSel[i + 1].Lotto_Campagna !== "" && dsRigheSel[i].Lotto_Campagna !== dsRigheSel[i + 1].Lotto_Campagna)*/
        //if (dsRigheSel[i].Sa_Cod_Campagna !== dsRigheSel[i + 1].Sa_Cod_Campagna ||
        //    dsRigheSel[i].Impianto_Veg_Cod !== dsRigheSel[i + 1].Impianto_Veg_Cod ||
        //    (dsRigheSel[i].Udm_Cod_Campagna !== 0 && dsRigheSel[i + 1].Udm_Cod_Campagna !== 0 && dsRigheSel[i].Udm_Cod_Campagna !== dsRigheSel[i + 1].Udm_Cod_Campagna) ||
        //    (dsRigheSel[i].Mat_Cod_Campagna !== 0 && dsRigheSel[i + 1].Mat_Cod_Campagna !== 0 && dsRigheSel[i].Mat_Cod_Campagna !== dsRigheSel[i + 1].Mat_Cod_Campagna) ||
        //    (dsRigheSel[i].Lotto_Campagna !== "" && dsRigheSel[i + 1].Lotto_Campagna !== "" && dsRigheSel[i].Lotto_Campagna !== dsRigheSel[i + 1].Lotto_Campagna) ) {
        //    isSelezioneCoerente = false;
        //    break;
        //}

        if (dsRigheSel[i].Sa_Cod_Campagna !== dsRigheSel[i + 1].Sa_Cod_Campagna) {
            // non è possibile associare insieme queste raccolte perché gli impianti di provenienza appartengono a centri aziendali diversi
            isSelezioneCoerente = false;
            break;
        }
        
        if (dsRigheSel[i].Impianto_Veg_Cod !== dsRigheSel[i + 1].Impianto_Veg_Cod) {
            // non è possibile associare insieme queste raccolte perché gli impianti di provenienza appartengono a specie vegetali diverse
            isSelezioneCoerente = false;
            break;
        }
        
        if (dsRigheSel[i].Udm_Cod_Campagna !== 0 && dsRigheSel[i + 1].Udm_Cod_Campagna !== 0 && dsRigheSel[i].Udm_Cod_Campagna !== dsRigheSel[i + 1].Udm_Cod_Campagna) {
            // non è possibile associare insieme queste raccolte perché sono indicate con diverse unità di misura
            isSelezioneCoerente = false;
            break;
        }
        
        //if (dsRigheSel[i].Mat_Cod_Campagna !== 0 && dsRigheSel[i + 1].Mat_Cod_Campagna !== 0 && dsRigheSel[i].Mat_Cod_Campagna !== dsRigheSel[i + 1].Mat_Cod_Campagna) {
        //    // non è possibile associare insieme queste raccolte perché utilizzano prodotti diversi
        //    isSelezioneCoerente = false;
        //    break;
        //}
        
        //if (dsRigheSel[i].Lotto_Campagna !== "" && dsRigheSel[i + 1].Lotto_Campagna !== "" && dsRigheSel[i].Lotto_Campagna !== dsRigheSel[i + 1].Lotto_Campagna) {
        //    // non è possibile associare insieme queste raccolte perché utilizzano lotti prodotti diversi
        //    isSelezioneCoerente = false;
        //    break;
        //}

    }

    return isSelezioneCoerente;
}

function raccolteConfUC_LeggiImpostazioneProponiPeso(saCod, vegCod, forzaLettura) {
    if (raccolteConfUC_impProponiPeso === null || forzaLettura === true) {
        raccolteConfUC_impProponiPeso = raccolteConfUC_WSImpostazioneProponiPeso(raccolteConfUC_piva, saCod, vegCod);
    }

    return raccolteConfUC_impProponiPeso;
}

// #endregion

// #region Funzioni Interne UC

function raccolteConfUC_popolaGrigliaRaccolte() {
    // Creazione Griglia
    var funzioniCRUD = {
        funzioneRead: raccolteConfUC_grigliaRaccolteRead,
        UtenteAbilitatoInserimentoModifica: false,
        UtenteAbilitatoCancellazione: false,
        checkBoxFunction: raccolteConfUC_grigliaRaccolteCheckRow,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: true
    };
    var idModel = "Chiave";
    var campiKendoModel = {
        Chiave: { type: "string" },

        Sa_Cod_Campagna: { type: "number" },
        Sa_Nome_Campagna: { type: "string" },
        Sa_Cod_Carico: { type: "number" },
        Sa_Nome_Carico: { type: "string" },
        Fabbricato_Des: { type: "string" },
        Destinazione_Des: { type: "string" },

        Id_Agenda: { type: "number" },
        Id_Mov_Det_Campagna: { type: "number" },
        Id_Mov_Det_Carico: { type: "number" },
        Conf_Id_Mov_Det: { type: "number" },

        Des_Lib: { type: "string" },
        //Appezza: { type: "string" }, //"number"
        //App_Nome: { type: "string" },

        //Impianto_Val_Cod: { type: "string" },
        Impianto_Veg_Cod: { type: "number" },
        Impianto_Veg_Des: { type: "string" },
        Impianto_Cul_Cod: { type: "string" }, //"number"
        //Impianto_Cul_Des: { type: "string" },
        //Esercizio_Lotto: { type: "string" },
        Campagna_Des: { type: "string" },
        //Esercizio_Regolamento_Cod: { type: "string" }, //"number"
        //Esercizio_Regolamento_Des: { type: "string" },
        //Esercizio_Biologico: { type: "string" },

        Mat_Cod_Campagna: { type: "number" }, //"number"
        CTE_Mat_Des: { type: "string" },
        CTE_Cod_Articolo: { type: "string" }, //"number"

        Data_Movimento: { type: "date" },
        Qta_Dettaglio: { type: "number" },
        Udm_Cod_Campagna: { type: "number" },
        Udm_Sim_Campagna: { type: "string" },

        Progetto_Lotto_Carico: { type: "string" },
        Lotto_Campagna: { type: "string" },

        Tipo_Associazione: { type: "string" },
        //Imb_Mat_Cod: { type: "number" },
        //Imb_Mat_Des: { type: "string" },
        //Imb_Qta: { type: "number" },
        //Peso_Teorico: { type: "number" },
    };
    var colonneKendoGrid = [
        //{ field: "Id_Agenda", title: "Id Agenda", filterable: { multi: true, search: true }, hidden: true, width: "110px" },
        //{ field: "Id_Mov_Det_Campagna", title: "Id_Mov_Det Raccolta", filterable: { multi: true, search: true }, hidden: true, width: "110px" },
        //{ field: "Id_Mov_Det_Carico", title: "Id_Mov_Det Carico", filterable: { multi: true, search: true }, hidden: true, width: "110px" },

        { field: "Des_Lib", title: TraduzioneMultiResx(resxRaccolteConfUC, "DescrizioneRaccolta", "Desc. Raccolta"), filterable: { multi: true, search: true }, width: "230px" },
        { field: "Data_Movimento", title: TraduzioneMultiResx(resxRaccolteConfUC, "DataRaccolta", "Data Raccolta"), format: "{0:dd/MM/yyyy}", width: "130px" },

        { field: "Impianto_Veg_Des", title: TraduzioneMultiResx(resxRaccolteConfUC, "SpecieImp", "Specie Impianto"), filterable: { multi: true, search: true }, hidden: true, width: "150px" },
        { field: "Campagna_Des", title: TraduzioneMultiResx(resxRaccolteConfUC, "Campagna", "Campagna"), filterable: { multi: true, search: true }, width: "450px", encoded: false },

        //{ field: "Appezza", title: "Codice Appezzamento", filterable: { multi: true, search: true }, hidden: true, width: "150px" },
        //{ field: "App_Nome", title: "Appezzamento", filterable: { multi: true, search: true }, width: "130px" },

        //{ field: "Impianto_Val_Cod", title: "Codice Impianto", filterable: { multi: true, search: true }, width: "150px" },
        //{ field: "Impianto_Veg_Cod", title: "Id Specie Impianto", filterable: { multi: true, search: true }, hidden: true, width: "130px" },
        
        //{ field: "Impianto_Cul_Cod", title: "Id Varietà", filterable: { multi: true, search: true }, hidden: true, width: "110px" },
        //{ field: "Impianto_Cul_Des", title: "Varietà Impianto", filterable: { multi: true, search: true }, width: "145px" },
        //{ field: "Esercizio_Lotto", title: "Lotto Esercizio", filterable: { multi: true, search: true }, width: "150px" },
        
        //{ field: "Esercizio_Regolamento_Cod", title: "Id Regolamento", filterable: { multi: true, search: true }, hidden: true, width: "110px" },
        //{ field: "Esercizio_Regolamento_Des", title: "Regolamento", filterable: { multi: true, search: true }, hidden: true, width: "270px" },
        //{ field: "Esercizio_Biologico", title: "Biologico", filterable: { multi: true, search: true }, width: "100px" },

        //{ field: "Mat_Cod_Campagna", title: "Prodotto Mat_Cod", filterable: { multi: true, search: true }, hidden: true, width: "130px" },
        { field: "CTE_Mat_Des", title: TraduzioneMultiResx(resxRaccolteConfUC, "ProdottoRaccolto", "Prodotto Raccolto"), filterable: { multi: true, search: true }, width: "200px" },
        { field: "CTE_Cod_Articolo", title: TraduzioneMultiResx(resxRaccolteConfUC, "CodiceArticoloAbbr", "Cod Articolo"), filterable: { multi: true, search: true }, width: "120px" },

        { field: "Qta_Dettaglio", title: TraduzioneMultiResx(resxRaccolteConfUC, "RisorsaQuantità", "Quantità"), filterable: { multi: true, search: true }, width: "95px" },
        //{ field: "Udm_Cod_Campagna", title: "U.d.M. Cod", filterable: { multi: true, search: true }, hidden: true, width: "90px" },
        { field: "Udm_Sim_Campagna", title: TraduzioneMultiResx(resxRaccolteConfUC, "UnitàDiMisuraAbbr", "U.d.M."), filterable: { multi: true, search: true }, width: "85px" },

        { field: "Progetto_Lotto_Carico", title: TraduzioneMultiResx(resxRaccolteConfUC, "LottoEsercizio", "Lotto Esercizio"), filterable: { multi: true, search: true }, width: "200px" },
        { field: "Destinazione_Des", title: TraduzioneMultiResx(resxRaccolteConfUC, "Destinazione", "Destinazione"), filterable: { multi: true, search: true }, width: "300px", encoded: false },
        { field: "Lotto_Campagna", title: TraduzioneMultiResx(resxRaccolteConfUC, "LottoProdotto", "Lotto Prodotto"), filterable: { multi: true, search: true }, width: "200px" },
        //{ field: "Imb_Mat_Cod", title: "Imballaggio Mat_Cod", filterable: { multi: true, search: true }, hidden: true },
        //{ field: "Imb_Mat_Des", title: "Imballaggio", filterable: { multi: true, search: true } },
        //{ field: "Imb_Qta", title: "Imballaggio Quantità", filterable: { multi: true, search: true } },
        //{ field: "Peso_Teorico", title: "Peso Teorico", filterable: { multi: true, search: true } },
    ];

    var parametriPerLettura = [];
    var parametriDataSource = {
        serverFiltering: false,
        pagesize: 10
    };

    var parametriKendoGrid = {
        excel: false,
        pdf: false,
        editable: false,
        groupable: false,
        //toolbarCommands: ["raccolteConfUC_tmplNuovaRaccolta"],
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        pageable: { pageSizes: [5, 10, 30], buttonCount: 4 },
        reorderable: true
    };
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: raccolteConfUC_grigliaRaccolteDataBound };

    creaKendoGrid(
        "raccolteConfUC_boxGrigliaRaccolte", // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD, //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid, // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi // funzioni da chiamare all'inizio e alla fine dei vari eventi
    );
}

function raccolteConfUC_grigliaRaccolteRead(options) {
    var elenco = [];

    elenco = raccolteConfUC_CercaRaccolte(raccolteConfUC_piva, raccolteConfUC_dataMov, raccolteConfUC_idMovDetConf);

    options.success(elenco);
}

function raccolteConfUC_grigliaRaccolteDataBound(ev) {

    if (raccolteConfUC_idMovDetConf !== null && raccolteConfUC_idMovDetConf !== 0 && raccolteConfUC_azioniPostLettura) {

        // Utilizzo questa variabile per eseguire questa associazione solo dopo la lettura degli elementi, in quanto l'evento dataBound
        // viene eseguito ogni volta che la griglia subisce un refresh od un cambio pagina, di conseguenza se l'utente toglie una delle righe
        // associate non devo ri-selezionarle
        raccolteConfUC_azioniPostLettura = false;

        // Pre-seleziono le raccolte associate in precedenza.
        let ds = ev.sender.dataSource;
        for (let i = 0; i < ds.data().length; i++) {
            if (ds.at(i).Conf_Id_Mov_Det === raccolteConfUC_idMovDetConf) {
                ds.at(i).Selected = true;
            }
        }

    }

    // Imposto graficamente le righe pre-selezionate
    let rows = ev.sender.tbody.children();
    for (let j = 0; j < rows.length; j++) {
        // Il ciclo lavora sulle righe attualmente renderizzate, funziona perché al cambio pagina viene ri-eseguito il dataBound
        let row = $(rows[j]);
        let dataItem = ev.sender.dataItem(row);
        if (dataItem.Selected === true) {
            row.find("input[type='checkbox']").prop("checked", true);
            row.addClass(GIAS_K_STATE_SELECTED);
        }
    }

    //kendo_AggiustaDimensioneColonne("#" + ev.sender.element[0].id);
}

function raccolteConfUC_grigliaRaccolteCheckRow(ev) {

    let checked = ev.currentTarget.checked;
    let row = $(ev.currentTarget).parents("tr").eq(0);
    //console.log(row);
    let kgrid = row.parents(".k-grid").eq(0).data("kendoGrid");
    //console.log(kgrid);
    let dataItem = kgrid.dataItem(row);
    //console.log(dataItem);

    if (!checked) {
        if (raccolteConfUC_idMovDetConf !== null && raccolteConfUC_idMovDetConf !== 0) {
            if (dataItem.Conf_Id_Mov_Det === raccolteConfUC_idMovDetConf) {

                var kendoConfirm = $("<div></div>").kendoConfirm({
                    content: TraduzioneMultiResx(resxRaccolteConfUC, "ConfermaCancAssociazioneRaccoltaConf", "Sicuro di voler cancellare l'associazione di questa raccolta col conferimento?"),
                    title: TraduzioneMultiResx(resxRaccolteConfUC, "Attenzione", "Attenzione"),
                    messages: {
                        okText: TraduzioneMultiResx(resxRaccolteConfUC, "Prosegui", "Prosegui"),
                        cancel: TraduzioneMultiResx(resxRaccolteConfUC, "Annulla", "Annulla")
                    },
                    //width: "40%",
                    //height: "60%",

                }).data("kendoConfirm");
                kendoConfirm.result.done(function () {
                    dataItem.Selected = false;
                    row.removeClass(GIAS_K_STATE_SELECTED);

                    raccolteConfUC_observerRaccoltaRimossa.forEach(function (jqueryElem) { jqueryElem.trigger("raccoltaRimossa"); });

                    raccolteConfUC_eventiRaccoltaSel(kgrid);

                });

                kendoConfirm.result.fail(function () {
                    row.find("input[type='checkbox']").prop("checked", true);
                });

                kendoConfirm.open();
            }
            else {
                dataItem.Selected = false;
                row.removeClass(GIAS_K_STATE_SELECTED);

                raccolteConfUC_observerRaccoltaRimossa.forEach(function (jqueryElem) { jqueryElem.trigger("raccoltaRimossa"); });

                raccolteConfUC_eventiRaccoltaSel(kgrid);
            }
        }
        else {
            dataItem.Selected = false;
            row.removeClass(GIAS_K_STATE_SELECTED);

            raccolteConfUC_observerRaccoltaRimossa.forEach(function (jqueryElem) { jqueryElem.trigger("raccoltaRimossa"); });

            raccolteConfUC_eventiRaccoltaSel(kgrid);
        }

    }
    else {
        dataItem.Selected = true;
        row.addClass(GIAS_K_STATE_SELECTED);

        raccolteConfUC_eventiRaccoltaSel(kgrid);
    }
}

function raccolteConfUC_eventiRaccoltaSel(kgrid) {
    let arrSel = [];
    let dsKGrid = kgrid.dataSource.data();
    for (let riga of dsKGrid) {
        if (riga.Selected === true) {
            arrSel.push(riga);
        }
    }

    if (arrSel.length === 0) {
        raccolteConfUC_observerNoRigheSel.forEach(function (jqueryElem) { jqueryElem.trigger("nessunaRaccoltaSel"); });
    }

    if (arrSel.length === 1) {
        raccolteConfUC_observerUnaRigaSel.forEach(function (jqueryElem) { jqueryElem.trigger("primaRaccoltaSel"); });
    }
}

function raccolteConfUC_nuovaRaccolta() {
}

// #endregion