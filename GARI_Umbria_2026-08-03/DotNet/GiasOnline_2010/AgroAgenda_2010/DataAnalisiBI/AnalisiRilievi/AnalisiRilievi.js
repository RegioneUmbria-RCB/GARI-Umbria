
function creaRadarChart(titolo, categs, series) {

    var divChart = creaChartDiv();

    let domChart = document.getElementById(divChart);
    domChart.style.border = "1px solid #ccc";
    domChart.style.borderRadius = "4px";

    $("#" + divChart).kendoChart({
        theme: "Bootstrap", //"Material"
        chartArea: {
            height: 600
        },
        title: {
            text: titolo
        },
        legend: {
            position: "right",
            item: {
                visual: visualLegendItem
            }
        },
        transitions: false,
        seriesDefaults: {
            type: "radarLine",
            style: "smooth",
            markers: {
                visible: false
            },
            highlight: {
                visible: false/*,
                toggle: function (e) {
                    e.preventDefault();
                    var needRedraw = false;
                    var chart = $("#" + divChart).data("kendoChart");
                    var s_cnt = chart.options.series.length
                    for (var s = 0; s < s_cnt; s++) {
                        var stype = "radarLine";
                        var sopac = 1;
                        if (s == e.series.index && e.show) {
                            stype = "radarArea";
                            sopac = 0.5;
                        }
                        if (chart.options.series[s].type != stype) {
                            chart.options.series[s].type = stype;
                            chart.options.series[s].opacity = sopac;
                            needRedraw = true;
                        }
                    }
                    if (needRedraw) {
                        chart.redraw();
                    }
                }*/
            }
        },
        series: series,
        categoryAxis: {
            categories: categs,
            labels: {
                //visual: function (e) {

                //    // The actual label
                //    var labelVisual = e.createVisual();
                //    var bbox = labelVisual.bbox();

                //    // An invisible rectangle to serve as a hot zone
                //    var sink = kendo.drawing.Path.fromRect(bbox, {
                //        stroke: null,
                //        fill: {
                //            color: "#fff",
                //            opacity: 0
                //        }
                //    });

                //    // Maintain reference for event handlers
                //    sink.labelVisual = true;

                //    var visual = new kendo.drawing.Group();
                //    visual.append(labelVisual, sink);
                //    return visual;

                ////    //var orig = e.createVisual();
                ////    ////orig.options.cursor = "pointer";
                ////    //console.log(orig.children[0]);

                ////    //orig.children[0]._position.x -= orig.children[0].chartElement.box.width() / 2;

                ////    //return orig;
                ////    //var rect = new kendo.geometry.Rect([e.rect.origin.x, e.rect.origin.y], e.rect.size);
                ////    var path = new kendo.drawing.Rect(e.rect,
                ////        {
                ////            stroke: {
                ////                color: "#9999b6",
                ////                width: 2
                ////            }
                ////        }
                ////    );
                ////    var layout = new kendo.drawing.Layout(e.rect, {
                ////        wrap: false
                ////    });
                ////    layout.append(path);
                ////    //layout.append(new kendo.drawing.Text(e.text));
                ////    layout.reflow();
                ////    return layout;
                //}
            }
        },
        valueAxis: {
            visible: false
        },
        tooltip: {
            visible: false
        },
        legendItemClick: function (e) {
            // click su legendItem con shiftKey premuto la serie passa da radarLine a radarArea e viceversa...
            if (typeof event != "undefined") {
                if (typeof event.shiftKey != "undefined") {
                    if (event.shiftKey) {

                        e.preventDefault();

                        if (e.sender.options.series[e.seriesIndex].visible) {
                            if (e.sender.options.series[e.seriesIndex].type === "radarArea") {
                                e.sender.options.series[e.seriesIndex].type = "radarLine";
                                e.sender.options.series[e.seriesIndex].opacity = 1;
                            } else {
                                e.sender.options.series[e.seriesIndex].type = "radarArea";
                                e.sender.options.series[e.seriesIndex].opacity = 0.5;
                                e.sender.options.series[e.seriesIndex].line = { width: e.sender.options.series[e.seriesIndex].width, style: e.sender.options.series[e.seriesIndex].style };
                            }
                            e.sender.redraw();
                        }
                    }
                }
            }
            /*
                        // click su legendItem la serie passa da radarLine a radarArea a hidden...
                        // forse per uno schermo touch va meglio questa soluzione...
                        e.preventDefault();
                        if (!e.sender.options.series[e.seriesIndex].visible) {
                            e.sender.options.series[e.seriesIndex].visible = true;
                        } else {
                            if (e.sender.options.series[e.seriesIndex].type === "radarArea") {
                                e.sender.options.series[e.seriesIndex].type = "radarLine";
                                e.sender.options.series[e.seriesIndex].opacity = 1;
                                e.sender.options.series[e.seriesIndex].visible = false;
                            } else {
                                e.sender.options.series[e.seriesIndex].type = "radarArea";
                                e.sender.options.series[e.seriesIndex].opacity = 0.5;
                            }
                        }
                        e.sender.redraw();
            */
        }
        //legendItemHover: function (e) {
        //    e.preventDefault();
        //    var needRedraw = false;
        //    var s_cnt = e.sender.options.series.length
        //    for (var s = 0; s < s_cnt; s++) {
        //        var stype = "radarLine";
        //        var sopac = 1;
        //        if (s == e.seriesIndex && e.sender.options.series[s].visible) {
        //            stype = "radarArea";
        //            sopac = 0.5;
        //        }
        //        if (e.sender.options.series[s].type != stype) {
        //            e.sender.options.series[s].type = stype;
        //            e.sender.options.series[s].opacity = sopac;
        //            needRedraw = true;
        //        }
        //    }
        //    if (needRedraw) {
        //        e.sender.redraw();
        //    }
        //}
    });

    aggiungiPulsantiChart(divChart, false);
}



