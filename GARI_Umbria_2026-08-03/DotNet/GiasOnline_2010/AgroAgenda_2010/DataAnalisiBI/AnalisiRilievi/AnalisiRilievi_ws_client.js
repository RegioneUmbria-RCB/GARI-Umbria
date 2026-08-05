var indirizzohttp = "./AnalisiRilievi.aspx";

function ParAnalisi_Inizializza() {
    var rval = [];

    ajaxAgronicaSync(url_meteo_ws + "/ParametriAnalisi_Elenco",
        "",
        true,
        function (risposta) {

            rval = JSON.parse(risposta.RispostaStringa);
        },
        null
    );

    return rval;
}

function analizzaRilievi(successCallback) { // ex CaricaRigheMaturazione

    if (typeof successCallback !== "function") {

        successCallback = analizzaRilieviSuccess;
    }

    let ddl = $("#cmbParametroAnalisi").getKendoDropDownList();
    let Par_Cod = ddl.value().split("|");
    if (Par_Cod.length < 2) {
        return;
    }

    let DSS_Analisi_tipologia_elaborazione = Par_Cod[0];
    let SchemaDiElaborazione = Par_Cod[1];
    let IDTestataTemp = 0;
    if (filtroneImpostato) {
        IDTestataTemp = parseInt(document.getElementById(idTestataTemp).value);
    }
    let param = {
        DataDa: $("#txt_DataDa").val(),
        DataA: $("#txt_DataA").val(),
        DSS_Analisi_tipologia_elaborazione: DSS_Analisi_tipologia_elaborazione,
        SchemaDiElaborazione: SchemaDiElaborazione,
        IDTestataTemp: IDTestataTemp
    };

    ajaxAgronica(url_meteo_ws + "/KendoPivotInizializza_Curve",
        JSON.stringify(param),
        function (risposta) {

            let kendodata = JSON.parse(risposta.RispostaStringa);

            successCallback(kendodata, DSS_Analisi_tipologia_elaborazione);
        },
        function (risposta) {
            meteoAlert(TraduzioneMultiResx(datiMeteoResx, "AnalizzaRilievi", "Analizza rilievi"), risposta.Errore);
        }
    );
}


function analizzaRilieviSuccess(kendodata, DSS_Analisi_tipologia_elaborazione) {

    if (parseInt(DSS_Analisi_tipologia_elaborazione) === 1) {
        kendodata.kendo_columns.splice(0, 0,
            {
                command: {
                    iconClass: "fa fa-pencil-square-o fa-lg",
                    text: "&nbsp",
                    name: "OpAgenda",
                    click: function (e) {
                        e.preventDefault();
                        // e.target is the DOM element representing the button
                        let tr = $(e.target).closest("tr"); // get the current table row (tr)
                        // get the data bound to the current table row
                        let data = this.dataItem(tr);
                        _editCommand(data);
                    }
                },
                title: TraduzioneMultiResx(datiMeteoResx, "Operazioni", "Operazioni"),
                headerAttributes: {
                    "class": "disable-reorder"
                }
            }
        );
    }

    kendodata.footer = [
        { field: "Data", aggregate: "count", template: "<div style='text-align:right'>" + TraduzioneMultiResx(datiMeteoResx, "Elementi", "Elementi") + ": #:kendo.toString(count, '0')#</div>" },
        { field: "sup_imp", aggregate: "sum", template: "<div style='text-align:right'>#= sumDistinctSupImp(data)#</div>" }
    ];

    var divKendoGrid = kendoGrid_Inizializza_2(kendodata);

    GlobalMeteoTabstrip.showTabDati(true);

    $("#" + divKendoGrid).css("opacity", "0");

    _creaToolBar(divKendoGrid);

    $("#" + divKendoGrid).css("opacity", "1");

    _creaButtonGroup(divKendoGrid);

    //Operazione lentissima...
    let grid = $("#" + divKendoGrid).data("kendoGrid");
    if (grid.dataSource.data().length > 0) {
        for (let c = 0; c < grid.columns.length; c++) {
            grid.autoFitColumn(c);
        }
    }
}

