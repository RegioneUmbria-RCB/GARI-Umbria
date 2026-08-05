
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
//
//$(...).MonitorSuolo({ Configurazione json });
//
//Configurazione:
//      url_meteoWS: "../DataAnalisiBI/Meteo/MeteoWS.aspx" (Obbligatorio)
//      window_title: Stringa titolo finestra popup
//      msg_wait: Stringa messaggio per attesa elaborazione...
//      msg_na: Stringa messaggio se elaborazione non disponibile...
//      msg_err: Stringa messaggio se errore in elaborazione...
//      fun_callback: Funzione chiamata al termine dell'elaborazione (parametro status: -1 errore, 0 nulla da visualizzare, 1 ok)
//
//$(...).data('MonitorSuolo').show(piva)
//
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

(function ($) {

    $.MonitorSuolo = function (elem, options) {

        var plugin = this;

        plugin.settings = {};
        plugin.$element = $(elem); 
        plugin.element = elem; 
        plugin.wrapper = null;

        // plugin's default options this is private property and is accessible only from inside the plugin
        let defaults = {
            window_title: TraduzioneMultiResx(datiMeteoResx, "MonitoraggioSuolo", "Monitoraggio suolo"),
            msg_na: "",
            msg_err: "",
            fun_callback: function () { }
        };

        // the plugin's final properties are the merged default and user-provided options (if any)
        plugin.settings = $.extend({}, defaults, options);


        //-------------------------------------------------------------------------------------------------
        // public methods
        //-------------------------------------------------------------------------------------------------


        plugin.show = function (piva) {

            _remove_wrapper();

            plugin.wrapper = WidgetCommon.createElement("div", "", "width:100%; height:100%;");
            plugin.element.appendChild(plugin.wrapper);

            $(plugin.wrapper).StyleLoader({ type: "DotCircle", color: "#333" });
            
            ajaxAgronica(plugin.settings.url_meteoWS + "/DatiMeteo_ElaboraMonitoraggioSuolo",
                JSON.stringify({ piva: piva }),
                function (risposta) {

                    let result = risposta.RispostaStringa;

                    let stazioni = result.Stazioni;

                    if (stazioni === null || stazioni.length === 0) {

                        _show_marquee(plugin.settings.msg_na);

                        plugin.settings.fun_callback(0);

                    } else {

                        plugin.stazioni = [];

                        $.each(stazioni, function (idx, elem) {

                            let staz = {
                                Descrizione: elem.Descrizione,
                                AlertSerie: JSON.parse(elem.AlertSerie),
                                SogliaInf: elem.SogliaInf,
                                SogliaSup: elem.SogliaSup,
                                Meteo: {
                                    Table: JSON.parse(elem.Meteo.Meteo_Table),
                                    Charts: JSON.parse(elem.Meteo.Meteo_Charts)
                                }
                            }

                            $.each(staz.Meteo.Charts, function (i, c) {
                                $.each(c.series, function (j, s) {
                                    s.tooltipTemplate = "0.00";
                                });
                            });

                            plugin.stazioni.push(staz);
                        });

                        plugin.settings.fun_callback(1);

                        _show_summary();
                    }
                },
                function (risposta) {

                    _show_marquee(plugin.settings.msg_err);

                    plugin.settings.fun_callback(-1);

                    if (typeof risposta.Errore === "string") {

                        if (risposta.Errore !== "") {

                            plugin.ErrorResponse = risposta.Errore;

                            plugin.wrapper.onclick = function (e) {

                                if (!e.ctrlKey) {
                                    return;
                                }

                                let dlgElem = WidgetCommon.createElement("div");
                                document.body.appendChild(dlgElem);
                                let $dlgElem = $(dlgElem);

                                let w = $(window).width() * 0.9;
                                let h = $(window).height() * 0.9;
                                $dlgElem.kendoDialog({
                                    title: plugin.settings.msg_err,
                                    maxWidth: w,
                                    maxHeight: h,
                                    closable: false,
                                    modal: true,
                                    visible: false,
                                    content: plugin.ErrorResponse,
                                    actions: [
                                        { text: TraduzioneMultiResx(datiMeteoResx, "Chiudi", "Chiudi") }
                                    ],
                                    close: function (e) {
                                        this.destroy();
                                    }
                                });

                                $dlgElem.data("kendoDialog").open();
                            };
                        }
                    }
                },
                null,
                false
            );
        };


        //-------------------------------------------------------------------------------------------------
        // private methods
        //-------------------------------------------------------------------------------------------------


        var _remove_wrapper = function () {
            if (plugin.wrapper !== null) {
                plugin.wrapper.remove();
            }
            plugin.wrapper = null;
        };


        var _show_marquee = function (msg) {

            _remove_wrapper();

            if (typeof msg !== "string") {
                return;
            }
            if (msg === "") {
                return;
            }

            let h = plugin.$element.height();
            if (h == 0) {
                h = 50;
            }
            plugin.wrapper = WidgetCommon.createElement("div", "", "height: " + h + "px;");
            plugin.element.appendChild(plugin.wrapper);

            WidgetCommon.showMarquee(plugin.wrapper, msg);
        };


        var _show_summary = function () {

            _remove_wrapper();

            if (plugin.stazioni === null || plugin.stazioni.length === 0) {
                return;
            }

            plugin.wrapper = WidgetCommon.createElement("div", "staz-container", "position:relative; height:100%; width:100%; display:flex; flex-flow:column nowrap; user-select:none;");
            plugin.element.appendChild(plugin.wrapper);

            let css = ".suolo-header { ";
            css += "    font-size: 15px; ";
            css += "    font-weight: bold; ";
            css += "    padding: 3px; ";
            css += "    text-align: center; ";
            css += "    height: 1.8em; ";
            css += "} ";
            css += ".suolo-footer { ";
            css += "    display: grid; ";
            css += "    grid-template-columns: auto 1fr auto; "; 
            css += "    font-weight: bold; ";
            css += "    padding: 3px; ";
            css += "    font-size: 11px; "
            css += "    color: #4169E1; ";
            css += "} ";
            css += ".suolo-footer > span { white-space: nowrap; overflow: hidden; text-overflow: ellipsis;} ";
            css += ".suolo-chart { ";
            css += "    width: 100%; ";
            css += "    flex-grow: 1; ";
            css += "    cursor: pointer; ";
            css += "} ";
            css += ".suolo-chart.scrollable { padding: 0px 15px; } ";
            css += ".suolo-scroller-arrow { ";
            css += "    position: absolute; ";
            css += "    top: 50%; ";
            css += "    transform: translateY(-50%);";
            css += "    cursor: pointer; ";
            css += "    opacity: 0.25; ";
            css += "} ";
            css += ".suolo-scroller-arrow:hover { opacity: 1; } ";
            css += ".suolo-scroller-arrow > .fa { margin: 0px; } ";
            css += ".suolo-na { ";
            css += "    width: 100%; ";
            css += "    height: 100%; ";
            css += "    display: flex; ";
            css += "    align-items: center; ";
            css += "    justify-content: center; ";
            css += "    overflow: hidden; ";
            css += "} ";
            css += ".suolo-na > .dot { ";
            css += "    width: 8px; ";
            css += "    height: 8px; ";
            css += "    background-color: #ccc; ";
            css += "    margin: 0px 4px; ";
            css += "    border-radius: 50%; ";
            css += "} ";

            // 29/02/2024: Add hardcoded new styles

            if (GiasVersioneMaster === "2022") {
            //if (true) {
                css += ".k-tabstrip-content .chart-block div:nth-child(0), .k-tabstrip-content .chart-block div:nth-child(1) { background-color: #05315d; color: white; } ";
                css += ".chart-help-tooltip { background-color: #05315d; color: white; } ";
            }

            // ------------------------------------


            let cssStyle = document.createElement("style");
            cssStyle.type = "text/css";
            cssStyle.innerHTML = css;
            plugin.wrapper.appendChild(cssStyle);

            let scrollCapab = (plugin.stazioni.length > 1);

            let header = WidgetCommon.createElement("div", "suolo-header");
            plugin.wrapper.appendChild(header);
            $(header).FontAdjustLabel();

            let chartwrapper = WidgetCommon.createElement("div", "suolo-chart" + (scrollCapab ? " scrollable" : ""));
            plugin.wrapper.appendChild(chartwrapper);

            chartwrapper.onclick = _open_window;

            let footer = WidgetCommon.createElement("div", "suolo-footer");
            footer.innerHTML = "<span>...</span><div></div><span>...</span>";
            plugin.wrapper.appendChild(footer);

            if (scrollCapab) {

                let leftArrow = WidgetCommon.createElement("div", "suolo-scroller-arrow");
                leftArrow.innerHTML = "<span class='fa fa-angle-left fa-3x'></span>";
                plugin.wrapper.appendChild(leftArrow);

                let rightArrow = WidgetCommon.createElement("div", "suolo-scroller-arrow", "right: 0px;");
                rightArrow.innerHTML = "<span class='fa fa-angle-right fa-3x'></span>";
                plugin.wrapper.appendChild(rightArrow);

                leftArrow.onclick = function () {
                    _summary_scroll(-1);
                };
                rightArrow.onclick = function () {
                    _summary_scroll(1);
                };
            }

            _summary_scroll(0);
        };


        var _summary_scroll = function (dir) {

            let container = plugin.$element.find(".staz-container");
            let curr_idx = parseInt(container.data("staz-index"));
            if (isNaN(curr_idx)) {
                curr_idx = 0;
            }
            let new_idx = curr_idx + dir;
            new_idx = Math.min(Math.max(0, new_idx), plugin.stazioni.length - 1);
            container.data("staz-index", new_idx);

            if (dir !== 0 && new_idx === curr_idx) {
                return;
            }

            let staz = plugin.stazioni[new_idx];

            $(plugin.$element.find(".suolo-header")).data("FontAdjustLabel").show(staz.Descrizione);

            let as = staz.AlertSerie;
            let d = ["\xa0", "\xa0"];
            if (as.length > 0) {
                d[0] = kendo.toString(new Date(as[0].data), "dd MMM yyyy");
                d[1] = kendo.toString(new Date(as[as.length - 1].data), "dd MMM yyyy");
            }
            plugin.$element.find(".suolo-footer").find("span").each(function (i, e) {
                $(e).text(d[i % 2]);
            });

            let chartwrapper = plugin.$element.find(".suolo-chart")[0];

            $(chartwrapper).children().each(function (i, e) {
                let s = $(e).data("kendoSparkline");
                if (s !== undefined) {
                    s.destroy();
                }
                e.remove();
            });

            if (staz.AlertSerie.length === 0) {

                let suolo_na = WidgetCommon.createElement("div", "suolo-na");
                chartwrapper.appendChild(suolo_na);
                for (let d = 0; d < 3; d++) {
                    let dot = WidgetCommon.createElement("span", "dot");
                    suolo_na.appendChild(dot);
                }
                return;
            }

            let sparkline = WidgetCommon.createElement("span", "", "width: 100%; height: 100%");
            chartwrapper.appendChild(sparkline);

            if (staz.sparkline === undefined) {

                let series = [];
                let data = [];
                $.each(staz.AlertSerie, function (i, e) {

                    let idx = e.value + 1; // -1 se non ho valori per la data...
                    while (idx >= series.length) {
                        series.push({
                            line: { style: "step" },
                            data: data.slice(),
                            color: ""
                        });
                    }
                    series[idx].color = e.color;

                    data.push(0);

                    $.each(series, function (j, s) {
                        if (j === idx) {
                            s.data.push((e.value === 0 ? 1 : (e.value === 1 ? 1.2 : (e.value === 2 ? 0.8 : 0))));
                        } else {
                            s.data.push(0);
                        }
                    });
                });

                let s = 1
                while (s < series.length) {
                    if (series[s].color === "") {
                        series.splice(s, 1);
                    } else {
                        s++;
                    }
                }
                if (series.length > 1 && series[0].color === "") {
                    series.splice(0, 1);
                }

                staz.sparkline = series;
            }

            $(sparkline).kendoSparkline({
                theme: "Bootstrap",
                transitions: false,
                chartArea: {
                    background: "",
                    margin: { left: 3, top: 0, right: 3, bottom: 0 }
                },
                plotArea: { margin: 0 },
                valueAxis: { min: 0, max: 1.2 },
                type: "area",
                series: staz.sparkline,
                //series: [
                //    {
                //        data: staz.AlertSerie,
                //        type: "column",
                //        gap: 0.5,
                //        valueField: "value",
                //        colorField: "color"
                //    }
                //],
                categoryAxis: {
                    visible: true,
                    color: "#4169E1",
                    majorTicks: {
                        visible: false
                    },
                    minorTicks: {
                        visible: true,
                        skip: 1,
                        step: 2,
                        width: 3,
                        size: 3,
                        color: "#4169E1"
                    },
                    crosshair: {
                        visible: false
                    }
                },
                tooltip: {
                    visible: false
                }
            });
        };


        var _open_window = function () {

            let winElem = WidgetCommon.openFrameWindow("MonitorSuolo.aspx", plugin.settings.window_title);

            let height = $(winElem).data("kendoWindow").element.height();
            let idx = parseInt(plugin.$element.find(".staz-container").data("staz-index"));

            let replyObj = {
                stazioni: plugin.stazioni,
                selected: ((isNaN(idx)) ? 0 : idx),
                height: height
            };

            WidgetCommon.listenOnceAndReply("frameIsListening", "MonitorSuolo", replyObj);
        };


        //-----------------------------------------------------------------------------------------
        // fire up the plugin! 
        //-----------------------------------------------------------------------------------------
        if (typeof options.piva === 'string' && options.piva !== '') {

            plugin.show(options.piva);

        } else {

            _show_marquee(options.msg_wait);
        }


        $(window).resize(function () {

            plugin.$element.find(".suolo-chart").find("span").each(function (i, e) {
                let s = $(e).data("kendoSparkline");
                if (s !== undefined) {
                    s.resize();
                }
            });
        });


    }; //MonitorSuolo

    //Add the plugin to the jQuery.fn object
    $.fn.MonitorSuolo = function (options) {

        //Controllo che siano state passate le impostazioni obbligatorie...
        if (typeof options.url_meteoWS !== "string" || $.trim(options.url_meteoWS) === "") {
            return;
        }

        // iterate through the DOM elements we are attaching the plugin to
        return this.each(function () {
            // if plugin has not already been attached to the element
            if (undefined == $(this).data("MonitorSuolo")) {
                // create a new instance of the plugin
                // pass the DOM element and the user-provided options as arguments
                var plugin = new $.MonitorSuolo(this, options);

                // in the jQuery version of the element store a reference to the plugin object
                // you can later access the plugin and its methods and properties like
                // element.data('pluginName').publicMethod(arg1, arg2, ... argn) or
                // element.data('pluginName').settings.propertyName
                $(this).data("MonitorSuolo", plugin);
            }
        });
    };

})(jQuery);
