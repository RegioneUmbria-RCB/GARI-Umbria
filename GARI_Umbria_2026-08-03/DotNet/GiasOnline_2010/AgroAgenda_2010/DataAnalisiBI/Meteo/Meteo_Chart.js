
function creaKendoChart(titolo, horzAxisCfg, vertAxes, series, datasource, divChart, areaHeight) {
    return _creaKendoChart(titolo, horzAxisCfg, vertAxes, series, datasource, divChart, areaHeight, false);
}

function creaKendoChart2(titolo, horzAxisCfg, vertAxes, series, datasource, divChart, areaHeight) {
    return _creaKendoChart(titolo, horzAxisCfg, vertAxes, series, datasource, divChart, areaHeight, true);
}

function _creaKendoChart(titolo, horzAxisCfg, vertAxes, series, datasource, divChart, areaHeight, sharedTooltip) {

    if (datasource.length === 0) {
        return "";
    }

    if (divChart === undefined) {
        divChart = creaChartDiv();
    }

    let domChart = document.getElementById(divChart);
    domChart.style.border = "1px solid #ccc";
    domChart.style.borderRadius = "4px";

    if (areaHeight === undefined) {
        areaHeight = 450;
    }

    if (typeof horzAxisCfg === "string") {

        horzAxisCfg = {
            field: horzAxisCfg,
            baseUnit: "days"
        }
    } else {

        if (horzAxisCfg.baseUnit == undefined || horzAxisCfg.baseUnit === "") {

            horzAxisCfg.baseUnit = "days";
        }
    }

    if (horzAxisCfg.labels_format == undefined || horzAxisCfg.labels_format === "") {

        let dt0 = new Date(datasource[0][horzAxisCfg.field]);
        let dt1 = new Date(datasource[datasource.length - 1][horzAxisCfg.field]);

        let y0 = dt0.getFullYear();
        let y1 = dt1.getFullYear();

        if (y0 != y1) {

            horzAxisCfg.labels_format = "dd/MM/yyyy";
        } else {

            let m0 = dt0.getMonth();
            let m1 = dt1.getMonth();

            if (m0 != m1) {

                horzAxisCfg.labels_format = "dd/MM";

                if (horzAxisCfg.title == undefined)
                    horzAxisCfg.title = kendo.toString(dt0, 'yyyy');
            } else {

                horzAxisCfg.labels_format = "dd";

                if (horzAxisCfg.title == undefined)
                    horzAxisCfg.title = kendo.toString(dt0, 'MMMM yyyy');
            }
        }
        if (horzAxisCfg.baseUnit === "hours") {

            horzAxisCfg.labels_format += " HH'h'";

        } else if (horzAxisCfg.baseUnit === "minutes") {

            horzAxisCfg.labels_format += "\nHH:mm";
        }
    }

    let vertIndicator = null;
    if (horzAxisCfg.vertIndicator !== undefined) {
        vertIndicator = horzAxisCfg.vertIndicator;
    }

    let baseUnitStep = horzAxisCfg.baseUnitStep;
    
    horzAxisCfg = {
        field: horzAxisCfg.field,
        title: {
            text: horzAxisCfg.title
        },
        name: "date_axis",
        type: "date",
        baseUnit: horzAxisCfg.baseUnit,
        labels: {
            format: "{0:" + horzAxisCfg.labels_format + "}",
            step: Number.MAX_SAFE_INTEGER //valore molto grande lo reimposto nell'evento redraw...
        },
        majorGridLines: {
            visible: false
        },
        majorTicks: {
            visible: false
        },
        minorGridLines: {
            visible: true,
            dashType: "dot", //"dash",
            step: Number.MAX_SAFE_INTEGER
        },
        minorTicks: {
            visible: true,
            step: Number.MAX_SAFE_INTEGER
        }
    };

    if (sharedTooltip) {

        horzAxisCfg.crosshair = {
            visible: true,
            width: 1,
            dashType: "dash",
            color: "#bfbfbf"
        };
    }

    if (baseUnitStep !== undefined) {
        horzAxisCfg.baseUnitStep = baseUnitStep;
    }

    let plotBands = null;
    if (typeof vertAxes === "object") {

        if (vertAxes.hasOwnProperty('bands')) {

            plotBands = vertAxes.bands;
            vertAxes = vertAxes.axes;
        }
    }

    if (typeof vertAxes === "string") {

        vertAxes = [
            {
                title: {
                    text: vertAxes
                }
            }
        ];
    } else if (typeof vertAxes === "object") {

        if (!Array.isArray(vertAxes)) {

            vertAxes = [vertAxes];
        }
    }

    if (plotBands !== null) {

        let axisForBands = vertAxes[0];

        let axisName = "";
        if (typeof plotBands.Field === "string") {

            if (plotBands.Field !== "") {

                let s = 0;
                while (s < series.length) {

                    if (series[s].field === plotBands.Field) {
                        if (typeof series[s].axis === "string") {
                            axisName = series[s].axis;
                        }
                        s = series.length;
                    }
                    s++;
                }
            }
        }
        if (axisName !== "") {

            let a = 0
            while (a < vertAxes.length) {
                if (typeof vertAxes[a].name === "string") {
                    if (vertAxes[a].name === axisName) {
                        axisForBands = vertAxes[a];
                        a = vertAxes.length;
                    }
                }
                a++;
            }
        }

        axisForBands.plotBands = [];
        let from = 0;
        for (b = 0; b < plotBands.Bands.length; b++) {
            let band = plotBands.Bands[b];
            axisForBands.plotBands.push({
                from: from,
                to: band.StopValue,
                color: band.Color,
                opacity: band.Opacity
            });
            from = band.StopValue;
        }
    }

    if (vertAxes[0].majorGridLines == undefined) {
        vertAxes[0].majorGridLines = {
            dashType: "dot" //"dash"
        };
    }
        

    if (vertAxes.length > 1) {

        horzAxisCfg.axisCrossingValues = [new Date(1900, 0, 1), new Date(2100, 11, 31)];

        for (i = 1; i < vertAxes.length; i++) {
            if (i % 2 === 1 && vertAxes[i].title !== undefined) {
                vertAxes[i].title.rotation = 90;
            }
            if (i > 1)
                horzAxisCfg.axisCrossingValues.push(horzAxisCfg.axisCrossingValues[i % 2]);
        }
    }

    let grouped = false;
    let needSeriesClick = false;

    $.each(series, function (i, s) {

        if (sharedTooltip) {

            if (s.highlight === undefined) {

                s.highlight = { visible: false };

            } else {

                s.highlight.visible = false;
            }

            if (s.type === undefined || s.type === "line" || s.type === "area" || s.type === "rangeArea") {

                s.highlight.visual = visualSeriesHighlightCrosshair;
            }

        } else {

            let tooltipObj = s.tooltipTemplate;
            let type = typeof tooltipObj;

            if (type != "undefined") {

                needSeriesClick = true;

                delete s.tooltipTemplate;

                let strValue = "value";
                let strFormat = "0";
                let showSeriesName = false
                let strTemplate = "";
                if (type == "string") {

                    strFormat = tooltipObj;

                } else if (type == "object") {

                    if (tooltipObj.field != undefined) {
                        strValue = "dataItem['" + tooltipObj.field + "']";
                    }

                    if (tooltipObj.showSeriesName != undefined) {
                        showSeriesName = tooltipObj.showSeriesName;
                    }

                    strFormat = tooltipObj.format;

                    if (tooltipObj.hasOwnProperty("template")) {
                        strTemplate = tooltipObj.template;
                    }
                }

                if (s.highlight === undefined) {

                    s.highlight = { visible: true };
                } else {

                    s.highlight.visible = true;
                }

                if (strTemplate === "") {

                    let dateformat = "dd/MM/yyyy";

                    if (horzAxisCfg.baseUnit === "hours") {

                        dateformat = 'dd/MM/yyyy HH"h"';

                    } else if (horzAxisCfg.baseUnit === "minutes") {

                        dateformat = 'dd/MM/yyyy HH:mm';
                    }

                    strTemplate = "#: kendo.toString(category, '" + dateformat + "') #<br />";

                    if (showSeriesName) {

                        strTemplate += "#: series.name #: ";
                    }

                    if (s.type === "rangeArea") {

                        strTemplate += "#: kendo.toString(value.from, '" + strFormat + "') # ÷ #: kendo.toString(value.to, '" + strFormat + "') #"
                    } else {

                        strTemplate += "#: kendo.toString(" + strValue + ", '" + strFormat + "') #";
                    }
                }

                s.tooltip = {
                    visible: false,
                    template: strTemplate
                };

                if (s.highlight === undefined) {

                    s.highlight = { visible: true };

                } else {

                    s.highlight.visible = true;
                }

                if (s.type === undefined || s.type === "line" || s.type === "area" || s.type === "rangeArea") {

                    s.highlight.visual = visualSeriesHighlight;
                }
            }
        }

        grouped = grouped || s.hasOwnProperty("gruppo");
    });

    if (vertIndicator !== null) {

        let arrVI = [];
        if (Array.isArray(vertIndicator)) {

            arrVI = vertIndicator;

        } else {

            arrVI.push(vertIndicator);
        }

        $.each(arrVI, function (index, vertInd) {

            let objDef = {
                label: {
                    text: "",
                    color: "#000000",
                    font: "14px sans-serif"
                },
                line: {
                    color: "#000000",
                    width: 2,
                    dashType: "solid"
                }
            };

            if (typeof vertInd.field === "string") {

                if (vertInd.field.trim() !== "") {

                    let obj_ext = $.extend(true, {}, objDef, vertInd);

                    series.push({
                        name: obj_ext.label.text,
                        field: obj_ext.field,
                        color: obj_ext.line.color,
                        dashType: obj_ext.line.dashType,
                        width: obj_ext.line.width,
                        labels: {
                            font: obj_ext.label.font,
                            color: obj_ext.label.color
                        },
                        visibleInLegend: false,
                        markers: {
                            visible: true,
                            visual: visualVerticaIndicator
                        }
                    });
                }
            }
        });
    }

    vertAxes[0].axisCrossingValues = [Number.NEGATIVE_INFINITY];
    if (vertAxes[0].name === undefined || vertAxes[0].name === "") {

        vertAxes[0].name = "value_axis_zero";
    }

    let chartCfg = {
        theme: "Bootstrap", //"Material"
        chartArea: {
            height: areaHeight
        },
        transitions: false,
        title: {
            visible: (titolo != ""),
            text: titolo
        },
        legend: {
            position: "top",
            spacing: 25,
            item: {
                visual: visualLegendItem
            }
        },
        legendItemHover: function (e) {
            e.preventDefault(); // evito che si evidenzi tutta la serie se c'è l'highlight attivo sulla serie...
        },
        seriesDefaults: {
            type: "line",
            style: "smooth",
            missingValues: "gap",
            markers: {
                visible: false
            },
            highlight: {
                visible: false
            },
            tooltip: {
                visible: false
            }
        },
        pannable: {
            key: "ctrl",
            lock: "y"
        },
        zoomable: {
            selection: {
                key: "none",
                lock: "y"
            },
            mousewheel: false
        },
        dataSource: {
            data: datasource
        },
        categoryAxis: horzAxisCfg,
        valueAxes: vertAxes,
        series: series
    };

    if (needSeriesClick) {
        chartCfg.seriesClick = seriesClick;
    }

    let hasTouchCapabilities = 'ontouchstart' in window && (navigator.maxTouchPoints || navigator.msMaxTouchPoints);

    if (hasTouchCapabilities) {

        let stylesheet = document.getElementById("chart-touch-stylesheet");
        if (!stylesheet) {

            let css = ".k-chart { touch-action: pinch-zoom pan-y !important; }";

            let style = document.createElement('style');
            style.id = "chart-touch-stylesheet";
            style.type = 'text/css';
            style.innerHTML = css;

            document.getElementsByTagName('head')[0].appendChild(style);
        }

        //chartCfg.renderAs = "canvas";
        ////chartCfg.pannable = false;
        //chartCfg.pannable = {
        //    lock: "y"
        //};
        //chartCfg.zoomable = false;

        // *** MOBILE *** Zoom con gestures, non servono più i pulsanti +/-
        chartCfg.pannable = {
            lock: "y"
        };
        chartCfg.zoomable = {
            selection: {
                lock: "y"
            },
            mousewheel: {
                lock: "y"
            }
        };
    }

    let panes = [];
    $.each(vertAxes, function (idx, axis) {
        if (axis.hasOwnProperty("pane")) {

            if (typeof axis.pane === "object") {
                let found = false;
                let ipane = 0;
                while (!found && ipane < panes.length) {
                    found = (panes[ipane].name === axis.pane.name);
                    ipane++;
                }
                if (!found) {
                    panes.push(axis.pane);
                }
                axis.pane = axis.pane.name;
            }
        }
    });

    if (panes.length > 0) {
        chartCfg.panes = panes;
    }        

    $("#" + divChart).kendoChart(chartCfg);
    
    let chart = $("#" + divChart).getKendoChart();

    chartVisualHelper(chart);

    chart.redraw();

    aggiungiPulsantiChart(divChart, !hasTouchCapabilities);

    if (grouped) {

        chart.bind("legendItemClick", function (e) {

            $.each(this.options.series, function (index, series) {

                if (e.series.gruppo !== undefined) {

                    if (index !== e.seriesIndex) {

                        if (series.gruppo === e.series.gruppo) {

                            series.visible = !e.series.visible;
                        }
                    }
                }
            });
        });

        chart.one("dataBound", function (e) {

            let oColors = {};

            $.each(this.options.series, function (index, series) {

                if (series.gruppo !== undefined) {

                    let grp = "p" + series.gruppo;

                    if (!oColors.hasOwnProperty(grp)) {

                        oColors[grp] = series.color;
                    } else {

                        series.visibleInLegend = false;
                    }
                }
            });

            $.each(this.options.series, function (index, series) {

                if (series.gruppo !== undefined) {

                    series.color = oColors["p" + series.gruppo];
                }
            });
        });

        chart.refresh();
    }

    if (sharedTooltip) {

        creaSharedTooltip(divChart);
    }

    return divChart;
}