function sumDistinctSupImp(data) {
    let sum = 0;
    let divGrid = kendoGetGridDiv();
    let grid = $("#" + divGrid).data("kendoGrid");
    if (grid !== undefined) {
        let data = grid.dataSource.view();
        let idx = 0;
        let seen = {};

        while (idx < data.length) {
            let item = data[idx++];
            let aF = item.ChiaveImpianto.split("_");
            aF.splice(-1);
            let text = "F_" + aF.join("_");

            if (!seen.hasOwnProperty(text)) {
                sum += Math.max(0, item.sup_imp);
                seen[text] = true;
            }
        }
    }
    if (sum > 0) {
        return TraduzioneMultiResx(datiMeteoResx, "Totale", "Totale") + ": " + kendo.toString(sum, '0.0000');
    }
    return "";
}

function _editCommand(data) {

    //let chiave = $("#divKendoOut_Child1").data("kendoGrid").dataSource.data()[0].ChiaveImpianto.split("_");
    let chiave = data.ChiaveImpianto.split("_");
    let lav_cod = 0;
    let chiaveAlberoCodes = "63§" + chiave[0] + "§" + chiave[1] + "§0§" + chiave[2] + "§" + chiave[3] + "§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§" + chiave[4] + "§§0§0";
    let lat = "";
    let lng = "";
    let idAgenda = chiave[chiave.length - 1];
    let param = "{ lav_cod: '" + lav_cod + "', ChiaveAlbero: '" + chiaveAlberoCodes + "', lat: '" + lat + "', lng: '" + lng + "', idAgenda: '" + idAgenda + "' }";

    ajaxAgronica("../../../Gis/Gis.aspx/clickOpAgendaBS_idAgenda",
        param,
        function (risposta) {
            ModalKendoApri("../" + risposta.RispostaStringa, "Op. Agenda"); //i18n
        }, null);

}

