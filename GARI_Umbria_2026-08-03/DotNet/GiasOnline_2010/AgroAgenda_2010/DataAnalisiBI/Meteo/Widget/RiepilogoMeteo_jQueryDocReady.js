
$(document).ready(function () {

    let mainContainer = document.getElementById("MainContainer");

    let div_loader = WidgetCommon.createElement("div", "window-loader");
    mainContainer.appendChild(div_loader);

    $(div_loader).StyleLoader({ type: "DotCircle", color: "#333" });

    let divTabStrip = WidgetCommon.createElement("div", "zero_opacity", "font-size:larger;", "W-Meteo-TabStrip");
    mainContainer.appendChild(divTabStrip);

    $(divTabStrip).kendoTabStrip({
        animation: false,
        show: function (e) {
            $(".meteo-sensore-text.calc-size").each(function (i, el) {

                let $el = $(el);

                $el.removeClass("meteo-sensore-text");

                let span = $($(el).children()[0]);
                let spanH = span.innerHeight();

                if (spanH > 0) {

                    $el.removeClass("calc-size");

                    let controlH = $(el.closest(".meteo-sensore")).innerHeight() - 6;

                    while (spanH > controlH) {

                        let px = parseInt($el.css("font-size"));
                        px--;

                        if (px >= 9) {

                            $el.css("font-size", px + "px");
                            spanH = span.innerHeight();

                        } else {

                            spanH = 0;
                        }
                    }
                }

                $(el).addClass("meteo-sensore-text");
            });
        }
    });

    window.addEventListener("message",
        function (event) {

            if (verificaOriginSecondaria(self, location.href, event) == false) {
                return false;
            }

            ShowRiepilogo(event.data);
        },
        false);

    window.parent.postMessage("frameIsListening", ottieniTargetOrigin(window));
});


function ShowRiepilogo(data) {

    if (data.stazioni == undefined || data.stazioni == null || data.stazioni.lenght == 0)
        return;

    let divTabstrip = document.getElementById("W-Meteo-TabStrip");
    let tabstrip = $(divTabstrip).data("kendoTabStrip");

    for (let s = 0; s < data.stazioni.length; s++) {

        let stazione = data.stazioni[s];
        let id_div_stazione = "win-stazione-" + s;

        tabstrip.append({
            text: stazione.Descrizione,
            content: "<div id='" + id_div_stazione + "'></div>"
        });

        document.getElementById(id_div_stazione).setAttribute("data-stazione-idx", s);

        OutputStazione(stazione, document.getElementById(id_div_stazione));
    }

    tabstrip.select(data.selected);

    let tabh = Math.floor(data.height) - ($(divTabstrip).height() - $($(divTabstrip).find(".k-content")[data.selected]).height());

    $(divTabstrip).find(".k-content").each(function (i, e) {
        $(e).height(Math.floor(tabh));
    });

    $(divTabstrip).removeClass("zero_opacity");

    $(".window-loader").each(function (i, e) { e.remove(); });
}