function creaSharedTooltip(divChart) {
    
    let stylesheet = document.getElementById("chart-tooltip-stylesheet");
    if (!stylesheet) {

        let css = ".custom-chart-tooltip { padding: 0px; display: block; overflow: hidden; background: #fefefe; border: 1px solid #ddd; color: #333; font-size: 12px; user-select: none; } ";
        css += ".chart-tooltip-hidden { display: none !important; } ";
        css += ".chart-tooltip-transparent { opacity: 0 !important; }";

        let style = document.createElement('style');
        style.id = "chart-tooltip-stylesheet";
        style.type = 'text/css';
        style.innerHTML = css;

        document.getElementsByTagName('head')[0].appendChild(style);
    }

    let chartElem = document.getElementById(divChart);
    let tooltipWrapper = document.createElement("div");
    tooltipWrapper.className = "custom-chart-tooltip k-tooltip chart-tooltip-hidden";
    chartElem.appendChild(tooltipWrapper);

    $(tooltipWrapper).chartSharedTooltip({
        parentChart: $("#" + divChart).getKendoChart()
    });
}

function seriesClick(e) {

    if (e.originalEvent.type === "contextmenu")
        e.originalEvent.preventDefault();     // Disable browser context menu

    if (e.series.tooltip == undefined || e.series.tooltip.visible == true)
        return;

    if (e.series.highlight == undefined || e.series.highlight.visible != true)
        return;

    e.sender.showTooltip(function (point) {

        if (point.series.field !== e.series.field)
            return false;

        return (point.category === e.category);
    });
}

