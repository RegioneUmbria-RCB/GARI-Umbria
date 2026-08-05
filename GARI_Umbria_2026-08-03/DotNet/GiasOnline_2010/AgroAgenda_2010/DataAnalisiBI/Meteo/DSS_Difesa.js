


function elabPeriodoPlugin() {
    return $("#elab-periodo").data("elabPeriodo");
}

function configurazioneModelli() {

    DSS_Difesa_ConfigurazioneModelli(url_meteo_ws, function () {

        let specie_ddl = $("#cmbSpecieVegetale").getKendoDropDownList();
        let specie_value = specie_ddl.value();

        let modelli_ddl = $("#cmbAvModAlg").getKendoDropDownList();
        let modelli_value = modelli_ddl.value();

        specie_ddl.dataSource.read();

        specie_ddl.value(specie_value);
        specie_ddl.trigger("change");

        modelli_ddl.value(modelli_value);
        modelli_ddl.trigger("change");
    });
}



//*************************************************************************************************
//*************************************************************************************************
//*************************************************************************************************



function DoY_from_Date(dt) {

    let day = dt.getDate();
    let month = dt.getMonth();

    let ms = 1000 * 60 * 60 * 24;

    let d0 = Math.floor((new Date(1999, 0, 1)).getTime() / ms); //1999 Anno non bisestile
    let d1 = Math.floor((new Date(1999, month, day)).getTime() / ms);

    return d1 - d0 + 1;
}

function Date_from_DoY(doy, year) {

    year = parseInt(year);

    if (doy > 365) {

        doy -= 365;
        year += 1;
    }

    let dt = new Date(1999, 0, 1); // Non bisestile
    dt.setDate(doy);
    dt.setFullYear(year);
    return dt;
};



//*************************************************************************************************
//*************************************************************************************************
//*************************************************************************************************



(function ($) {
    var DoY_DatePicker = kendo.ui.DatePicker.extend({
        options: {
            name: "DoY_DatePicker"
        },
        init: function (element, options) {

            if (!options) {
                options = {};
            }

            let currentYear = new Date().getFullYear();
            let min_date = new Date(currentYear, 0, 1);
            let max_date = new Date(currentYear, 11, 31);

            options.dateInput = true;
            options.format = "d MMMM";
            options.min = min_date;
            options.max = max_date;
            options.footer = false;
            options.open = function () {

                let calendar = this.dateView.calendar;

                //calendar.element.find(".k-calendar-th");

                if (calendar) {

                    calendar.unbind("navigate").bind("navigate", function (e) {
                        let widget = e.sender;
                        let viewName = widget.view().name;
                        let disable = viewName !== "month";

                        widget.wrapper.find(".k-calendar-nav-fast")
                            .toggleClass(GIAS_K_STATE_DISABLED, disable)
                            .attr("aria-disabled", disable);
                    }); 
                }
            }

            // The base call to the widget initialization.
            kendo.ui.DatePicker.fn.init.call(this, element, options);

            this.wrapper.addClass(["k-input-solid", "k-input-md", "k-rounded-md"]);
        },

        doy: function (value) {

            let curr_dt = this.value();

            if (value === undefined) {

                return DoY_from_Date(curr_dt);
            }

            let dt = new Date(curr_dt.getFullYear(), 0, 1);

            dt.setDate(dt.getDate() + value - 1);

            this.value(dt);
        },

        setRange: function (start_doy, end_doy, year) {

            let currValue = this.value();
            let currDoY = -1
            if (currValue) {

                currDoY = DoY_from_Date(currValue);
            }

            this.setOptions({
                min: new Date(year, 0, 1),
                max: new Date(year, 11, 31)
            });

            if (currDoY >= 0) {

                this.value(Date_from_DoY(currDoY, year));
            }
        }
    });
    kendo.ui.plugin(DoY_DatePicker);
})(jQuery);



//*************************************************************************************************
//*************************************************************************************************
//*************************************************************************************************