function OutputStazione(stazione, div_stazione) {

    let dataInizio = kendo.parseDate(stazione.Meteo.Table.kendo_rows[0].DataOra);
    let dataFine = kendo.parseDate(stazione.Meteo.Table.kendo_rows[stazione.Meteo.Table.kendo_rows.length - 1].DataOra);
    let hours = (dataFine.getTime() - dataInizio.getTime()) / 3600000;

    let periodoText = "<span style='margin-right: 10px;'>" + kendo.toString(dataInizio, "dd MMM yyyy HH:mm") + "</span>";
    periodoText += "<span class='fa fa-arrow-left'></span>";
    periodoText += "<span style='margin: 0px 5px;'>" + kendo.toString(hours, "0") + " h</span>";
    periodoText += "<span class='fa fa-arrow-right'></span>";
    periodoText += "<span style='margin-left: 10px;'>" + kendo.toString(dataFine, "dd MMM yyyy HH:mm") + "</span>";

    let divPeriodo = WidgetCommon.createElement("div", "", "margin-bottom: 10px; text-align: center; font-size: 18px; cursor: default;");
    divPeriodo.innerHTML = periodoText;
    div_stazione.appendChild(divPeriodo);

    let div_grid = WidgetCommon.createElement("div", "meteo-grid");
    div_stazione.appendChild(div_grid);

    $.each(stazione.Meteo.Riepilogo,
        function (idx, riep) {

            let div_cell = WidgetCommon.createElement("div", "k-block meteo-cell");
            div_grid.appendChild(div_cell);

            let divSensore = WidgetCommon.createElement("div", "k-block meteo-sensore");
            div_cell.appendChild(divSensore);

            let divSensoreText = WidgetCommon.createElement("div", "meteo-sensore-text calc-size");
            divSensoreText.innerHTML = "<span>" + riep.Etichetta + "</span>";
            divSensore.appendChild(divSensoreText);

            if (riep.UM !== "") {

                let div_um = WidgetCommon.createElement("div", "meteo-um");
                div_um.innerHTML = "<span>" + riep.UM + "</span>";

                divSensore.appendChild(div_um);
            }

            let div_inside_container = WidgetCommon.createElement("div", "inside-container");
            div_cell.appendChild(div_inside_container);

            let div_inside = WidgetCommon.createElement("div", "inside-cell");
            div_inside_container.appendChild(div_inside);

            div_inside.appendChild(WidgetCommon.createElement("div", "ripple"));

            let div_cont = WidgetCommon.createElement("div", "", "position: relative;");
            div_inside.appendChild(div_cont);

            let div_val = WidgetCommon.createElement("div", "meteo-value");
            div_cont.appendChild(div_val);

            let div_aggr = WidgetCommon.createElement("div", "meteo-aggr");
            div_cont.appendChild(div_aggr);

            if (riep.hasOwnProperty("val_dir")) {

                //degrees2cardinal
                let cardinals = ["N", "NE", "E", "SE", "S", "SW", "W", "NW", "N"];
                let div = 450.0;
                //let cardinals = ["N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N"];
                //let div = 225.0;
                let deg2card = cardinals[Math.round(((riep.val_dir * 10.0) % 3600) / div)];
                let rot = Math.round(riep.val_dir);

                let spanText = "<span style='margin-right: 5px;'>";
                //spanText += kendo.toString(riep.val_last, "0.00") + " " + riep.UM + " - " + deg2card;
                spanText += kendo.toString(riep.val_last, "0.00") + " - " + deg2card;
                spanText += "</span>"
                spanText += "<span class='fa fa-long-arrow-down wind-dir' style='transform: rotate(" + rot + "deg);'></span>";
                spanText += "<span style='margin-left: 5px; font-size: 10px;'>" + kendo.toString(riep.val_dir, "0.00") + "°</span>";

                div_val.innerHTML = spanText;

            } else {

                div_val.innerHTML = "<span>" + kendo.toString(riep.val_last, "0.00") + "</span>";

                $.each([{ field: "val_min", output: TraduzioneMultiResx(datiMeteoResx, "AggrMin", "Min") },
                    { field: "val_avg", output: TraduzioneMultiResx(datiMeteoResx, "AggrAvg", "Media") },
                    { field: "val_max", output: TraduzioneMultiResx(datiMeteoResx, "AggrMax", "Max") },
                    { field: "val_sum", output: TraduzioneMultiResx(datiMeteoResx, "AggrSum", "Somma") }],
                    function (i, e) {
                        if (riep.hasOwnProperty(e.field)) {

                            let div_aggr_ch = WidgetCommon.createElement("div");
                            div_aggr.appendChild(div_aggr_ch);
                            let div_aggr_txt = WidgetCommon.createElement("div", "meteo-aggr-text");
                            div_aggr_ch.appendChild(div_aggr_txt);
                            let div_aggr_val = WidgetCommon.createElement("div", "meteo-aggr-value");
                            div_aggr_ch.appendChild(div_aggr_val);

                            div_aggr_txt.innerHTML = e.output;
                            div_aggr_val.innerHTML = kendo.toString(riep[e.field], "0.00");
                        }
                    }
                );
            }

            let div_sparkline = WidgetCommon.createElement("div", "k-block");
            let sparkline = WidgetCommon.createElement("span", "", "width: 100%;");
            div_sparkline.appendChild(sparkline);
            div_cell.appendChild(div_sparkline);

            CreaSparkline(sparkline, stazione.Meteo, riep.NomeColonna);

            div_inside.setAttribute("data-stazione-div", div_stazione.id);
            div_inside.setAttribute("data-sensore", riep.NomeColonna);
            div_inside.onclick = function (e) { CreaChart(e, stazione); };
        }
    );
}


