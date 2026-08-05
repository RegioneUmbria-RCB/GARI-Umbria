
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
//
//Plugin per impostazione indicatori DSS (dati meteo / modelli previsionali)
//
//$(...).DSS_Summary({ Configurazione json });
//
//Configurazione:
//      url_meteoWS: "../DataAnalisiBI/Meteo/MeteoWS.aspx" (Obbligatorio)
//      summary: Visualizzazione riepilogo oppure completa (default true)
//      view_grid: Visualizzazione griglia (default true)
//      msg_summary: Stringa messaggio in sovrapposizione al riepilogo...
//      window_title: Stringa titolo finestra popup
//      msg_wait: Stringa messaggio per attesa elaborazione...
//      msg_na: Stringa messaggio se elaborazione non disponibile...
//      msg_err: Stringa messaggio se errore in elaborazione...
//      fun_callback: Funzione chiamata al termine dell'elaborazione (parametro status: -1 errore, 0 nulla da visualizzare, 1 ok)
//
//
//$(...).data('DSS_Summary').show(piva)
//
//
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

(function ($) {

    $.DSS_Summary = function (elem, options) {

        // to avoid confusions, use "plugin" to reference the current instance of the object
        var plugin = this;
        // this will hold the merged default, and user-provided options plugin's properties will be available through this object like:
        // plugin.settings.propertyName from inside the plugin or element.data('pluginName').settings.propertyName from outside the plugin,
        // where "element" is the element the plugin is attached to;
        plugin.settings = {};
        plugin.$element = $(elem); // reference to the jQuery version of DOM element
        plugin.element = elem; // reference to the actual DOM element
        plugin.wrapper = null;
        plugin.indicatori = null;

        // plugin's default options this is private property and is accessible only from inside the plugin
        let defaults = {
            summary: true,
            view_grid: true,
            msg_summary: "",
            window_title: TraduzioneMultiResx(datiMeteoResx, "IndicatoriDSS", "Indicatori DSS"),
            msg_wait: "",
            msg_na: "",
            msg_err: "",
            fun_callback: null,
            gauge_click_callback: null,
            wait_callback: null,
            date_range: false
        };

        // the plugin's final properties are the merged default and user-provided options (if any)
        plugin.settings = $.extend({}, defaults, options);
        plugin.settings.url_meteoWS += "/ModelliPrevisionali_ElaboraIndicatori";


        //-------------------------------------------------------------------------------------------------
        // public methods
        //-------------------------------------------------------------------------------------------------


        plugin.show = function (piva, rag_soc) {
            _show_loader();

            plugin.settings.piva = piva;

            plugin.settings.rag_soc = rag_soc;

            setTimeout(_pollingFunc, 500);
        };


        //-------------------------------------------------------------------------------------------------
        // private methods
        //-------------------------------------------------------------------------------------------------


        var _callback = function (status) {
            if (typeof plugin.settings.fun_callback === 'function') {
                plugin.settings.fun_callback(status);
            }
        }


        var _pollingFunc = function () {

            let parExtra = "";

            if (plugin.settings.date_range) {
                let obj = _dataRange_cookie(null);
                if (obj !== null) {
                    parExtra = JSON.stringify({ DataInizio: obj.startDate, DataFine: obj.endDate });
                }
            }

            let params = {
                piva: plugin.settings.piva,
                parExtra: parExtra
            };

            //*********************************************************
            //DEBUG
            //*********************************************************
            //_gestisci_risposta(dbg_indicatori());
            //return;
            //*********************************************************
            //*********************************************************
            //*********************************************************

            ajaxAgronica(plugin.settings.url_meteoWS,
                JSON.stringify(params),
                function (risposta) {

                    _gestisci_risposta(risposta.RispostaStringa);
                },
                function (risposta) {

                    _show_msg(plugin.settings.msg_err);

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
    

        var _remove_wrapper = function () {
            if (plugin.wrapper !== null) {
                plugin.wrapper.remove();
            }
            plugin.wrapper = null;
        };


        var _show_loader = function () {

            _remove_wrapper();

            plugin.wrapper = WidgetCommon.createElement("div", "", "width:100%; height:100%;");
            plugin.element.appendChild(plugin.wrapper);

            $(plugin.wrapper).StyleLoader({ type: "DotCircle", color: "#333" });
        };


        var _show_msg = function (msg) {
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


        var _wait = function () {

            setTimeout(_pollingFunc, 2000);

            if (typeof plugin.settings.wait_callback === 'function') {

                plugin.settings.wait_callback();
                plugin.settings.wait_callback = null;
            }
        }


        var _gestisci_risposta = function (result) {

            if (result === null) {
                _wait();
                return;
            }
            if (result.Stato === 0) { // InAttesa
                _wait();
                return;
            }
            if (result.Stato === -1) { // NonDisponibili
                _show_msg(plugin.settings.msg_na);
                _callback(0);
                return;
            }
            //if (result.Stato === 1) // Pronti

            plugin.indicatori = result.Indicatori.filter((elem) => elem.Stazione != null && elem.Specie != null);

            plugin.indicatori.sort(function (elem0, elem1) {
                let v0 = [elem0.Stazione.toUpperCase(), elem0.Specie.toUpperCase()];
                let v1 = [elem1.Stazione.toUpperCase(), elem1.Specie.toUpperCase()];
                let cmp = 0;
                for (let idx = 0; (cmp === 0 && idx < v0.length); idx++) {
                    if (v0[idx] < v1[idx]) {
                        cmp = -1;
                    } else if (v0[idx] > v1[idx]) {
                        cmp = 1;
                    }
                }
                return cmp;
            });

            $.each(plugin.indicatori, function (i, indic) {
                switch (indic.Risultato.Status) {
                    case -1: // Status_OutOfRange
                        indic.Risultato.Status = "outofrange";
                        break;
                    case 1: // Status_Warning
                        indic.Risultato.Status = "warning";
                        break;
                    case 2: // Status_Error
                        indic.Risultato.Status = "error";
                        break;
                    case 3: // Status_ProgressBefore
                        indic.Risultato.Status = "progress";
                        break;
                    default: //case 0: // Status_Valid
                        indic.Risultato.Status = "valid";
                }

                if (indic.Risultato.MeteoStatus == undefined) {
                    indic.Risultato.MeteoStatus = 0;
                    indic.Risultato.MeteoMsg = "";
                }

                switch (indic.Risultato.MeteoStatus) {
                    case 1:
                        indic.Risultato.MeteoStatus = "info";
                        break;
                    case 2:
                        indic.Risultato.MeteoStatus = "warning";
                        break;
                    default:
                        indic.Risultato.MeteoStatus = "none";
                        break;
                }
            });

            _callback(1);

            if (plugin.settings.summary) {

                _show_summary();

            } else {

                _show_gauges();
            }
        };


        var _show_summary = function () {

            _remove_wrapper();

            if (plugin.indicatori === null || plugin.indicatori.length === 0) {
                return;
            }

            let colors = {};
            let col = "";
            $.each(plugin.indicatori, function (i, indic) {

                switch (indic.Risultato.Status) {
                    case "error":
                        col = "#ccc";
                        break;
                    case "outofrange":
                        col = "#ccc";
                        break;
                    case "progress":
                        col = "#ccc";
                        break;
                    default:
                        $.each(indic.Risultato.Bands, function (b, band) {
                            if (indic.Risultato.Value <= band.Value) {
                                col = band.Color;
                                return false;
                            }
                        });
                }

                if (col !== "") {
                    col = "C_" + col;
                    if (!colors.hasOwnProperty(col)) {
                        colors[col] = 0;
                    }
                    colors[col] += 1;
                    col = "";
                }
            });

            let serie_data = [];
            $.each(Object.keys(colors), function (i, k) {
                serie_data.push({
                    value: colors[k],
                    color: k.substring(2)
                });
            });

            plugin.wrapper = WidgetCommon.createElement("div", "", "position:relative; cursor:pointer;");
            plugin.wrapper.onclick = _open_window;
            plugin.element.appendChild(plugin.wrapper);
            plugin.wrapper.appendChild(WidgetCommon.createElement("div", "", "", "gauge-summary"));

            let h = plugin.$element.height();
            if (h == 0) {
                h = 100;
            }

            $("#gauge-summary").kendoChart({
                theme: "Bootstrap",
                transitions: false,
                chartArea: { height: h, background: "" },
                legend: { visible: false },
                series: [{
                    type: "pie",
                    markers: { visible: false },
                    highlight: { visible: false },
                    overlay: { gradient: "roundedBevel" },
                    border: { color: "#ccc" },
                    data: serie_data
                }]
            });

            if (plugin.settings.msg_summary !== "") {
                let container = WidgetCommon.createElement("div", "", "position:absolute; top:0; height:1.7em; width:100%; opacity:0.33;");
                plugin.wrapper.appendChild(container);

                WidgetCommon.showMarquee(container, plugin.settings.msg_summary);
            }

            if (plugin.settings.date_range) {

                let container = WidgetCommon.createElement("div", "", "position:absolute; bottom:0; width:100%; cursor:default;");
                plugin.wrapper.appendChild(container);
                let eStyle = document.createElement("style");
                eStyle.type = "text/css";
                eStyle.innerHTML = ".outDataRange.disabled { color: #BBB; }";
                container.appendChild(eStyle);

                let div = WidgetCommon.createElement("div", "", "display:grid; grid-template-columns: auto 1fr auto; grid-gap: 5px; align-items: center; justify-items: center;");
                container.appendChild(div);

                let divCalendar = WidgetCommon.createElement("div", "fa fa-calendar-o fa-lg", "margin:5px; cursor:pointer;");
                let spanTxt = WidgetCommon.createElement("span", "outDataRange", "white-space:nowrap; overflow:hidden;");
                //let divEraser = WidgetCommon.createElement("div", "fa fa-eraser fa-lg", "margin:5px; cursor:pointer;");
                let divEraser = WidgetCommon.createElement("div", "", "cursor:pointer;");
                let spanEraser = WidgetCommon.createElement("span", "fa-stack");
                spanEraser.innerHTML = "<i class='fa fa-calendar-o fa-stack-1x' style='margin: 0px;'></i><i class='fa fa-ban fa-stack-2x text-danger' style='margin: 0px;' ></i>";
                divEraser.appendChild(spanEraser);

                div.appendChild(divCalendar);
                div.appendChild(spanTxt);
                div.appendChild(divEraser);

                _dataRange_default();
                let obj = _dataRange_cookie(null);
                if (obj !== null) {
                    plugin.settings.data_range_options = obj;
                }
                _dataRange_output(div);

                container.onclick = function (e) {
                    e.stopPropagation();
                };

                divCalendar.onclick = function (e) {
                    _dataRange_dialog(e.currentTarget.parentNode);
                };

                divEraser.onclick = function (e) {
                    _dataRange_default();
                    _dataRange_cookie(plugin.settings.data_range_options);
                    _dataRange_output(e.currentTarget.parentNode);
                    _dataRange_refresh();
                };

            }
        };


        var _show_gauges = function () {

            _remove_wrapper();

            let parent = $(plugin.element.parentElement);
            let parent_h = parent.height();

            DSS_Gauges.showGauges(plugin.settings.rag_soc, plugin.indicatori, plugin.element, parent_h, plugin.settings.view_grid, plugin.settings.gauge_click_callback, url_meteo_ws);
        };


        var _dataRange_default = function () {
            let oggi = new Date();
            oggi.setHours(0, 0, 0, 0);
            let d0 = new Date(oggi.getFullYear(), 0, 1);
            let d1 = new Date(oggi);

            plugin.settings.data_range_options = {
                apply: false,
                startDate: d0,
                endDate: d1
            };
        };


        var _dataRange_output = function (parentDiv) {

            let span = $(parentDiv).find(".outDataRange");
            if (span.length > 0) {
                span[0].innerHTML = kendo.toString(plugin.settings.data_range_options.startDate, 'd') + " - " + kendo.toString(plugin.settings.data_range_options.endDate, 'd');
                if (plugin.settings.data_range_options.apply) {
                    $(span[0]).removeClass("disabled");
                } else {
                    $(span[0]).addClass("disabled");
                }
            }
        };


        var _dataRange_cookie = function (obj) {

            let cookieName = "DSS.Indicatori.Periodo." + plugin.settings.piva;

            if (obj == null) {

                let jsonDSS = $.cookie(cookieName);

                if (jsonDSS != null) {
                    obj = JSON.parse(jsonDSS);
                    obj.startDate = new Date(obj.startDate);
                    obj.endDate = new Date(obj.endDate);
                }

            } else {

                let date = new Date();
                date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
                $.cookie(cookieName, JSON.stringify(obj), { expires: date, path: "/" });
            }

            return obj;
        };


        var _dataRange_dialog = function (currentTarget) {

            let win_el = document.createElement("div");
            win_el.id = "id_DSS_DataRange_kendoDlg";
            document.body.appendChild(win_el);
            let $win_el = $("#" + win_el.id);

            let content = "<div style='display: grid; grid-template-columns: 1fr 1fr; grid-gap: 10px; margin-bottom: 15px;'>";
            content += "<input style='width: 100%;' id='DSS_DataInizio' />";
            content += "<input style='width: 100%;' id='DSS_DataFine' />";
            content += "</div>";
            content += "<span id='staticNotification'></span>";
            content += "<div id='Dlg_DSS_Notify'></div>";

            $win_el.kendoDialog({
                title: TraduzioneMultiResx(datiMeteoResx, "ImpostazionePeriodoCalcoloIndicatori", "Impostazione periodo calcolo indicatori emergenze DSS"),
                closable: false,
                modal: true,
                visible: false,
                content: content,
                open: function () {
                    $("#DSS_DataInizio").kendoDatePicker({ footer: false });
                    $("#DSS_DataFine").kendoDatePicker({ footer: false });

                    $("#DSS_DataInizio").data("kendoDatePicker").value(plugin.settings.data_range_options.startDate);
                    $("#DSS_DataFine").data("kendoDatePicker").value(plugin.settings.data_range_options.endDate);

                    $("#staticNotification").kendoNotification({ appendTo: "#Dlg_DSS_Notify", autoHideAfter: 0, animation: false });
                },
                actions: [
                    {
                        text: 'OK',
                        action: function () {

                            let d0 = $("#DSS_DataInizio").data("kendoDatePicker").value();
                            let d1 = $("#DSS_DataFine").data("kendoDatePicker").value();

                            let notification = $("#staticNotification").data("kendoNotification");
                            notification.hide();
                            if (d0 === null) {
                                notification.error(TraduzioneMultiResx(datiMeteoResx, "DataInizioPeriodoNonValida", "Data inizio periodo non valida"));
                                return false;
                            }
                            if (d1 === null) {
                                notification.error(TraduzioneMultiResx(datiMeteoResx, "DataFinePeriodoNonValida", "Data fine periodo non valida"));
                                return false;
                            }
                            if (d0 >= d1) {
                                notification.error(TraduzioneMultiResx(datiMeteoResx, "DataInizioMaggioreUgualeDataFine", "Data inizio maggiore o uguale a data fine"));
                                return false;
                            }

                            //if (d0.getFullYear() !== d1.getFullYear()) {
                            //    notification.error("Anno data inizio diverso da anno data fine");
                            //    return false;
                            //}

                            plugin.settings.data_range_options = {
                                apply: true,
                                startDate: d0,
                                endDate: d1
                            };

                            _dataRange_cookie(plugin.settings.data_range_options);
                            _dataRange_output(currentTarget);
                            _dataRange_refresh();
                        }
                    },
                    {
                        text: TraduzioneMultiResx(datiMeteoResx, "Annulla", "Annulla")
                    }
                ],
                close: function (e) {
                    this.destroy();
                }
            });

            $win_el.data("kendoDialog").open();
        };


        var _dataRange_refresh = function () {

            _show_loader();

            setTimeout(_pollingFunc, 500);
        };

         
        var _open_window = function () {

            let winElem = WidgetCommon.openFrameWindow("DSS_Gauges.aspx", plugin.settings.window_title);

            let height = $(winElem).data("kendoWindow").element.height();

            let replyObj = {
                elemID: 'Gauges-Window',
                indicatori: plugin.indicatori,
                view_grid: plugin.settings.view_grid,
                height: height
            };

            WidgetCommon.listenOnceAndReply("frameIsListening", "DSS_Gauges", replyObj);

            window.addEventListener("message",
                function (event) {

                    if (verificaOriginSecondaria(self, location.href, event) == false) {
                        return false;
                    }

                    if (event.data.sourceId === "Gauges-Window") {

                        if (typeof plugin.settings.gauge_click_callback === "function") {

                            plugin.settings.gauge_click_callback(event.data.callbackParams);
                        }
                    }
                },
                false
            );
        };


        //-----------------------------------------------------------------------------------------
        // fire up the plugin! 
        //-----------------------------------------------------------------------------------------


        if (typeof plugin.settings.piva === 'string' && plugin.settings.piva !== '') {

            plugin.show(plugin.settings.piva, plugin.settings.rag_soc);

        } else {

            _show_msg(plugin.settings.msg_wait);
        }


        $(window).resize(function (e) {

            let elem = $(plugin.element).children().find("#gauge-summary");
            if (elem.length > 0) {
                let chart = $(elem).data("kendoChart");
                if (chart !== undefined) {
                    chart.resize();
                }
            }
        });

    }; //DSS_Summary

    //Add the plugin to the jQuery.fn object
    $.fn.DSS_Summary = function (options) {

        //Controllo che siano stati passati le impostazioni obbligatorie...
        if (typeof options.url_meteoWS !== "string" || $.trim(options.url_meteoWS) === "") {
            return;
        }

        // iterate through the DOM elements we are attaching the plugin to
        return this.each(function () {
            // if plugin has not already been attached to the element
            if (undefined == $(this).data("DSS_Summary")) {
                // create a new instance of the plugin
                // pass the DOM element and the user-provided options as arguments
                var plugin = new $.DSS_Summary(this, options);

                // in the jQuery version of the element store a reference to the plugin object
                // you can later access the plugin and its methods and properties like
                // element.data('pluginName').publicMethod(arg1, arg2, ... argn) or
                // element.data('pluginName').settings.propertyName
                $(this).data("DSS_Summary", plugin);
            }
        });

    };

})(jQuery);

function dbg_indicatori() {

    let Indicatori = [
        {
            "Parametri": {
                "Tipo_Sorgente": 2,
                "Stazione_Cod": 244,
                "Mod_Cod": 16,
                "Veg_Cod": 64,
                "Avv_Cod": 47,
                "Alg_Cod": 0,
                "ParametriElaborazione": "{}"
            },
            "Risultato": {
                "DataInizio": "2021-01-01 00:00:00Z",
                "DataFine": "2021-06-25 00:00:00Z",
                "Scale_Min": 0,
                "Scale_Max": 1,
                "Bands": [
                    { "Color": "#008000", "Value": 0.2 },
                    { "Color": "#c0ff00", "Value": 0.4 },
                    { "Color": "#fff000", "Value": 0.6 },
                    { "Color": "#ff8000", "Value": 0.8 },
                    { "Color": "#f00000", "Value": 7.922816251426434e+28 }
                ],
                "Value": 0,
                "Status": 0,
                "StatusMsg": "",
                "AuxMsg": ""
            },
            "Stazione": "Agrisfera - Palazzina (DSS)",
            "Modello": "Peronospora della vite [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Peronospora della Vite",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 43,
                "Mod_Cod": 19,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 244,
                "Tipo_Sorgente": 2,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 8
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 24
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 40,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "Agrisfera - Palazzina (DSS)",
            "Modello": "Oidio della vite [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Mal bianco (Oidio) della Vite",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 35,
                "Mod_Cod": 20,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 244,
                "Tipo_Sorgente": 2,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 1.38
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 1.72
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 2.5,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 1.29887197137193
            },
            "Stazione": "Agrisfera - Palazzina (DSS)",
            "Modello": "Botrite della vite [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Botrite (=Muffa grigia)",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 532,
                "Mod_Cod": 35,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 244,
                "Tipo_Sorgente": 2,
                "Veg_Cod": 52
            },
            "Risultato": {
                "AuxMsg": "Presenza dello stadio Adulti",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 100
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 200
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 300,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 4.044365846537817
            },
            "Stazione": "Agrisfera - Palazzina (DSS)",
            "Modello": "Nottua del pomodoro (Helicoverpa armigera) [Agronomica 3.0]",
            "Specie": "Pomodoro",
            "Avversita": "Nottua gialla del Pomodoro",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 138,
                "Mod_Cod": 36,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 244,
                "Tipo_Sorgente": 2,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "Presenza dello stadio Larve",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 100
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 200
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 300,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 203.5586797462038
            },
            "Stazione": "Agrisfera - Palazzina (DSS)",
            "Modello": "Tignoletta della vite (Lobesia botrana) [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Tignoletta della Vite (=L. botrana)",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 46,
                "Mod_Cod": 42,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 244,
                "Tipo_Sorgente": 2,
                "Veg_Cod": 46
            },
            "Risultato": {
                "AuxMsg": "Probabilità di infezione 0%",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 1.4
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 2.55
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 5,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "Agrisfera - Palazzina (DSS)",
            "Modello": "Modello MISP/IPI [Agronomica 3.0]",
            "Specie": "Patata",
            "Avversita": "Peronospora della Patata e del Pomodoro",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 46,
                "Mod_Cod": 42,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 244,
                "Tipo_Sorgente": 2,
                "Veg_Cod": 52
            },
            "Risultato": {
                "AuxMsg": "Probabilità di infezione 0%",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 1.4
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 2.55
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 5,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "Agrisfera - Palazzina (DSS)",
            "Modello": "Modello MISP/IPI [Agronomica 3.0]",
            "Specie": "Pomodoro",
            "Avversita": "Peronospora della Patata e del Pomodoro",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 47,
                "Mod_Cod": 16,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 70,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "",
                "Bands": [],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 0,
                "Scale_Min": 0,
                "Status": 2,
                "StatusMsg": "Fine ciclo malattia, oospore esaurite al 26/05/2021",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO AMAZZONIA",
            "Modello": "Peronospora della vite [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Peronospora della Vite",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 43,
                "Mod_Cod": 19,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 70,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 8
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 24
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 40,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO AMAZZONIA",
            "Modello": "Oidio della vite [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Mal bianco (Oidio) della Vite",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 35,
                "Mod_Cod": 20,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 70,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 1.38
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 1.72
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 3.18800975396272,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 2.89819068542065
            },
            "Stazione": "MONITORAGGIO AMAZZONIA",
            "Modello": "Botrite della vite [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Botrite (=Muffa grigia)",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 532,
                "Mod_Cod": 35,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 70,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 52
            },
            "Risultato": {
                "AuxMsg": "Presenza dello stadio Adulti",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 100
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 200
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 300,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 1.239808341268858
            },
            "Stazione": "MONITORAGGIO AMAZZONIA",
            "Modello": "Nottua del pomodoro (Helicoverpa armigera) [Agronomica 3.0]",
            "Specie": "Pomodoro",
            "Avversita": "Nottua gialla del Pomodoro",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 138,
                "Mod_Cod": 36,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 70,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "Presenza dello stadio Uova",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 100
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 200
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 300,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 106.81275009485786
            },
            "Stazione": "MONITORAGGIO AMAZZONIA",
            "Modello": "Tignoletta della vite (Lobesia botrana) [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Tignoletta della Vite (=L. botrana)",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 46,
                "Mod_Cod": 42,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 70,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 46
            },
            "Risultato": {
                "AuxMsg": "Probabilità di infezione 0%",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 1.4
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 2.55
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 5,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO AMAZZONIA",
            "Modello": "Modello MISP/IPI [Agronomica 3.0]",
            "Specie": "Patata",
            "Avversita": "Peronospora della Patata e del Pomodoro",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 46,
                "Mod_Cod": 42,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 70,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 52
            },
            "Risultato": {
                "AuxMsg": "Probabilità di infezione 0%",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 1.4
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 2.55
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 5,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO AMAZZONIA",
            "Modello": "Modello MISP/IPI [Agronomica 3.0]",
            "Specie": "Pomodoro",
            "Avversita": "Peronospora della Patata e del Pomodoro",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 47,
                "Mod_Cod": 16,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 75,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 0.71
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 0.85
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 1,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO MARIANNA",
            "Modello": "Peronospora della vite [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Peronospora della Vite",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 43,
                "Mod_Cod": 19,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 75,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 8
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 24
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 40,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO MARIANNA",
            "Modello": "Oidio della vite [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Mal bianco (Oidio) della Vite",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 35,
                "Mod_Cod": 20,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 75,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 1.38
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 1.72
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 2.5,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 1.21717953439855
            },
            "Stazione": "MONITORAGGIO MARIANNA",
            "Modello": "Botrite della vite [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Botrite (=Muffa grigia)",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 532,
                "Mod_Cod": 35,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 75,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 52
            },
            "Risultato": {
                "AuxMsg": "Presenza dello stadio Adulti",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 100
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 200
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 300,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 10.397895906821266
            },
            "Stazione": "MONITORAGGIO MARIANNA",
            "Modello": "Nottua del pomodoro (Helicoverpa armigera) [Agronomica 3.0]",
            "Specie": "Pomodoro",
            "Avversita": "Nottua gialla del Pomodoro",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 138,
                "Mod_Cod": 36,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 75,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "Presenza dello stadio Larve",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 100
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 200
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 300,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 209.0188280105178
            },
            "Stazione": "MONITORAGGIO MARIANNA",
            "Modello": "Tignoletta della vite (Lobesia botrana) [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Tignoletta della Vite (=L. botrana)",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 46,
                "Mod_Cod": 42,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 75,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 46
            },
            "Risultato": {
                "AuxMsg": "Probabilità di infezione 0%",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 1.4
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 2.55
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 5,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO MARIANNA",
            "Modello": "Modello MISP/IPI [Agronomica 3.0]",
            "Specie": "Patata",
            "Avversita": "Peronospora della Patata e del Pomodoro",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 46,
                "Mod_Cod": 42,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 75,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 52
            },
            "Risultato": {
                "AuxMsg": "Probabilità di infezione 0%",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 1.4
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 2.55
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 5,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO MARIANNA",
            "Modello": "Modello MISP/IPI [Agronomica 3.0]",
            "Specie": "Pomodoro",
            "Avversita": "Peronospora della Patata e del Pomodoro",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 47,
                "Mod_Cod": 16,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 102,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 0.71
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 0.85
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 1,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO SAVARNA PIVOT",
            "Modello": "Peronospora della vite [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Peronospora della Vite",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 43,
                "Mod_Cod": 19,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 102,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 8
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 24
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 40,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO SAVARNA PIVOT",
            "Modello": "Oidio della vite [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Mal bianco (Oidio) della Vite",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 35,
                "Mod_Cod": 20,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 102,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 1.38
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 1.72
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 2.5,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 1.15156044166413
            },
            "Stazione": "MONITORAGGIO SAVARNA PIVOT",
            "Modello": "Botrite della vite [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Botrite (=Muffa grigia)",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 532,
                "Mod_Cod": 35,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 102,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 52
            },
            "Risultato": {
                "AuxMsg": "Presenza dello stadio Adulti",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 100
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 200
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 300,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 7.949890875937162
            },
            "Stazione": "MONITORAGGIO SAVARNA PIVOT",
            "Modello": "Nottua del pomodoro (Helicoverpa armigera) [Agronomica 3.0]",
            "Specie": "Pomodoro",
            "Avversita": "Nottua gialla del Pomodoro",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 138,
                "Mod_Cod": 36,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 102,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "Presenza dello stadio Larve",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 100
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 200
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 300,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 206.56221728683745
            },
            "Stazione": "MONITORAGGIO SAVARNA PIVOT",
            "Modello": "Tignoletta della vite (Lobesia botrana) [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Tignoletta della Vite (=L. botrana)",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 46,
                "Mod_Cod": 42,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 102,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 46
            },
            "Risultato": {
                "AuxMsg": "Probabilità di infezione 0%",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 1.4
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 2.55
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 5,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO SAVARNA PIVOT",
            "Modello": "Modello MISP/IPI [Agronomica 3.0]",
            "Specie": "Patata",
            "Avversita": "Peronospora della Patata e del Pomodoro",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 46,
                "Mod_Cod": 42,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 102,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 52
            },
            "Risultato": {
                "AuxMsg": "Probabilità di infezione 0%",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 1.4
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 2.55
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 5,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO SAVARNA PIVOT",
            "Modello": "Modello MISP/IPI [Agronomica 3.0]",
            "Specie": "Pomodoro",
            "Avversita": "Peronospora della Patata e del Pomodoro",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 47,
                "Mod_Cod": 16,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 144,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 0.71
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 0.85
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 1,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO CARLINA",
            "Modello": "Peronospora della vite [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Peronospora della Vite",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 43,
                "Mod_Cod": 19,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 144,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 8
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 24
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 40,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO CARLINA",
            "Modello": "Oidio della vite [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Mal bianco (Oidio) della Vite",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 35,
                "Mod_Cod": 20,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 144,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 1.38
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 1.72
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 2.5,
                "Scale_Min": 0,
                "Status": 3,
                "StatusMsg": "Non è stato raggiunto l'inizio del ciclo avversità",
                "Value": 22
            },
            "Stazione": "MONITORAGGIO CARLINA",
            "Modello": "Botrite della vite [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Botrite (=Muffa grigia)",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 532,
                "Mod_Cod": 35,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 144,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 52
            },
            "Risultato": {
                "AuxMsg": "Presenza dello stadio Adulti",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 100
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 200
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 300,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 2.176485974233556
            },
            "Stazione": "MONITORAGGIO CARLINA",
            "Modello": "Nottua del pomodoro (Helicoverpa armigera) [Agronomica 3.0]",
            "Specie": "Pomodoro",
            "Avversita": "Nottua gialla del Pomodoro",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 138,
                "Mod_Cod": 36,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 144,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 64
            },
            "Risultato": {
                "AuxMsg": "Presenza dello stadio Larve",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 100
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 200
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 300,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 202.27186100641488
            },
            "Stazione": "MONITORAGGIO CARLINA",
            "Modello": "Tignoletta della vite (Lobesia botrana) [Agronomica 3.0]",
            "Specie": "Vite",
            "Avversita": "Tignoletta della Vite (=L. botrana)",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 46,
                "Mod_Cod": 42,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 144,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 46
            },
            "Risultato": {
                "AuxMsg": "Probabilità di infezione 0%",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 1.4
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 2.55
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 5,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO CARLINA",
            "Modello": "Modello MISP/IPI [Agronomica 3.0]",
            "Specie": "Patata",
            "Avversita": "Peronospora della Patata e del Pomodoro",
            "DescrParametri": ""
        },
        {
            "Parametri": {
                "Alg_Cod": 0,
                "Avv_Cod": 46,
                "Mod_Cod": 42,
                "ParametriElaborazione": "{}",
                "Stazione_Cod": 144,
                "Tipo_Sorgente": 1,
                "Veg_Cod": 52
            },
            "Risultato": {
                "AuxMsg": "Probabilità di infezione 0%",
                "Bands": [
                    {
                        "Color": "#008000",
                        "Value": 1.4
                    },
                    {
                        "Color": "#ffff00",
                        "Value": 2.55
                    },
                    {
                        "Color": "#ff0000",
                        "Value": 7.922816251426434e28
                    }
                ],
                "DataFine": "2021-06-25 00:00:00Z",
                "DataInizio": "2021-01-01 00:00:00Z",
                "Scale_Max": 5,
                "Scale_Min": 0,
                "Status": 0,
                "StatusMsg": "",
                "Value": 0
            },
            "Stazione": "MONITORAGGIO CARLINA",
            "Modello": "Modello MISP/IPI [Agronomica 3.0]",
            "Specie": "Pomodoro",
            "Avversita": "Peronospora della Patata e del Pomodoro",
            "DescrParametri": ""
        }
    ];

    return {
        Stato: 1,
        Indicatori: [Indicatori[0], Indicatori[1], Indicatori[2], Indicatori[3], Indicatori[4], Indicatori[5], Indicatori[6], Indicatori[7], Indicatori[8]],
        XXX_Indicatori: Indicatori
    };
}