function chartVisualHelper(chart) {

    //Occorre questa chiamata per non avere un errore se si utilizza la versione Kendo da "2019.1.115" in avanti
    chart.redraw();

    let axisOptions = chart.getAxis("date_axis").options;
    let str = kendo.format(axisOptions.labels.format, new Date(1900, 0, 1));
    let text = new kendo.drawing.Text(str,
        new kendo.geometry.Point(0, 0),
        { font: axisOptions.labels.font });

    let helper = {
        unit_factor: 1000 * 3600 * 24, // default per "days"
        label_width: text.bbox().size.width + 30,
        unitsPerRange: function (range) {
            return Math.round((range.max.getTime() - range.min.getTime()) / this.unit_factor);
        }
    };

    if (axisOptions.baseUnit === "hours") {
        helper.unit_factor = 1000 * 3600;
    }

    if (axisOptions.baseUnit === "minutes") {
        helper.unit_factor = 1000 * 60;
    }

    if (axisOptions.baseUnitStep !== undefined) {
        helper.unit_factor *= axisOptions.baseUnitStep;
    }

    helper.renderFunc = function (e) {

        let this_chart = e.sender;

        let horzAxis = this_chart.getAxis("date_axis");
        let bbox = this_chart.plotArea().backgroundVisual.bbox();

        //------------------------------------------------------------------
        // Questo codice può essere eseguito solo la prima volta
        // maxNumLabels non cambia (a meno di resize della finestra)
        let maxNumLabels = Math.round(bbox.size.width / this.label_width);
        //------------------------------------------------------------------

        let upr = this.unitsPerRange(horzAxis.range());
        let step = Math.max(1, Math.round(upr / maxNumLabels));

        if (horzAxis.options.labels.step != step) {

            let axis = this_chart.options.categoryAxis;
            if (!Array.isArray(axis)) {
                axis = [axis];
            }
            axis.forEach(function (elem) {
                elem.labels.step = step;
                elem.minorGridLines.step = step * 2;
                elem.minorGridLines.skip = 1;
                elem.minorTicks.step = step * 2;
                elem.minorTicks.skip = 1;
            });

            this_chart.unbind("render", render_handler);

            this_chart.redraw();

            this_chart.bind("render", render_handler);
        }
    };


    let render_handler = function (e) { helper.renderFunc(e) };
    chart.bind("render", render_handler);


    helper.paneRenderFunc = function (e) {

        if (e.index !== 0) {
            return;
        }
        if (!e.pane.chartsVisual) {
            return;
        }

        let this_chart = e.sender;
        let valueAxis;
        if (Array.isArray(this_chart.options.valueAxis)) {
            valueAxis = this_chart.getAxis(this_chart.options.valueAxis[0].name);
        } else {
            valueAxis = this_chart.getAxis(this_chart.options.valueAxis.name);
        }

        if (valueAxis.range().min * valueAxis.range().max >= 0) {
            return;
        }

        let horzAxis = this_chart.options.categoryAxis;
        if (Array.isArray(horzAxis)) {
            horzAxis = horzAxis[0];
        }

        let box = this_chart.plotArea().backgroundVisual.bbox();
        let slotZero = valueAxis.slot(0);
        let y = Math.round(slotZero.origin.y) + 0.5;
        let x0 = box.origin.x;
        let x1 = x0 + box.size.width;
        let left = new kendo.geometry.Point(x0, y);
        let right = new kendo.geometry.Point(x1, y);
        let path = new kendo.drawing.Path({ stroke: { color: horzAxis.line.color, width: 1, dashType: "solid" } });
        path.moveTo(left).lineTo(right);

        e.pane.chartsVisual.insert(0, path);
    }

    let paneRender_handler = function (e) { helper.paneRenderFunc(e) };
    chart.bind("paneRender", paneRender_handler);

    helper.zoomElem = null;
    helper.minPos = 0;
    helper.maxPos = 0;
    helper.zoomPos = 0;
    helper.cancelZoom = function () {
        if (this.zoomElem === null) {
            return;
        }
        this.zoomElem.remove();
        this.zoomElem = null;
    }

    helper.zoomStartFunc = function (e) {
        let $marquee = $(".k-marquee");
        if ($marquee.length > 0) {

            $(".k-marquee").css("display", "none");



            //$(".k-marquee").on('DOMNodeRemoved', function (e) {
            //    $(e.target).find('.element').each(function () {
            //        console.log(e);
            //    })
            //});        



            //$color = $(".k-marquee > .k-marquee-color");
            //if ($color.length > 0) {
            //    $(".k-marquee > .k-marquee-color").css({ "background-color": "rgb(64, 64, 64)", "opacity": "0.25" });
            //    //$(".k-marquee > .k-marquee-color").css({ "background-color": "rgb(128, 128, 128)", "opacity": "0.5", "border": "1px solid rgb(128, 128, 128)" });
            //}

            let parentElem = e.sender.element[0];
            this.zoomPos = e.originalEvent.x.location - e.originalEvent.x.initialDelta - $(parentElem).offset().left;

            let bbox = e.sender.plotArea().backgroundVisual.bbox();
            this.minPos = bbox.origin.x;
            this.maxPos = bbox.origin.x + bbox.size.width;
            this.zoomElem = document.createElement("div");
            this.zoomElem.style.cssText = "position:absolute; background-color: rgba(64, 64, 64, 0.25);  top:" + bbox.origin.y + "px; height:" + bbox.size.height + "px; left:" + this.zoomPos + "px; width: 0px;";
            parentElem.appendChild(this.zoomElem);
        }
    };

    helper.zoomFunc = function (e) {

        var delta = e.originalEvent.x.initialDelta;
        if (delta == undefined  //mousewheel event???
            || delta < 0) //zoom out
            return;

        this.cancelZoom();

        if (e.axisRanges.date_axis === undefined) {
            e.preventDefault();
            return;
        }

        if (this.unitsPerRange(e.axisRanges.date_axis) <= 2) // zoom al massimo di 2 baseUnit
            e.preventDefault();
    };

    helper.zoomEndFunc = function (e) {

        if (e.originalEvent !== undefined) {

            let delta = e.originalEvent.x.initialDelta;
            if (delta == undefined) //mousewheel event???
                return;

            this.cancelZoom();

            if (delta < 0) {// zoom out
                let optSeries = e.sender.options.series;
                e.sender.setOptions({ series: optSeries });
                return;
            }
        } else {

            this.cancelZoom();
        }

        //Se ho un asse verticale chiamata "dynamic_zoom" devo impostare dinamicamente il max dell'asse verticale (cha altrimenti avrebbe un max fisso)
        let axis_name = "dynamic_zoom";
        let this_chart = e.sender;
        if (this_chart.getAxis(axis_name) !== undefined) {
            let axis = null;
            if (Array.isArray(this_chart.options.valueAxis)) {
                let a = 0;
                while (axis === null && a < this_chart.options.valueAxis.length) {
                    if (this_chart.options.valueAxis[a].name === axis_name) {
                        axis = this_chart.options.valueAxis[a];
                    }
                    a++;
                }
            } else {
                if (this_chart.options.valueAxis.name === axis_name) {
                    axis = this_chart.options.valueAxis
                }
            }
            if (axis !== null) {
                delete axis.max;
            }
        }


        e.sender.redraw();
    };

    chart.bind("zoomStart", function (e) { helper.zoomStartFunc(e) });
    chart.bind("zoom", function (e) { helper.zoomFunc(e); });
    chart.bind("zoomEnd", function (e) { helper.zoomEndFunc(e) });

    helper.mousemove = function (e) {

        if (this.zoomElem !== null) {

            //let cursorX = e.clientX - e.currentTarget.offsetLeft;
            let cursorX = e.clientX - $(this.zoomElem.parentElement).offset().left;
            cursorX = Math.min(cursorX, this.maxPos);
            let left = this.zoomPos;
            let width = cursorX - left;
            if (width < 0) {
                left = Math.max(cursorX, this.minPos);
                width = this.zoomPos - left;
            }

            this.zoomElem.style.left = left + "px";
            this.zoomElem.style.width = width + "px";
        }
    };

    helper.mouseup = function (e) {

        //Baco in kendo se area selezionata per zoom troppo piccola (width ~ 0)
        if (this.zoomElem !== null) {

            //$($(this.zoomElem.parentElement).data("kendoChart")._zoomSelection._marquee).width();
            this.cancelZoom();
            //e.stopPropagation();
        }
    };

    $(chart.element).mousemove(function (e) { helper.mousemove(e) });
    $(chart.element).mouseup(function (e) { helper.mouseup(e); });
}

