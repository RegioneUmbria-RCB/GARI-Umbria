

//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
//
//Plugin per impostazione riepilogo dati meteo
//
//$(...).RiepilogoMeteo({ Configurazione json });
//
//Configurazione:
//      url_meteoWS: "../DataAnalisiBI/Meteo/MeteoWS.aspx" (Obbligatorio)
//      window_title: Stringa titolo finestra popup
//      msg_wait: Stringa messaggio per attesa elaborazione...
//      msg_na: Stringa messaggio se elaborazione non disponibile...
//      msg_err: Stringa messaggio se errore in elaborazione...
//      fun_callback: Funzione chiamata al termine dell'elaborazione (parametro status: -1 errore, 0 nulla da visualizzare, 1 ok)
//
//
//$(...).data('RiepilogoMeteo').show(piva)
//
//
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------


(function ($) {

    $.RiepilogoMeteo = function (elem, options) {

        // to avoid confusions, use "plugin" to reference the current instance of the object
        var plugin = this;
        // this will hold the merged default, and user-provided options plugin's properties will be available through this object like:
        // plugin.settings.propertyName from inside the plugin or element.data('pluginName').settings.propertyName from outside the plugin,
        // where "element" is the element the plugin is attached to;
        plugin.settings = {};
        plugin.$element = $(elem); // reference to the jQuery version of DOM element
        plugin.element = elem; // reference to the actual DOM element
        plugin.wrapper = null;
        plugin.stazioni = null;

        // plugin's default options this is private property and is accessible only from inside the plugin
        let defaults = {
            window_title: TraduzioneMultiResx(datiMeteoResx, "DatiMeteo", "Dati meteo"),
            msg_wait: "",
            msg_na: "",
            msg_err: "",
            fun_callback: null,
            fun_callbackShowResults: null
        };

        // the plugin's final properties are the merged default and user-provided options (if any)
        plugin.settings = $.extend({}, defaults, options);
        plugin.settings.url_meteoWS += "/DatiMeteo_ElaboraRiepilogo";


        //-------------------------------------------------------------------------------------------------
        // public methods
        //-------------------------------------------------------------------------------------------------


        plugin.show = function (piva) {

            _remove_wrapper();

            plugin.wrapper = WidgetCommon.createElement("div", "", "width:100%; height:100%;");
            plugin.element.appendChild(plugin.wrapper);
       
            $(plugin.wrapper).StyleLoader({ type: "DotCircle", color: "#333" });

            plugin.settings.piva = piva;

            setTimeout(_pollingFunc, 500);
        };


        //-------------------------------------------------------------------------------------------------
        // private methods
        //-------------------------------------------------------------------------------------------------


        var _pollingFunc = function () {

            let params = {
                piva: plugin.settings.piva
            };

            ajaxAgronica(plugin.settings.url_meteoWS,
                JSON.stringify(params),
                function (risposta) {

                    let result = risposta.RispostaStringa;

                    let stazioni = result.Stazioni;

                    if (stazioni === null || stazioni.length === 0) {

                        //not available
                        _show_marquee(plugin.settings.msg_na);

                        _callback(0);

                    } else {

                        plugin.stazioni = [];

                        $.each(stazioni,
                            function (idx, elem) {

                                plugin.stazioni.push({
                                    Descrizione: elem.Descrizione,
                                    UltimoAggiornamento: elem.UltimoAggiornamento,
                                    Meteo: {
                                        Table: JSON.parse(elem.Meteo.Meteo_Table),
                                        Charts: JSON.parse(elem.Meteo.Meteo_Charts),
                                        Riepilogo: JSON.parse(elem.Meteo.Meteo_RiepilogoSensori)
                                    }
                                });
                            }
                        );

                        _callback(1);

                        if (plugin.settings.summary) {
                            _show_summary();
                        } else {
                            _remove_wrapper();
                            if (typeof plugin.settings.fun_callbackShowResults === 'function') {                                
                                plugin.settings.fun_callbackShowResults(plugin.stazioni);
                            }
                            
                        }
                    }
                },
                function (risposta) {

                    _show_marquee(plugin.settings.msg_err);

                    _callback(-1);

                    plugin.ErrorResponse = risposta.Errore;
                    plugin.wrapper.onclick = function (e) {
                        WidgetCommon.ctrlClickMsg(e, plugin.ErrorResponse, plugin.settings.msg_err);
                    };
                },
                null,
                false
            );
        };


        var _callback = function (status) {
            if (typeof plugin.settings.fun_callback === 'function') {
                plugin.settings.fun_callback(status);
            }
        }

 
        var _remove_wrapper = function () {
            if (plugin.wrapper !== null) {
                plugin.wrapper.remove();
            }
            plugin.wrapper = null;
        };


        var _show_marquee = function (msg) {

            _remove_wrapper();

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

            let scrollCapab = (plugin.stazioni.length >= 2);

            plugin.wrapper = WidgetCommon.createElement("div", "", "position:relative; height:100%; cursor: default;");
            plugin.element.appendChild(plugin.wrapper);

            let cssHTML = ".scroll-arrow { opacity: 0.2; cursor: pointer; position: absolute;  top: 50%; transform: translateY(-50%); } ";
            cssHTML += " .scroll-arrow:hover { opacity: 0.8 !important; } ";
            cssHTML += " .summary-chart { height: 100%; margin: 0px 2px; cursor: pointer; }"; //display: grid; grid-template-rows: 1fr 1fr; grid-row-gap: 1px; grid-auto-columns: 100%; } ";
            cssHTML += " .summary-chart-child { height: 50%; width: 100%;  background-color: #fff; border-radius: 4px; border: 1px solid #ccc; } ";
            cssHTML += " .summary-chart-child-hidden { display: none; } ";
            cssHTML += " .summary-text { font-size: 11px; height: 1.5em; text-align: center; font-weight: bold; padding: 0 7px; } ";

            let cssStyle = document.createElement("style");
            cssStyle.type = "text/css";
            cssStyle.innerHTML = cssHTML;
            plugin.wrapper.appendChild(cssStyle);

            let container = WidgetCommon.createElement("div", "", "height:100%; display: flex; flex-flow: column nowrap;", "summary-container-METEO");
            plugin.wrapper.appendChild(container);

            let title = WidgetCommon.createElement("div", "summary-text summary-title");

            let bodyStyle = "flex-grow: 1;";
            if (scrollCapab) {
                bodyStyle += " padding: 0px 16px; ";
            }
            let body = WidgetCommon.createElement("div", "", bodyStyle);
            let footer = WidgetCommon.createElement("div", "summary-text summary-footer");
            footer.setAttribute("title", TraduzioneMultiResx(datiMeteoResx, "UltimoDatoMeteoDisponibile", "Ultimo dato meteo disponibile"));

            container.appendChild(title);
            container.appendChild(body);
            container.appendChild(footer);

            $(title).FontAdjustLabel();

            let bodyChart = WidgetCommon.createElement("div", "summary-chart", "height: 100%;", "stazione");
            body.appendChild(bodyChart);
            let chart_1 = WidgetCommon.createElement("div", "summary-chart-child", "margin-bottom: 1px;", "summary-chart-0");
            let chart_2 = WidgetCommon.createElement("div", "summary-chart-child", "margin-top: 1px;", "summary-chart-1");
            bodyChart.appendChild(chart_1);
            bodyChart.appendChild(chart_2);

            if (scrollCapab) {

                let leftArrow = WidgetCommon.createElement("div", "scroll-arrow", "left: 0;");
                let spanLeftArrow = WidgetCommon.createElement("span", "fa fa-angle-left fa-3x", "margin: 0px;");
                leftArrow.appendChild(spanLeftArrow);

                let rightArrow = WidgetCommon.createElement("div", "scroll-arrow", "right: 0;");
                let spanRightArrow = WidgetCommon.createElement("span", "fa fa-angle-right fa-3x", "margin:0px;");
                rightArrow.appendChild(spanRightArrow);

                plugin.wrapper.appendChild(leftArrow);
                plugin.wrapper.appendChild(rightArrow);

                leftArrow.onclick = function () {
                    _summary_scroll(-1);
                };

                rightArrow.onclick = function () {
                    _summary_scroll(1);
                };
            }

            bodyChart.setAttribute("data-scroll", "0");
            _summary_scroll(0);

            bodyChart.onclick = _open_window;
        };


        var _summary_scroll = function (dir) {

            let idx = parseInt($("#summary-container-METEO").attr("data-scroll"));
            if (isNaN(idx)) {
                idx = 0;
            }
            idx += parseInt(dir);
            if (idx < 0 || idx >= plugin.stazioni.length) {
                return;
            }

            let stazione = plugin.stazioni[idx];

            let $title = $("#summary-container-METEO .summary-title");
            let $chart = $("#summary-container-METEO .summary-chart");
            let $footer = $("#summary-container-METEO .summary-footer");

            if ($title.length) {

                $title.data("FontAdjustLabel").show(stazione.Descrizione);
            }

            if ($chart.length) {
                let divChart = $chart[0];
                //while (divChart.firstChild) {
                //    divChart.removeChild(divChart.firstChild);
                //}

                _output_stazione_summary(stazione, divChart);
            }

            if ($footer.length) {

                let aggiornamento = "";
                //if (stazione.Meteo.length > 0) {
                //    let dtFormat = "dd MMM - HH:mm";
                //    let ultimo = stazione.Meteo[stazione.Meteo.length - 1];
                //    let adesso = new Date(Date.now());
                //    if (adesso.getFullYear() !== ultimo.DataOra.getFullYear()) {
                //        dtFormat = "dd MMM yyyy";
                //    }
                //    aggiornamento = kendo.toString(ultimo.DataOra, dtFormat);
                //}

                let dtFormat = "dd MMM - HH:mm";
                let ultimo = new Date(stazione.UltimoAggiornamento);
                let adesso = new Date(Date.now());
                if (adesso.getFullYear() !== ultimo.getFullYear()) {
                    dtFormat = "dd MMM yyyy";
                }
                aggiornamento = kendo.toString(ultimo, dtFormat);

                $footer.html(aggiornamento);
            }

            $("#summary-container-METEO").attr("data-scroll", idx);
        };


        var _output_stazione_summary = function (stazione, divChart) {

            let template_tooltip = "<div style='margin-bottom:5px; font-weight:bold;'>#: kendo.toString(category, 'dd MMMM - HH') #</div>";
            template_tooltip += "<table>";
            template_tooltip += "# for (var i = 0; i < points.length; i++) { #";
            template_tooltip += "<tr>";
            template_tooltip += "<td><div style='width:15px; height:3px; background-color:#: points[i].series.color#;'></div></td>";
            template_tooltip += "<td>#: points[i].series.name#</td>";
            template_tooltip += "<td>:</td>";
            template_tooltip += "<td style='text-align:right;'>#: kendo.toString(points[i].value, '0.0') #</td>";
            template_tooltip += "</tr>";
            template_tooltip += "# } #";
            template_tooltip += "</table>";

            $(divChart).children().each(function (idx, elem) {
                $(elem).addClass("summary-chart-child-hidden");
                while (elem.firstChild) {
                    elem.removeChild(elem.firstChild);
                }
            });

            let maxOutput = $(divChart).children().length;
            let iOutput = 0;
            let maxChart = stazione.Meteo.Charts.length;
            let iChart = 0;

            while (iOutput < maxOutput && iChart < maxChart) {

                let chart = stazione.Meteo.Charts[iChart];
                let chartElem = $(divChart).children()[iOutput];

                let series = [];
                $.each(chart.series,
                    function (i, s) {

                        let s_ = $.extend({}, { missingValues: "interpolate", gap: 0.25 }, s);

                        if (s_.FunAggreg === "avg" || s_.FunAggreg === "sum" || s_.FunAggreg === "max" || s_.FunAggreg === "min") {

                            s_.aggregate = s_.FunAggreg;

                            series.push(s_);

                        } else if (s_.FunAggreg !== "") {

                            s_.aggregate = "avg";

                            series.push(s_);
                        }
                    }
                );

                if (series.length > 0) {

                    let axis = [];

                    $.each(chart.axis,
                        function (i, a) {
                            axis.push({ name: a.name });
                        }
                    );

                    let horizField = "";
                    if (typeof chart.horizAxis === "string") {
                        horizField = chart.horizAxis;
                    } else {
                        horizField = chart.horizAxis.field;
                    }

                    $(chartElem).removeClass("summary-chart-child-hidden");

                    let id_sparkline = "sparkline-" + iOutput;
                    let sparkline = WidgetCommon.createElement("span", "", "width: 100%; height: 100%", id_sparkline);

                    chartElem.appendChild(sparkline);

                    let chart_cfg = {
                        dataSource: stazione.Meteo.Table.kendo_rows,
                        theme: "Bootstrap",
                        transitions: false,
                        chartArea: {
                            background: "",
                            margin: { left: 3, top: 2, right: 3, bottom: 0 }
                        },
                        plotArea: { margin: 0 },
                        series: series,
                        categoryAxis: {
                            type: "date",
                            field: horizField,
                            baseUnit: "hours",
                            visible: true,
                            line: { color: "#808080", dashType: "dot" },
                            majorTicks: { visible: false },
                            labels: { visible: false },
                            crosshair: { visible: true, width: 1, color: "#808080" }
                        },
                        valueAxis: axis,
                        tooltip: {
                            visible: true,
                            shared: true,
                            background: "#808080",
                            //opacity: 0.8,
                            border: { color: "#808080" },
                            sharedTemplate: kendo.template(template_tooltip)
                        }
                    };

                    $("#" + id_sparkline).kendoSparkline(chart_cfg);

                    iOutput++;
                }

                iChart++;
            }
        }


        var _open_window = function () {

            let winElem = WidgetCommon.openFrameWindow("RiepilogoMeteo.aspx", plugin.settings.window_title);
           
            $(winElem).addClass("METEO-window");

            let height = $(winElem).data("kendoWindow").element.height();
            let idx = parseInt($("#summary-container-METEO").attr("data-scroll"));

            let replyObj = {
                stazioni: plugin.stazioni,
                selected: ((isNaN(idx)) ? 0 : idx),
                height: height
            };

            WidgetCommon.listenOnceAndReply("frameIsListening", "RiepilogoMeteo", replyObj);
        };


        //-----------------------------------------------------------------------------------------
        // fire up the plugin! 
        //-----------------------------------------------------------------------------------------


        if (typeof plugin.settings.piva === 'string' && plugin.settings.piva !== '') {

            plugin.show(plugin.settings.piva);

        } else {

            _show_marquee(plugin.settings.msg_wait);
        }


        $(window).resize(function (e) {

            plugin.$element.find(".summary-chart").find("span").each(function (idx, elem) {
                let chart = $(elem).data("kendoSparkline");
                if (chart !== undefined) {
                    chart.resize();
                }
            });

            let kendoWindow = $(document.getElementsByClassName("METEO-window")).getKendoWindow();

            if (kendoWindow) {

                kendoWindow.center();

                let $METEO_window = $(".METEO-window");

                $METEO_window.find(".k-widget.k-tabstrip").each(function () {

                    let tabh = $METEO_window.height() - ($(this).height() - $(this).find(".k-content").height());
                    $(this).find(".k-content").height(Math.floor(tabh))
                });

                $METEO_window.find(".meteo-grid").each(function () {

                    $(this).find(".k-sparkline").each(function () {

                        let sparkline = $(this).getKendoSparkline();

                        if (sparkline) {

                            sparkline.resize();
                        }
                    });
                });

                $METEO_window.find(".k-chart").each(function () {

                    let chart = $(this).getKendoChart();

                    if (chart) {

                        chart.resize();
                    }
                });
            }
        });


    }; //RiepilogoMeteo

    //Add the plugin to the jQuery.fn object
    $.fn.RiepilogoMeteo = function (options) {

        //Controllo che siano stati passati le impostazioni obbligatorie...
        if (typeof options.url_meteoWS !== "string" || $.trim(options.url_meteoWS) === "") {
            return;
        }

        // iterate through the DOM elements we are attaching the plugin to
        return this.each(function () {

            // if plugin has not already been attached to the element
            if (undefined == $(this).data("RiepilogoMeteo")) {

                // create a new instance of the plugin
                // pass the DOM element and the user-provided options as arguments
                var plugin = new $.RiepilogoMeteo(this, options);

                // in the jQuery version of the element store a reference to the plugin object
                // you can later access the plugin and its methods and properties like
                // element.data('pluginName').publicMethod(arg1, arg2, ... argn) or
                // element.data('pluginName').settings.propertyName
                $(this).data("RiepilogoMeteo", plugin);
            }
        });
    };
})(jQuery);
