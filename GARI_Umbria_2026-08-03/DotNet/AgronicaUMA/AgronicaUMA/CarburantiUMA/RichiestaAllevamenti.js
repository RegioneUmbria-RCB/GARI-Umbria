// #region API User Control

// Lo UserControl necessita delle seguenti variabili inizializzate:
// pivaSelezionata
// richiesta_cod
// indirizzohttp
// elencoCarburanti
// modifica_richiesto
// modifica_assegnato
// richiestaRinuncia
//
// Delle seguenti funzioni:
// AddErrorClass
// RemoveErrorClass

const larghezzaStdCampoCarbAllev = 120;

async function ucUmaAllevamenti_initGrid() {
    // Procedo solo se ho l'id della richiesta_testata
    if (richiesta_cod <= 0) {
        return false;
    }

    var IDControllo = "tab_griglia_allevamenti";
    var kgridAllev = KendoGrid(IDControllo);
    if (kgridAllev !== undefined) {
        kgridAllev.destroy();
    }

    var objFunzioneSubmit = {};
    // If necessario perché non posso passare al creaKendoGrid i tre flag a false con "funzione" valorizzata
    if (modifica_richiesto === true) {
        // Imposto una funzione fittizia per avere il salvataggio in funzione esterna e comunque impostare i flag per avere 
        // i pulsanti di insert / cancellazione e la possibilità di modificare le righe
        objFunzioneSubmit = {
            funzione: () => { },
            flagInsert: !richiestaRinuncia,
            flagUpdate: !richiestaRinuncia,
            flagDelete: true
        }
    }
    if (modifica_assegnato == true && !richiestaRinuncia) {
        // omettiPulsantiSalva deve essere a true
        objFunzioneSubmit = {
            funzione: () => { },
            flagInsert: false,
            flagUpdate: true,
            flagDelete: false
        }
    }

    var funzioniCRUD = {
        funzioneRead: ucUmaAllevamenti_gridRead,
        //checkBoxFunction: gridContattiUCCheckRow,
        UtenteAbilitatoInserimentoModifica: true,
        UtenteAbilitatoCancellazione: true,
        //funzioneInsert: fnAbilitaPulsateInsert,
        funzioneSubmit: objFunzioneSubmit,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla : false
    };

    var idModel = "Chiave";
    var campiKendoModel = {
        Chiave: { editable: false, type: "string" },
        Piva: { editable: false, type: "string" },
        UMA_All_Cod: { editable: false, type: "string" },
        Richiesta_Cod: { editable: false, type: "number" },
        Totale_Capi: { editable: !richiestaRinuncia && modifica_richiesto, type: "number" },
        Tipo_Carburante: { editable: false, type: "number" },
        Car_Cod: { editable: false, type: "number" },
        Carburante_Calcolato: { editable: false, type: "number" },
        Carburante_Richiesto: { editable: !richiestaRinuncia && modifica_richiesto, type: "number" },
        Carburante_Approvato: { editable: !richiestaRinuncia && modifica_assegnato, type: "number" },
        UMA_All_Des: { editable: !richiestaRinuncia && modifica_richiesto, type: "string" },
        Car_Des: { editable: !richiestaRinuncia && modifica_richiesto, type: "string" },
        Note_Compilatore: { editable: !richiestaRinuncia && modifica_richiesto, type: "string" },
        Note_Approvatore: { editable: !richiestaRinuncia && modifica_assegnato, type: "string" },
        Incluso_nel_Calcolo: { editable: false, type: "string" },
        //Validita_Inizio: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1") },
        //Validita_Fine: { editable: colonna_editabile, type: "date", defaultValue: new Date("2100/12/31") },

    };

    var styleOut = "vertical-align: middle; text-align: center;";
    var colonneKendoGrid = [
        { field: "UMA_All_Des", title: "Allevamento", headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, hidden: false, width: 350, editor: ucUmaAllevamenti_ddlTipiEditor },
        { field: "Totale_Capi", title: "Quantità", headerAttributes: { style: styleOut }, hidden: false, width: 90, format: "{0:n0}", editor: ucUmaAllevamenti_ntbCapiEditor },
        { field: "Car_Des", title: "Carburante", headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, hidden: false, width: 110, editor: ucUmaAllevamenti_ddlCarbEditor},
        { field: "Carburante_Calcolato", title: "Carburante Calcolato (lt)", headerAttributes: { style: styleOut }, hidden: false, width: larghezzaStdCampoCarbAllev, format: "{0:n0}", editor: ucUmaAllevamenti_ntbCarbEditor, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n0')#</div>" },
        { field: "Carburante_Richiesto", title: "Carburante Richiesto (lt)", headerAttributes: { style: styleOut }, hidden: false, width: larghezzaStdCampoCarbAllev, format: "{0:n0}", editor: ucUmaAllevamenti_ntbCarbEditor, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n0')#</div>" },
        { field: "Carburante_Approvato", title: "Carburante Assegnato (lt)", headerAttributes: { style: styleOut }, hidden: false, width: larghezzaStdCampoCarbAllev, format: "{0:n0}", editor: ucUmaAllevamenti_ntbCarbEditor, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n0')#</div>" },
        { field: "Note_Compilatore", title: "Note Compilatore", headerAttributes: { style: styleOut }, filterable: false, hidden: false },
        { field: "Note_Approvatore", title: "Note Approvatore", headerAttributes: { style: styleOut }, filterable: false, hidden: false },
        { field: "Incluso_nel_Calcolo", title: "Soggetto a Controllo Capi Allevabili", headerAttributes: { style: styleOut }, filterable: false, hidden: false },
    ];

    var parametriPerLettura = [];
    var parametriDataSource = {
        aggregate: [
            { field: "Carburante_Calcolato", aggregate: "sum" },
            { field: "Carburante_Richiesto", aggregate: "sum" },
            { field: "Carburante_Approvato", aggregate: "sum" },
        ]
    };

    var parametriKendoGrid = {
        columnMenu: true,
        pageable: { pageSizes: [50] },
        pdf: false,
        groupable: false,
        sortable: true,
        //columnMenu: false
        // colonneCustomKendoGrid: colCustKendoGrid
    };
    var funzioniPrimaDopoEventi = {
        /* funzioneDaChiamareDopoSave: HideTabDettagli, 
         * funzioneDaChiamareDopoEdit: onEditGrigliaDettagliImpianti,  
         * funzioneDaChiamareDopoDelete: HideTabDettagli */
        funzioneDaChiamareDopoDataBound: ucUmaAllevamenti_gridDataBound
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["UMA_All_Cod"];

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
}


function ucUmaAllevamenti_reloadGrid() {
    var kGrid = KendoGrid("tab_griglia_allevamenti");
    if (kGrid !== undefined) {
        var storage_key = "ucUmaAllevamenti_ricerca_" + pivaSelezionata + "_" + richiesta_cod;
        storageRemoveItem(storage_key);
        kGrid.dataSource.read();
    }
}

function ucUmaAllevamenti_errorGridSolved() {
    var kGrid = KendoGrid("tab_griglia_allevamenti");
    if (kGrid !== undefined) {
        return kGrid.element.find(".errorCell").length === 0;
    }
    else {
        return true;
    }
}

// #endregion


// #region Funzioni Interne User Control

async function ucUmaAllevamenti_gridRead(options) {
    var dtRichiesteAllevamenti = [];
    dtRichiesteAllevamenti = await ucUmaAllevamenti_ricerca();
    options.success(dtRichiesteAllevamenti);
}

function ucUmaAllevamenti_gridDataBound(ev) {
    //var gridId = ev.sender.element[0].id;

    //var indexCol = ev.sender.thead.find("th[data-field='" + + "']").index();
    var indexColUmaAllDes = ev.sender.thead.find("th[data-field='UMA_All_Des']").index();
    var indexColTotaleCapi = ev.sender.thead.find("th[data-field='Totale_Capi']").index();
    var indexColCarbRich = ev.sender.thead.find("th[data-field='Carburante_Richiesto']").index();
    var indexColCarbAppr = ev.sender.thead.find("th[data-field='Carburante_Assegnato']").index();

    var dtConfigAllev = ucUmaAllevamenti_elencoConfig();
    var dsGrid = ev.sender.dataSource.data();
    // Devo verificare se, per ogni configurazione sono stati inseriti massimo n allevamenti del tipo m
    dtConfigAllev.forEach(function (drConfig) {
        var dsGridFilter = dsGrid.filter(function (dataitem) {
            return dataitem.UMA_All_Cod === drConfig.UMA_All_Cod ; 
        });
        if (dsGridFilter.length > 0) {
            var row = ev.sender.tbody.find("tr[data-uid='" + dsGridFilter[0].uid + "']").eq(0);
            if (dsGridFilter.length > drConfig.N_Max_Allevamenti) {
                // Imposto l'errore sulla prima delle celle che trovo in errore, in quanto assumo essere la più recente
                AddErrorClass(row, indexColUmaAllDes, "errorCell", "È possibile inserire questa tipologia di allevamento solo " + drConfig.N_Max_Allevamenti + " volta/e");
            }
            else {
                RemoveErrorClass(row, indexColUmaAllDes, "errorCell");
            }
        }
    });

    // Gestisco warning su carburante richiesto ed approvato in relazione a quello calcolato
    dsGrid.forEach(function (dataitem) {
        var row = ev.sender.tbody.find("tr[data-uid='" + dataitem.uid + "']").eq(0);

        if (dataitem.Totale_Capi === null) {
            AddErrorClass(row, indexColTotaleCapi, "errorCell", "Inserire un valore");
        }
        else {
            RemoveErrorClass(row, indexColTotaleCapi, "errorCell");
        }

        if (dataitem.Carburante_Richiesto < 0) {
            AddErrorClass(row, indexColCarbRich, "errorCell", "Il carburante richiesto è minore di zero");
        }
        else {
            RemoveErrorClass(row, indexColCarbRich, "errorCell");
        }

        if (dataitem.Carburante_Richiesto > dataitem.Carburante_Calcolato) {
            //AddErrorClass(row, indexColCarbRich, "warningCell", "Il carburante richiesto risulta maggiore di quello calcolato");
            AddErrorClass(row, indexColCarbRich, "errorCell", "Il carburante richiesto risulta maggiore di quello calcolato");
        }
        else {
            //RemoveErrorClass(row, indexColCarbRich, "warningCell");
            RemoveErrorClass(row, indexColCarbRich, "errorCell");
        }

        if (dataitem.Carburante_Approvato < 0) {
            AddErrorClass(row, indexColCarbAppr, "errorCell", "Il carburante approvato è minore di zero");
        }
        else {
            RemoveErrorClass(row, indexColCarbAppr, "errorCell");
        }

        if (dataitem.Carburante_Approvato > dataitem.Carburante_Calcolato) {
            //AddErrorClass(row, indexColCarbAppr, "warningCell", "Il carburante approvato risulta maggiore di quello calcolato");
            AddErrorClass(row, indexColCarbAppr, "errorCell", "Il carburante approvato risulta maggiore di quello calcolato");
        }
        else {
            //RemoveErrorClass(row, indexColCarbAppr, "warningCell");
            RemoveErrorClass(row, indexColCarbAppr, "errorCell");
        }
    });
}

function ucUmaAllevamenti_ddlTipiEditor(container, options) {
    var arrTipi = [];
    arrTipi = ucUmaAllevamenti_elencoTipi();
    creaDropDownEditor(container, "UMA_All_Des", "UMA_All_Cod", arrTipi, ucUmaAllevamenti_ddlTipiChange);
}

function ucUmaAllevamenti_ddlTipiChange(ev) {
    var dataItem = ev.sender.dataItem();
    var gridID = ev.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    var indexColUmaAllDes = grid.thead.find("th[data-field='UMA_All_Des']").index();

    model.dirty = true;

    model.UMA_All_Cod = dataItem.UMA_All_Cod;
    model.dirtyFields.UMA_All_Cod = true;

    model.UMA_All_Des = dataItem.UMA_All_Des;
    model.dirtyFields.UMA_All_Des = true;

    ucUmaAllevamenti_calcolaFabbisogno(model);

    if (model.Tipo_Carburante === 0) {
        // In caso sto cambiando allevamento di una riga nuova, imposto il gasolio come carburante "predefinito"
        model.Tipo_Carburante = 2;
        model.Car_Cod = 2;
        model.Car_Des = "Gasolio";
    }

    //kendoFastRedrawRow(grid, row);
    grid.refresh(); // In questo modo ri-effettuo anche l'evento dataBound per i controlli di validità
}

function ucUmaAllevamenti_ntbCapiEditor(container, options) {
    $('<input name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            decimals: 0,
            value: 0,
            min: 0,
            format: options.format,
            restrictDecimals: true,
            change: ucUmaAllevamenti_ntbCapiChange
        });
}

function ucUmaAllevamenti_ntbCapiChange(ev) {
    var gridID = ev.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    ucUmaAllevamenti_calcolaFabbisogno(model);

    //kendoFastRedrawRow(grid, row);
    grid.refresh(); // In questo modo ri-effettuo anche l'evento dataBound per i controlli di validità
}

function ucUmaAllevamenti_ddlCarbEditor(container, options) {
    if (Array.isArray(elencoCarburanti)) {
        // elencoCarburanti è una variabile globale data dalla pagina chiamante
        // Accetto solo benzina e gasolio
        var carbPerAllevamenti = elencoCarburanti.filter(function (elem) {
            if (elem.Car_Cod === "2" || elem.Car_Cod === "3") {
                return true;
            }
            else {
                return false;
            }
        });
        creaDropDownEditor(container, "TipoCarb", "Car_Cod", carbPerAllevamenti, ucUmaAllevamenti_ddlCarbChange);
    }
}

function ucUmaAllevamenti_ddlCarbChange(ev) {
    var dataItem = ev.sender.dataItem();
    var gridID = ev.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.dirty = true;

    model.Car_Des = dataItem.TipoCarb;
    model.dirtyFields.Car_Des = true;

    model.Tipo_Carburante = parseInt(dataItem.Car_Cod);
    model.dirtyFields.Tipo_Carburante = true;

    model.Car_Cod = model.Tipo_Carburante;
    model.dirtyFields.Car_Cod = true;

    ucUmaAllevamenti_calcolaFabbisogno(model);

    grid.refresh();
}

function ucUmaAllevamenti_calcolaFabbisogno(model) {
    if (model.UMA_All_Cod != "" && model.Totale_Capi !== null && model.Tipo_Carburante !== 0) {
        var dtConfigAllev = ucUmaAllevamenti_elencoConfig();

        var rowCarbPerCapo = dtConfigAllev.filter((elem) => {
            return elem.UMA_All_Cod == model.UMA_All_Cod
        })[0];

        var carbCalcolato = 0;
        if (rowCarbPerCapo !== undefined && rowCarbPerCapo !== null) {
            var carbPerCapo = 0;
            if (model.Tipo_Carburante === 2) {
                // Gasolio
                carbPerCapo = rowCarbPerCapo.Gasolio_Lt;
            }
            if (model.Tipo_Carburante === 3) {
                // Benzina
                carbPerCapo = rowCarbPerCapo.Benzina_Lt;
            }
            var qtaAggCarro = carbPerCapo === 0 ? 0 : rowCarbPerCapo.Qta_Aggiuntiva_Carro;
            // I litri aggiuntivi del carro sono da sommare ad ogni capo
            carbCalcolato = parseFloat((model.Totale_Capi * (carbPerCapo + qtaAggCarro)).toFixed(4));
        }
        model.Carburante_Calcolato = carbCalcolato;
        model.Carburante_Richiesto = carbCalcolato;

        model.dirtyFields.Carburante_Calcolato = carbCalcolato === 0 ? false : true;
        model.dirtyFields.Carburante_Richiesto = carbCalcolato === 0 ? false : true;

    }
}

function ucUmaAllevamenti_ntbCarbEditor(container, options) {
    //numberEditor4decimals(container, options);

    $('<input name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            decimals: 0,
            value: 0,
            min: 0,
            format: options.format,
            restrictDecimals: false
        });

    $("input[name='" + options.field + "']").data("kendoNumericTextBox").bind("change", ucUmaAllevamenti_ntbCarbChange);
}

function ucUmaAllevamenti_ntbCarbChange(ev) {
    var gridID = ev.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    grid.refresh();
}


// #endregion