function visualLegendItem(e) {

    let color = {
        color: e.options.markers.background
    }

    if (e.series.opacity) {
        color.opacity = e.series.opacity;
    }
    //if (e.series.type === "radarLine") {
    //    fill = "none";
    //}
    let marker = new kendo.drawing.Rect(new kendo.geometry.Rect([0, 0], [10, 10]),
        {
            fill: color,
            stroke: color
        }
    );

    let label = new kendo.drawing.Text(e.series.name, [0, 0], {
        fill: {
            color: e.options.labels.color
        }
    });

    let layout = new kendo.drawing.Layout(new kendo.geometry.Rect([0, 0], [Number.MAX_SAFE_INTEGER, 0]),
        {
            spacing: 5,
            wrap: false,
            alignItems: "center",
            cursor: "pointer"
        }
    );

    layout.append(marker, label);
    layout.reflow()

    return layout;

    //var color = e.options.markers.background;
    //var labelColor = e.options.labels.color;

    //// Define the target dimensions for the legend item
    //var rect = new kendo.geometry.Rect([0, 0], [100, 50]);

    //// A layout will hold the checkbox and the default visual
    ////
    //// http://docs.telerik.com/kendo-ui/api/javascript/drawing/layout
    //var layout = new kendo.drawing.Layout(rect, {
    //    spacing: 5,
    //    alignItems: "center"
    //});

    //// Cheat a bit by rendering the checkbox using the Unicode ballot symbol
    ////
    //// http://docs.telerik.com/kendo-ui/api/javascript/dataviz/drawing/text
    //var cbSymbol = e.active ? "☑" : "☐";
    //var cb = new kendo.drawing.Text(cbSymbol, [0, 0], {
    //    fill: {
    //        color: labelColor
    //    },
    //    font: "14px sans-serif"
    //});
}

