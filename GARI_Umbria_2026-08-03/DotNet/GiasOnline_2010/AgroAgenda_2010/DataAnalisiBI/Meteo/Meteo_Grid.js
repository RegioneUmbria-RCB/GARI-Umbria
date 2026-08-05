


function kendoGrid_Inizializza(kendodata) {

    var divKendoGrid = creaGridDiv();

    if (typeof kendodata.group_columns === "object") {
        //Raggruppo...
        let icol = 0;
        while (icol < kendodata.kendo_columns.length) {

            let gruppo = kendodata.kendo_columns[icol].gruppoColonne;
            if (gruppo === undefined) {
                //non è una colonna da raggruppare ad un gruppo...
                icol++;
            } else {

                let col = kendodata.kendo_columns.splice(icol, 1)[0];

                let gruppi = gruppo.split("|");
                let columns = kendodata.kendo_columns;
                let parent_col = null;
                for (let g = 0; g < gruppi.length; g++) {

                    if (parent_col != null) {
                        columns = parent_col.columns;
                    }
                    let idx = columns.findIndex(elem => elem.title === gruppi[g]);
                    if (idx < 0) {
                        //non è ancora stata inserita una colonna per questo gruppo...
                        parent_col = {
                            title: gruppi[g],
                            columns: []
                        };
                        if (kendodata.group_columns.css_group !== undefined) {
                            parent_col.headerAttributes = { class: kendodata.group_columns.css_group };
                        }

                        if (g === 0) {
                            columns.splice(icol, 0, parent_col);
                            icol++;
                        } else {
                            columns.push(parent_col);
                        }
                    }
                    else {
                        parent_col = columns[idx];
                    }
                }

                parent_col.columns.push(col);
            }
        }
    }

    if (typeof kendodata.indicators != "undefined") {

        if (Array.isArray(kendodata.indicators)) {

            $.each(kendodata.indicators, function (idx, indic) {

                let colonna = column_for_field(kendodata.kendo_columns, indic.fieldVal);

                if (colonna != null) {

                    let fval = indic.fieldVal;
                    let tmpl = '# if (' + fval + ' != undefined && ' + fval + ' != null) { #';
                    if (!indic.fieldHidden) {
                        tmpl += '#: kendo.format("' + colonna.format + '", ' + fval + ') #';
                    }
                    if (indic.type === "colors") {

                        tmpl += '<div class="semaforo" style="background-color: #:' + indic.fieldClr + ' #"></div>';

                    } else if (indic.type === "numbers") {

                        let indic_class = "";
                        if (typeof indic.class === "string") {
                            indic_class = indic.class;
                        }

                        tmpl += '<div class="indicator-number level#:' + indic.fieldClr + ' #">';
                        let l = parseInt(indic.levels);
                        for (i = 0; i < l; i++) {

                            tmpl += '<div class="indicator-number-elem ' + indic_class + '"></div>';

                        }
                        tmpl += '</div>';

                    }
                    tmpl += '# } #';
                    colonna.template = tmpl;
                }
            });
        }
    }

    let divGridHdrTooltipContainer = impostaColonneTooltip(divKendoGrid, kendodata);

    let idExportBtn = divKendoGrid + "ExportBtn";
    let tbar_template = "";
    tbar_template += "<div id='" + idExportBtn + "' class='k-button'><span class='fa fa-file-excel-o' style='font-size:larger;'></span></div>";
    let tbar_css = {
        "padding": "7px",
        "display": "flex"
    };

    let xlsName = "Export tabella.xlsx";
    if (typeof kendodata.title === "string" && kendodata.title !== "") {
        tbar_template += "<div style='text-align:center; font-size:150%; margin:auto;'><span>" + kendodata.title + "</span></div>";
        tbar_template += "<div class='k-button'style='visibility:hidden;'><span class='fa fa-file-excel-o' style='font-size:larger;'></span></div>";
        tbar_css = $.extend({}, { "justify-content": "flex-end" }, tbar_css);

        xlsName = kendodata.title + ".xlsx";
    }

    var objConfig = {
        dataSource: {
            data: kendodata.kendo_rows,
            schema: {
                model: {
                    fields: kendodata.kendo_model
                }
            }
        },
        columns: kendodata.kendo_columns,
        resizable: true,
        //height: 450,
        pageable: false,
        toolbar: kendo.template(tbar_template),
        excel: {
            fileName: xlsName,
            filterable: false
        },
        excelExport: excelExport
    };

    //let grid_title = "";
    //if (typeof kendodata.title === "string" && kendodata.title !== "") {
    //    objConfig = $.extend({}, { toolbar: kendo.template("<div style='text-align: center; font-size: 150%;'><span>" + kendodata.title + "</span></div>") }, objConfig);
    //}
    //objConfig = $.extend({}, { toolbar: kendo.template("<div style='text-align: center; font-size: 150%;'><span>" + grid_title + "</span></div>") }, objConfig);

    $("#" + divKendoGrid).kendoGrid(objConfig);

    $("#" + divKendoGrid).find(".k-grid-toolbar").css(tbar_css);
    $("#" + divKendoGrid + " .k-grid-content").css("height", "400px");
    //$("#" + divKendoGrid + " .k-grid-content tbody tr td").css("line-height", "20px");

    if (divGridHdrTooltipContainer !== null) {

        let gridHdr = $("#" + divKendoGrid).find(".k-grid-header");
        gridHdr.append(divGridHdrTooltipContainer);

        $("#" + divKendoGrid).kendoTooltip({
            filter: "span.hasToolTip",
            showOn: "click",
            autoHide: false,
            animation: false,
            show: function (e) {
                this.popup.element.addClass("grid-tooltip");
                if (e.sender.arrow.hasClass("k-callout-n")) {
                    this.popup.element.css("margin-top", ".8em");
                } else {
                    this.popup.element.css("margin-top", "-.8em");
                }
                //this.popup.element.find(".k-tooltip-content").css({
                //    "margin": "20px 15px 15px",
                //    "padding-right": "0px"
                //});
                //this.popup.element.attr('style', function (i, s) { return s + 'opacity: 1 !important;' });
            },
            content: function (e) {
                let id = "#" + e.target.attr("data-id");
                return $(id).html();
            }
        });
    }

    $("#" + idExportBtn).click(function () {
        $("#" + divKendoGrid).data("kendoGrid").saveAsExcel();
    });

    return divKendoGrid; 
}

