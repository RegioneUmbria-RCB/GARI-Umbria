
$(document).ready(function () {

    let mainContainer = document.getElementById("MainContainer");

    let div_loader = WidgetCommon.createElement("div", "window-loader");
    mainContainer.appendChild(div_loader);

    $(div_loader).StyleLoader({ type: "DotCircle", color: "#333" });

    let divTabStrip = WidgetCommon.createElement("div", "zero_opacity", "font-size: larger;", "Monitor-tabstrip");
    mainContainer.appendChild(divTabStrip);

    $(divTabStrip).kendoTabStrip({
        animation: false,
    });

    window.addEventListener("message",
        function (event) {

            if (verificaOriginSecondaria(self, location.href, event) == false) {
                return false;
            }

            ShowMonitor(event.data);
        },
        false);

    window.parent.postMessage("frameIsListening", ottieniTargetOrigin(window));
});


function ShowMonitor(data) {

    if (data.stazioni == undefined || data.stazioni == null || data.stazioni.lenght == 0)
        return;

    let divTabstrip = document.getElementById("Monitor-tabstrip");
    let tabstrip = $(divTabstrip).data("kendoTabStrip");

    for (let s = 0; s < data.stazioni.length; s++) {

        let stazione = data.stazioni[s];
        let id_div_stazione = "suolo-stazione-" + s;

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
    let gg = (dataFine.getTime() - dataInizio.getTime()) / (3600000 * 24);

    let periodoText = "<span style='margin-right: 10px;'>" + kendo.toString(dataInizio, "dd MMM yyyy") + "</span>";
    periodoText += "<span class='fa fa-arrow-left'></span>";
    periodoText += "<span style='margin: 0px 5px;'>" + kendo.toString(gg, "0") + " gg</span>";
    periodoText += "<span class='fa fa-arrow-right'></span>";
    periodoText += "<span style='margin-left: 10px;'>" + kendo.toString(dataFine, "dd MMM yyyy") + "</span>";

    let divPeriodo = WidgetCommon.createElement("div", "", "margin-bottom: 10px; text-align: center; font-size: 18px; cursor: default;");
    divPeriodo.innerHTML = periodoText;
    div_stazione.appendChild(divPeriodo);

    let divChartContainer = WidgetCommon.createElement("div");
    div_stazione.appendChild(divChartContainer);

    $.each(stazione.Meteo.Charts, function (i, chart) {

        let divChartId = div_stazione.id + "-chart-" + i;
        let divChart = WidgetCommon.createElement("div", "chart-block", "", divChartId);
        divChartContainer.appendChild(divChart);

        creaKendoChart2("", chart.horizAxis, chart.axis, chart.series, stazione.Meteo.Table.kendo_rows, divChart.id);
    });
}