function aggiungiPulsantiChart(divChart, bHelp) {

    let chartElem = document.getElementById(divChart);

    chartElem.style.overflow = "hidden";

    if (!bHelp) {

        // *** MOBILE *** Non più necessario
        //let divZoomCtrl = document.createElement("div");
        //containerBtns.appendChild(divZoomCtrl);

        //$(divZoomCtrl).chartZoomCtrl({ chart: $("#" + divChart).getKendoChart() });

        return;
    }


    let cornerLeft_bk = document.createElement("div");
    cornerLeft_bk.style.position = "absolute";
    cornerLeft_bk.style.left = "-32px";
    cornerLeft_bk.style.top = "-32px";
    cornerLeft_bk.style.width= "64px";
    cornerLeft_bk.style.height = "64px";
    cornerLeft_bk.style.backgroundColor = "#f5f5f5";
    cornerLeft_bk.style.transform = "rotate(45deg)";
    cornerLeft_bk.style.cursor = "pointer";
    cornerLeft_bk.classList.add("gias-chart-corner-button-top-left");

    // 29/02/2024: Add hardcoded new styles

    if (GiasVersioneMaster === "2022") {
    //if (true) {
        cornerLeft_bk.style.backgroundColor = "#05315d";
    }

    chartElem.appendChild(cornerLeft_bk);

    let cornerLeft = document.createElement("div");
    cornerLeft.style.position = "absolute";
    cornerLeft.style.left = "-2px";
    cornerLeft.style.top = "-1px";
    cornerLeft.style.cursor = "pointer";
    cornerLeft.style.color = "#333";
    cornerLeft.style.fontSize = "20px";
    cornerLeft.style.pointerEvents = "none";

    // 29/02/2024: Add hardcoded new styles

    if (GiasVersioneMaster === "2022") {
    //if (true) {
        cornerLeft.style.color = "white";
    }

    chartElem.appendChild(cornerLeft);

    let spanFloppy = document.createElement("span");
    spanFloppy.className = "fa fa-floppy-o";
    spanFloppy.classList.add("gias-chart-corner-button-download");

    cornerLeft.appendChild(spanFloppy);

    cornerLeft_bk.onclick = function () {

        let this_chart = $(this).closest(".k-chart");

        let kendoChart = this_chart.getKendoChart();

        if (kendoChart === undefined) {
            return;
        }

        let fname = kendoChart.options.title.text;
        if (fname == "") {
            fname = "Grafico.png";
        }

        kendoChart.exportImage().done(function (data) {
            kendo.saveAs({
                dataURI: data,
                fileName: fname
            });
        });
    };



    let stylesheet = document.getElementById("chart-help-tooltip-stylesheet");
    if (!stylesheet) {

        let style = document.createElement('style');
        style.id = "chart-help-tooltip-stylesheet";
        style.type = 'text/css';

        let css = "";
        css += ".with-transition { transition: all 300ms ease-in; } ";
        css += ".chart-help-tooltip { ";
        css += "position: absolute; ";
        css += "top: 0; ";
        css += "right: 0; ";
        css += "transform: translate(0%, 0%); ";
        css += "font-size: 13px; ";
        css += "color: #333; ";
        css += "padding: 10px 10px 10px 30px; ";
        css += "background-color: #f5f5f5; ";
        css += "border-bottom-left-radius: 4px; ";
        css += "cursor: pointer; ";
        css += "pointer-events: all; ";
        css += "} ";
        css += ".chart-help-tooltip .corner { ";
        css += "position: absolute; ";
        css += "width: 64px; ";
        css += "height: 64px; ";
        css += "left: -64px; ";
        css += "top: -19px; ";
        css += "background-color: transparent; ";
        css += "transform-origin: bottom right; ";
        css += "transform: rotate(90deg); ";
        css += "} ";
        css += ".chart-help-tooltip .fa { ";
        css += "font-size: 22px; ";
        css += "position: absolute; ";
        css += "left: 20px; ";
        css += "bottom: 3px; ";
        css += "transform: rotate(270deg); ";
        css += "} ";
        css += ".chart-help-tooltip.chart-help-tooltip-hidden .corner { ";
        css += "transform: rotate(45deg) !important; ";
        css += "background-color: #f5f5f5; ";
        css += "} ";
        css += ".chart-help-tooltip.chart-help-tooltip-hidden .fa { ";
        css += "transform: rotate(315deg) !important; ";
        css += "} ";
        css += ".chart-help-tooltip-hidden { ";
        css += "transform: translate(100%, 0%) !important; ";
        css += "} ";

        style.innerHTML = css;

        document.getElementsByTagName('head')[0].appendChild(style);
    }

    let tooltip = document.createElement("div");
    tooltip.className = "chart-help-tooltip chart-help-tooltip-hidden with-transition";

    let corner = document.createElement("div");
    corner.className = "corner with-transition";
    corner.classList.add("gias-chart-corner-button-top-right");
    tooltip.appendChild(corner);

    let spanQuestion = document.createElement("span");
    spanQuestion.className = "fa fa-question with-transition";
    corner.appendChild(spanQuestion);

    let tt_content = document.createElement("div");
    tt_content.style.display = "grid";
    tt_content.style.gridTemplateColumns = "auto auto";
    tt_content.style.gap = "5px 10px";

    let a_content = [
        { hdr: "Zoom:", des: TraduzioneMultiResx(datiMeteoResx, "PulsanteSinistroMouseTrascinaSinistraDestra", "Pulsante sinistro del mouse e trascina da sinistra verso destra.") },
        { hdr: "Unzoom:", des: TraduzioneMultiResx(datiMeteoResx, "PulsanteSinistroMouseTrascinaDestraSinistra", "Pulsante sinistro del mouse e trascina da destra verso sinistra.") },
        { hdr: "Pan:", des: TraduzioneMultiResx(datiMeteoResx, "TastoControlPiùPulsanteSinistroTrascina", "Tasto Ctrl + pulsante sinistro del mouse e trascina.") },
        { hdr: "Mostra/Nascondi serie:", des: TraduzioneMultiResx(datiMeteoResx, "ClickSerieInLegenda", "Click serie in legenda.") },
    ];

    $.each(a_content, function (idx, elem) {
        let col0 = document.createElement("div");
        col0.style.fontWeight = "bold";
        col0.style.justifySelf = "end";
        col0.textContent = elem.hdr;
        let col1 = document.createElement("div");
        col1.textContent = elem.des;
        tt_content.appendChild(col0);
        tt_content.appendChild(col1);
    });

    tooltip.appendChild(tt_content);

    chartElem.appendChild(tooltip);

    tooltip.onclick = function () {
        $(this).toggleClass("chart-help-tooltip-hidden");
        $(this).find('.gias-chart-corner-button-top-right').toggleClass("gias-chart-corner-button-top-right-open");
    };
}