(function ($) {

    $.elabPeriodo = function (elem, opts) {

        var plugin = this;

        plugin.changePeriod = function () {
        };

        if (typeof opts.changePeriod === "function") {
            plugin.changePeriod = opts.changePeriod;
        }

        plugin.dayForecast = 0;

        if (typeof opts.dayForecast === "number" && opts.dayForecast >= 0) {
            plugin.dayForecast = opts.dayForecast;
        }

        let wrapper = document.createElement("div");
        wrapper.style.display = "grid";
        wrapper.style.gridTemplateColumns = "1fr auto auto auto";
        wrapper.style.alignItems = "center";
        wrapper.style.gap = "10px";
        elem.appendChild(wrapper);

        let yearSelector = document.createElement("input");
        yearSelector.style.width = "-webkit-fill-available";
        wrapper.appendChild(yearSelector);

        let startDoYelem = document.createElement("div");
        startDoYelem.className = "k-textbox";
        //startDoYelem.style.backgroundColor = "#fafafa";
        startDoYelem.style.padding = "0px 5px";
        startDoYelem.style.display = "flex";
        startDoYelem.style.alignItems = "center";
        wrapper.appendChild(startDoYelem);

        let startDoYtext = document.createElement("span");
        startDoYtext.style.overflow = "hidden";
        startDoYtext.style.whiteSpace = "nowrap";
        startDoYtext.style.textOverflow = "ellipsis";
        startDoYelem.appendChild(startDoYtext);

        let infoDiv = document.createElement("div");
        let infoSpan = document.createElement("span");
        infoSpan.className = "k-icon k-i-information info-forecast";
        infoDiv.appendChild(infoSpan);
        wrapper.appendChild(infoDiv);

        let endDoYelem = document.createElement("input");
        wrapper.appendChild(endDoYelem);

        let currY = new Date().getFullYear();

        $(yearSelector).kendoDropDownList({
            autoWidth: true,
            dataSource: Array.from({ length: 10 }, () => currY--),
            change: function (e) {
                _sayRange(false);
                plugin.changePeriod(_startDoY, _endDoY, e.sender.value());
            }
        });

        $(".info-forecast").kendoPopover({
            position: "top",
            showOn: "mouseenter",
            body: function (e) {
                return "<div>Impostando una data futura (max +7 gg) si ottengono proiezioni che includono i dati meteo previsionali.</div>";
            }
        });

        $(endDoYelem).kendoDoY_DatePicker();

        var _yearSelector = $(yearSelector).getKendoDropDownList();
        var _$startDoY = $(startDoYtext);
        var _endDoYPicker = $(endDoYelem).data("kendoDoY_DatePicker");
        var _startDoY = 1;
        var _endDoY = 365;

        var _sayRange = function (keepDateRange) {

            let year = _yearSelector.value();
            let startDate = Date_from_DoY(_startDoY, year);

            _$startDoY.text(kendo.toString(startDate, "d MMMM"));

            let endDate = Date_from_DoY(_endDoY, year);
            let now = new Date();
            now.setHours(0, 0, 0, 0);

            if (keepDateRange === false && endDate.getTime() > now.getTime()) {

                endDate = now;

                endDate.setDate(endDate.getDate() + plugin.dayForecast);
            }

            if (endDate.getTime() < startDate.getTime()) {

                endDate = startDate;
            }

            _endDoYPicker.setOptions({
                min: startDate,
                max: new Date(endDate.getFullYear(), 11, 31)
            });

            _endDoYPicker.value(endDate);
        };

        plugin.setRange = function (range) {

            if (typeof range !== "object") {

                return;
            }

            if (typeof range.start === "number" && typeof range.end === "number") {

                _startDoY = range.start;
                _endDoY = range.end;

                _sayRange(false);

                return;
            }

            if (Object.prototype.toString.call(range.start) === "[object Date]" && Object.prototype.toString.call(range.end) === "[object Date]") {

                let year = range.start.getFullYear();

                _yearSelector.value(year);
                if (_yearSelector.value() != year) {
                    _yearSelector.select(0);
                }

                _startDoY = DoY_from_Date(range.start);
                _endDoY = DoY_from_Date(range.end);
                if (_endDoY < _startDoY) {
                    _endDoY += 365;
                }

                _sayRange(true);
                return;
            }
        }

        plugin.getRange = function () {
            let year = _yearSelector.value();
            return {
                start: Date_from_DoY(_startDoY, year), 
                end: _endDoYPicker.value()
            };
        }

    }; // elabPeriodo

    $.fn.elabPeriodo = function (opts) {
        return this.each(function () {
            if (undefined == $(this).data('elabPeriodo')) {

                var plugin = new $.elabPeriodo(this, opts);

                $(this).data('elabPeriodo', plugin);
            }
        });
    };
})(jQuery);