function _creaToolBar(divKendoGrid) {

    let grid = $("#" + divKendoGrid).getKendoGrid();
    let gridTB = $("#" + divKendoGrid).find(".k-grid-toolbar");

    // Create the column menu items.
    let id_menu = divKendoGrid + "-column-menu";
    let $menu = $("<ul id='" + id_menu + "'></ul>");

    let observable = {};

    // Loop over all the columns and add them to the column menu.
    $.each(grid.columns, function (idx, column) {

        // A column must have a title to be added.
        if ($.trim(column.title).length > 0) {

            if (column.field !== undefined) {
                if (column.field.substr(0, 4) === "Col_") {

                    if (grid.dataSource.options.schema.model.fields[column.field].type === "number") {
                        // Add columns to the column menu.
                        //$menu.append(kendo.format("<li>" +
                        //    "<input type='checkbox' id='{0}'class='k-checkbox' data-field='{0}' data-title='{1}' {2}>" +
                        //    "<label class='k-checkbox-label' for='{0}' style='margin: 1px 0px 1px 0px; width: 100%;'>{1}</label>" +
                        //    "</li>",
                        //    column.field, column.title, column.hidden ? "" : "checked"));
                        $menu.append(kendo.format("<li>" +
                            "<input type='checkbox' id='{2}' class='k-checkbox' data-bind='checked: {0}, events: { change: ColumnCheck }' value='{0}'>" +
                            "<label class='k-checkbox-label' for='{2}' style='margin: 1px 0px 1px 0px; width: -webkit-fill-available;'>{1}</label>" +
                            "</li>",
                            column.field, column.title, divKendoGrid + "-menu-chk-" + column.field));

                        observable[column.field] = !column.hidden;
                    }
                }
            }
        }
    });

    let id_columnBtn = divKendoGrid + "-columnBtn";
    gridTB.append("<div id='" + id_columnBtn + "' class='k-button'><span class='fa fa-columns'></span>" + TraduzioneMultiResx(datiMeteoResx, "GestisciColonne", "Gestisci colonne") + "</div>");
    gridTB.append($menu);

    $("#" + id_menu).kendoContextMenu({
        appendTo: "#" + divKendoGrid,
        target: "#" + id_columnBtn,
        closeOnClick: false,
        alignToAnchor: true,
        showOn: "click"//,
        //select: function (e) {
        //    // Get the selected column.
        //    var $item = $(e.item);
        //    var $input = $item.find(":checkbox");
        //    if ($input.attr("disabled") || $item.attr("data-role") === "menutitle") {
        //        return;
        //    }

        //    let datafield = $input.attr("data-field");
        //    //console.log(datafield + " " + $input.is(":checked"));
        //    let c = 0;
        //    while (c < grid.columns.length) {
        //        let column = grid.columns[c];
        //        if (column.field === datafield) {
        //            if ($input.is(":checked")) {
        //                grid.showColumn(column);
        //            } else {
        //                grid.hideColumn(column);
        //            }
        //            c = grid.columns.length;                    
        //        }
        //        c++;
        //    }
        //}
    });

    observable.ColumnCheck = function (e) {
        let c = 0;
        while (c < grid.columns.length) {
            let column = grid.columns[c];
            if (column.field === e.target.value) {
                if (e.data[e.target.value]) {
                    grid.showColumn(column);
                } else {
                    grid.hideColumn(column);
                }
                c = grid.columns.length;
            }
            c++;
        }
    };
    window.kendo.bind($("#" + id_menu), kendo.observable(observable));


    //gridTB.append('<div id="columnBtn"><span class="fa fa-columns"></span><span id="columnMenuButton"></span>Gestisci colonne</div>');

    //$("#columnMenuButton").kendoColumnMenu({
    //    filterable: false,
    //    sortable: false,
    //    dataSource: grid.dataSource,
    //    columns: grid.columns,
    //    owner: grid,
    //    open: function (e) {
    //        var menu = e.container.children().data("kendoMenu");
    //        menu.open(menu.element.find("li:first"));
    //    }
    //});

    //$("#columnMenuButton").find("span").removeClass("k-icon k-i-more-vertical");

    //$("#columnBtn").kendoButton({
    //    click: function (e) {
    //        e.sender.element.find("a").click();
    //    }
    //});


    let id_refreshChartBtn = divKendoGrid + "-refreshChartBtn";
    gridTB.append("<div id='" + id_refreshChartBtn + "'><span class='fa fa-refresh'></span>" + TraduzioneMultiResx(datiMeteoResx, "AggiornaGrafico", "Aggiorna grafico") + "</div>");

    $("#" + id_refreshChartBtn).kendoButton({
        click: function (e) {

            _creaGrafico(divKendoGrid, parseInt($('#' + divKendoGrid).attr("chart-idx")));
        }
    });

    let id_backToGIS = divKendoGrid + "-backToGIS";
    gridTB.append("<div id='" + id_backToGIS + "' style='float: right;'><span class='fa fa-globe'></span>" + TraduzioneMultiResx(datiMeteoResx, "TornaAlGIS", "Torna al GIS") + "</div>");

    $("#" + id_backToGIS).kendoButton({
        click: function (e) {

            if (!filtroneImpostato) {
                document.getElementById(idTestataTemp).value = 0;
            }

            // Libreria linq.js eliminata, se questo pezzo di codice serve va riscritto con vanilla javascript
            //document.getElementById(idFiltroGriglia).value = "";
            //let dataSource = grid.dataSource;
            //let filters = dataSource.filter();
            //if (filters != undefined && filters != null) {

            //    let allData = dataSource.data();
            //    let query = new kendo.data.Query(allData);
            //    let data = query.filter(filters).data;
            //    let filtered = $.Enumerable.From(data).Select(function (x) {
            //        let obj = new Object
            //        obj.ChiaveImpianto = x.ChiaveImpianto;
            //        return obj;
            //    }).Distinct().ToArray();

            //    document.getElementById(idFiltroGriglia).value = JSON.stringify(filtered);
            //}

            $("#" + id_Btn_BackToGIS).click();
        }
    });
}