function visualColumnSeries(e) {

    if (e.value == null)
        return;

    let color = e.dataItem[e.series.colorField];
    if (color == "" || color == undefined)
        return;

/*    var gradient = new kendo.drawing.LinearGradient({
        type: "linear",
        start: [0.5, 1],
        end: [0.5, 0],
        stops: [
            {
                offset: 0,
                color: e.options.color,
                opacity: 0
            },
            {
                offset: 0.25,
                color: e.options.color,
                opacity: 0.25
            },
            {
                offset: 0.75,
                color: color,
                opacity: 0.75
            },
            {
                offset: 1,
                color: color
            }
        ]
    });
*/
    let stroke_color = e.options.color;
    if (stroke_color === "none") {
        stroke_color = color;
    }
    let path = new kendo.drawing.Path.fromRect(e.rect,
        {
            fill: { color: color },
            //fill: gradient,
            stroke: { color: stroke_color }
        }
    );

    return path;
}

function visualSeriesHighlight(e) {
    //let gradient = new kendo.drawing.RadialGradient({// Center and radius are relative to shape size
    //    center: [0.5, 0.5],
    //    radius: 0.5,
    //    stops: [
    //        {
    //            offset: 0,
    //            color: e.options.color,
    //            opacity: 1
    //        },
    //        {
    //            offset: 0.3,
    //            color: e.options.color,
    //            opacity: 1
    //        },
    //        {
    //            offset: 1,
    //            color: e.options.color,
    //            opacity: 0
    //        }
    //    ]
    //});

    let group = new kendo.drawing.Group();

    if (e.rect !== undefined) {

        let circleGeometry = new kendo.geometry.Circle(e.rect.center(), 3);
        let circle = new kendo.drawing.Circle(circleGeometry, {
            fill: { color: e.options.color },//gradient,
            stroke: { color: e.options.color }//null
        });

        group.append(circle);

    } else {

        //rangeArea???
        let pointFrom = e.from.rect.center();
        let pointTo = e.to.rect.center();
        let circleGeometryFrom = new kendo.geometry.Circle(pointFrom, 3);
        let circleGeometryTo = new kendo.geometry.Circle(pointTo, 3);
        let circleFrom = new kendo.drawing.Circle(circleGeometryFrom, {
            fill: { color: e.options.color },//gradient,
            stroke: { color: e.options.color }//null
        });
        let circleTo = new kendo.drawing.Circle(circleGeometryTo, {
            fill: { color: e.options.color },//gradient,
            stroke: { color: e.options.color }//null
        });
        let path = new kendo.drawing.Path({ stroke: { color: e.options.color, width: 1, dashType: "dash" } });
        path.moveTo(pointFrom);
        path.lineTo(pointTo);

        group.append(circleFrom);
        group.append(circleTo);
        group.append(path);
    }

    return group;
}