function impostaColonneTooltip(divKendoGrid, kendodata) {

    let divGridHdrTooltipContainer = null;

    if (kendodata.tooltip != undefined) {

        let arrTooltip = kendodata.tooltip;
        if (!Array.isArray(arrTooltip)) {
            arrTooltip = [arrTooltip];
        }

        for (itt = 0; itt < arrTooltip.length; itt++) {

            let col_tooltip = null;
            if (typeof arrTooltip[itt].column == "string") {
                col_tooltip = column_for_field(kendodata.kendo_columns, arrTooltip[itt].column);
            } else {
                col_tooltip = column_for_field(kendodata.kendo_columns, arrTooltip[itt].column.column, arrTooltip[itt].column.parent);
            }

            if (col_tooltip != null) {

                if (divGridHdrTooltipContainer === null) {
                    divGridHdrTooltipContainer = document.createElement("div");
                    divGridHdrTooltipContainer.id = divKendoGrid + "-tooltip-container";
                    divGridHdrTooltipContainer.style.cssText = "display: none;";
                }
                let divTT = document.createElement("div");
                divTT.id = divKendoGrid + "-tooltip-" + itt;
                divTT.innerHTML = arrTooltip[itt].content;
                divGridHdrTooltipContainer.appendChild(divTT);

                let hdrtmplt = "<div style='position:relative; padding-right:25px;'>";
                hdrtmplt += col_tooltip.title;
                hdrtmplt += "<div style='position:absolute; right:0; top:50%; transform:translateY(-50%); font-size:18px;'>";
                hdrtmplt += "<span class='fa fa-question-circle fa-fw hasToolTip' style='margin:0px; cursor:pointer;' data-id='" + divTT.id + "'></span>";
                hdrtmplt += "</div>";
                hdrtmplt += "</div>";

                col_tooltip.headerTemplate = hdrtmplt;
            }
        }
    }

    return divGridHdrTooltipContainer;
}

function columnArray(column) {
    let cArray = new Array;
    if (column.columns == undefined) {

        let col = { format: "" };
        // {0:...}
        if (typeof column.format === "string") {
            if (column.format.startsWith("{")) {
                let f = column.format.slice(1, -1);
                let pos = f.indexOf(":");
                if (pos > 0) {
                    f = f.slice(pos + 1);
                    pos = f.indexOf("\\");
                    if (pos >= 0) {
                        f = f.slice(0, pos) + f.slice(pos + 1);
                    }
                    col.format = f;
                }
            }
        }

        cArray.push(col);

    } else {
        for (let c = 0; c < column.columns.length; c++) {
            cArray = cArray.concat(columnArray(column.columns[c]));
        }
    }
    return cArray;
}