function _creaButtonGroup(divKendoGrid) {

    let meteoOutput = $("#divKendoOut").data("meteoOutput");

    let div_id = meteoOutput.creaDiv();

    $("#" + div_id).css("text-align", "center");
    $('<div id="selectChart"> ' +
        '<span class="customBtn">' + TraduzioneMultiResx(datiMeteoResx, 'GraficoAdIstogramma', 'Grafico ad istogramma') + '</span>' +
        '<span class="customBtn">' + TraduzioneMultiResx(datiMeteoResx, 'GraficoALinee', 'Grafico a linee') + '</span>' +
        '<span class="customBtn">' + TraduzioneMultiResx(datiMeteoResx, 'GraficoRadarPerData', 'Grafico radar per data') + '</span>' +
        '<span class="customBtn">' + TraduzioneMultiResx(datiMeteoResx, 'GraficoRadarPerImpianto', 'Grafico radar per impianto') + '</span>' +
        '</div>').appendTo("#" + div_id);

    //$("#" + div_id).css({ "margin-top": "25px" });

    //var selectChart = '<div id="selectChart" style="position: absolute; left: 50%; transform: translateX(-50%);">'+
    //    '<span class="customBtn">Grafico ad istogramma</span>' +
    //    '<span class="customBtn">Grafico a linee</span>' +
    //    '<span class="customBtn">Grafico radar</span>' +
    //    '</div>';

    //var btnsDiv = '<div id="chartToolBar">' +
    //    '<div id="refreshChart"><span class="fa fa-refresh"></span>Aggiorna grafico</div>' +
    //    selectChart +
    //    '</div>';

    //$(btnsDiv).appendTo("#" + div_id);

    //$("#refreshChart").kendoButton({
    //    click: function (e) {

    //        var currentChart = $("#selectChart").data("kendoButtonGroup").current().index();
    //        CurveMaturazione_CreaGrafico(divKendoGrid, currentChart);
    //    }
    //});

    $("#selectChart").kendoButtonGroup({
        //index: 0,
        select: function (e) {

            let attr_chart_idx = parseInt($('#' + divKendoGrid).attr("chart-idx"));
            let chart_idx = this.current().index();
            if (attr_chart_idx !== chart_idx) {

                $("#" + divKendoGrid).attr("chart-idx", chart_idx);

                _creaGrafico(divKendoGrid, chart_idx);
            }
        }
    });

    $.fn.widest = function () {
        return this.length ? this.width(Math.max.apply(Math, this.map(function () {
            return $(this).width();
        }))) : this;
    };

    $(".customBtn").widest();
}

function _creaGrafico(divGridData, tipoGrafico) {

    let meteoOutput = $("#divKendoOut").data("meteoOutput");
    meteoOutput.clearCharts();

    if (tipoGrafico !== 0 && tipoGrafico !== 1 && tipoGrafico !== 2 && tipoGrafico !== 3) {
        return;
    }

    let loader = new PulseLoader();

    loader.show(TraduzioneMultiResx(datiMeteoResx, "ElaborazioniGrafici", "Elaborazione grafici..."));

    setTimeout(function () {

        let grid = $("#" + divGridData).data("kendoGrid");

        __creaGrafico(grid.dataSource, grid.columns, tipoGrafico);

        loader.hide();

    }, 10);
}

