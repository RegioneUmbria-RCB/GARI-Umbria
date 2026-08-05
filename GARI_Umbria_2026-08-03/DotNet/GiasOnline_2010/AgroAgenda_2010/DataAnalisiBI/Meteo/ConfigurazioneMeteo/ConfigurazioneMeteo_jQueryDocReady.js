
$(document).ready(function () {

    // Evita l'utilizzo dell'invio
    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });

    $("#kendoWindowiFrameGeneric").css("display", "none");

    $("#tabstrip").kendoResizableTabStrip({
        animation: false,
        scrollable: false,
        activate: function (e) {

            let idx = $(e.item).index();

            if (idx === 0) {

                let sorgenti = [];

                if ($("#anagArea").length > 0) {

                    let attr = parseInt($("#stazSrc").attr("data-reload"));
                    $("#stazSrc").removeAttr("data-reload");

                    if (!isNaN(attr) && attr === 1) {
                        sorgenti.push(1, 2);
                    }
                }
                if ($("#aliasArea").length > 0) {

                    let attr = parseInt($("#aliasArea").attr("data-reload"));
                    $("#aliasArea").removeAttr("data-reload");

                    if (!isNaN(attr) && attr === 1) {
                        sorgenti.push(4);
                    }
                }
                if (sorgenti.length > 0)
                    _applicazioni_stazioni_reload(sorgenti);

            } else {

                if (idx === 1) {

                    setTimeout(function () {
                        let datasource = $("#stazSrc").getKendoListView().dataSource;
                        if (datasource.total() === 0) {
                            datasource.read();
                        }
                    }, 500);
                }
            }
        }
    });

    let tabTitle = [
        { id: "tabApplicazione", title: TraduzioneMultiResx(datiMeteoResx, "titoloTabStazioniApplicazione", "Stazioni per applicazione") },
        { id: "tabAnagrafica", title: TraduzioneMultiResx(datiMeteoResx, "titoloTabAnagraficaStazioni", "Anagrafica stazioni") },
        { id: "tabAlias", title: TraduzioneMultiResx(datiMeteoResx, "...", "Stazioni pubbliche preferite") }
    ];

    $("#tabstrip").data("kendoResizableTabStrip").tabGroup.children().each(function (i, e) {
        let title = "...";
        let elem = tabTitle.find(t => t.id == e.id);
        if (elem != undefined) {
            title = elem.title;
        }
        $(e).find(".k-link").text(title);
    });


    _applicazioni_jQueryDocReady();

    if ($("#anagArea").length > 0) {

        _anagrafica_jQueryDocReady();
    }

    if ($("#aliasArea").length > 0) {

        _alias_jQueryDocReady();
    }

    let container = document.createElement("div");
    container.className = "__hidden__";
    container.id = "g-map-container";
    container.style.position = "relative";
    container.style.border = "1px solid #ccc";
    container.style.backgroundColor = "#fff";
    document.getElementById("mainContainer").appendChild(container);

    let map = document.createElement("div");
    map.id = "g-map-wrapper";
    map.style.width = "calc(100% - 10px)";
    map.style.left = "5px";
    map.style.height = "calc(100% - 10px)";
    map.style.top = "5px";
    container.appendChild(map);

    $(map).simpleMap({
        openCallback: function () {
            $("#tabstrip").addClass("__hidden__");
            $("#g-map-container").removeClass("__hidden__");
        },
        closeCallback: function () {
            $("#g-map-container").addClass("__hidden__");
            $("#tabstrip").removeClass("__hidden__");
        }
    });



    _resizeAll();

    jQuery(window).on("resize", function (event) {
        _resizeAll();
    });

    document.getElementById("tabstrip").style.opacity = "1";

});

function _resizeAll() {

    let winH = $(window).height();
    let ftrH = $(".AgronicaFooter").outerHeight();
    if (!$(".AgronicaFooter").is(":visible")) {
        ftrH = 0;
    }

    let innerH = Math.floor((winH - ftrH - 10) / 10) * 10;
    if (innerH < 500) {
        innerH = 500;
        document.body.style.overflowY = "auto";
    } else {
        document.body.style.overflowY = "hidden";
    }

    let ts = $("#tabstrip").data("kendoResizableTabStrip");

    let contH = ts.fitHeight(innerH);

    _ridimensiona_ApplicazioniTabstrip(contH);
    _ridimensiona_AnagraficaTabstrip(contH);
    _ridimensiona_AliasTabstrip(contH);

    let top = $("#tabstrip").position().top
    if ($("#tabstrip").hasClass("__hidden__")) {
        top = $("#g-map-container").position().top;
    } 
    $("#g-map-container").height(innerH - top);

}