function CreaSparkline(dom, meteo, nomeColonna) {

    let data = [];
    $.each(meteo.Table.kendo_rows,
        function (i, elem) {
            if (elem.hasOwnProperty(nomeColonna)) {
                if (elem[nomeColonna] !== null) {
                    data.push(elem[nomeColonna]);
                }
            }
        }
    );

    let color = "#CCC";
    let ich = 0;
    while (ich < meteo.Charts.length) {

        let _chart = meteo.Charts[ich];
        let iser = 0;
        while (iser < _chart.series.length) {

            if (_chart.series[iser].field === nomeColonna) {

                color = _chart.series[iser].color;
                iser = _chart.series.length;
                ich = meteo.Charts.length;
            }
            iser++;
        }
        ich++;
    }

    $(dom).kendoSparkline({
        theme: "Bootstrap",
        chartArea: {
            height: "30px",
            background: "transparent"
        },
        series: [{
            type: "area",
            color: color,
            opacity: 0.25,
            line: { opacity: 1 },
            data: data
        }],
        tooltip: { visible: false },
        categoryAxis: { crosshair: { visible: false } }
    });
}


function CreaChart(e, stazione) {

    let divStazioneId = e.currentTarget.getAttribute("data-stazione-div");
    let divStazione = document.getElementById(divStazioneId);
    if (divStazione === null) {
        return;
    }

    let rifChart = e.currentTarget.getAttribute("data-sensore");

    let ich = 0;
    let chart = null;
    let divChartId = "";

    while (ich < stazione.Meteo.Charts.length && chart === null) {
        let _chart = stazione.Meteo.Charts[ich];
        let iser = 0;
        while (iser < _chart.series.length) {

            if (_chart.series[iser].field === rifChart) {
                chart = _chart;
                divChartId = divStazioneId + "-chart-" + ich;

                iser = _chart.series.length;
            }

            iser++;
        }

        ich++;
    }

    if (chart === null) {
        return;
    }

    let divChart = document.getElementById(divChartId);

    if (divChart === null) {

        divChart = WidgetCommon.createElement("div", "chart-block", "", divChartId);
        divStazione.appendChild(divChart);

        let isScrollBar = (divStazione.parentElement.scrollHeight > divStazione.parentElement.clientHeight);

        CreaGrafico(stazione, chart, divChart);

        if (!isScrollBar) {
            if (divStazione.parentElement.scrollHeight > divStazione.parentElement.clientHeight) {
                $("#" + divStazione.id + " .k-chart").each(function (idx, elem) {
                    if (elem.id !== divChartId) {

                        kendoChart = $(elem).data("kendoChart");
                        if (kendoChart !== undefined) {

                            kendoChart.resize();
                        }
                    }
                });
            }
        }
    }

    let top = $(divChart).position().top - ($(divStazione.parentElement).outerHeight() - $(divStazione.parentElement).height());
    $(divStazione.parentElement).animate({ scrollTop: top });
}


function CreaGrafico(stazione, chart, divChart) {

    let series = [];
    $.each(chart.series,
        function (i, s) {

            let s_ = $.extend({}, { missingValues: "interpolate", gap: 0.25, tooltipTemplate: "0.00" }, s);

            if (s_.type === "line" && chart.horizAxis.baseUnit === "minutes") {

                s_.markers = {
                    visible: true,
                    //size: 4,
                    //border: { width: 0 },
                    //background: function (e) {
                    //    return e.series.color;
                    //},
                    visual: function (e) {
                        let center = e.rect.center();
                        let path = new kendo.drawing.Path({
                            stroke: {
                                color: e.series.color,
                                width: 2
                            }
                        }).moveTo(center.x, center.y).lineTo(center.x, center.y - 4);
                        return path;
                    }
                };
            }

            series.push(s_);
        }
    );

    creaKendoChart2("", chart.horizAxis, chart.axis, series, stazione.Meteo.Table.kendo_rows, divChart.id);
}

