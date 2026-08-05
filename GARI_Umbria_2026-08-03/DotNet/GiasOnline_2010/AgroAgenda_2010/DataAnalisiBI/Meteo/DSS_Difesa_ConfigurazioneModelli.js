
function DSS_Difesa_ConfigurazioneModelli(url_meteo_ws, successCallback) {

    let win_el = document.createElement("div");
    document.body.appendChild(win_el);
    let $win_el = $(win_el);

    $win_el.data("can-close", false);

    $win_el.kendoDialog({
        title: "Configurazione modelli previsionali DSS Difesa",
        width: "90%",
        closable: false,
        modal: true,
        visible: false,
        content: "<div id='___modelli_grid___'></div>",
        actions: [
            {
                text: "<span class='fa fa-check fa-fw'></span><span>Salva configurazione</span>",
                action: function (e) {

                    let grid = $("#___modelli_grid___").getKendoGrid();

                    if (grid.dataSource.hasChanges()) {

                        let thisDlg = e.sender;

                        grid.dataSource.sync().then(function () {

                            // se tutto andato a buon fine...
                            thisDlg.element.data("can-close", true);
                            thisDlg.close();

                            if (typeof successCallback === "function") {
                                successCallback();
                            }
                        });

                        return false;
                    }
                }
            },
            {
                text: "<span class='fa fa-times fa-fw'></span><span>Annulla</span>",
                action: function (e) {

                    let thisDlg = e.sender;

                    if (!thisDlg.element.data("can-close")) {

                        if ($("#___modelli_grid___").getKendoGrid().dataSource.hasChanges()) {

                            _queryClose(function () {
                                thisDlg.element.data("can-close", true);
                                thisDlg.close();
                            });
                            return false;
                        }
                    }
                }
            }
        ],
        close: function (e) {
            //$("body").removeClass("body_overflow_hidden");
            this.destroy();
        },
        open: function (e) { 
            e.sender.element.css("opacity", "0");
            //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
            //$("body").addClass("body_overflow_hidden");
            _creaGrigliaConfigurazioneModelli("___modelli_grid___", url_meteo_ws);
        },
        show: function (e) {
            e.sender.element.css("opacity", "1");
        }
    });

    let parent = $win_el.parent();
    parent.css("padding-top", "48px");
    let titlebar = parent.find(".k-window-titlebar");
    titlebar.css({
        "margin-top": "-48px",
        "font-size": "larger"
    });

    let styleElem = document.createElement('style');
    styleElem.type = "text/css";
    //styleElem.innerHTML = ".body_overflow_hidden { overflow: hidden !important; }";
    styleElem.innerHTML = ".k-grouping-row .k-reset { font-size: 14px; color: #3f51b5; } ";
    styleElem.innerHTML += ".k-grouping-row .k-icon.k-i-expand { outline: none !important; } ";
    styleElem.innerHTML += ".k-grouping-row .k-icon.k-i-collapse { outline: none !important; } ";
    styleElem.innerHTML += "#___modelli_grid___ tr { background-color: #ffffff; } ";
    styleElem.innerHTML += ".k-dialog-buttongroup .k-button span { vertical-align: middle; } ";
    styleElem.innerHTML += ".k-dialog-buttongroup .k-button .fa { font-size: 22px; } ";
    styleElem.innerHTML += ".k-dialog-buttongroup .k-button .fa.fa-check { color: #5cb85c; } ";
    styleElem.innerHTML += ".k-dialog-buttongroup .k-button .fa.fa-times { color: #d9534f; } ";
    $(styleElem).appendTo(parent);

    $win_el.getKendoDialog().open();
}