function visualSeriesHighlightCrosshair(e) {

    let group = visualSeriesHighlight(e);

    let x;

    if (e.rect) {

        x = e.rect.center().x;

    } else {

        x = e.from.rect.center().x;
    }

    let box = e.sender.plotArea().backgroundVisual.bbox();//.clippedBBox()
    let y0 = box.origin.y;
    let y1 = y0 + box.size.height;
    let top = new kendo.geometry.Point(x, y0);
    let bottom = new kendo.geometry.Point(x, y1);
    let path = new kendo.drawing.Path({ stroke: { color: "#bfbfbf", width: 1, dashType: "dash" } });
    path.moveTo(top);
    path.lineTo(bottom);

    group.insert(0, path);

    return group;
}

function visualVerticaIndicator(e) {

    let line = new kendo.drawing.Path({
        stroke: {
            color: e.series.color,
            width: e.series.width,
            dashType: e.series.dashType
        }
    });

    let x = e.rect.center().x;
    let box = e.sender.plotArea().backgroundVisual.bbox();//.clippedBBox()
    let y0 = box.origin.y;
    let y1 = y0 + box.size.height;

    line.moveTo(x, y0).lineTo(x, y1);

    let center = line.bbox().center();
    let label = new kendo.drawing.Text(e.series.name, center, {
        font: e.series.labels.font,
        fill: { color: e.series.labels.color },
        transform: kendo.geometry.transform().rotate(270, center)
    });
    let sz = label.bbox().size;
    label.transform(label.transform().translate(-(sz.height / 2), -(sz.width + 3)));

    return new kendo.drawing.Group().append(line, label);
}





(function ($) {

    $.chartZoomCtrl = function (elem, opts) {

        var plugin = this;

        plugin.$element = $(elem);
        plugin.element = elem;
        plugin.chart = opts.chart;
        plugin.min_dt = null;
        plugin.max_dt = null;

        let _setMinMax = function (min_dt, max_dt) {

            let axis = plugin.chart.options.categoryAxis;

            if (!Array.isArray(axis)) {
                axis = [axis];
            }
            
            $.each(axis, function (i, a) {
                a.min = min_dt;
                a.max = max_dt;
            });

            plugin.chart.refresh();
        }

        let _zoomIn = function () {

            let horz_axis = plugin.chart.getAxis("date_axis");

            let range = horz_axis.range();
            if (plugin.min_dt === null) {
                plugin.min_dt = new Date(range.min);
            }
            if (plugin.max_dt === null) {
                plugin.max_dt = new Date(range.max);
            }

            let min_dt = new Date(range.min);
            let max_dt = new Date(range.max);

            let currRange = max_dt.getTime() - min_dt.getTime();
            let rangePerc = currRange * 0.1;

            let ms_x_h = 60 * 60 * 1000;
            let minRange = 5 * ms_x_h; 
            let fact = ms_x_h;

            if (horz_axis.options.baseUnit == "days") {

                minRange = 5 * 24 * ms_x_h;
                fact = 24 * ms_x_h;
            }

            rangePerc = Math.max(1, Math.round(rangePerc / fact)) * fact;

            min_dt.setTime(min_dt.getTime() + rangePerc);
            max_dt.setTime(max_dt.getTime() - rangePerc);

            currRange = max_dt.getTime() - min_dt.getTime();

            if (currRange < minRange) {
                return;
            }

            _setMinMax(min_dt, max_dt);
        }

        let _zoomReset = function () {
            if (plugin.min_dt === null || plugin.max_dt === null) {
                return;
            }

            _setMinMax(plugin.min_dt, plugin.max_dt);
        }

        let divZoomIn = document.createElement("div");
        divZoomIn.className = "k-button";
        divZoomIn.style.cssText = "margin-right: 3px; padding: 1px 3px; pointer-events: all;";
        divZoomIn.innerHTML = "<span class='fa fa-search-plus fa-fw fa-2x'></span>";
        divZoomIn.onclick = _zoomIn;

        plugin.element.appendChild(divZoomIn);

        let divZoomReset = document.createElement("div");
        divZoomReset.className = "k-button";
        divZoomReset.style.cssText = "margin-left: 3px; padding: 1px 3px; pointer-events: all;";
        divZoomReset.innerHTML = "<span class='fa fa-search-minus fa-fw fa-2x'></span>";
        divZoomReset.onclick = _zoomReset;

        plugin.element.appendChild(divZoomReset);
    };

    $.fn.chartZoomCtrl = function (opts) {
        return this.each(function () {
            
            if (undefined == $(this).data('chartZoomCtrl')) {
                var plugin = new $.chartZoomCtrl(this, opts);

                $(this).data('chartZoomCtrl', plugin);
            }
        });
    };

})(jQuery);



