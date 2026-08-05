
/**
* Creazione di una griglia Kendo
*
* @param {string} IDPivotConfigurator Rappresenta il selector JQuery del div a cui si associa il configuratore pivot
* @param {array}  parametriKendoPivotConfigurator Contiene i parametri da utilizzare per il configuratore pivot
* @param {string} IDPivotGrid Rappresenta il selector JQuery del div a cui si associa la pivot
* @param {string} funzione da chiamare per la lettura dei dati
* @param {object} model contiene il modello con cui creare il datasource
* @param {object} cubeDimensionsKendoPivot contiene le dimensione del cubo con cui creare il datasource
* @param {object} cubeMeasuresKendoPivot contiene i valori del cubo con cui creare il datasource
* @param {object} colonneDefaultKendoPivotGrid contiene le colonne di default che verranno mostrate
* @param {object} righeDefaultKendoPivotGrid contiene le righe di default che verranno mostrate
* @param {object} misureDefaultKendoPivotGrid contiene i valori di default che verranno mostrate
* @param {array} parametriPerLettura contiene i parametri da utilizzare per la lettura
* @param {array}  parametriDataSourcePivotGrid contiene i parametri da utilizzare configurare il datasource
* @param {array}  parametriKendoPivotGrid contiene i parametri da utilizzare configurare la pivot
* @param {object} funzioniPrimaDopoEventiPivotGrid contiene le funzioni da chiamare prima e dopo gli eventi
*/
function creaKendoPivotGrid(
    IDPivotConfigurator,  // ID Configurator grid
    parametriKendoPivotConfigurator,   // parametri griglia [{ chiave - valore}]
    IDPivotGrid,  // ID Pivot grid
    funzioneReadPivotGrid,
    modelKendoPivot,
    cubeDimensionsKendoPivot,
    cubeMeasuresKendoPivot,
    colonneDefaultKendoPivotGrid,
    righeDefaultKendoPivotGrid,
    misureDefaultKendoPivotGrid,
    parametriPerLettura, // parametri da passare alla lettura
    parametriDataSourcePivotGrid, // parametri data source { chiave - valore}
    parametriKendoPivotGrid,   // parametri griglia [{ chiave - valore}]
    funzioniPrimaDopoEventiPivotGrid // funzioni da chiamare all'inizio e alla fine dei vari eventi { chiave - valore}:
                            //{   
                            //    funzioneDaChiamarePrimaDelDataBinding: yyyyy, // funzione da chiamare all'inizio del databinding
                            //    funzioneDaChiamareDopoDataBinding: yyyyy, // funzione da chiamare alla fine del databinding
                            //    funzioneDaChiamarePrimaDelDataBound: yyyyy, // funzione da chiamare all'inizio del databound
                            //    funzioneDaChiamareDopoDataBound: yyyyy, // funzione da chiamare alla fine del databound
                            //}

    ) {

    if (IDPivotConfigurator == null)
    {
        alert("Non mi hai passato l'ID del DIV che contiene il configuratore Pivot");
        return;
    }

    if (IDPivotGrid == null) {
        alert("Non mi hai passato l'ID del DIV che contiene la Pivot");
        return;
    }
        
    if (funzioneReadPivotGrid == null)
    {
        alert("Non mi hai passato la funzione da chiamare in lettura");
        return;
    }

    if (modelKendoPivot == null) {
        alert("Non mi hai passato il model della Pivot");
        return;
    }
    
    if (cubeDimensionsKendoPivot == null) {
        alert("Non mi hai passato le dimensioni del cubo della Pivot");
        return;
    }

    if (cubeMeasuresKendoPivot == null) {
        alert("Non mi hai passato le misure del cubo della Pivot");
        return;
    }

    if (colonneDefaultKendoPivotGrid == null) {
        alert("Non mi hai passato le colonne di default della Pivot");
        return;
    }

    if (righeDefaultKendoPivotGrid == null) {
        alert("Non mi hai passato le righe di default della Pivot");
        return;
    }

    if (misureDefaultKendoPivotGrid == null) {
        alert("Non mi hai passato le misure di default della Pivot");
        return;
    }

    funzioniPrimaDopoEventiPivotGrid = (typeof funzioniPrimaDopoEventiPivotGrid === 'undefined') ? {} : funzioniPrimaDopoEventiPivotGrid;
    errFound = false;
    for (var k in funzioniPrimaDopoEventiPivotGrid) {
        if (k != "funzioneDaChiamarePrimaDelDataBinding" &&
            k != "funzioneDaChiamareDopoDataBinding" &&
            k != "funzioneDaChiamarePrimaDelDataBound" &&
            k != "funzioneDaChiamareDopoDataBound" && 
            k != "expandMember" && 
            k != "collapseMember" 
            ) {
            errFound = true;
            alert("Fra le funzioni da chiamare prima o dopo agli eventi mi hai passato la chiave " + k + " che non è gestita");
        }
    }
    if (errFound)
        return;

    // Per le funzioni di accesso al database è necessario fare delle funzioni anonime 
    // altrimenti le chiama subito nel momento in cui crea la griglia
    var readFunction = null;
    if (funzioneReadPivotGrid != null) {
        if (parametriPerLettura != null && parametriPerLettura.length > 0)
        {
            readFunction = function (options) {
                funzioneReadPivotGrid(options, parametriPerLettura.toString());
            };
        }  
        else
        {
            readFunction = function (options) {
                funzioneReadPivotGrid(options);
                 
            };
        }
    }

    parametriDataSourcePivotGrid = (typeof parametriDataSourcePivotGrid === 'undefined' || parametriDataSourcePivotGrid == null) ? {} : parametriDataSourcePivotGrid;
    parametriDataSourcePivotGrid.filters = (typeof parametriDataSourcePivotGrid.filters === 'undefined') ? null : parametriDataSourcePivotGrid.filters;
    parametriKendoPivotGrid = (typeof parametriKendoPivotGrid === 'undefined' || parametriKendoPivotGrid == null) ? {} : parametriKendoPivotGrid;
    parametriKendoPivotGrid.sortable = (typeof parametriKendoPivotGrid.sortable === 'undefined') ? true : parametriKendoPivotGrid.sortable;
    parametriKendoPivotGrid.filterable = (typeof parametriKendoPivotGrid.filterable === 'undefined') ? true : parametriKendoPivotGrid.filterable;
    parametriKendoPivotGrid.height = (typeof parametriKendoPivotGrid.height === 'undefined') ? null : parametriKendoPivotGrid.height;
    parametriKendoPivotConfigurator = (typeof parametriKendoPivotConfigurator === 'undefined' || parametriKendoPivotConfigurator == null) ? {} : parametriKendoPivotConfigurator;
    parametriKendoPivotConfigurator.height = (typeof parametriKendoPivotConfigurator.height === 'undefined') ? null : parametriKendoPivotConfigurator.height;

    if (parametriKendoPivotGrid.filterable)
    {
        parametriKendoPivotGrid.filterable = {
        messages: {
            clear: "Pulisci",
            filter: "Applica",
            isFalse: "No",
            isTrue: "Sì"
            }
        }
    }

    //if (parametriKendoPivotGrid.sortable)
    //{
    //    parametriKendoPivotGrid.sortable = {
    //        mode: "multiple"
    //    }
    //}
          
    var dataSourceKendoPivotGrid = new kendo.data.PivotDataSource({
        //data: getDatiJson(),
         transport: {
            read: readFunction,
        },
        schema: {
            model: modelKendoPivot,
            cube: {
                dimensions: cubeDimensionsKendoPivot,
                measures: cubeMeasuresKendoPivot
            }
        },
        columns: colonneDefaultKendoPivotGrid,
        rows: righeDefaultKendoPivotGrid,
        measures: misureDefaultKendoPivotGrid,
        filter: parametriDataSourcePivotGrid.filters,
        error: function (e) {
            alert("Errore: " + kendo.stringify(e.errors[0]));
        }
    });

    var originalMouseLeave = kendo.ui.Menu.fn._mouseleave;
    var mouseLeave = function (e) {
        var that = this;
        clearTimeout(this._timeoutHandle);
        this._timeoutHandle = setTimeout(function () {
            originalMouseLeave.call(that, e);
        }, 1000);
    }

    kendo.ui.Menu.fn._mouseleave = mouseLeave; // function() {};

    var originalMouseEnter = kendo.ui.Menu.fn._mouseenter;
    var mouseEnter = function (e) {
        clearTimeout(this._timeoutHandle);
        originalMouseEnter.call(this, e);
    }
       
    var pivotGrid = $("#" + IDPivotGrid).data("kendoPivotGrid");
    if (pivotGrid != null) {
        pivotGrid.destroy();
        $("#" + IDPivotGrid).empty();
    }
    var pivotConfigurator = $("#" + IDPivotConfigurator);
    if (pivotConfigurator != null) {
        //pivotConfigurator.destroy();
        $("#" + IDPivotConfigurator).empty();
    }

    kendo.ui.Menu.fn._mouseenter = mouseEnter;

    var kendo_pivot_grid = $("#" + IDPivotGrid).kendoPivotGrid(
      {
          dataSource: dataSourceKendoPivotGrid,
          sortable: parametriKendoPivotGrid.sortable,
          filterable: parametriKendoPivotGrid.filterable,
          height: parametriKendoPivotGrid.height,
          rowHeaderTemplate: parametriKendoPivotGrid.rowHeaderTemplate,
          columnHeaderTemplate: parametriKendoPivotGrid.columnHeaderTemplate,
          dataCellTemplate: parametriKendoPivotGrid.dataCellTemplate,
          dataBinding: onDataBindingKendoPivotGrid,
          dataBound: onDataBoundKendoPivotGrid,
          collapseMember: collapseMember,
          expandMember: expandMember,          
          excel: {
              filterable: true
          },
          pdf: {
              author: "Agronica Group"
          },
          messages: {
              measureFields: "Trascina qui i valori",
              columnFields: "Trascina qui le colonne",
              rowFields: "Trascina qui le righe",                                          
              fieldMenu: {
                info: "Mostra dati con valore che:",
                sortAscending: "Ordinamento A...Z",
                sortDescending: "Ordinamento Z...A",
                filterFields: "Filtro campi",
                filter: "Filtra",
                include: "Includi Campi ...",
                title: "Campi da includere",
                clear: "Pulisci",
                ok: "Ok",
                cancel: "Cancella",
                operators: {
                    contains: "Contiene",
                    doesnotcontain: "Non contiene",
                    startswith: "Inizia con",
                    endswith: "Finisce con",
                    eq: "Uguale a",
                    neq: "Non uguale a"
                }
            }
          }
      }).data("kendoPivotGrid");
    


    var kendo_pivot_configurator = $("#" + IDPivotConfigurator).kendoPivotConfigurator({
        dataSource: dataSourceKendoPivotGrid,
        filterable: true,
        height: parametriKendoPivotConfigurator.height,
        messages: {
            measures: "Trascina qui i valori",
            columns: "Trascina qui le colonne",
            rows: "Trascina qui le righe",
            measuresLabel: "Valori",
            rowsLabel: "Righe",
            columnsLabel: "Colonne",
            fieldsLabel: "Campi",
            fieldMenu: {
                info: "Mostra dati con valore che:",
                sortAscending: "Ordinamento A...Z",
                sortDescending: "Ordinamento Z...A",
                filterFields: "Filtro campi",
                filter: "Filtra",
                include: "Includi Campi ...",
                title: "Campi da includere",
                clear: "Pulisci",
                ok: "Ok",
                cancel: "Cancella",
                operators: {
                    contains: "Contiene",
                    doesnotcontain: "Non contiene",
                    startswith: "Inizia con",
                    endswith: "Finisce con",
                    eq: "Uguale a",
                    neq: "Non uguale a"
                }
            }
        }
    });

    function onDataBindingKendoPivotGrid(e) {
        if (funzioniPrimaDopoEventiPivotGrid.funzioneDaChiamarePrimaDelDataBinding != null)
            funzioniPrimaDopoEventiPivotGrid.funzioneDaChiamarePrimaDelDataBinding(e);

        if (funzioniPrimaDopoEventiPivotGrid.funzioneDaChiamareDopoDataBinding != null)
            funzioniPrimaDopoEventiPivotGrid.funzioneDaChiamareDopoDataBinding(e);
    }

    function onDataBoundKendoPivotGrid(e) {
        if (funzioniPrimaDopoEventiPivotGrid.funzioneDaChiamarePrimaDelDataBound != null)
            funzioniPrimaDopoEventiPivotGrid.funzioneDaChiamarePrimaDelDataBound(e);


        var gridId = e.sender.element[0].id;
        var grid = $("#" + gridId).data("kendoPivotGrid");
        var data = grid.dataSource.data();

        if (parametriKendoPivotGrid.chartCfg !== undefined) {

            //divchart, group, category, format, type) {
                initChart(
                    convertData(grid.dataSource, collapsed),
                    parametriKendoPivotGrid.chartCfg.divchart, //"#divKendoChartCMaturazione",
                    parametriKendoPivotGrid.chartCfg.group, //"column",
                    parametriKendoPivotGrid.chartCfg.category, //"row",
                    parametriKendoPivotGrid.chartCfg.format, //"{0}",
                    parametriKendoPivotGrid.chartCfg.type, //"line"
                    parametriKendoPivotGrid.chartCfg.sort //"line"
                );
        }

        if (funzioniPrimaDopoEventiPivotGrid.funzioneDaChiamareDopoDataBound != null)
            funzioniPrimaDopoEventiPivotGrid.funzioneDaChiamareDopoDataBound(e);
    }

    function collapseMember(e) {
        if (funzioniPrimaDopoEventiPivotGrid.collapseMember != null)
            funzioniPrimaDopoEventiPivotGrid.collapseMember(e);
    }

    function expandMember(e) {
        if (funzioniPrimaDopoEventiPivotGrid.expandMember != null)
            funzioniPrimaDopoEventiPivotGrid.expandMember(e);
    }   


    $("#exportExcel").click(function () {
        //if (confirm("Vuoi espandere i primi due livelli di righe e colonne?"))
        //{
        //    var dataSource = kendo_pivot_grid.dataSource;
        //    var col = dataSource.columns();
        //    var row = dataSource.rows();

        //    dataSource.expandColumn(col[0].name[0]);
        //    var children = dataSource.axes().columns.tuples[0].members[0].children;
        //    for (y = 0; y < children.length; y++) {
        //        if (children[y].members.length > 1)
        //        {
        //            var ar = [];
        //            var name = children[y].members[0].name;
        //            var nextLevel = children[y].members[1].name;
        //            if (nextLevel != "Measures")
        //            {
        //                ar.push(name);
        //                ar.push(nextLevel);
        //                dataSource.expandColumn(ar);
        //            }
        //        }
        //    }

        //    dataSource.expandRow(row[0].name[0]);
        //    var children = dataSource.axes().rows.tuples[0].members[0].children;
        //    for (y = 0; y < children.length; y++) {
        //        if (children[y].members.length > 1)
        //        {
        //            var ar = [];
        //            var name = children[y].members[0].name;
        //            var nextLevel = children[y].members[1].name;
        //            if (nextLevel != "Measures") {
        //                ar.push(name);
        //                ar.push(nextLevel);
        //                dataSource.expandRow(ar);
        //            }
        //        }
        //    }
        //}

        kendo_pivot_grid.saveAsExcel();
    });

    $("#exportPdf").click(function () {
        kendo_pivot_grid.saveAsPDF();
    });

    return kendo_pivot_grid;

}

 