function _leggiImpostazioni(url_meteo_ws) {

    let data = [];

    ajaxAgronicaSync(url_meteo_ws + "/LeggiModelliXImpostazioni",
        "",
        false,
        function (risposta) {

            let data_ = JSON.parse(risposta.RispostaStringa);

            $.each(data_, function (i, e) {
                $.each(e.Modelli, function (j, m) {

                    let curr_year = new Date().getFullYear();

                    let start_dt = new Date(1999, 0, 1);
                    start_dt.setDate(start_dt.getDate() + m.InizioPeriodo_gg - 1);
                    start_dt.setFullYear(curr_year);

                    let gg = m.FinePeriodo_gg;
                    if (gg > 365) {

                        gg -= 365;
                        curr_year += 1;
                    } 
                    let end_dt = new Date(1999, 0, 1);
                    end_dt.setDate(end_dt.getDate() + gg - 1);
                    end_dt.setFullYear(curr_year);

                    let o = {
                        key: e.Veg_Cod + "_" + m.Avv_Cod + "_" + m.Mod_Cod + "_" + m.Alg_Cod,
                        Veg_Cod: e.Veg_Cod,
                        Veg_Des: e.Veg_Des,
                        Mod_Cod: m.Mod_Cod,
                        Mod_Des: m.Mod_Des,
                        Avv_Cod: m.Avv_Cod,
                        Avv_Des: m.Avv_Des,
                        Alg_Cod: m.Alg_Cod,
                        Alg_Des: m.Alg_Des,
                        Mod_Des_Agg: m.Mod_Des_Agg,
                        InizioPeriodo_dt: start_dt,
                        FinePeriodo_dt: end_dt,
                        Full_Des: m.Full_Des
                    }
                    data.push(o);
                });
            });
        },
        function (risposta) {
        },
        null,
        false
    );

    return data;
}

function _scriviImpostazioni(url_meteo_ws, data) {

    let limp = [];
    $.each(data, function (i, e) {

        let start_d = e.InizioPeriodo_dt.getDate();
        let start_m = e.InizioPeriodo_dt.getMonth();
        let start_y = e.InizioPeriodo_dt.getFullYear();

        let end_d = e.FinePeriodo_dt.getDate()
        let end_m = e.FinePeriodo_dt.getMonth();
        let end_y = e.FinePeriodo_dt.getFullYear();

        let ms = 1000 * 60 * 60 * 24;
        let d0 = Math.floor((new Date(1999, 0, 1)).getTime() / ms);
        let d1 = Math.floor((new Date(1999, start_m, start_d)).getTime() / ms); 
        let d2 = Math.floor((new Date(1999, end_m, end_d)).getTime() / ms); 

        let start_doy = d1 - d0 + 1;
        let end_doy = d2 - d0 + 1;
        if (end_y > start_y) {
            end_doy += 365;
        }

        limp.push({
            Mod_Cod: e.Mod_Cod,
            Veg_Cod: e.Veg_Cod,
            Avv_Cod: e.Avv_Cod,
            Alg_Cod: e.Alg_Cod,
            InizioPeriodo_gg: start_doy,
            FinePeriodo_gg: end_doy
        });
    });

    let retValue = false;

    ajaxAgronicaSync(url_meteo_ws + "/ScriviModelliXImpostazioni",
        JSON.stringify({ listaImpostazioni: limp }),
        false,
        function (risposta) {
            retValue = true;
        },
        function (risposta) {
        },
        null,
        false
    );

    return retValue;
}