function PulseLoader() {
    if (!(this instanceof PulseLoader)) return new PulseLoader();
    this.elem = null;
}

PulseLoader.prototype = {
    show: function (text) {
        if (this.elem !== null) {
            hide();
        }

        let keyframes = "";
        let animation_name = "scaleDown";

        let cssText = ".loader-overlay { position: fixed; width: 100%; height: 100%; top: 0px; left: 0px; z-index: 10001; } ";
        cssText += ".loader-overlay-color { width: 100%; height: 100%; background-color: white; opacity: 0.5; } ";
        cssText += ".loader-center-container { position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); border: 1px solid #A9A9A9; border-radius: 4px; padding: 15px; background-color: #FAFAFA; box-shadow: 0 0 7px 0 #A9A9A9; } ";
        cssText += ".loader-text { margin-bottom: 15px; font-size: larger; } ";
        cssText += ".loader-container { display: flex; justify-content: center; align-items: center; } ";
        cssText += ".dot-loader { ";
        cssText += "height: 15px; ";
        cssText += "width: 15px; ";
        cssText += "border-radius: 50%; ";
        cssText += "background-color: #A9A9A9; ";
        cssText += "position: relative; ";
        cssText += "transform: scale(0); ";

        $.each(['-moz-', '-webkit-', '-ms-', ''], function (p, pref) {
            cssText += pref + "animation: 1.2s " + animation_name + " ease-in-out infinite; ";

            keyframes += "@" + pref + "keyframes " + animation_name + " { ";
            keyframes += "0%, 80%, 100% { " + pref + "transform: scale(0); } ";
            keyframes += "40% { " + pref + "transform: scale(1); }";
            keyframes += " } ";
        });

        cssText += "} ";
        cssText += ".dot-loader:nth-child(2) { ";
        cssText += "margin: 0 5px; ";
        cssText += "animation-delay: .15555s; ";
        cssText += "} ";
        cssText += ".dot-loader:nth-child(3) { ";
        cssText += "animation-delay: .30000s; ";
        cssText += "} ";
        cssText += keyframes;

        let loaderStyle = document.createElement('style');
        loaderStyle.type = "text/css";
        loaderStyle.innerHTML = cssText;

        this.elem = document.createElement("div");
        this.elem.className = "loader-overlay";
        this.elem.appendChild(loaderStyle);
        let colorElem = document.createElement("div");
        colorElem.className = "loader-overlay-color";
        this.elem.appendChild(colorElem);
        let centerElem = document.createElement("div");
        centerElem.className = "loader-center-container";
        this.elem.appendChild(centerElem);
        if (typeof text === "string" && text !== "") {
            let textElem = document.createElement("div");
            textElem.className = "loader-text";
            textElem.innerHTML = text;
            centerElem.appendChild(textElem);
        }
        let loaderElem = document.createElement("div");
        loaderElem.className = "loader-container";
        for (let i = 0; i < 3; i++) {
            let dotElem = document.createElement("div");
            dotElem.className = "dot-loader";
            loaderElem.appendChild(dotElem);
        }
        centerElem.appendChild(loaderElem);
        document.body.appendChild(this.elem);
    },

    hide: function () {
        if (this.elem === null) {
            return;
        }
        this.elem.remove();
        this.elem = null;
    }
};