function excelExport(e) {
    let colArray = new Array();
    for (let c = 0; c < e.sender.columns.length; c++){
        colArray = colArray.concat(columnArray(e.sender.columns[c]));
    }

    let sheet = e.workbook.sheets[0];
    for (let r = 0; r < sheet.rows.length; r++) {

        let row = sheet.rows[r];
        if (row.type === "header") {

            for (let c = 0; c < row.cells.length; c++) {
                row.cells[c].background = "#428BCA";
                row.cells[c].verticalAlign = "center";
                if (row.cells[c].colSpan > 1) {
                    row.cells[c].textAlign = "center";
                }
            }

        } else if (row.type === "data") {

            for (let c = 0; c < row.cells.length; c++) {

                let val = row.cells[c].value;
                let col = (c < colArray.length ? colArray[c] : null);
                if (col != null) {

                    if (col.format !== "") {
                        if (typeof val === "number") {

                            row.cells[c].format = col.format;

                        } else if (Object.prototype.toString.call(val) === '[object Date]') {

                            row.cells[c].textAlign = "left";
                            row.cells[c].format = col.format;

                        }
                    }
                }
            }
        }
    }
}


/*
function kendoGrid_Inizializza_2__(kendodata) {

    let filterSource = new kendo.data.DataSource({
        data: []
    });

    for (let col = 0; col < kendodata.kendo_columns.length; col++) {

        if (kendodata.kendo_columns[col].field !== undefined) {

            if (kendodata.kendo_model[kendodata.kendo_columns[col].field].type === "number") {
                if (kendodata.kendo_columns[col].filterable != undefined) {
                    kendodata.kendo_columns[col].filterable = true
                }
            } else if (kendodata.kendo_model[kendodata.kendo_columns[col].field].type === "date") {
                if (kendodata.kendo_columns[col].filterable != undefined && kendodata.kendo_columns[col].filterable != false) {
                    kendodata.kendo_columns[col].filterable = true
                }
            } else {
                if (kendodata.kendo_columns[col].filterable != undefined && kendodata.kendo_columns[col].filterable != false) {
                    if (typeof kendodata.kendo_columns[col].filterable === "object") {
                        if (kendodata.kendo_columns[col].filterable.multi === true) {
                            kendodata.kendo_columns[col].filterable.dataSource = filterSource;
                        }
                    }
                }
            }

        }

    }

    var divKendoGrid = creaGridDiv();

    kendoRiempiGridData(divKendoGrid, kendodata.kendo_rows);

    let objConfig = {
        dataSource: {
            transport: {
                read: function (options) {
                    let hdKendoDati = $("#" + divKendoGrid).attr("kendo-data");
                    options.success(JSON.parse($("#" + hdKendoDati).val()));
                }
            },
            schema: {
                model: {
                    fields: kendodata.kendo_model
                }
            },
            change: function (e) {
                filterSource.data(e.items);
            }
        },
        columns: kendodata.kendo_columns,
        resizable: true,
        sortable: true,
        reorderable: true,
        filterable: true,
        filterMenuInit: function (e) {
            let grid = e.sender;
            e.container.data("kendoPopup").bind("open", function () {
                filterSource.sort({ field: e.field, dir: "asc" });

                let uniqueDsResult = [];
                let items = grid.dataSource.view();
                let index = 0;
                let seen = {};

                while (index < items.length) {
                    let item = items[index++];
                    let text = item[e.field];

                    if (text !== undefined && text !== null && !seen.hasOwnProperty(text)) {
                        uniqueDsResult.push(item);
                        seen[text] = true;
                    }
                }

                filterSource.data(uniqueDsResult);
            })
        },
        toolbar: ["excel"],
        excel: {
            allPages: true,
            filterable: true
        },
        pageable: false
    };

    $("#" + divKendoGrid).kendoGrid(objConfig);

    $("#" + divKendoGrid + " .k-grid-content").css("height", "400px");

    return divKendoGrid;

}
*/