function __creaGrafico(dataSource, columns, tipoGrafico) {

    let ds_rows = dataSource.data();
    let filters = dataSource.filter();
    if (filters !== undefined && filters !== null) {
        let query = new kendo.data.Query(ds_rows);
        ds_rows = query.filter(filters).data;
    }

    let categs = [];
    let fields = [];

    $.each(columns, function (idx, col) {

        if (col.hidden != true && col.field !== undefined && col.field.substr(0, 4) === "Col_") {
            if (dataSource.options.schema.model.fields[col.field].type === "number") {
                categs.push(col.title);
                fields.push(col.field);
            }
        }
    });

    if (tipoGrafico === 0 || tipoGrafico === 1) {

        //Grafico Barre/Linee per data

        let charts = [];
        $.each(fields, function (idx, field) {

            let axis = { title: { text: "" } };

            if (field === "Col__1") {
                //indice di ravaz
                axis.max = 16;
                axis.plotBands = [
                    { from: 8, to: 12, color: "green", opacity: 0.3 },
                    { from: 0, to: 8, color: "yellow", opacity: 0.3 },
                    { from: 12, to: 1000, color: "red", opacity: 0.3 }
                ];
            }

            charts.push({
                title: categs[idx],
                field: field,
                data: [],
                series: [],
                vertAxes: axis,
                seen: {}
            });
        });

        $.each(ds_rows, function (idx, row) {

            let aname = [row.Impresa, row.Azienda];
            let split_field = row.ChiaveImpianto.split("_"); //PIVA, sa_cod, appezza, id_reg, chiaveAnalisi
            if (split_field[2] !== "0") {
                aname.push(row.Appezzamento);
                split_field.splice(-1);
            } else {
                if (row.hasOwnProperty("Analisi")) {
                    aname.push(row.Analisi);
                }
            }
            let series_name = aname.join(" - ");
            let serie_field = "F_" + split_field.join("_");

            $.each(charts, function (c, chart) {

                let data = { Data: row.Data };
                data[serie_field] = row[chart.field];

                chart.data.push(data);

                if (!chart.seen.hasOwnProperty(serie_field)) {

                    chart.seen[serie_field] = true;

                    let ser = {
                        name: series_name,
                        field: serie_field,
                        type: "column"
                    };

                    if (tipoGrafico === 1) {
                        ser.type = "line";
                        ser.missingValues = "interpolate";
                        //ser.markers = { visible: true };
                    }

                    chart.series.push(ser);
                }
            });
        });

        $.each(charts, function (idx, chart) {
            creaKendoChart(chart.title, "Data", chart.vertAxes, chart.series, chart.data);
        });

    } else if (tipoGrafico === 2 || tipoGrafico === 3) {

        //Grafico Radar

        let charts = [];

        if (tipoGrafico === 2) {

            //Confronto tra impianti/analisi per stessa data...

            let curr_data = new Date('1900-01-01');
            let chart = null;

            $.each(ds_rows, function (idx, row) {

                if (row.Data.getTime() != curr_data.getTime()) {

                    curr_data = kendo.parseDate(row.Data);

                    if (chart !== null) {
                        charts.push(chart);
                    }
                    chart = {
                        title: TraduzioneMultiResx(datiMeteoResx, "AnalisiDelGiornoScelto_", "Analisi del ") + kendo.toString(curr_data, "dd/MM/yyyy"),
                        series: []
                    };
                }

                let aname = [row.Impresa, row.Azienda];
                let split_field = row.ChiaveImpianto.split("_"); //PIVA, sa_cod, appezza, id_reg, chiaveAnalisi
                if (split_field[2] !== "0") {
                    aname.push(row.Appezzamento);
                } else {
                    if (row.hasOwnProperty("Analisi")) {
                        aname.push(row.Analisi);
                    }
                }
                let serie = {
                    name: aname.join(" - "),
                    data: []
                };

                $.each(fields, function (f, field) {
                    serie.data.push(row[field]);
                });

                chart.series.push(serie);
            });

            if (chart != null) {
                charts.push(chart);
            }

        } else { //tipoGrafico === 3

            //Confronto tra date per stesso impianto/analisi...

            let seen = {};

            $.each(ds_rows, function (idx, row) {

                let aname = [row.Impresa, row.Azienda];
                let analisi = "";
                let split_field = row.ChiaveImpianto.split("_"); //PIVA, sa_cod, appezza, id_reg, chiaveAnalisi
                if (split_field[2] !== "0") {
                    aname.push(row.Appezzamento);
                    split_field.splice(-1);
                } else {
                    if (row.hasOwnProperty("Analisi")) {
                        analisi = " " + row.Analisi;
                    }
                }
                split_field.splice(-1);
                let serie_field = "F_" + split_field.join("_");

                let serie = {
                    name: kendo.format(TraduzioneMultiResx(datiMeteoResx, "AnalisiSpecificaDelGiornoScelto_", "Analisi{0} del "), analisi) + kendo.toString(kendo.parseDate(row.Data), "dd/MM/yyyy"),
                    data: []
                };

                $.each(fields, function (f, field) {
                    serie.data.push(row[field]);
                });

                if (seen.hasOwnProperty(serie_field)) {

                    charts[seen[serie_field]].series.push(serie);

                } else {

                    seen[serie_field] = charts.length;

                    charts.push({
                        title: aname.join(" - "),
                        series: [serie]
                    });
                }
            });
        }

        $.each(charts, function (idx, chart) {
            creaRadarChart(chart.title, categs, chart.series);
        });
    }
}

function FiltraImpiantiConFiltroRicercaNG() {
    var param = kendo.stringify({
        "piva": currentPiva
    });
    ajaxAgronica(indirizzohttp + "/Link_Pagina_FiltroRicercaNG",
        param,
        function (risposta) {
            apriFinestraFiltroRicercaNG(risposta.RispostaStringa)
        }, function (risposta) {
            kendo.alert(risposta.Errore)
    });
}

//where .disable-reorder class is for disabling column
kendo.ui.Grid.fn._reorderable = function (reorderable) {
    return function () {
        reorderable.call(this);

        var dropTargets = $(this.element).find('th.disable-reorder');

        dropTargets.each(function (idx, item) {
            $(item).data("kendoDropTarget").destroy();
        });

        var draggable = $(this.element).data("kendoDraggable");

        if (draggable) {

            draggable.bind("dragstart", function (e) {
                if ($(e.currentTarget).hasClass("disable-reorder"))
                    e.preventDefault();
            });

        }
    }
}(kendo.ui.Grid.fn._reorderable);