/*
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
in scriptMapsDiPartenza_BS.js
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
 */

function ModalKendoApri(indirizzo, descrizione) {

    $('<div id="idKendoModal_iFrameGeneric" style="display: none;"></div>').appendTo('body');
    let $kendomodal = $("#idKendoModal_iFrameGeneric");

    $kendomodal.kendoWindow({
        //actions: [],
        title: descrizione,
        height: "90%",
        width: "90%",
        draggable: false,
        visible: false,
        modal: true,
        resizable: false,
        content: indirizzo,
        iframe: true,
        open: function (e) { //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
            $("body").addClass("overflow_hidden");
        },
        close: function (e) {

            $("body").removeClass("overflow_hidden");

            $kendomodal.data("kendoWindow").destroy();
        }
        /*
                activate: function(e) {
                    var h = $("#idKendoModal_iFrameGeneric").height();
                    var footH = $("#idKendoModal_iFrameGeneric .window-footer").outerHeight(true);
                    var contH = h - footH;
                    $("#idKendoModal_iFrameGeneric .container").height(contH).css("overflow", "auto");
                }
        */
    });

    let parent = $kendomodal.parent();
    parent.find('.k-window-title').css('text-align', 'center');
    parent.css('padding-top', '48px');
    let titlebar = parent.find('.k-window-titlebar');
    titlebar.css({
        "margin-top": "-48px",
        "height": "35px",
        "line-height": "35px",
        "vertical-align": "middle"
    });

    $kendomodal.data("kendoWindow").center().open();
}

function ModalKendoChiudi() {
    let $kendomodal = $("#idKendoModal_iFrameGeneric");
    let kendoWindow = $kendomodal.data("kendoWindow");
    if (kendoWindow != undefined) {
        kendoWindow.close();
    }
}

function apriFinestraFiltroRicercaNG(url) {
    window.addEventListener('message', chiudiFinestraFiltroRicercaNG);

    $(document.body).append('<div id="filtro_ricerca_ng"></div>');

    $('#filtro_ricerca_ng').kendoWindow({
        title: "Filtra Impianti",
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            $('#filtro_ricerca_ng').kendoWindow('destroy');
        }
    }).data('kendoWindow').center().maximize();
}

function chiudiFinestraFiltroRicercaNG(event) {
    let kWin = $('#filtro_ricerca_ng').data("kendoWindow");
    let urlKWin = kWin.options.content.url;

    if (verificaOriginSecondaria(window, urlKWin, event) &&
        (event != null && event.data != null) && (event.data.messaggio != null) &&
        event.data.messaggio.includes("chiudiWindowGiasNG")) {

        CreaEntitaDaChiavi(event.data.inData.chiavi)

        kWin.close();
    }
}

function CreaEntitaDaChiavi(chiavi) {

    var param = kendo.stringify({
        "chiavi": chiavi
    });

    ajaxAgronica(indirizzohttp + "/CreaEntitaDaChiavi",
        param,
        function (risposta) {
            location.reload() //forziamo il reload della pagina per poter leggere gli impianti 
        }, function (risposta) {
            kendo.alert(risposta.Errore)
        }, null, true);
}

