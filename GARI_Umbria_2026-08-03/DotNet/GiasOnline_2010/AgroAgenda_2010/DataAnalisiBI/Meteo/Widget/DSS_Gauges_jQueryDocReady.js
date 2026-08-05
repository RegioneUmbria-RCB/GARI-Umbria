
var parentWinId;

$(document).ready(function () {
    
    let mainContainer = document.getElementById("MainContainer");

    let div_loader = WidgetCommon.createElement("div", "window-loader");
    mainContainer.appendChild(div_loader);

    $(div_loader).StyleLoader({ type: "DotCircle", color: "#333" });

    let gaugesContainer = WidgetCommon.createElement("div", "zero_opacity", "", "gauges-container");
    mainContainer.appendChild(gaugesContainer);

    window.addEventListener("message",
        function (event) {

            if (verificaOriginSecondaria(self, location.href, event) == false) {
                return false;
            }

            parentWinId = event.data.elemID;
            let rag_soc = "";

            if (event.data.rag_soc !== undefined && event.data.rag_soc !== null && event.data.rag_soc !== "")
                rag_soc = event.data.rag_soc;

            DSS_Gauges.showGauges(rag_soc, event.data.indicatori, gaugesContainer, event.data.height, event.data.view_grid, gauge_click, url_meteo_ws);

            $(gaugesContainer).removeClass("zero_opacity");

            $(".window-loader").each(function (i, e) { e.remove(); });
        },
        false
    );

    window.parent.postMessage("frameIsListening", ottieniTargetOrigin(window));
});



function gauge_click(params) {
    SetSessionParametri(params);
}