(function ($) {

    $.chartSharedTooltip = function (elem, opts) {

        var plugin = this;

        plugin.element = elem;
        plugin.$element = $(elem);
        plugin.parentChart = opts.parentChart;
        plugin.horzAxis = plugin.parentChart.options.categoryAxis;

        if (Array.isArray(plugin.horzAxis)) {

            plugin.horzAxis = plugin.horzAxis[plugin.horzAxis.length - 1];
        }

        plugin.element.onclick = function (e) {
            _hideTooltip();
            _cancelHighlight();
        }

        let output_format = "dd/MM/yyyy";

        if (plugin.horzAxis.baseUnit === "hours") {

            output_format += " HH'h'";

        } else if (plugin.horzAxis.baseUnit === "minutes") {

            output_format += "\nHH:mm";
        }

        plugin.elemDataOra = document.createElement("div");
        plugin.elemDataOra.style.cssText = "padding: 4px 8px; background-color: #ddd; text-align: center; font-weight: bold;";
        $(plugin.elemDataOra).data("output-format", output_format);
        plugin.element.appendChild(plugin.elemDataOra);

        let divSeries = document.createElement("div");
        divSeries.className = "tooltip-series-grid";
        divSeries.style.cssText = "display: grid; grid-template-columns: auto auto auto; gap: 4px 8px; align-items: center; padding: 8px 8px 4px;"
        plugin.element.appendChild(divSeries);

        let _fillTooltip = function (category, gridElem) {

            let needNearest = (plugin.horzAxis.baseUnit === "minutes");
            let categories = [];

            $.each(plugin.parentChart.options.series, function (i, series) {

                let chartSeries = plugin.parentChart.findSeriesByName(series.name);

                if (chartSeries) {

                    let category_time = category.getTime();
                    let nearest_pt = null;
                    let nearest_diff = 0;
                    let pt_hl = null;

                    chartSeries.toggleHighlight(true, function (pt) {

                        let this_time = pt.category.getTime();

                        if (this_time == category_time) {

                            pt_hl = pt;
                            return true;

                        } else {

                            if (needNearest) {

                                if (nearest_pt === null) {

                                    nearest_pt = pt;
                                    nearest_diff = Math.abs(category_time - this_time);

                                } else {

                                    let curr_diff = Math.abs(category_time - this_time);

                                    if (curr_diff < nearest_diff) {

                                        nearest_pt = pt;
                                        nearest_diff = curr_diff;
                                    }
                                }
                            }
                        }
                        return false;
                    });

                    if (pt_hl == null && nearest_pt != null) {

                        let minutes = Math.abs(category_time - nearest_pt.category.getTime()) / (60 * 1000);

                        if (minutes < 10) {

                            pt_hl = nearest_pt;

                            chartSeries.toggleHighlight(true, pt_hl);
                        }
                    }

                    if (pt_hl != null) {

                        categories.push(pt_hl.category);

                        let output_format = "0.00";

                        let tooltipObj = series.tooltipTemplate;
                        let type = typeof tooltipObj;
                        if (type != "undefined") {

                            if (type == "string") {

                                output_format = tooltipObj;
                            }
                        }

                        let divColor = document.createElement("div");
                        divColor.style.cssText = "height: 12px; width: 12px; background-color: " + series.color + ";";
                        if (series.opacity) {
                            divColor.style.cssText += " opacity: " + series.opacity + ";";
                        }
                        let divDescr = document.createElement("div");
                        divDescr.textContent = series.name;
                        let divValue = document.createElement("div");
                        divValue.style.cssText = "justify-self: end; font-weight: bold;";

                        if (pt_hl.value != undefined && pt_hl.value != null) {

                            if (typeof pt_hl.value === "number") {

                                divValue.textContent = kendo.toString(pt_hl.value, output_format);

                            } else {

                                divValue.innerHTML = "<div style='text-align:end;'>" + kendo.toString(pt_hl.value.from, output_format) + "</div>" +
                                    "<div style='text-align:end;'>" + kendo.toString(pt_hl.value.to, output_format) + "</div>";
                            }
                        }

                        gridElem.appendChild(divColor);
                        gridElem.appendChild(divDescr);
                        gridElem.appendChild(divValue);
                    }
                }
            });

            if (categories.length === 0) {
                return null;
            }

            plugin.elemDataOra.textContent = kendo.toString(new Date(categories[0]), $(plugin.elemDataOra).data("output-format"));

            return categories[0];
        };

        plugin.parentChart.bind("plotAreaClick", function (e) {

            if (e.originalEvent.type === "contextmenu") {
                // Disable browser context menu
                e.originalEvent.preventDefault();
            }

            _hideTooltip();
            _cancelHighlight();

            let gridElem = null;
            plugin.$element.find(".tooltip-series-grid").each(function () {
                while (this.firstChild) {
                    this.removeChild(this.lastChild);
                }
                gridElem = this;
            });

            let category = e.category;
            if (Array.isArray(category)) {
                category = category[category.length - 1];
            }

            category = _fillTooltip(category, gridElem);

            if (category !== null) {

                let slot = plugin.parentChart.findAxisByName(plugin.horzAxis.name).slot(category, category);

                if (slot) {

                    plugin.$element.addClass("chart-tooltip-transparent");
                    plugin.$element.removeClass("chart-tooltip-hidden");

                    let bbox = plugin.parentChart.plotArea().backgroundVisual.bbox();
                    let top = bbox.origin.y;
                    let left = bbox.origin.x;
                    let max_right = bbox.origin.x + bbox.size.width;
                    let this_width = plugin.$element.outerWidth();
                    left = Math.max(left, slot.origin.x + (slot.size.width / 2) - (this_width / 2));
                    if (left + this_width > max_right) {
                        left = max_right - this_width;
                    }

                    plugin.$element.css({ top: top + "px", left: left + "px" });

                    plugin.$element.removeClass("chart-tooltip-transparent");
                }
            }
        });

        let _hideTooltip = function () {

            plugin.$element.addClass("chart-tooltip-hidden");
        }

        let _cancelHighlight = function () {

            let idx = 0;
            let series = plugin.parentChart.findSeriesByIndex(idx);

            while (series) {

                series.toggleHighlight(false);

                idx++;
                series = plugin.parentChart.findSeriesByIndex(idx);
            }
        }

        plugin.parentChart.bind("paneRender", function (e) {
            _hideTooltip();
            _cancelHighlight();
        });
    };

    $.fn.chartSharedTooltip = function (opts) {
        return this.each(function () {

            if (undefined == $(this).data("chartSharedTooltip")) {

                var plugin = new $.chartSharedTooltip(this, opts);

                $(this).data("chartSharedTooltip", plugin);
            }
        });
    };

})(jQuery);