function _creaGrigliaConfigurazioneModelli(divId, url_meteo_ws) {

    $("#" + divId).kendoGrid({
        dataSource: {
            transport: {
                read: function (options) {
                    options.success(_leggiImpostazioni(url_meteo_ws));
                },
                update: function (options) {
                    if (_scriviImpostazioni(url_meteo_ws, options.data.models)) {
                        options.success();
                    } else {
                        options.error();
                    }
                }
            },
            batch: true,
            schema: {
                model: {
                    id: "key",
                    fields: {
                        InizioPeriodo_dt: {
                            validation: {
                                datepickerValidation: datepickerValidation
                            }
                        },
                        FinePeriodo_dt: {
                            validation: {
                                datepickerValidation: datepickerValidation
                            }
                        }

                    }
                }
            },
            group: [
                { field: "Veg_Des" }
            ]
        },
        editable: true,
        columns: [
            {
                field: "Veg_Des",
                groupHeaderTemplate: "#= value #",
                hidden: true
            },
            {
                field: "Full_Des",
                title: "Modello",
                editable: function (dataItem) { return false; }
            },
            {
                field: "InizioPeriodo_dt",
                title: "Inizio periodo elaborazione",
                template: "#= kendo.toString(InizioPeriodo_dt, 'd MMMM') #",
                editor: function (container, options) {
                    _editDOY(container, options, 0);
                }
            },
            {
                field: "FinePeriodo_dt",
                title: "Fine periodo elaborazione",
                template: "#= kendo.toString(FinePeriodo_dt, 'd MMMM') #",
                editor: function (container, options) {
                    _editDOY(container, options, 1);
                }
            }
        ]
    });

    let h = Math.floor($(window).height() * 0.6);

    $("#" + divId + " .k-grid-content").css("height", h + "px");
}

function datepickerValidation(input) {

    if (input.is("[data-role='datepicker']")) {

        let datepicker = input.getKendoDatePicker();

        if (datepicker) {

            let date = datepicker.value();

            if (date === null) {

                input.attr("data-datepickerValidation-msg", "Data non valida");

                return false;

            } else {

                let minDate = $(input).data("min-date");
                let maxDate = $(input).data("max-date");
                if (minDate && maxDate) {

                    if (date < minDate || maxDate < date) {

                        input.attr("data-datepickerValidation-msg", "Data non ammessa");

                        return false;
                    }
                }
            }
        }
    }
    return true;
}

function _editDOY(container, options, addYear) {

    let currYear = new Date().getFullYear();

    let min_date = new Date(currYear, 0, 1);
    let max_date = new Date(currYear + addYear, 11, 31);

    let input = $("<input name='" + options.field + "'/>");
    input.appendTo(container);
    input.kendoDatePicker({
        dateInput: true,
        format: "d MMMM",
        value: options.model[options.field],
        min: min_date,
        max: max_date,
        footer: false,
        open: function () {

            let calendar = this.dateView.calendar;

            if (calendar) {

                calendar.unbind("navigate").bind("navigate", function (e) {
                    let widget = e.sender;
                    let viewName = widget.view().name;
                    let disable = viewName !== "month";

                    widget.wrapper.find(".k-nav-fast")
                        .toggleClass(GIAS_K_STATE_DISABLED, disable)
                        .attr("aria-disabled", disable);
                });
            }
        }
    });

    let minDate = null;
    let maxDate = null;
    if (options.field === "InizioPeriodo_dt") {
        maxDate = new Date(options.model.FinePeriodo_dt);
        maxDate.setDate(maxDate.getDate() - 1);
        minDate = min_date;
    } else if (options.field === "FinePeriodo_dt") {
        minDate = new Date(options.model.InizioPeriodo_dt);
        minDate.setDate(minDate.getDate() + 1);
        maxDate = max_date;
    }
    if (minDate != null && maxDate != null) {
        $(input).data("min-date", minDate)
        $(input).data("max-date", maxDate)
    }
}

function _queryClose(yesCallback) {

    let dlgElem = document.createElement("div");
    document.body.appendChild(dlgElem);

    $(dlgElem).kendoDialog({
        title: "Annullamento modifiche",
        content: "Confermi l'annullamento di tutte le modifiche effettuate?",
        visisble: false,
        close: function (e) {
            this.destroy();
        },
        actions: [
            {
                text: "Si",
                action: function (e) {
                    if (typeof yesCallback === "function") {
                        yesCallback();
                    }
                }
            },
            {
                text: "No"
            }
        ]
    }).data("kendoDialog").open();
}
