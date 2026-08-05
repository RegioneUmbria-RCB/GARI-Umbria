var hasTheDropDownRefreshed = false;
function get_Cmb_Report(id, tab) {
    WaitFrame.show();
    return new Promise((resolve, reject) => {
        let onLoad = true;
        $("#" + id).kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "desc",
            dataValueField: "cod",
            dataSource: { transport: { read: CaricaComboCmb_Report } },
            //open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                this.trigger("change");
                resolve(this);
            },
            change: async function (e) {
                nomeReport = this.value();
                if (firstTime) {
                    firstTime = false;
                    await leggiGrid();
                } else {
                    if (nomeReport != lastReport) {
                        lastReport = nomeReport;
                        if (nomeReport != "") {
                            let params = await leggiReport(nomeReport, false);
                            aggiornaPaginaDaReport(params);

                        }
                    }
                }
                resolve(this);
            },
            open: function (e) {
            },
            optionLabel: 'SELEZIONA'
        }).data("kendoDropDownList");

    });
}



function kendoGridPianoColturale(IDControllo, hd) {


    var funzioniCRUD = { funzioneRead: kReadKendoPianoColturale_Read };
    var idModel = "chiave";
    var campiKendoModel = kReadKendoPianoColturale_mod();
    var colonneKendoGrid = kReadKendoPianoColturale_col();
    var parametriPerLettura = [];
    var parametriDataSource = { aggregate: [] };
    var parametriKendoGrid = {
        pagesize: 10,
        groupable: false,
        scrollable: true,
        sortable: true,
        resizable: true,
        reorderable: true,
        columnMenu: true,
        filterable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        pdf: true,
        excel: true
    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: kendoGridFlatResizeColonne

    };
    var mostraRigheCancellate = true;
    colonneDisabilitateSoloInModifica = ["chiave"]

    creaKendoGrid(IDControllo.replace("#", ""), // rappresenta l'ID del div a cui si associa la griglia
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

    WaitFrame.hide();
    //personalizzaGriglia(IDControllo);
}
function personalizzaGriglia(idDiv) {
    var grid = $("#" + idDiv).data('kendoGrid');
    if (grid !== undefined && grid !== null && customGrid != undefined && customGrid != null && customGrid !== "") {
        setPersonalizzazioniGrigliaKendo(grid, JSON.parse(customGrid));
        //customGrid = null;
    }
}


//Aggiorna le personalizzazioni della kendoGrid
function setPersonalizzazioniGrigliaKendo(grid, options) {
    try {
        var dataSource = grid.dataSource;
        var savedColumns = options.columns;

        //NUMERO DI RIGHE PER PAGINA
        if (options.pageSize)
            dataSource.pageSize(options.pageSize);

        //RIORDINAMENTO COLONNE
        var indOrd = 0;
        for (let i = 0; i < savedColumns.length; i++) {
            let col;
            //Cerca la colonna salvata tra le colonne reali in base al field (campo in tabella) o al titolo della colonna
            if (savedColumns[i].field && savedColumns[i].field != null) {
                col = grid.columns.find(function (v, index) { return grid.columns[index].field == savedColumns[i].field; });
            } else if (savedColumns[i].title) {
                col = grid.columns.find(function (v, index) { return grid.columns[index].title == savedColumns[i].title; });
            }
            if (col) {
                //Sposta la colonna in testa
                if (savedColumns[i].hidden != true)
                    grid.reorderColumn(indOrd, col);
                indOrd++;
            }
        }

        //MOSTRA/NASCONDE COLONNE
        for (let i = 0; i < savedColumns.length; i++) {
            let col;
            //Cerca la colonna salvata tra le colonne reali in base al field (campo in tabella) o al titolo della colonna
            if (savedColumns[i].field && savedColumns[i].field != null) {
                col = grid.columns.find(function (v, index) { return grid.columns[index].field == savedColumns[i].field; });
            } else if (savedColumns[i].title) {
                col = grid.columns.find(function (v, index) { return grid.columns[index].title == savedColumns[i].title; });
            }
            if (col) {
                if (savedColumns[i].hidden == true) {
                    grid.hideColumn(col);
                } else { //else if (savedColumns[i].hidden == false) 
                    grid.showColumn(col);
                }
            }
        }

        //ORDINAMENTO (ASC/DESC) DELLE COLONNE
        if (options.sort) {
            var ordinam = options.sort;
            for (let i = ordinam.length - 1; i >= 0; i--) {
                let col = grid.columns.find(function (v, index) { return grid.columns[index].field == ordinam[i].field; });
                if (col == undefined) {
                    ordinam.splice(i, 1);
                }
            }
            if (ordinam.length > 0)
                dataSource.sort(options.sort);
        }

        //FILTRI PER COLONNE
        if (options.filter) {
            var filtri = options.filter.filters;
            for (let i = filtri.length - 1; i >= 0; i--) {
                let col = grid.columns.find(function (v, index) { return grid.columns[index].field == filtri[i].field; });
                if (col == undefined) {
                    filtri.splice(i, 1);
                }
                // Sistemazione della data che era stata memorizzata a DB in formato GMT
                if (col.field !== undefined && grid.dataSource.options.schema.model.fields[col.field].type === "date")
                    filtri[i].value = kendo.parseDate(filtri[i].value);
            }
            if (filtri.length > 0)
                dataSource.filter(options.filter);
        }

        //RAGGRUPPAMENTO IN TOOLBAR
        if (options.group) {
            var raggrup = options.group;
            for (let i = raggrup.length - 1; i >= 0; i--) {
                let col = grid.columns.find(function (v, index) { return grid.columns[index].field == raggrup[i].field; });
                if (col == undefined) {
                    raggrup.splice(i, 1);
                }
            }
            if (raggrup.length > 0)
                dataSource.group(options.group);
        }

    } catch (err) {
        console.log(err);
    }
}

function kReadKendoPianoColturale_Read(options) {

    var risp = JSON.parse($("#hdKendoPianoColturale").val());
    options.success(risp.kendo_rows);
}

function kReadKendoPianoColturale_mod() {

    var hd = "#hdKendoPianoColturale";

    var data = $(hd).val();
    var jSonParsed_Kendo = JSON.parse(data);


    //console.log(data);
    return jSonParsed_Kendo.kendo_model;
}
function kReadKendoPianoColturale_col() {

    var hd = "#hdKendoPianoColturale";

    var data = $(hd).val();
    var jSonParsed_Kendo = JSON.parse(data);

    //var colonnaStato = {
    //    field: "Stato",
    //    title: TraduzioneMultiResx(prenotazionePianteResx, "StatoCondizione", "Stato"),
    //    template: kendo.template($("#StatoOrdiniTemplate").html()),
    //    filterable: { multi: true, search: true }
    //};

    //jSonParsed_Kendo.kendo_columns.splice(1, 0, colonnaStato);


    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}

function kendoGridFlatResizeColonne(jQuerySelector) {

    var grid = $("#ppKendoPianoColturale").data("kendoGrid");

    for (let i = 0; i < grid.columns.length; i++) {
        if (grid.columns[i].width === undefined) {
            grid.autoFitColumn(i);
        }
    }
}


//Aggiorna i parametri della pagina per farli coincidere con quelli del report selezionato
async function aggiornaPaginaDaReport(params) {



    if (params.paramsGriglia != undefined && params.paramsGriglia != null && params.paramsGriglia != "") {
        customGrid = params.paramsGriglia;
    }

    //leggiGrid();
    personalizzaGriglia("ppKendoPianoColturale");

}

//Gestione salvataggio report
async function gestioneBtnSalva() {
    let nomeReport = Cmb_Report.value();
    kendo.prompt("Inserire il nome del report (se già presente verrà sovrascritto)", nomeReport).then(async function (nome) {
        if (nome != undefined && nome != null && nome != "") {

            nome = SanitizeTesto_MantieniVirgoletteECaratteriAccentati(nome);
            if (nome.trim() == "") {
                kendo.alert("Inserire un nome report valido");
                return false
            }

            WaitFrame.show();
            let report = buildReport("", nome);
            await salvaReport(report);
            Cmb_Report.dataSource.read();
            setKendoDropDownHeight();
            Cmb_Report.refresh();
            Cmb_Report.value(nome);
            await leggiReport(nome, false);
            hasTheDropDownRefreshed = true;
            setKendoDropDownHeight();
            WaitFrame.hide();
        } else {
            kendo.alert("Nome report non inserito");
        }
    });
}

//Gestione cancellazione report
async function gestioneBtnCancella() {
    let nomeReport = Cmb_Report.value();
    if (nomeReport != undefined && nomeReport != null && nomeReport != "") {
        WaitFrame.show();
        await cancellaReport(nomeReport);

        hasTheDropDownRefreshed = true;
        setKendoDropDownHeight();
        WaitFrame.hide();
    } else {
        kendo.alert("Selezionare un report da cancellare");
    }
}
//Crea il report da salvare
function buildReport(idTab, nomeReport) {
    let report;

    try {

        idDiv = "ppKendoPianoColturale";

        let paramsGriglia = getParametriGriglia(idDiv);

        report = {
            "nomeReport": nomeReport,
            "paramsGriglia": paramsGriglia
        };

    } catch (err) {
        console.log(err);
    }

    return report;
}

//Recupera i parametri della KendoGrid passata
function getParametriGriglia(idDiv) {
    var jsonDaSalvare = "";
    var grid = $('#' + idDiv).data('kendoGrid');
    if (grid !== undefined) {
        var dataSource = grid.dataSource;
        var columns = grid.columns;
        var pageSize = dataSource.pageSize();
        var sort = dataSource.sort();
        var filter = dataSource.filter();
        var group = dataSource.group();
        var options = {
            "pagina": location.pathname,
            "nomeDiv": idDiv,
            "columns": columns,
            "pageSize": pageSize,
            "sort": sort,
            "filter": filter,
            "group": group
        };
        jsonDaSalvare = kendo.stringify(options);
    }
    return jsonDaSalvare;
}


function setKendoDropDownHeight(height = null) {
    if (height == null) {
        var height = $(window).height() * 0.4;
    }
    height = Math.max(height, 150);
    Cmb_Report.setOptions({ height: height });
    Cmb_Report.refresh();

}