function kendoGrid_Inizializza_2(kendodata) {

    let filterSource = new kendo.data.DataSource({
        data: []
    });


    for (let col = 0; col < kendodata.kendo_columns.length; col++) {

        if (kendodata.kendo_columns[col].field !== undefined) {

            if (kendodata.kendo_model[kendodata.kendo_columns[col].field].type === "number") {
                if (kendodata.kendo_columns[col].filterable != undefined) {
                    kendodata.kendo_columns[col].filterable = true
                }
            } else if (kendodata.kendo_model[kendodata.kendo_columns[col].field].type === "date") {
                if (kendodata.kendo_columns[col].filterable != undefined && kendodata.kendo_columns[col].filterable != false) {
                    kendodata.kendo_columns[col].filterable = true
                }
            } else {
                if (kendodata.kendo_columns[col].filterable != undefined && kendodata.kendo_columns[col].filterable != false) {
                    if (typeof kendodata.kendo_columns[col].filterable === "object") {
                        if (kendodata.kendo_columns[col].filterable.multi === true) {
                            kendodata.kendo_columns[col].filterable.dataSource = filterSource;
                        }
                    }
                }
            }

        }

    }

    var divKendoGrid = creaGridDiv();

    kendoRiempiGridData(divKendoGrid, kendodata.kendo_rows);

    var funzioniCRUD = {
        funzioneRead: function(options, parametri) {
            let hdKendoDati = $("#" + parametri[0]).attr("kendo-data");
            options.success(JSON.parse($("#" + hdKendoDati).val()));
        }
    };

    var idModel = "kendoKey";

    var parametriPerLettura = [divKendoGrid];
    var parametriDataSource = { };

    if (kendodata.footer !== undefined) {

        parametriDataSource.aggregate = [];

        for (let i = 0; i < kendodata.footer.length; i++) {
            let col = 0;
            while (col < kendodata.kendo_columns.length) {
                if (kendodata.kendo_columns[col].field === kendodata.footer[i].field) {
                    kendodata.kendo_columns[col].footerTemplate = kendodata.footer[i].template;
                    col = kendodata.kendo_columns.length;
                }
                col++;
            }

            parametriDataSource.aggregate.push({
                field: kendodata.footer[i].field,
                aggregate: kendodata.footer[i].aggregate
            });

        }
    }

    var parametriKendoGrid = {
        columnMenu: false,
        impostaColonneKendoGridDaCookie: false,
        toolbarCommands: [],
        excel: true,
        pdf: false,
        sortable: true,
        reorderable: true,
        groupable: false,
        pageable: false
        //height: 450
        //pageable: true,
        //pagesize: 20
    };

    var funzioniPrimaDopoEventi = { };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    creaKendoGrid(
        divKendoGrid, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        kendodata.kendo_model, // campi modello
        kendodata.kendo_columns, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );

    // nascondo la toolbar della kendogrid (in fondo rimane uno spazio inutilizzato come se la griglia non venisse ridimensionata correttamente)
    //$("#" + divKendoGrid + " .k-grid-toolbar").hide();

    $("#" + divKendoGrid + " .k-grid-content").css("height", "400px");



    let grid = $("#" + divKendoGrid).data("kendoGrid");
    let dataSource = grid.dataSource;

    dataSource.bind("change", function (e) {
        filterSource.data(e.items);
    });

    grid.bind("filterMenuInit", function (e) {
        let grid = e.sender;
        e.container.data("kendoPopup").bind("open", function () {

            let filterMultiCheck = grid.thead.find("[data-field=" + e.field + "]").data("kendoFilterMultiCheck");
            if (filterMultiCheck !== undefined) {

                filterSource.sort({ field: e.field, dir: "asc" });

                let uniqueDsResult = [];
                let items = grid.dataSource.view();
                let seen = {};
                for (let idx = 0; idx < items.length; idx++) {
                    let item = items[idx];
                    let text = item[e.field];

                    if (text !== undefined && text !== null && !seen.hasOwnProperty(text)) {
                        uniqueDsResult.push(item);
                        seen[text] = true;
                    }
                }

                filterSource.data(uniqueDsResult);

            }

        });
    });



    return divKendoGrid;
}



function title_for_field(kendogrid, field) {
    var columns = kendogrid.columns;
    var c = 0;
    var found = false;
    var title = "";
    while (!found && c < columns.length) {
        if (columns[c].columns == undefined) {

            if (columns[c].field == field) {
                found = true;
                title = columns[c].title;
            }

        } else {

            var cc = 0;
            while (!found && cc < columns[c].columns.length) {

                if (columns[c].columns[cc].field == field) {
                    found = true;
                    title = columns[c].title + " - " + columns[c].columns[cc].title;
                }
                cc++;
            }

        }
        c++;
    }
    return title;
}

function column_for_field(columns, field, takeParent) {
    if (takeParent == undefined)
        takeParent = false;
    var column;
    var c = 0;
    var found = false;
    while (!found && c < columns.length) {

        if (columns[c].columns == undefined) {

            if (columns[c].field == field) {
                found = true;
                column = columns[c];
            }

        } else {
            var cc = 0;
            while (!found && cc < columns[c].columns.length) {

                if (columns[c].columns[cc].field == field) {
                    found = true;
                    column = columns[c].columns[cc];
                }
                cc++;
            }
            if (takeParent && found) {
                column = columns[c];
            }
        }

        c++;
    }
    if (found) {
        return column;
    } else {
        return null;
    }
}

